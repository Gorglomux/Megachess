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

        GlobalHelper.UI().EnableButton(GlobalHelper.UI().endTurnButton);
        gmRef = gm;
        Debug.Log("Starting unit place phase");
        //Select or drag units from the reserve to the spawn cells. 
        GlobalHelper.UI().SetButtonBottomRightText("Fight !");
        //Change bottom button to "Validate placement"

        GlobalHelper.GlobalVariables.indicatorManager.ShowSpawnableCells();
        GlobalHelper.UI().OnChangePhase += EndPlaceState;
        GlobalHelper.UI().OnResetTurn += OnResetTurn;
        GlobalHelper.GetRoom().OnBoardUpdate += CheckUnitsLeft;
        GlobalHelper.GlobalVariables.player.BackupInventory();
        canReset = true;
        CheckUnitsLeft();
        GlobalHelper.UI().ShowTopInfos();
        GlobalHelper.UI().nextFight.StartAnimate("Selection Phase");
    }

    public void OnResetTurn()
    {
        if (!canReset)
        {
            return;
        }
        canReset = false;
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
            GlobalHelper.UI().SetBottomText("Drag up to " + unitsLeft + GlobalHelper.PluralOrSingular(" unit"," units",unitsLeft) +" from the reserve to the highlighted cells"); 

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
    public UnitData capturedThisFight = null;
    bool canReset = false;
    public bool enemyTurn = false;
    public GameManager gmRef;
    public void OnEntry(GameManager gm)
    {
        GlobalHelper.UI().EnableButton(GlobalHelper.UI().abilityButton);
        GlobalHelper.UI().EnableButton(GlobalHelper.UI().endTurnButton);
        gmRef = gm;
        gmRef.extraTurns = 0; //Not sure if that is a good idea  
        GlobalHelper.UI().SetButtonBottomRightText("End Turn");
        GlobalHelper.UI().SetBottomText("Kill all enemies");
        GlobalHelper.UI().OnChangePhase += onChangePhase;
        GlobalHelper.UI().OnResetTurn += OnResetTurn;

        //CheckGameWinState();
        GlobalHelper.GetRoom().OnBoardUpdate += CheckGameWinState;
        GlobalHelper.GetRoom().OnBoardUpdate += AutoEndTurn;
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
        GlobalHelper.UI().nextFight.StopAnimate().onComplete += () =>
        {
            GlobalHelper.UI().nextFight.StartAnimate("Player Turn");
        };

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
        GlobalHelper.UI().nextFight.StopAnimate();
        GlobalHelper.UI().OnResetTurn -= OnResetTurn;
        canReset = false;
        Debug.Log("Ending fight place phase");
        RoomView r = GlobalHelper.GetRoom();

        GlobalHelper.UI().OnChangePhase -= onChangePhase;
        GlobalHelper.GetRoom().OnKillUnit -= TryCapture;
        GlobalHelper.GetRoom().OnBoardUpdate -= CheckGameWinState;
        GlobalHelper.GetRoom().OnBoardUpdate -= AutoEndTurn;

        r.CleanUpFight();
        GlobalHelper.UI().DisableButton(GlobalHelper.UI().abilityButton);
    }

    public void OnUpdate(GameManager gm)
    {
    }
    public void onChangePhase()
    {
        canReset = false;
        if (!enemyTurn)
        {
            GlobalHelper.GetRoom().StartCoroutine(EndTurn());

        }
    }
    public IEnumerator EndTurn()
    {
        UIManager ui = GlobalHelper.UI();
        ui.DisableButton(ui.abilityButton);
        ui.DisableButton(ui.endTurnButton);
        if (endFight)
        {
            yield break;
        }
        enemyTurn = true;
        yield return GlobalHelper.UI().nextFight.StopAnimate().WaitForCompletion();
        yield return GlobalHelper.UI().nextFight.StartAnimate("Enemy Turn").WaitForCompletion();

        Debug.Log("Ending turn");
        if(gmRef.extraTurns > 0)
        {
            gmRef.extraTurns--;
            GlobalHelper.UI().captureManager.DisplayAtPosition(Vector3.zero, "Skipped Enemy turn!");

        }
        else
        {
            foreach (Unit u in GlobalHelper.GetRoom().GetEnemies())
            {
                u.RefreshActions();
                yield return u.StartCoroutine(u.EnemyAttack());
            }
        }


        //yield return GlobalHelper.UI().nextFight.AnimateText("Player").WaitForCompletion();

        yield return GlobalHelper.UI().nextFight.StopAnimate().WaitForCompletion();
        foreach (Unit u in GlobalHelper.GetRoom().GetAllies())
        {
            u.StartIdle();
            u.RefreshActions();
        }
        yield return GlobalHelper.UI().nextFight.StartAnimate("Player Turn").WaitForCompletion();

        GlobalHelper.GlobalVariables.gameInfos.currentTurn++;
        GlobalHelper.UI().OnEndTurn();
        enemyTurn = false;
        gmRef.OnEndTurn();
        canReset = true;
        ui.EnableButton(ui.abilityButton);
        ui.EnableButton(ui.endTurnButton);
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
    public void AutoEndTurn()
    {
        Debug.Log("Checking auto end turn");
        RoomView r = GlobalHelper.GetRoom();
        List<Unit> units = r.GetAllies();
        bool endTurn = true;
        foreach(Unit u in units)
        {
            if (u.actionsLeft > 0)
            {
                endTurn = false;
            }
        }
        if (endTurn)
        {
            onChangePhase();
        }
    }


    public bool endFight = false;
    public void CheckGameWinState()
    {
        endFight = false;
        Debug.Log("Checking win state");
        RoomView r = GlobalHelper.GetRoom();
        List<Unit> units = r.getAllUnits();

        GlobalHelper.UI().SetBottomText(r.GetEnemies().Count +GlobalHelper.PluralOrSingular(" enemy", " enemies", r.GetEnemies().Count) + " remaining");
        if (r.GetEnemies().Count == 0)
        {
            GlobalHelper.UI().nextFight.StopAnimate();
            GlobalHelper.UI().SetBottomText("Fight won, going to the next fight !");
            r.StartCoroutine(GoToNextFight());
            endFight = true;
        }

        if (r.GetAllies().Count == 0)
        {
            GlobalHelper.UI().nextFight.StopAnimate();
            GlobalHelper.UI().SetBottomText("GAME OVER. Close the game I didn't implement this.");

            endFight = true;
        }
    }

    public IEnumerator GoToNextFight()
    {
        RoomView r = GlobalHelper.GetRoom();
        UIManager ui = GlobalHelper.UI();
        ui.HideTopInfos();
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
        GameInfos gf = GlobalHelper.GlobalVariables.gameInfos;

        int moneyEarned =Mathf.Clamp(gf.currentRoom.roomData.par - gf.currentTurn,0,99999)+1;
        GlobalHelper.GlobalVariables.player.money += moneyEarned;
        GlobalHelper.UI().fightWinUI.OnNextPressed += LoadNext;
        gmRef.OnRoomCleared();
        yield return GlobalHelper.UI().fightWinUI.Show(moneyEarned, capturedThisFight.unitName);
        
    }
    public void LoadNext()
    {
        GlobalHelper.GetGameManager().ChangeState(new ChangeRoomState());

        GlobalHelper.UI().fightWinUI.OnNextPressed -= LoadNext;
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
            GlobalHelper.UI().UpdateRoomCount(gmRef.roomIndex+1 ,GlobalHelper.GlobalVariables.gameInfos.areaSize);
        }
        else
        {
            gmRef.OnAreaCleared();
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

