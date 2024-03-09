using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TutorialFightState : IState
{
    public UnitData capturedThisFight = null;
    bool canReset = false;
    public bool enemyTurn = false;
    public GameManager gmRef;

    public int tutorialCount;
    public bool dramaticTutorialDelay = false;
    public void OnEntry(GameManager gm)
    {
        if(tutorialCount == 0)
        {
            GlobalHelper.UI().EnableButton(GlobalHelper.UI().abilityButton.button);
            GlobalHelper.UI().EnableButton(GlobalHelper.UI().endTurnButton);
            gmRef = gm;
            gmRef.extraTurns = 0; //Not sure if that is a good idea  
            GlobalHelper.UI().SetButtonBottomRightText("End Turn");
            GlobalHelper.GetGameManager().OnKillUnit += DramaticDelay;
        }
        else
        {
            GlobalHelper.UI().OnResetTurn += OnResetTurn;

            gmRef.extraTurns = 0; //Not sure if that is a good idea  
            //CheckGameWinState();
            GlobalHelper.GetRoom().OnBoardUpdate += CheckGameWinState;
            gmRef.OnKillUnit += TryCapture;
            GlobalHelper.GlobalVariables.gameInfos.currentTurn = 1;
            GlobalHelper.UI().nextFight.StopAnimate().onComplete += () =>
            {
                GlobalHelper.UI().nextFight.StartAnimate("Player Turn");
            };

        }

        GlobalHelper.UI().OnChangePhase += onChangePhase;
        GlobalHelper.GetRoom().OnBoardUpdate += AutoEndTurn;
        GlobalHelper.GetRoom().OnBoardUpdate += OnbD;
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
    public void DramaticDelay(object o)
    {
        dramaticTutorialDelay = true;
    }
    void OnbD()
    {
        Debug.Log("board update");
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
        RoomView r = GlobalHelper.GetRoom();

        GlobalHelper.UI().OnChangePhase -= onChangePhase;
        gmRef.OnKillUnit -= TryCapture;
        GlobalHelper.GetRoom().OnBoardUpdate -= CheckGameWinState;
        GlobalHelper.GetRoom().OnBoardUpdate -= AutoEndTurn;

        r.CleanUpFight();
        GlobalHelper.UI().DisableButton(GlobalHelper.UI().abilityButton.button);
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
        ui.DisableButton(ui.abilityButton.button);
        ui.DisableButton(ui.endTurnButton);
        if (endFight)
        {
            yield break;
        }
        enemyTurn = true;
        if(tutorialCount > 0)
        {

            yield return GlobalHelper.UI().nextFight.StopAnimate().WaitForCompletion();
            yield return GlobalHelper.UI().nextFight.StartAnimate("Enemy Turn").WaitForCompletion();
        }
        else
        {
            yield return null;
            yield return null;
        }
        if (dramaticTutorialDelay)
        {
            yield return new WaitForSeconds(6);
        }

        if (gmRef.extraTurns > 0)
        {
            gmRef.extraTurns--;
            GlobalHelper.UI().captureManager.DisplayAtPosition(Vector3.zero, "Skipped Enemy turn!");

        }
        else
        {
            List<Unit> enemies = GlobalHelper.GetRoom().GetEnemies();
            for (int i = 0; i < enemies.Count; i++)
            {
                Debug.Log(enemies.Count);
                enemies = GlobalHelper.GetRoom().GetEnemies();
                Unit u = enemies[i];
                u.RefreshActions();

                if(u.unitData.unitName == "King")
                {
                    yield return new WaitForSeconds(0.2f);
                    Vector3 p1 = GlobalHelper.UI().bottomText.transform.position;
                    Tween t = GlobalHelper.getCamMovement().ZoomToPosition(u.GetWorldPosition() + (p1- u.GetWorldPosition()) / 4f, 0.8f, 0.5f);
                    t.onComplete += () =>
                    {
                        GlobalHelper.getCamMovement().ShakeCamera(3f, 1);
                        GlobalHelper.UI().SetBottomText("Don't worry, they will forget you after a while.");
                    };
                    GlobalHelper.TWEEN_OVERSHOOT_MOVE = 20;
                    yield return t.WaitForCompletion();
                    yield return new WaitForSeconds(4);
                }
                yield return u.StartCoroutine(u.EnemyAttack());
                GlobalHelper.TWEEN_OVERSHOOT_MOVE = 4;
            }
            GlobalHelper.getCamMovement().ResetCameraPosition();
            GlobalHelper.getCamMovement().ResetZoomPosition();

        }

        if (tutorialCount > 0)
        {
            yield return GlobalHelper.UI().nextFight.StopAnimate().WaitForCompletion();
        }
        else
        {
            yield return null;
        }
        foreach (Unit u in GlobalHelper.GetRoom().GetAllies())
        {
            u.StartIdle();
            u.RefreshActions();
        }
        GlobalHelper.GlobalVariables.gameInfos.currentTurn++;
        GlobalHelper.UI().OnEndTurn();
        enemyTurn = false;
        gmRef.OnEndTurn();
        canReset = true;
        if(tutorialCount > 0)
        {
            ui.EnableButton(ui.abilityButton.button);
            ui.EnableButton(ui.endTurnButton);

        }

        if (tutorialCount > 0)
        {
            yield return GlobalHelper.UI().nextFight.StartAnimate("Player Turn").WaitForCompletion();
        }


    }
    public void TryCapture(object o)
    {
        if (!(o is Unit))
        {
            return;
        }
        Unit u = (Unit)o;
        if (u.isEnemy)
        {
            if (capturedThisFight == null)
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
        foreach (Unit u in units)
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
        RoomView r = GlobalHelper.GetRoom();
        List<Unit> units = r.getAllUnits();

        GlobalHelper.UI().SetBottomText(r.GetEnemies().Count + GlobalHelper.PluralOrSingular(" enemy", " enemies", r.GetEnemies().Count) + " remaining");
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
            if(tutorialCount == 0)
            {
                //Enable Black screen
                //Change scene to Tut 2 

            }
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
        foreach (Unit u in r.getAllUnits())
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

        int moneyEarned = Mathf.Clamp(gf.currentRoom.roomData.par - gf.currentTurn, 0, 99999) + 1;
        GlobalHelper.GlobalVariables.player.money += moneyEarned;
        GlobalHelper.UI().fightWinUI.OnNextPressed += LoadNext;
        gmRef.OnRoomCleared(GlobalHelper.GetGameManager().currentRoom);
        yield return GlobalHelper.UI().fightWinUI.Show(moneyEarned, capturedThisFight.unitName);

    }
    public void LoadNext()
    {
        GlobalHelper.GetGameManager().ChangeState(new ChangeRoomState());

        GlobalHelper.UI().fightWinUI.OnNextPressed -= LoadNext;
    }
}

