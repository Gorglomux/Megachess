using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialRoom1 : TutorialRoomBase
{
    public RoomView r;
    Unit king;
    Unit enemyKing;
    public override void Initialize()
    {
        print("Loading fog of war and bottom sentence");
        r = GetComponent<RoomView>();
         king = r.CreateUnit(GlobalHelper.GetUnitData("King"), false);
        king.OnHit += FadeInScreen;
        king.health = 13;
        king.startingHealth = 13;
        r.PlaceUnitOnMap(king, new Vector3Int(-6,0));


        enemyKing = r.CreateUnit(GlobalHelper.GetUnitData("King"), true);
        enemyKing.transform.parent = r.unitsParent.transform;
        enemyKing.transform.position = Vector3.zero;
        enemyKing.transform.localPosition= r.GetCenter(new Vector3Int(2, -1));

        //Put a king at the position 
        StartCoroutine(startTutorial());
        EnableFogOfWar(5);
        titleScreen.material.SetFloat("_PaletteIndex",king.basePaletteIndex);
        titleScreen.material.SetFloat("_Dither",12);

        GlobalHelper.UI().abilityButton.gameObject.SetActive(false);
        GlobalHelper.UI().endTurnButton.gameObject.SetActive(false);
        GlobalHelper.UI().resetFightButton.gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        king.OnHit -= FadeInScreen;
    }
    public IEnumerator startTutorial()
    {
        yield return new WaitForSeconds(2);
        EnableFogOfWar(0);
        GlobalHelper.UI().SetBottomText("Click on your king to select it.");
        GlobalHelper.getCamMovement().ShakeCamera(4f, 0.8f);

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
                GlobalHelper.UI().SetBottomText("Move to the right to continue. ");
                //GlobalHelper.UI().ShakeButtonBottomRightText();
                GlobalHelper.getCamMovement().ShakeCamera(3f, 0.8f);
                EnableFogOfWar(1);
                break;
            case 2:
                GlobalHelper.UI().SetBottomText("You can also drag and drop the king to move. ");
                GlobalHelper.getCamMovement().ShakeCamera(3f, 0.8f);

                EnableFogOfWar(2);
                break;
            case 3:
                GlobalHelper.UI().SetBottomText("Kill enemy units by moving into them. ");
                GlobalHelper.getCamMovement().ShakeCamera(3f, 0.8f);


                break;
            case 4:
                //Time scale == 0 ? 
                //Say the line 

                StartCoroutine(EndTuto1Cutscene());
                break;
            case 5:
                break;

        }

    }
    public IEnumerator EndTuto1Cutscene()
    {
        yield return new WaitForSeconds(2);
        GlobalHelper.getCamMovement().ShakeCamera(3f, 1);
        GlobalHelper.UI().SetBottomText("Did you think going into my lair was going to be this easy?");

        EnableFogOfWar(3);
        yield return new WaitForSeconds(3);
        GlobalHelper.getCamMovement().ShakeCamera(3f, 1);
        EnableFogOfWar(4);
        //GlobalHelper.getCamMovement().ZoomToPosition(titleScreen.transform.position,0.6f,12);

    }

    public Image titleScreen;
    int fadeInStage = 0;
    int totalFadeInStages = 13;
    public void FadeInScreen(object o)
    {
        fadeInStage++;
        float f = Mathf.Lerp(0,13 , 1 - ((float)fadeInStage / (float)totalFadeInStages));
        titleScreen.material.SetFloat("_Dither", f);

        if(fadeInStage == 11)
        {
            r.PlaceUnitOnMap(enemyKing, new Vector3Int(2, -1));
        }
        if(fadeInStage == totalFadeInStages)
        {
            EnableFogOfWar(5);
            StartCoroutine(corEndTuto1());
        }

    }
    public IEnumerator corEndTuto1()
    {
        yield return new WaitForSeconds(3);
        GlobalHelper.getCamMovement().ShakeCamera(5f, 1);
        titleScreen.material.SetFloat("_Dither", 12);
        titleScreen.gameObject.SetActive(false);
        GlobalHelper.UI().SetBottomText("");
        yield return new WaitForSeconds(3);
        //This is where we change scenes

        GlobalHelper.GlobalVariables.bloodSplatManager.Cleanup();
        GlobalHelper.GetGameManager().ChangeState(new ChangeRoomState());
    }
}
