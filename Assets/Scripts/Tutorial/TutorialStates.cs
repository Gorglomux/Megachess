using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TutorialFightState : IState
{
    public UnitData capturedThisFight = null;
    public GameManager gmRef;

    public int tutorialCount;
    public bool dramaticTutorialDelay = false;

    public TutorialFightState(int count)
    {
        tutorialCount = count;
    }
    public void OnEntry(GameManager gm)
    {
        GlobalHelper.UI().HidePauseButton();
        gmRef = gm;
        if (tutorialCount == 0)
        {

            gmRef.extraTurns = 0; //Not sure if that is a good idea  
            GlobalHelper.GetGameManager().OnKillUnit += DramaticDelay;
        }
        else
        {
            GlobalHelper.UI().SetButtonBottomRightText("End Turn");
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

        GlobalHelper.UI().EnableButton(GlobalHelper.UI().endTurnButton);
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
        gmRef.playerTurn = true;
    }
    public void DramaticDelay(object o)
    {
        dramaticTutorialDelay = true;
    }
    void OnbD()
    {
        Debug.Log("board update");
    }
    public void OnExit(GameManager gm)
    {
        GlobalHelper.UI().nextFight.StopAnimate();
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
        if (gmRef.playerTurn)
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
        gmRef.playerTurn = false;
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
                        AudioManager.instance.PlaySound("sfx_drum_lowpitch", 1f, UnityEngine.Random.Range(0.9f, 1f));
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
        gmRef.playerTurn = true;
        gmRef.OnEndTurn();
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
            if (r.roomData.name != "A0R0")
            {
            AudioManager.instance.PlaySound("dialogue", 1f, UnityEngine.Random.Range(0.9f, 1f));

            }
            onChangePhase();
        }
    }


    public bool endFight = false;
    public void CheckGameWinState()
    {
        if (endFight)
        {
            return;
        }
        endFight = false;
        RoomView r = GlobalHelper.GetRoom();
        List<Unit> units = r.getAllUnits();

        //GlobalHelper.UI().SetBottomText(r.GetEnemies().Count + GlobalHelper.PluralOrSingular(" enemy", " enemies", r.GetEnemies().Count) + " remaining");
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
            if(GlobalHelper.GlobalVariables.player.money <= 0)
            {
                GlobalHelper.GlobalVariables.player.money = 1;
            }
            else
            {

                GlobalHelper.getCamMovement().ShakeCamera(2f, 0.5f);
                GlobalHelper.UI().resetFightButton.gameObject.SetActive(true);
                GlobalHelper.UI().SetBottomText("You can reset a fight by pressing the button at the bottom left, at the cost of 1 $.");
            }

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
        GlobalHelper.UI().fightWinUI.OnNextPressed += LoadNext;
        gmRef.OnRoomCleared(GlobalHelper.GetGameManager().currentRoom);
        List<UnitData> cap = new List<UnitData>();
        cap.Add(capturedThisFight);
        yield return GlobalHelper.UI().fightWinUI.Show(-1,-1,moneyEarned, cap);
        GlobalHelper.GlobalVariables.player.ClearInventory();

    }
    public void LoadNext()
    {
        GlobalHelper.GetGameManager().ChangeState(new ChangeRoomState());

        GlobalHelper.UI().fightWinUI.OnNextPressed -= LoadNext;
    }
}


public class TutorialUnitPlaceState : IState
{
    GameManager gmRef;
    public int tutorialCount;
    public TutorialUnitPlaceState(int count)
    {
        tutorialCount = count;
    }
    public void OnEntry(GameManager gm)
    {

        //GlobalHelper.UI().EnableButton(GlobalHelper.UI().endTurnButton);
        gmRef = gm;
        Debug.Log("Starting unit place phase");
        //Select or drag units from the reserve to the spawn cells. 
        GlobalHelper.UI().SetButtonBottomRightText("Fight !");
        //Change bottom button to "Validate placement"

        //GlobalHelper.GlobalVariables.indicatorManager.ShowSpawnableCells();
        GlobalHelper.UI().OnChangePhase += EndPlaceState;
        GlobalHelper.GetRoom().OnBoardUpdate += CheckUnitsLeft;
        //GlobalHelper.GlobalVariables.player.BackupInventory();
        CheckUnitsLeft();
        GlobalHelper.UI().ShowTopInfos();
        GlobalHelper.UI().nextFight.StartAnimate("Selection Phase");
        gm.OnStartFight(null);
    }

    public void OnExit(GameManager gm)
    {

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
        if (GlobalHelper.GetRoom().GetAllies().Count > 0)
        {
            gmRef.ChangeState(new TutorialFightState(tutorialCount));
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
            //GlobalHelper.UI().SetBottomText("Drag up to " + unitsLeft + GlobalHelper.PluralOrSingular(" unit", " units", unitsLeft) + " from the reserve to the highlighted cells");

        }
        else
        {
            //GlobalHelper.UI().ShakeButtonBottomRightText();
           // GlobalHelper.UI().SetBottomText("Press the Fight button to start the fight!");

        }
    }
}