using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IState
{
    void OnEntry(GameManager gm);

    void OnUpdate(GameManager gm);

    void OnExit(GameManager gm);
}

public class UnitPlaceState : IState
{
    GameManager gmRef;
    public void OnEntry(GameManager gm)
    {
        gmRef = gm;
        Debug.Log("Starting unit place phase");
        //Select or drag units from the reserve to the spawn cells. 
        GlobalHelper.UI().SetBottomText("Drag up to x units from the reserve to the highlighted cells"); // FAIRE EN SORTE QUE LE X POINTE VERS UN SCRIPTABLEOBJECT? 
        GlobalHelper.UI().SetButtonBottomRightText("Fight !");
        //Change bottom button to "Validate placement"

        GlobalHelper.GlobalVariables.indicatorManager.ShowSpawnableCells();
        GlobalHelper.UI().OnChangePhase += EndPlaceState;
        GlobalHelper.GetRoom().OnBoardUpdate += CheckUnitsLeft;
        CheckUnitsLeft();
    }

    public void OnExit(GameManager gm)
    {
        Debug.Log("Ending unit place phase");

        //Link the end turn button to there. 
        GlobalHelper.GlobalVariables.indicatorManager.HideSpawnableCells();
        GlobalHelper.UI().OnChangePhase -= EndPlaceState;
    }

    public void OnUpdate(GameManager gm)
    {
    }
    public void EndPlaceState()
    {
        GlobalHelper.GetRoom().CheckMegasOnGrid();
        gmRef.ChangeState(new FightState());
    }
    public void CheckUnitsLeft()
    {
        RoomView r = GlobalHelper.GetRoom();
        int unitLeft = r.roomData.maxUnits - r.GetAllies().Count;
        Debug.Log("Todo : check units left");
        GlobalHelper.UI().SetBottomText("Drag up to "+unitLeft+" units from the reserve to the highlighted cells"); // FAIRE EN SORTE QUE LE X POINTE VERS UN SCRIPTABLEOBJECT? 
    }
}


public class FightState : IState
{
    public event Action OnEndTurn = delegate { };
    public Unit capturedThisFight = null;
    public void OnEntry(GameManager gm)
    {
        GlobalHelper.UI().SetButtonBottomRightText("End Turn");
        GlobalHelper.UI().SetBottomText("Kill all enemies");
        GlobalHelper.UI().OnChangePhase += EndTurn;
        OnEndTurn += GlobalHelper.UI().OnEndTurn;

        CheckGameWinState();
        GlobalHelper.GetRoom().OnBoardUpdate += CheckGameWinState;
        GlobalHelper.GetRoom().OnKillUnit += TryCapture;
        GlobalHelper.GlobalVariables.gameInfos.currentTurn = 1;
    }

    public void OnExit(GameManager gm)
    {
        Debug.Log("Ending fight place phase");
        RoomView r = GlobalHelper.GetRoom();
        r.destroyedUnitsThisFight.Remove(capturedThisFight);
        r.CleanUpFight();
    }

    public void OnUpdate(GameManager gm)
    {
    }

    public void EndTurn()
    {
        Debug.Log("Ending turn");
        foreach(Unit u in GlobalHelper.GetRoom().getAllUnits())
        {
            if (u.isEnemy)
            {
                u.EnemyAttack();
            }
            else
            {
                u.RefreshActions();
            }
        }
        GlobalHelper.GlobalVariables.gameInfos.currentTurn++;
        OnEndTurn();
    }
    public void TryCapture(Unit u)
    {
        if (u.isEnemy)
        {
            if(capturedThisFight == null)
            {
                capturedThisFight = u;
                //Animation de capture a la position de l'unit. Pour les megas je sais pas
                GlobalHelper.GlobalVariables.player.AddUnit(u);
            }
        }
    }
    public void CheckGameWinState()
    {
        Debug.Log("Checking win state");
        RoomView r = GlobalHelper.GetRoom();
        List<Unit> units = r.getAllUnits();

        GlobalHelper.UI().SetBottomText(r.GetEnemies().Count + " remaining");
        if (r.GetEnemies().Count == 0)
        {
            GlobalHelper.UI().SetBottomText("Fight won, going to the next fight !");
        }

        if (r.GetAllies().Count == 0)
        {
            GlobalHelper.UI().SetBottomText("GAME OVER. Close the game I didn't implement this.");

        }
    }
}