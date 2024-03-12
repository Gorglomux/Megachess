using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        AudioManager.instance.SetBeforeFight();
        AudioManager.instance.SetInFight();
        GlobalHelper.UI().EnableButton(GlobalHelper.UI().endTurnButton);
        gmRef = gm;
        Debug.Log("Starting unit place phase");
        //Select or drag units from the reserve to the spawn cells. 
        GlobalHelper.UI().SetButtonBottomRightText("Fight !");
        //Change bottom button to "Validate placement"

        GlobalHelper.GlobalVariables.indicatorManager.ShowSpawnableCells();
        GlobalHelper.UI().OnChangePhase += EndPlaceState;
        GlobalHelper.GetRoom().OnBoardUpdate += CheckUnitsLeft;
        CheckUnitsLeft();
        GlobalHelper.UI().ShowTopInfos();
        GlobalHelper.UI().nextFight.StartAnimate("Selection Phase");
        gm.OnStartFight(null);

    }


    public void OnExit(GameManager gm)
    {
        Debug.Log("Ending unit place phase");

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
    public List<UnitData> capturedThisFight = new List<UnitData>();
    public GameManager gmRef;
    public bool endGame = false;
    public void OnEntry(GameManager gm)
    {
        GlobalHelper.UI().EnableButton(GlobalHelper.UI().abilityButton.button);
        GlobalHelper.UI().EnableButton(GlobalHelper.UI().endTurnButton);
        gmRef = gm;
        gmRef.extraTurns = 0; //Not sure if that is a good idea  
        GlobalHelper.UI().SetButtonBottomRightText("End Turn");
        GlobalHelper.UI().SetBottomText("Kill all enemies");
        GlobalHelper.UI().OnChangePhase += onChangePhase;
        //CheckGameWinState();
        GlobalHelper.GetRoom().OnBoardUpdate += CheckGameWinState;
        GlobalHelper.GetRoom().OnBoardUpdate += AutoEndTurn;
        gmRef.OnKillUnit += TryCapture;
        GlobalHelper.GlobalVariables.gameInfos.currentTurn = 1;
        foreach (Unit u in GlobalHelper.GetRoom().getAllUnits())
        {
            if (!u.isEnemy)
            {
                u.StartIdle();
                u.RefreshActions();
            }
        }
        GlobalHelper.UI().nextFight.StopAnimate().onComplete += () =>
        {
            GlobalHelper.UI().nextFight.StartAnimate("Player Turn");
        };
        gmRef.playerTurn = true;

        gmRef.OnStartTurn(null);
    }
    public void OnExit(GameManager gm)
    {
        GlobalHelper.UI().nextFight.StopAnimate();
        Debug.Log("Ending fight place phase");
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
            if (endGame)
            {
                Debug.LogError("Should show the end of game screen");
                gmRef.BackToTitle();
            }
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
        yield return GlobalHelper.UI().nextFight.StopAnimate().WaitForCompletion();
        yield return GlobalHelper.UI().nextFight.StartAnimate("Enemy Turn").WaitForCompletion();

        gmRef.OnEndTurn();
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
                if(u != null)
                {
                    u.RefreshActions();

                    yield return u.StartCoroutine(u.EnemyAttack());

                }
            }
            GlobalHelper.getCamMovement().ResetCameraPosition();
            GlobalHelper.getCamMovement().ResetZoomPosition();

        }


        //yield return GlobalHelper.UI().nextFight.AnimateText("Player").WaitForCompletion();

        yield return GlobalHelper.UI().nextFight.StopAnimate().WaitForCompletion();
        foreach (Unit u in GlobalHelper.GetRoom().GetAllies())
        {
            u.StartIdle();
            u.RefreshActions();
        }
        gmRef.OnStartTurn(null);
        GlobalHelper.GlobalVariables.gameInfos.currentTurn++;
        GlobalHelper.UI().OnEndTurn();
        gmRef.playerTurn = true;
        ui.EnableButton(ui.abilityButton.button);
        ui.EnableButton(ui.endTurnButton);

        yield return GlobalHelper.UI().nextFight.StartAnimate("Player Turn").WaitForCompletion();


    }
    public void TryCapture(object o)
    {
        if(!(o is Unit))
        {
            return;
        }
        Unit u = (Unit)o;
        if (u.isEnemy)
        {
            if(capturedThisFight.Count < gmRef.canCaptureThisFight)
            {
                capturedThisFight.Add(u.unitData);
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

        if (endFight)
        {
            return;
        }
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
            AudioManager.instance.SetInElevator();
            GlobalHelper.UI().nextFight.StopAnimate();
            GlobalHelper.UI().SetBottomText("You lost the fight. \nPress the Reset Fight button, or the Game Over Button if you want to end it all.");
            GlobalHelper.UI().SetButtonBottomRightText("GAME OVER");
            endGame = true;
        }
    }

    public IEnumerator GoToNextFight()
    {
        AudioManager.instance.PlaySound("sfx_drum_win", 1.2f, 1);
        if (gmRef.roomQueue.Count <=  0)
        {

            AudioManager.instance.PlayMainMusic();
        }
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
                    AudioManager.instance.PlaySound("sfx_chess_move", 1, UnityEngine.Random.Range(1.05f, 1.1f));
                };
                s.Join(t);
            }
        }


        yield return s.Play().WaitForCompletion();
        yield return new WaitForSeconds(0.5f);

        GlobalHelper.GlobalVariables.bloodSplatManager.Cleanup();
        //Make the tilemap disappear
        AudioManager.instance.SetInElevator();
        yield return r.HideTilemap();
        // Start Down Animation, and display the fight recap
        GameInfos gf = GlobalHelper.GlobalVariables.gameInfos;

        int moneyEarned =Mathf.Clamp(gf.currentRoom.parAfterEffects - gf.currentTurn+1,0,99999);
        GlobalHelper.UI().fightWinUI.OnNextPressed += LoadNext;
        gmRef.OnRoomCleared(GlobalHelper.GetGameManager().currentRoom);
        yield return GlobalHelper.UI().fightWinUI.Show(gf.currentRoom.parAfterEffects, GlobalHelper.GlobalVariables.gameInfos.currentTurn, moneyEarned, capturedThisFight);
        
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
        GlobalHelper.UI().ShowRoot();
        GlobalHelper.UI().HideTitleScreen();

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
        RoomView previousRoom = GlobalHelper.GetRoom();
        bool previousRoomIsTutorial = previousRoom != null && previousRoom.roomData.isTutorial;

        if (gmRef.shouldGetBackToTitle)
        {
            GlobalHelper.getCamMovement().ShakeCamera(4f, 0.8f);
            GlobalHelper.UI().ShowBlackScreen();
            yield return new WaitForSeconds(2);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            yield break;
        }
        Debug.Log("Loading next");
        Tween t = gmRef.LoadNextRoom();
        if (t!= null)
        {
            yield return t.WaitForCompletion();

            GlobalHelper.UI().UpdateRoomCount(gmRef.roomIndex+1 ,GlobalHelper.GlobalVariables.gameInfos.areaSize);
        }
        else
        {
            if (previousRoomIsTutorial)
            {
                if (gmRef.shouldGetBackToTitle || previousRoomIsTutorial)
                {
                    GlobalHelper.getCamMovement().ShakeCamera(4f, 0.8f);
                    GlobalHelper.UI().ShowBlackScreen();
                    yield return new WaitForSeconds(2);

                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);

                    yield break;
                }
            }
            else
            {
                if (gmRef.wasInShop || !gmRef.firstAreaLoaded)
                {
                    gmRef.wasInShop = false;
                    gmRef.OnAreaCleared(GlobalHelper.GlobalVariables.gameInfos.currentArea);
                    gmRef.areaBeaten++;
                    Debug.Log("Out of rooms !");
                    Tween tw = gmRef.LoadNextArea();
                    yield return tw.WaitForCompletion();
                }
                else
                {
                    gmRef.wasInShop = true;
                    gmRef.ChangeState(new ShopState());
                    yield break;
                }


            }
            //Go to shop state 
            //Go to areaChoiceState
        }

        yield return null;
        if (GlobalHelper.GetRoom().roomData.isTutorial )
        {
            RoomView r = GlobalHelper.GetRoom();
            if (r.roomData.name == "A0R0")
            {
                gmRef.ChangeState(new TutorialFightState(0));
            }
            else
            {
                string number = r.roomData.name.Substring(3);
                gmRef.ChangeState(new TutorialUnitPlaceState(int.Parse(number)));
            }

        }
        else
        {
            gmRef.ChangeState(new UnitPlaceState());
        }
    }
}


