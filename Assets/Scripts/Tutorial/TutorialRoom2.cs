using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TutorialRoom2 : TutorialRoomBase
{
    public RoomView r;


    public bool firstKill = false;
    public Player p;
    public override void Initialize()
    {
        tilemapEnemiesDelayed.gameObject.SetActive(false);
        print("Loading fog of war and bottom sentence");
        r = GetComponent<RoomView>();

        p = GlobalHelper.GlobalVariables.player;
        p.AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Bishop"), false));

        GlobalHelper.UI().abilityButton.gameObject.SetActive(false);
        GlobalHelper.UI().endTurnButton.gameObject.SetActive(false);
        GlobalHelper.UI().resetFightButton.gameObject.SetActive(false);
        r.OnBoardUpdate += showNextStage;

        GlobalHelper.GetGameManager().OnKillUnit += FirstKill;
        StartCoroutine(startTutorial());
        EnableFogOfWar(3);
        GlobalHelper.EnableMouse();

    }

    public void OnDestroy()
    {
        r.OnBoardUpdate -= showNextStage;
    }
    public void FirstKill(object o)
    {
        if (!firstKill)
        {
            firstKill = true;
            GlobalHelper.UI().SetBottomText("You will capture the first enemy unit you kill each fight.", -1, true);
            GlobalHelper.getCamMovement().ShakeCamera(4f, 0.8f);
        }
    }
    public Transform reserveTransform;
    public Transform cellTransform;
    public IEnumerator startTutorial()
    {
        yield return new WaitForSeconds(0.5f);
        EnableFogOfWar(0);
        yield return GlobalHelper.getCamMovement().ZoomToPosition(reserveTransform.position, 0.9f, 3).WaitForCompletion();
        GlobalHelper.UI().SetBottomText("This is your reserve. you can place units in the room during the selection phase.", -1, true);
        yield return new WaitForSeconds(3f);
        r.SpawnableCells.Add(new Vector3Int(-6, -2));
        GlobalHelper.GlobalVariables.indicatorManager.ShowSpawnableCells();
        GlobalHelper.UI().SetBottomText("Click on the bishop, and place it in the highlighted cell at the bottom left of the room.", -1, true);
        yield return GlobalHelper.getCamMovement().ZoomToPosition(cellTransform.position, 0.8f, 2).WaitForCompletion();
        yield return new WaitForSeconds(2);
        GlobalHelper.getCamMovement().ResetCameraPosition();
        GlobalHelper.getCamMovement().ResetZoomPosition();
        yield return new WaitForSeconds(3f);

    }
    int stage = 0;
    public void showNextStage()
    {
        stage++;
        switch (stage)
        {
            case 1:
                part2();
                break;
            case 2:
                part3();
                break;
            case 3:
                part4();
                break;
            case 4:
                part5();
                break;
        }
    }
    public void part2()
    {
        p.AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Rook"), false));
        GlobalHelper.getCamMovement().ShakeCamera(2f, 0.3f);
        GlobalHelper.UI().SetBottomText("Great, now do the same with the Rook.", -1, true);

        GlobalHelper.GlobalVariables.indicatorManager.HideSpawnableCells();
        r.SpawnableCells.Clear();
        r.SpawnableCells.Add(new Vector3Int(-4, -3));
        GlobalHelper.GlobalVariables.indicatorManager.ShowSpawnableCells();
    }

    public void part3()
    {

        GlobalHelper.getCamMovement().ShakeCamera(2f, 0.3f);
        GlobalHelper.UI().SetBottomText("Great, now do the same with the Knight.", -1, true);
        p.AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Knight"), false));

        GlobalHelper.GlobalVariables.indicatorManager.HideSpawnableCells();
        r.SpawnableCells.Clear();
        r.SpawnableCells.Add(new Vector3Int(-1, 0));
        GlobalHelper.GlobalVariables.indicatorManager.ShowSpawnableCells();
    }

    public void part4()
    {
        GlobalHelper.getCamMovement().ShakeCamera(2f, 0.3f);
        GlobalHelper.UI().SetBottomText("Finally, place a Pawn at this position.", -1, true);
        p.AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Pawn"), false));

        GlobalHelper.GlobalVariables.indicatorManager.HideSpawnableCells();
        r.SpawnableCells.Clear();
        r.SpawnableCells.Add(new Vector3Int(3, -4));
        GlobalHelper.GlobalVariables.indicatorManager.ShowSpawnableCells();

    }
    public Tilemap tilemapEnemiesDelayed;
    public IEnumerator SpawnEnemiesDelayed()
    {
        tilemapEnemiesDelayed.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        foreach (Vector3Int cell in tilemapEnemiesDelayed.cellBounds.allPositionsWithin)
        {
            if (!tilemapEnemiesDelayed.HasTile(cell))
            {
                continue;
            }
            print(tilemapEnemiesDelayed.HasTile(cell) + " " + cell);
            //Get the Unit data in the global variables 
            //print(position);
            TileBase entity = tilemapEnemiesDelayed.GetTile(cell);
            UnitData ud = GlobalHelper.GetUnitData(entity.name);
            if (ud == null)
            {
                Debug.LogError("Invalid Unit data for name : " + entity.name);
            }
            else
            {
                Unit u = GlobalHelper.GetRoom().CreateUnit(ud, true);
                GlobalHelper.GetRoom().PlaceUnitOnMap(u, cell);
        AudioManager.instance.PlaySound("sfx_chess_move", 1f, UnityEngine.Random.Range(0.9f, 1f));
                GlobalHelper.getCamMovement().ShakeCamera(0.5f, 0.2f);
                yield return GlobalHelper.getCamMovement().ZoomToPosition(u.GetWorldPosition(), 0.9f).WaitForCompletion();
                yield return new WaitForSeconds(0.5f);
            }

        }
        yield return new WaitForSeconds(0.5f);

        GlobalHelper.getCamMovement().ShakeCamera(2f, 0.3f);
        GlobalHelper.UI().SetBottomText("Press the Fight button and kill all the enemies..", -1, true);
        GlobalHelper.UI().endTurnButton.gameObject.SetActive(true);
        GlobalHelper.UI().EnableButton(GlobalHelper.UI().endTurnButton);

        GlobalHelper.getCamMovement().ResetCameraPosition();
        GlobalHelper.getCamMovement().ResetZoomPosition();
        //Spawn the Fight button here

        EnableFogOfWar(1);
        
        tilemapEnemiesDelayed.gameObject.SetActive(false);
    }
    public void part5()
    {
        StartCoroutine(SpawnEnemiesDelayed());

    }


    public override void TriggerEvent(int identifier)
    {
        if (triggeredEvents.Contains(identifier))
        {
            return;
        }
        triggeredEvents.Add(identifier);
        switch (identifier)
        {
            case 1:
                GlobalHelper.UI().SetBottomText("The pawn cannot move and attack in the same direction. Check the preview at the top left to know where it can attack from.",-1,true);
                //GlobalHelper.UI().ShakeButtonBottomRightText();
                GlobalHelper.getCamMovement().ShakeCamera(3f, 0.8f);
                EnableFogOfWar(2);
                break;
            case 2:

            case 3:

            case 4:

            case 5:
                break;

        }

    }

    public IEnumerator corEndTuto2()
    {
        yield return new WaitForSeconds(3);
        GlobalHelper.getCamMovement().ShakeCamera(5f, 1);
        GlobalHelper.UI().SetBottomText("");
        EnableFogOfWar(3);
        yield return new WaitForSeconds(3); 

    }


}
