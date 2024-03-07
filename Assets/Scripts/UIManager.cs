using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public event Action OnChangePhase = delegate { };
    public event Action OnResetTurn = delegate { };

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
    // Start is called before the first frame update
    void Start()
    {
        gameInfosRef = GlobalHelper.GlobalVariables.gameInfos;
        HideTopInfos();
        HideHoverInfos();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBottomText(string text)
    {
        //Typewrite here
        bottomText.text = text;
    }

    public void SetButtonBottomRightText(string text)
    {
        //Typewrite here
        buttonText.text = text;
    }

    public void OnButtonBottomRightClicked()
    {
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
        ParCount.text = "Par " + r.roomData.par.ToString();
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
        OnResetTurn();


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
        UnitNameText.gameObject.SetActive(true);
        if (hoverable is Unit)
        {
            Unit u = (Unit)hoverable;
            previewMove.ShowPreview(u);
            SetUnitName(u.unitData.unitName, u.megaSize);

        }
        //else if is ability, else if is shopitem...
    }

    public void HideHoverInfos()
    {
        previewMove.HidePreview();
        //EffectsContainer.Hide();
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
}
