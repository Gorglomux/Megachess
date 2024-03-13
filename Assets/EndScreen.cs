using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public TextMeshProUGUI newClassesText;
    public Button titleButton; 
    public TextMeshProUGUI madeByText;
    public TextMeshProUGUI thankYouPlaytestersText;
    public TextMeshProUGUI thankYouText;

    // Start is called before the first frame update
    void Start()
    {
        GlobalHelper.UI().HidePauseButton();
    }

    public void AnimateShow()
    {
        ValidateWin();
        StartCoroutine(corAnimateShow());
    }

    public IEnumerator corAnimateShow()
    {
        yield return new WaitForSeconds(3);
        AudioManager.instance.PlaySound("dialogue", 1, 1);
        GetNewClassesToUnlock();
        newClassesText.gameObject.SetActive(true);
        GlobalHelper.getCamMovement().ShakeCamera(2);
        yield return new WaitForSeconds(3);


        AudioManager.instance.PlaySound("dialogue", 1, 1);
        thankYouPlaytestersText.gameObject.SetActive(true);
        GlobalHelper.getCamMovement().ShakeCamera(2);
        yield return new WaitForSeconds(3);
        AudioManager.instance.PlaySound("dialogue", 1, 1);
        thankYouText.gameObject.SetActive(true);

        GlobalHelper.getCamMovement().ShakeCamera(2);
        titleButton.gameObject.SetActive(true);

        yield return new WaitForSeconds(5);
        AudioManager.instance.PlaySound("dialogue", 0.2f, 1);
        madeByText.gameObject.SetActive(true);

        GlobalHelper.getCamMovement().ShakeCamera(0.3f);

    }
    public void ValidateWin()
    {
        PlayerPrefs.SetInt(GlobalHelper.GlobalVariables.player.playerData.name, PlayerPrefs.GetInt(GlobalHelper.GlobalVariables.player.playerData.name) +1);

    }
    public void GetNewClassesToUnlock()
    {
        string classNames = "";
        int amount = GlobalHelper.GlobalVariables.gameInfos.classesUnlockAmount;
        int unlocked = 0;
        foreach(PlayerData playerData in GlobalHelper.playerDataList)
        {
            if(PlayerPrefs.GetInt(playerData.name, 0) <= 0 && unlocked < amount)
            {
                unlocked++;
                PlayerPrefs.SetInt(playerData.name, 1);
                classNames += playerData.playerName + ", ";
            }
        }
        classNames= classNames.Substring(0,classNames.Length-2);
        if (unlocked == 0)
        {
            classNames = "None, you unlocked them all !";
        }
        newClassesText.text = "New classes unlocked : " + classNames;

        PlayerPrefs.Save();
    } 
}
