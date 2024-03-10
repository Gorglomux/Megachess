using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TutorialRoom4 : TutorialRoomBase
{
    public RoomView r;


    public Player p;
    public override void Initialize()
    {
        r = GetComponent<RoomView>();


        p = GlobalHelper.GlobalVariables.player;
        p.AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Rook"), false));
        GlobalHelper.UI().abilityButton.gameObject.SetActive(true);
        GlobalHelper.UI().endTurnButton.gameObject.SetActive(true);
        GlobalHelper.UI().resetFightButton.gameObject.SetActive(true);
        GlobalHelper.GetGameManager().OnRoomCleared += SetTutoPlayed;
        p.ChangeAbility(GlobalHelper.GetAbilityData("ExtraTurn"));
        StartCoroutine(startTutorial());
    }

    public void OnDestroy()
    {

        GlobalHelper.GetGameManager().OnRoomCleared -= SetTutoPlayed;
    }
    public void SetTutoPlayed(object o)
    {
        PlayerPrefs.SetInt("PlayTutorial", 0);
        print("will skip tuto from now on");
    }
    public IEnumerator startTutorial()
    {
        yield return new WaitForSeconds(2);
        GlobalHelper.UI().SetBottomText("Abilities are useful to save units or clear rooms faster. ", -1, true);

        r.SpawnableCells.Add(new Vector3Int(-5, 3));
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

    }

    public void part3()
    {

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
                GlobalHelper.UI().SetBottomText("Use your ability to win this fight.", -1, true);
                //GlobalHelper.UI().ShakeButtonBottomRightText();
                GlobalHelper.getCamMovement().ShakeCamera(3f, 0.8f);
                break;
            case 2:

                break;

            case 3:

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
