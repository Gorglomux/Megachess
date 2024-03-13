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

    public TextMeshProUGUI fastTurnBonusText;
    public TextMeshProUGUI noteText;

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
        fastTurnBonusText.gameObject.SetActive(false);
        noteText.gameObject.SetActive(false);
    }
    public IEnumerator Show(int par, int turn, int moneyEarned, List<UnitData> CapturedThisFight)
    {
        gameObject.SetActive(true);
        float delay = 0.5f;
        fightWonText.gameObject.SetActive(true);
        GlobalHelper.getCamMovement().ShakeCamera(0.5f, delay);
        AudioManager.instance.PlaySound("dialogue", 1.2f, UnityEngine.Random.Range(0.8f, 0.9f));

        if (!GlobalHelper.GetRoom().roomData.isTutorial)
        {
            yield return new WaitForSeconds(delay);
            GlobalHelper.getCamMovement().ShakeCamera(0.5f, delay);

            fastTurnBonusText.gameObject.SetActive(true);
            fastTurnBonusText.text = String.Format("Par bonus : {3} (Par {0} - {1} {2})", par, turn, GlobalHelper.PluralOrSingular(" Turn taken", " Turns taken", turn), (int)Mathf.Clamp(par - turn, 0, 9999));
            AudioManager.instance.PlaySound("dialogue", 1.2f, UnityEngine.Random.Range(0.8f, 0.9f));

        }


        yield return new WaitForSeconds(delay);
        GlobalHelper.getCamMovement().ShakeCamera(0.5f, delay);
        moneyEarnedText.gameObject.SetActive(true);
        AudioManager.instance.PlaySound("dialogue", 1.2f, UnityEngine.Random.Range(0.8f, 0.9f));

        int moneyPlundered = GlobalHelper.GetGameManager().pillageMoney;
        string plunderedOrNot = "";
        if(moneyPlundered != 0)
        {
            plunderedOrNot = String.Format("+{0} Plundered", moneyPlundered);
        }
        moneyEarnedText.text = String.Format("$ Earned : {0} {1}",moneyEarned, plunderedOrNot);
        GlobalHelper.GlobalVariables.player.money += moneyEarned;
        GlobalHelper.GlobalVariables.player.money += moneyPlundered;
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
        if (!GlobalHelper.GetRoom().roomData.isTutorial)
        {
            noteText.gameObject.SetActive(true);
        }
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
