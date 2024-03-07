using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightWinUI : MonoBehaviour
{
    public TextMeshProUGUI fightWonText;
    public TextMeshProUGUI moneyEarnedText;
    public TextMeshProUGUI unitCapturedText;
    public Button nextButton;

    public Action OnNextPressed = delegate { };
    // Start is called before the first frame update
    void Start()
    {
        HideUI();
        gameObject.SetActive(false);
    }
    void HideUI()
    {
        fightWonText.gameObject.SetActive(false);
        moneyEarnedText.gameObject.SetActive(false);
        unitCapturedText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
    }
    public IEnumerator Show(int moneyEarned, string CapturedThisFight)
    {
        gameObject.SetActive(true);
        float delay = 0.5f;
        fightWonText.gameObject.SetActive(true);
        GlobalHelper.getCamMovement().ShakeCamera(0.5f, delay);
        yield return new WaitForSeconds(delay);
        GlobalHelper.getCamMovement().ShakeCamera(0.5f, delay);
        moneyEarnedText.gameObject.SetActive(true);
        moneyEarnedText.text = "$ Earned : "  + moneyEarned;
        yield return new WaitForSeconds(delay);

        GlobalHelper.getCamMovement().ShakeCamera(0.5f, delay);
        unitCapturedText.gameObject.SetActive(true);
        unitCapturedText.text = "Unit Captured : " + CapturedThisFight;
        yield return new WaitForSeconds(delay*2);
        GlobalHelper.getCamMovement().ShakeCamera(0.5f, delay);
        nextButton.gameObject.SetActive(true);

    }
    public void Hide()
    {
        StartCoroutine(corHide());
    }
    public IEnumerator corHide()
    {
        GlobalHelper.getCamMovement().ShakeCamera(2, 1);
        HideUI();
        yield return new WaitForSeconds(0.5f);
        OnNextPressed();

        gameObject.SetActive(false);
    }


}
