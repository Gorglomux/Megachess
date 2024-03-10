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
    public IEnumerator startTutorial()
    {
        yield return new WaitForSeconds(0.5f);
        EnableFogOfWar(0);
        GlobalHelper.UI().SetBottomText("This is your reserve. you can place units in the room during the selection phase.", -1, true);
        GlobalHelper.getCamMovement().ShakeCamera(4f, 0.8f);
        yield return new WaitForSeconds(3);
        GlobalHelper.UI().SetBottomText("Click on the bishop, and place it in the highlighted cell in the room.", -1, true);
        r.SpawnableCells.Add(new Vector3Int(-6,-2));
        GlobalHelper.GlobalVariables.indicatorManager.ShowSpawnableCells();

    }
    int stage = 0;
    public void showNextStage(){
        switch (stage)
        {
            case 0:
                part2();
                break;
            case 1:
                part3();
                break;
            case 2:
                part4();
                break;
            case 3:
                part5();
                break;
        }
        stage++;
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

    public void part5()
    {
        GlobalHelper.getCamMovement().ShakeCamera(2f, 0.3f);
        GlobalHelper.UI().SetBottomText("Press the Fight button to start the combat.", -1, true);
        GlobalHelper.UI().endTurnButton.gameObject.SetActive(true);
        GlobalHelper.UI().EnableButton(GlobalHelper.UI().endTurnButton);
        //Spawn the Fight button here

        EnableFogOfWar(1);
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
