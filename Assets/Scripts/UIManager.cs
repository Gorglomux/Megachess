using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public event Action OnChangePhase = delegate { };

    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI bottomText;

    [Header("Top Part")]
    public TextMeshProUGUI RoomName;
    public TextMeshProUGUI AreaName;
    public TextMeshProUGUI ParCount;
    public TextMeshProUGUI TurnCount;
    public TextMeshProUGUI RoomCount;


    private GameInfos _gameInfos;
    public GameInfos gameInfosRef { get { if (_gameInfos == null) { _gameInfos = GlobalHelper.GlobalVariables.gameInfos; } return _gameInfos; } set{ } } 
    public Material mainTextMaterial;

    public Reserve reserve;
    public CaptureTextManager captureManager;
    // Start is called before the first frame update
    void Start()
    {
        gameInfosRef = GlobalHelper.GlobalVariables.gameInfos; 
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
        if(bottomShakeTween != null)
        {
            bottomShakeTween.Kill();
        }
        bottomShakeTween = bottomText.transform.DOShakePosition(0.8f, new Vector3(10f, 0, 0), 8).SetEase(Ease.OutBounce);
        
        //bottomText.transform.DOPunchScale(bottomText.transform.localScale *1.22f, 1).SetEase(Ease.OutBounce);

    }
}
