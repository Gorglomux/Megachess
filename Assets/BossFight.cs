using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossFight : MonoBehaviour
{

    public RoomView r;
    public Tilemap KingTilemap;
    public Tilemap FogOfWarKingTilemap;
    public UnitData kingData;

    public Unit mainKingUnit; // Move by hand then instantiate at the end 
    public Transform kingMoveTarget;
    public Transform positionKingUnit;

    public List<GameObject> fogOfWar;
    public Transform zoomCameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        GlobalHelper.UI().HideRoot();
        GlobalHelper.UI().HidePauseMenu();
        GlobalHelper.UI().bottomText.gameObject.SetActive(false);
        GlobalHelper.UI().SetBottomText("", -1, false);
        r = GetComponent<RoomView>();
        StartCoroutine(corKingTalk());
        //Instantiate the king here 
        kingData = GlobalHelper.GetUnitData("King");
        mainKingUnit = r.CreateUnit(kingData, true);
        mainKingUnit.transform.position = positionKingUnit.position;
        StartCoroutine(AudioManager.instance.FadeOutMusic(AudioManager.instance.mainMusicAudioSource));

        GlobalHelper.DisableMouse();
    } 
    public IEnumerator corKingTalk()
    {

        EnableFogOfWar(0);
        GlobalHelper.UI().SetBottomText("", -1, false);
        GlobalHelper.UI().bottomText.gameObject.SetActive(false);
        yield return null;
        GlobalHelper.UI().SetBottomText("", -1, false);
        //Fog of war 0 
        //Screen shake 
        GlobalHelper.getCamMovement().ShakeCamera(4f, 1f);
        yield return new WaitForSeconds(2.5f);
        GlobalHelper.UI().bottomText.gameObject.SetActive(true);
        EnableFogOfWar(1);
        GlobalHelper.getCamMovement().ShakeCamera(4f, 1f);
        GlobalHelper.UI().SetBottomText("So you've finally decided to come.", -1, false);
        AudioManager.instance.PlaySound("sfx_drum_lowpitch", 1, UnityEngine.Random.Range(1, 1f));
        yield return new WaitForSeconds(4f);
        GlobalHelper.getCamMovement().ShakeCamera(4f, 1f);
        GlobalHelper.UI().SetBottomText("You're here to get your king back, aren't you? ", -1, false);
        AudioManager.instance.PlaySound("sfx_drum_lowpitch", 1, UnityEngine.Random.Range(1, 1f));
        yield return new WaitForSeconds(4f);

        //Fade fog of war king
        StartCoroutine(fadeFogOfWarKing());
    }
    List<Vector3Int> positionsSpawnKing = new List<Vector3Int>();
    public IEnumerator fadeFogOfWarKing()
    {
        GlobalHelper.UI().SetBottomText("YOU WILL HAVE TO HELP ME FIGURE OUT WICH ONE IT IS THEN.", -1, false);
        AudioManager.instance.PlaySound("sfx_drum_lowpitch", 1, UnityEngine.Random.Range(1, 1f));
        GlobalHelper.getCamMovement().ShakeCamera(6f,2);
        yield return new WaitForSeconds(3f);
        //Camera zoom on king transform
         

        foreach (Vector3Int cell in FogOfWarKingTilemap.cellBounds.allPositionsWithin)
        {
            if(FogOfWarKingTilemap.GetTile(cell) != null)
            {
                positionsSpawnKing.Add(cell);

            }
        }
        positionsSpawnKing = positionsSpawnKing.OrderBy(x => GlobalHelper.rand.Next(positionsSpawnKing.Count)).ToList();
        foreach (Vector3Int cell in positionsSpawnKing)
        {
            if(FogOfWarKingTilemap.GetTile(cell)!= null)
            {
                FogOfWarKingTilemap.SetTile(cell, null);
                GlobalHelper.getCamMovement().ShakeCamera(2, 0.3f);
                AudioManager.instance.PlaySound("sfx_chess_move", 1, UnityEngine.Random.Range(0.8f, 0.83f));
                yield return new WaitForSeconds(0.5f);
            }

        }
        yield return new WaitForSeconds(1);
        GlobalHelper.getCamMovement().ZoomToPosition(zoomCameraTransform.position, 0.6f, 3f);

        //Move king to king transform 
        //Add the final king to the tilemap after moving
        Tween t = mainKingUnit.transform.DOMove(kingMoveTarget.position, 3).SetEase(Ease.InBack, GlobalHelper.TWEEN_OVERSHOOT_MEGA);
        yield return t.WaitForCompletion();
        PlaceKingsOnMap();
        yield return null;// Adjust to sync with the merge 
        Destroy(mainKingUnit.gameObject);
        EnableFogOfWar(0);
        KingTilemap.gameObject.SetActive(false);
    }



    void PlaceKingsOnMap()
    {
        foreach (Vector3Int cell in positionsSpawnKing)
        {
            Unit u = r.CreateUnit(kingData, true);
            u.AddEffect(GlobalHelper.effectLookup(GlobalHelper.GetEffectData("Bloodlust")));
            r.PlaceUnitOnMap(u,cell);
            GlobalHelper.getCamMovement().ShakeCamera(0.5f, 0.2f);
        }
        r.CheckMegasOnGrid();
        StartCoroutine(fadeToBlackThenTitle());
    }

    public IEnumerator fadeToBlackThenTitle()
    {
        yield return null;

        EnableFogOfWar(0);
        GlobalHelper.getCamMovement().ShakeCamera(3, 3);
        GlobalHelper.getCamMovement().ZoomToPosition(zoomCameraTransform.position, 0.7f, 0.7f).WaitForCompletion();
        yield return new WaitForSeconds(3.9f);
        AudioManager.instance.PlayFightMusic("music_boss", true);
        yield return new WaitForSeconds(0.1f);
        AudioManager.instance.PlaySound("sfx_drum_lowpitch", 1.5f, UnityEngine.Random.Range(1, 1f));
        EnableFogOfWar(3);
        GlobalHelper.getCamMovement().ShakeCamera(15, 5);


        GlobalHelper.getCamMovement().ZoomToPosition(zoomCameraTransform.position, 0.6f, 0.5f);
        GlobalHelper.UI().ShowRoot();
        GlobalHelper.UI().ShowPauseButton();


        yield return new WaitForSeconds(3f);
        GlobalHelper.getCamMovement().ResetCameraPosition();
        GlobalHelper.getCamMovement().ResetZoomPosition();
        GlobalHelper.EnableMouse();
        //Show the UI here 
        //At the end of this the fight starts normally
    }

    public void EnableFogOfWar(int index)
    {
        foreach (GameObject go in fogOfWar)
        {
            go.SetActive(false);
        }

        if (fogOfWar.Count > index)
        {
            fogOfWar[index].SetActive(true);

        }

    }

}
