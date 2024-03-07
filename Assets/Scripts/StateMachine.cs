using DG.Tweening;
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
    bool canReset = false;
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
        GlobalHelper.UI().OnResetTurn += OnResetTurn;
        GlobalHelper.GetRoom().OnBoardUpdate += CheckUnitsLeft;
        GlobalHelper.GlobalVariables.player.BackupInventory();
        canReset = true;
        CheckUnitsLeft();
    }

    public void OnResetTurn()
    {
        if (!canReset)
        {
            return;
        }
        GameManager gm = GlobalHelper.GetGameManager();
        if (gm.currentState is FightState || gm.currentState is UnitPlaceState)
        {

            gm.CleanPreviousRoom();
            gm.LoadRoom(gm.currentRoom.roomData).onComplete += () => {

                GlobalHelper.GlobalVariables.player.RestoreBackup();
                gm.ChangeState(new UnitPlaceState());
            };

        }
    }
    public void OnExit(GameManager gm)
    {
        canReset = false;
        Debug.Log("Ending unit place phase");
        GlobalHelper.UI().OnResetTurn -= OnResetTurn;

        //Link the end turn button to there. 
        GlobalHelper.GlobalVariables.indicatorManager.HideSpawnableCells();
        GlobalHelper.UI().OnChangePhase -= EndPlaceState;
        GlobalHelper.GetRoom().OnBoardUpdate -= CheckUnitsLeft;
    }

    public void OnUpdate(GameManager gm)
    {
    }
    public void EndPlaceState()
    {
        if(GlobalHelper.GetRoom().GetAllies().Count > 0)
        {
            gmRef.ChangeState(new FightState());
        }
        else
        {
            //GlobalHelper.getCamMovement().ShakeCamera(1, 0.5f);
            GlobalHelper.UI().ShakeButtonBottomRightText();
        }
    }
    public void CheckUnitsLeft()
    {
        RoomView r = GlobalHelper.GetRoom();
        int unitsLeft = r.CheckUnitsLeft();
        if (unitsLeft > 0)
        {
            GlobalHelper.UI().SetBottomText("Drag up to " + unitsLeft + " units from the reserve to the highlighted cells"); 

        }
        else
        {
            GlobalHelper.UI().ShakeButtonBottomRightText();
            GlobalHelper.UI().SetBottomText("Press the Fight button to start the fight!");

        }
    }
}


public class FightState : IState
{
    public event Action OnEndTurn = delegate { };
    public UnitData capturedThisFight = null;
    bool canReset = false;
    public void OnEntry(GameManager gm)
    {
        GlobalHelper.UI().SetButtonBottomRightText("End Turn");
        GlobalHelper.UI().SetBottomText("Kill all enemies");
        GlobalHelper.UI().OnChangePhase += onChangePhase;
        GlobalHelper.UI().OnResetTurn += OnResetTurn;
        OnEndTurn += GlobalHelper.UI().OnEndTurn;

        //CheckGameWinState();
        GlobalHelper.GetRoom().OnBoardUpdate += CheckGameWinState;
        GlobalHelper.GetRoom().OnKillUnit += TryCapture;
        GlobalHelper.GlobalVariables.gameInfos.currentTurn = 1;
        foreach (Unit u in GlobalHelper.GetRoom().getAllUnits())
        {
            if (!u.isEnemy)
            {
                u.StartIdle();
                u.RefreshActions();
            }
        }
        canReset = true;

    }
    public void OnResetTurn()
    {
        if (!canReset)
        {
            return;
        }
        GameManager gm = GlobalHelper.GetGameManager();
        if (gm.currentState is FightState || gm.currentState is UnitPlaceState)
        {

            gm.CleanPreviousRoom();
            gm.LoadRoom(gm.currentRoom.roomData).onComplete += () => {

                GlobalHelper.GlobalVariables.player.RestoreBackup();
                gm.ChangeState(new UnitPlaceState());
            };

        }
    }

    public void OnExit(GameManager gm)
    {
        GlobalHelper.UI().OnResetTurn -= OnResetTurn;
        canReset = false;
        Debug.Log("Ending fight place phase");
        RoomView r = GlobalHelper.GetRoom();

        OnEndTurn -= GlobalHelper.UI().OnEndTurn;
        GlobalHelper.UI().OnChangePhase -= onChangePhase;
        GlobalHelper.GetRoom().OnKillUnit -= TryCapture;
        GlobalHelper.GetRoom().OnBoardUpdate -= CheckGameWinState;

        r.CleanUpFight();
    }

    public void OnUpdate(GameManager gm)
    {
    }
    public void onChangePhase()
    {
        canReset = false;
        GlobalHelper.GetRoom().StartCoroutine(EndTurn());
    }
    public IEnumerator EndTurn()
    {
        Debug.Log("Ending turn");
        foreach(Unit u in GlobalHelper.GetRoom().getAllUnits())
        {
            if (u.isEnemy)
            {
                u.RefreshActions();
                yield return u.StartCoroutine(u.EnemyAttack());
            }
            else
            {
                u.StartIdle();
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
                capturedThisFight = u.unitData;
                GlobalHelper.UI().captureManager.CaptureAtPosition(u.GetWorldPosition());
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
            r.StartCoroutine(GoToNextFight());
        }

        if (r.GetAllies().Count == 0)
        {
            GlobalHelper.UI().SetBottomText("GAME OVER. Close the game I didn't implement this.");

        }
    }

    public IEnumerator GoToNextFight()
    {
        RoomView r = GlobalHelper.GetRoom();
        UIManager ui = GlobalHelper.UI();
        Sequence s = DOTween.Sequence().Pause(); 
        //All units go back to their reserve spot 
        foreach(Unit u in r.getAllUnits())
        {
            if (!u.isEnemy)
            {
                //sequence with join and prepend = 0.2f

                s.PrependInterval(0.2f);
                Tween t = u.transform.DOMove(ui.reserve.transform.position, 1f).SetEase(Ease.InBack, 1.5f);
                t.onComplete += () =>
                {
                    //Add unit back
                    GlobalHelper.GlobalVariables.player.AddUnit(u);
                };
                s.Join(t);
            }
        }
        yield return s.Play().WaitForCompletion();
        yield return new WaitForSeconds(0.5f);

        GlobalHelper.GlobalVariables.bloodSplatManager.Cleanup();
        //Make the tilemap disappear
        yield return r.HideTilemap();
        // Start Down Animation, and display the fight recap

        //Change state HERE to NewFightState

        GlobalHelper.GetGameManager().ChangeState(new ChangeRoomState());
    }
}


public class ChangeRoomState : IState
{
    GameManager gmRef;
    public void OnEntry(GameManager gm)
    {
        gmRef = gm;


        gmRef.StartCoroutine(loadGame());

    }

    public void OnExit(GameManager gm)
    {
        Debug.Log("Starting Next fight State");

    }

    public void OnUpdate(GameManager gm)
    {
    }

    public IEnumerator loadGame()
    {
        Debug.Log("Loading next");
        Tween t = gmRef.LoadNextRoom();
        if (t!= null)
        {
            yield return t.WaitForCompletion();
        }
        else
        {
            Debug.Log("Out of rooms !");
            Tween tw = gmRef.LoadNextArea();
            yield return tw.WaitForCompletion();
            //Go to shop state 
            //Go to areaChoiceState
        }

        yield return null;
        gmRef.ChangeState(new UnitPlaceState());
    }
}

