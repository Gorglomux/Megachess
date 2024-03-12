using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Transform root;
    public Transform rootTitle;

    public event Action OnChangePhase = delegate { };
    public event Action OnResetTurnActivated = delegate { };

    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI bottomText;

    [Header("Top Part")]
    public TextMeshProUGUI RoomName;
    public TextMeshProUGUI AreaName;
    public TextMeshProUGUI ParCount;
    public TextMeshProUGUI TurnCount;
    public TextMeshProUGUI RoomCount;
    public TextMeshProUGUI MoneyText;


    private GameInfos _gameInfos;
    public GameInfos gameInfosRef { get { if (_gameInfos == null) { _gameInfos = GlobalHelper.GlobalVariables.gameInfos; } return _gameInfos; } set { } }
    public Material mainTextMaterial;

    public Reserve reserve;
    public CaptureTextManager captureManager;

    public NextFight nextFight;
    public FightWinUI fightWinUI;

    public PreviewMove previewMove;

    public GameObject rootTopInfos;
    public EffectContainerManager effectContainerManager;

    public Button endTurnButton;
    public AbilityButton abilityButton;
    public Button resetFightButton;

    public PassiveShop passiveShop;
    public PassivesMenu passivesMenu;
    public RectTransform ReserveRectTransform;

    public Button pauseButton;
    public PauseManager pauseManager;
    // Start is called before the first frame update
    void Start()
    {
        gameInfosRef = GlobalHelper.GlobalVariables.gameInfos;
        HideTopInfos();
        HideHoverInfos();
        HideShop();
        HidePassivesMenu();
        HidePauseMenu();
        //HideTitleScreen();
        DisableButton(abilityButton.button);
        SetBottomText("");



        DOTween.timeScale = PlayerPrefs.GetFloat("AnimationSpeed", 1);
        GlobalHelper.ScreenShakeMultiplier = PlayerPrefs.GetFloat("ScreenShake", 1);
    }
    public void HidePassivesMenu()
    {
        passivesMenu.gameObject.SetActive(false);

    }
    public void ShowPassivesMenu()
    {
        passivesMenu.gameObject.SetActive(true);
    }
    public void AddPassiveItem(PassiveData data)
    {
        ShowPassivesMenu();
        passivesMenu.AddPassive(data);
    }

    public void ShowPauseMenu()
    {
        pauseManager.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void HidePauseMenu()
    {
        pauseManager.gameObject.SetActive(false);
        Time.timeScale = 1;

    }

    // Update is called once per frame
    void Update()
    {

    }

    string originalBottomText;
    public void SetBottomText(string text, float duration = -1, bool playSound = false)
    {
        if(duration == -1)
        {
            originalBottomText = text;
            //Typewrite here
            bottomText.text = text;
        }
        if(BottomTextCoroutine == null)
        {
            originalBottomText = text;
            BottomTextCoroutine = StartCoroutine(corBottomTextCoroutine(text, duration, playSound));
        }
        else
        {
            StopCoroutine(BottomTextCoroutine);
            BottomTextCoroutine = StartCoroutine(corBottomTextCoroutine(text, duration, playSound));
        }

    }
    Coroutine BottomTextCoroutine = null;
    IEnumerator corBottomTextCoroutine(string text, float duration, bool playSound = false)
    {
        if(text != "" && playSound)
        {
            AudioManager.instance.PlaySound("dialogue", 1f, UnityEngine.Random.Range(0.9f, 1f));

        }
        bottomText.text = text;
        yield return new WaitForSeconds(duration);
        bottomText.text = originalBottomText;
    
    }

    public void SetButtonBottomRightText(string text)
    {
        //Typewrite here
        buttonText.text = text;
    }

    public void OnButtonBottomRightClicked()
    {
        AudioManager.instance.PlaySound("sfx_drum_lowpitch", 1f, UnityEngine.Random.Range(0.9f, 1f));
        OnChangePhase();
    }
    public void HideTopInfos()
    {
        rootTopInfos.gameObject.SetActive(false);
    }
    public void ShowTopInfos()
    {
        rootTopInfos.gameObject.SetActive(true);

    }
    public void OnRoomChange()
    {
        RoomView r = gameInfosRef.currentRoom;
        RoomName.text = r.roomData.roomName;
        ParCount.text = "Par " + r.parAfterEffects.ToString();
        TurnCount.text = "Turn 1";
        //RoomCount = String.Format("Room {0} / {1}",gameInfosRef.currentArea. TODO 
    }

    public void OnAreaChange()
    {
        Area a = gameInfosRef.currentArea;
        //THIS IS WHERE WE CHANGE THE TEXT COLOR
        AreaName.text = a.areaName;
        mainTextMaterial.SetColor("_FaceColor", a.colorText);
    }
    public void OnEndTurn()
    {
        TurnCount.text = "Turn " + gameInfosRef.currentTurn;
    }
    Tween bottomShakeTween;
    public void ShakeButtonBottomRightText()
    {
        if (bottomShakeTween != null)
        {
            bottomShakeTween.Kill();
        }
        bottomShakeTween = bottomText.transform.DOShakePosition(0.8f, new Vector3(10f, 0, 0), 8).SetEase(Ease.OutBounce);

        //bottomText.transform.DOPunchScale(bottomText.transform.localScale *1.22f, 1).SetEase(Ease.OutBounce);

    }
    public void OnResetButtonClicked()
    {

        AudioManager.instance.PlaySound("sfx_tap", 1.2f, UnityEngine.Random.Range(0.8f, 0.9f));
        OnResetTurn();


    }
    public void OnUIButtonHovered()
    {
        AudioManager.instance.PlaySound("sfx_tap", 1.2f, UnityEngine.Random.Range(1.1f, 1f));

    }
    public void UpdateRoomCount(int currentCount, int maxCount)
    {
        RoomCount.text = "Room " + currentCount.ToString() + " / " + maxCount.ToString();
    }
    public void UpdateMoneyCount()
    {
        MoneyText.text = "$" + GlobalHelper.GlobalVariables.player.money.ToString();
    }
    /// <summary>
    /// This method is called when hovering a unit in the reserve, shop OR in the grid.
    /// </summary>
    /// <param name="u"></param>
    public void ShowHoverInfos(object hoverable)
    {
        HideHoverInfos();
        UnitNameText.gameObject.SetActive(true);
        if (hoverable is PassiveData)
        {
            PassiveData p = (PassiveData)hoverable;
            UnitNameText.text = p.passiveName;
            EffectContainer ec = effectContainerManager.getNext();
            ec.FillInfos(p);
        }
        if (hoverable is Unit)
        {
            Unit u = (Unit)hoverable;
            previewMove.ShowPreview(u);
            SetUnitName(u.unitData.unitName, u.megaSize);
            foreach(BaseEffect e in u.currentEffects)
            {
                EffectContainer ec = effectContainerManager.getNext();
                ec.FillInfos(e);
            }

        }if(hoverable is BaseAbility)
        {
            BaseAbility ability= (BaseAbility)hoverable;

            UnitNameText.text = ability.abilityData.abilityName;
            EffectContainer ec = effectContainerManager.getNext();
            ec.FillInfos(ability);
            EffectContainer ecCooldown = effectContainerManager.getNext();
            ecCooldown.FillCooldown(ability);
        }
        if(hoverable is PlayerContainer)
        {
            PlayerContainer playerContainer = (PlayerContainer) hoverable;
            UnitNameText.text = playerContainer.playerData.playerName.ToUpper();
            EffectContainer ec = effectContainerManager.getNext();
            ec.FillInfos(playerContainer.playerData);
            EffectContainer ecAbility = effectContainerManager.getNext();
            ecAbility.FillAbility(GlobalHelper.abilityLookup(playerContainer.playerData.startingAbilityData), true);
        }
        if(hoverable is string)
        {
            string s = (string)hoverable;
            if(s == "reset")
            {
                Debug.Log("Showing hover infos");
                EffectContainer ec = effectContainerManager.getNext();
                ec.FillReset();
                UnitNameText.text = "RESET";
            }

        }

        //else if is ability, else if is shopitem...
    }

    public void HideHoverInfos()
    {
        previewMove.HidePreview();
        effectContainerManager.DisableContainers();
        UnitNameText.gameObject.SetActive(false);
    }
    public TextMeshProUGUI UnitNameText;
    public void SetUnitName(string name, int size)
    {
        UnitNameText.text = (GetMegaPrefix(size) + " " + name).ToUpper();
    }
    public string GetMegaPrefix(int value)
    {
        string output = "";
        switch (value)
        {
            case 2:
                output = "Mega";
                break;
            case 3:
                output = "Giga";
                break;
            case 4:
                output = "Ultra";
                break;
            case 5:
                output = "Macro?";
                break;
            case 6:
                output = "Omega?!";
                break;
            case 7:
                output = "Supra";
                break;

        }
        return output;
    }

    public void DisableButton(Button b)
    {
        if(b == null)
        {
            return;
        }
        b.enabled = false;
        b.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

    }

    public void EnableButton(Button b)
    {
        b.enabled = true;
        b.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public bool isResetting = false;
    public void OnResetTurn()
    {
        if (!GlobalHelper.CheckUnitFightState())
        {
            GlobalHelper.UI().ShakeButtonBottomRightText(); 
            SetBottomText("Can only reset turn while in a fight.");
        }
        if (!GlobalHelper.GlobalVariables.player.CanBuy(GlobalHelper.GetResetCost()))
        {
            GlobalHelper.UI().ShakeButtonBottomRightText();
            SetBottomText("Not enough money !");
        }
        if (!canReset())
        {
            return;
        }
        
        if(GlobalHelper.GetGameState() is FightState /*|| GlobalHelper.GetGameState() is UnitPlaceState*/)
        {
            GlobalHelper.GlobalVariables.player.Buy(GlobalHelper.GetResetCost());
            GlobalHelper.GetGameManager().currentResetCost++;

        }
        GlobalHelper.UI().HideHoverInfos();
        GlobalHelper.GlobalVariables.player.ClearInventory();
        isResetting = true;
        GameManager gm = GlobalHelper.GetGameManager();
        OnResetTurnActivated();
        GlobalHelper.GlobalVariables.bloodSplatManager.Cleanup();
        gm.CleanPreviousRoom();
        gm.ChangeState(null);
        GlobalHelper.getCamMovement().ResetCameraPosition(true);
        GlobalHelper.getCamMovement().ResetZoomPosition();
        GlobalHelper.GetGameManager().isActionInProgress = false;
        gm.LoadRoom(gm.currentRoom.roomData).onComplete += () => {

            if (gm.currentRoom.roomData.isTutorial)
            {
                gm.ChangeState(new TutorialUnitPlaceState(int.Parse(gm.currentRoom.roomData.name.Substring(3))));

            }
            else
            {
                print("Restoring backup !");
                GlobalHelper.GlobalVariables.player.RestoreBackup();
                gm.ChangeState(new UnitPlaceState());

            }

            isResetting = false;
        };
    }
    public bool canReset()
    {
        bool isCorrectState = GlobalHelper.CheckUnitFightState();
        bool canBuy = GlobalHelper.GlobalVariables.player.CanBuy(GlobalHelper.GetResetCost());
        return isCorrectState && !isResetting && canBuy;
    }


    
    public void LoadTitleScreen()
    {
        HideRoot();
        rootTitle.gameObject.SetActive(true);
        ShowPauseButton();
    }
    public void ShowPauseButton()
    {
        pauseButton.gameObject.SetActive(true);
    }
    public void HidePauseButton()
    {
        pauseButton.gameObject.SetActive(false);

    }
    public void HideRoot()
    {
        root.gameObject.SetActive(false);
    }
    public void ShowRoot()
    {
        root.gameObject.SetActive(true);
    }
    public void HideTitleScreen()
    {
        rootTitle.gameObject.SetActive(false);
    }

    public GameObject blackScreen;
    public void ShowBlackScreen()
    {
        blackScreen.gameObject.SetActive(true);
    }

    public void HideBlackScreen()
    {
        blackScreen.gameObject.SetActive(false);
    }


    public void LoadShop()
    {
        passiveShop.gameObject.SetActive(true);
        passiveShop.FillPassives();
    }

    public void HideShop()
    {
        passiveShop.gameObject.SetActive(false);

    }

    public void onHoverEnterResetButton()
    {
        //Check if unit place and send something else than reset 
        ShowHoverInfos("reset");
    }
    public void onHoverExitResetButton()
    {
        HideHoverInfos();
    }
}
