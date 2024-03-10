using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TutorialRoom3 : TutorialRoomBase
{
    public RoomView r;


    public Player p;
    public override void Initialize()
    {
        print("Loading fog of war and bottom sentence");
        r = GetComponent<RoomView>();


        p = GlobalHelper.GlobalVariables.player;
        p.ClearInventory();
        r.PlaceUnitOnMap(r.CreateUnit(GlobalHelper.GetUnitData("Knight"), false), new Vector3Int(-5, 3));
        r.PlaceUnitOnMap(r.CreateUnit(GlobalHelper.GetUnitData("Knight"), false), new Vector3Int(-4, 3));
        r.PlaceUnitOnMap(r.CreateUnit(GlobalHelper.GetUnitData("Knight"), false), new Vector3Int(-5, 2));
       

        p.AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Rook"), false));
        p.AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Rook"), false));
        p.AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Rook"), false));
        p.AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Rook"), false));

        GlobalHelper.UI().abilityButton.gameObject.SetActive(false);
        GlobalHelper.UI().ShowTopInfos();
        GlobalHelper.UI().endTurnButton.gameObject.SetActive(false);
        GlobalHelper.UI().resetFightButton.gameObject.SetActive(false);
        r.OnBoardUpdate += showNextStage;

        StartCoroutine(startTutorial());
    }

    public void OnDestroy()
    {
        r.OnBoardUpdate -= showNextStage;
    }
    public IEnumerator startTutorial()
    {
        yield return new WaitForSeconds(2);
        GlobalHelper.UI().SetBottomText("Play the 4 rooks in the highlighted cells.", -1, true);
        GlobalHelper.getCamMovement().ShakeCamera(4f, 0.8f);
        r.SpawnableCells.Add(new Vector3Int(-6,-3));
        r.SpawnableCells.Add(new Vector3Int(-5,-3));
        r.SpawnableCells.Add(new Vector3Int(-5,-4));
        r.SpawnableCells.Add(new Vector3Int(-6,-4));
        GlobalHelper.GlobalVariables.indicatorManager.ShowSpawnableCells();

    }
    int stage = 0;
    public void showNextStage(){
        switch (stage)
        {
            case 3:
                part2();
                break;
            case 4:
                part3();
                break;
            case 5:
                part4();
                break;
            case 6:
                part5();
                break;
        }
        stage++;
    }
    public void part2()
    {
        p.AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Knight"), false));
        GlobalHelper.getCamMovement().ShakeCamera(2f, 0.3f);
        GlobalHelper.UI().SetBottomText("Now do the same with the Knight.", -1, true);

        GlobalHelper.GlobalVariables.indicatorManager.HideSpawnableCells();
        r.SpawnableCells.Clear();
        r.SpawnableCells.Add(new Vector3Int(-5, 0));
        GlobalHelper.GlobalVariables.indicatorManager.ShowSpawnableCells();
    }

    public void part3()
    {
        GlobalHelper.getCamMovement().ShakeCamera(2f, 0.3f);
        GlobalHelper.UI().SetBottomText("Press the Fight button to start the combat.", -1, true);
        GlobalHelper.UI().endTurnButton.gameObject.SetActive(true);
        GlobalHelper.UI().EnableButton(GlobalHelper.UI().endTurnButton);

    }

    public void part4()
    {

    }

    public void part5()
    {

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
                GlobalHelper.UI().SetBottomText("Mega units are tougher than their smaller variants.", -1, true);
                //GlobalHelper.UI().ShakeButtonBottomRightText();
                GlobalHelper.getCamMovement().ShakeCamera(3f, 0.8f);
                break;
            case 2:
                GlobalHelper.UI().SetBottomText("You and the enemy can form mega units with ally units already present in the room.", -1, true);
                GlobalHelper.getCamMovement().ShakeCamera(3f, 0.8f);
                break;

            case 3:
                GlobalHelper.UI().SetBottomText("You earn extra gold at the end of the fight by clearing rooms quickly.", -1, true);
                GlobalHelper.getCamMovement().ShakeCamera(3f, 0.8f);
                break;
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
