using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopContainer : MonoBehaviour
{
    public PassiveData passiveData;
    public TextMeshProUGUI text;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI descriptionText;
    public Image spriteImage;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    public void FillPassiveData(PassiveData pa)
    {
        this.passiveData= pa;
        spriteImage.sprite = pa.passiveSprite;
        text.text = passiveData.passiveName;
        costText.text = "$" + passiveData.passiveCost.ToString();
        descriptionText.text = RegexReplace(pa.passiveDescription);


    }

    public string RegexReplace(string input)
    {
        string output = input;
        string regexPattern = @"<.+>";
        MatchCollection matches = Regex.Matches(input, regexPattern);
        foreach (Match match in matches)
        {
            string s = match.Value.Trim('<').Trim('>');
            if (GlobalHelper.GetEffectData(s))
            {
                EffectContainer effectContainer = GlobalHelper.UI().effectContainerManager.getNext();
                effectContainer.FillInfos(GlobalHelper.GetEffectData(s));

            }
            else if (GlobalHelper.GetAbilityData(s))
            {
                EffectContainer effectContainer = GlobalHelper.UI().effectContainerManager.getNext();
                effectContainer.FillInfos(GlobalHelper.GetAbilityData(s));
            }
            output = Regex.Replace(input, regexPattern, s);
        }
        return output;
    }


}