public class TitleScreenState : IState
{
    GameManager gmRef;
    public void OnEntry(GameManager gm)
    {
        gmRef = gm;

        GlobalHelper.UI().LoadTitleScreen();

        gm.paletteCoroutine = gm.StartCoroutine(gm.LoadPalette(UnityEngine.Random.Range(0, 15),Color.white));

    }

    public void OnExit(GameManager gm)
    {
        Debug.Log("Starting Change Room State");

    }

    public void OnUpdate(GameManager gm)
    {
    }

}


public class ShopState : IState
{
    GameManager gmRef;
    public void OnEntry(GameManager gm)
    {
        gmRef = gm;
        AudioManager.instance.SetInFight();
        GlobalHelper.UI().LoadShop();
        GlobalHelper.UI().AreaName.text = "The Shop";

    }

    public void OnExit(GameManager gm)
    {
        Debug.Log("Starting Change Room State");
        GlobalHelper.UI().HideShop();
        AudioManager.instance.SetBeforeFight();
        GlobalHelper.UI().EnableButton(GlobalHelper.UI().endTurnButton);
        GlobalHelper.UI().EnableButton(GlobalHelper.UI().abilityButton.button);
        GlobalHelper.UI().EnableButton(GlobalHelper.UI().resetFightButton);
    }

    public void OnUpdate(GameManager gm)
    {
    }

}

