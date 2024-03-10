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
    public IEnumerator Show(int par, int turn, int moneyEarned, List<UnitData> CapturedThisFight)
    {
        gameObject.SetActive(true);
        float delay = 0.5f;
        fightWonText.gameObject.SetActive(true);
        GlobalHelper.getCamMovement().ShakeCamera(0.5f, delay);
        AudioManager.instance.PlaySound("dialogue", 1.2f, UnityEngine.Random.Range(0.8f, 0.9f));

        yield return new WaitForSeconds(delay);
        GlobalHelper.getCamMovement().ShakeCamera(0.5f, delay);
        moneyEarnedText.gameObject.SetActive(true);
        AudioManager.instance.PlaySound("dialogue", 1.2f, UnityEngine.Random.Range(0.8f, 0.9f));

        moneyEarnedText.text = "$ Earned : "  + moneyEarned;
        GlobalHelper.GlobalVariables.player.money += moneyEarned;
        yield return new WaitForSeconds(delay);

        GlobalHelper.getCamMovement().ShakeCamera(0.5f, delay);
        unitCapturedText.gameObject.SetActive(true);
        AudioManager.instance.PlaySound("dialogue", 1.2f, UnityEngine.Random.Range(0.8f, 0.9f));


        string captured = "  ";
        foreach(UnitData ud in CapturedThisFight)
        {
            captured += ud.unitName + ", ";
        }
        captured = captured.Substring(0, captured.Length - 2);
        unitCapturedText.text = "Unit Captured : " + captured;
        yield return new WaitForSeconds(delay*2);
        AudioManager.instance.PlaySound("dialogue", 1.2f, UnityEngine.Random.Range(0.8f, 0.9f));
        GlobalHelper.getCamMovement().ShakeCamera(0.5f, delay);
        nextButton.gameObject.SetActive(true);

    }
    public void Hide()
    {
        StartCoroutine(corHide());
    }
    public IEnumerator corHide()
    {
        AudioManager.instance.PlaySound("dialogue", 1.2f, UnityEngine.Random.Range(0.8f, 0.9f));

        GlobalHelper.getCamMovement().ShakeCamera(2, 1);
        HideUI();
        yield return new WaitForSeconds(0.5f);
        OnNextPressed();

        gameObject.SetActive(false);
    }


}
