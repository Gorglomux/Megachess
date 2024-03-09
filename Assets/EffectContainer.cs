using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class EffectContainer : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FillInfos(object objectType)
    {
        if (objectType is BaseAbility)
        {
            FillAbility((BaseAbility) objectType);
        }if(objectType is BaseEffect)
        {
            FillEffect((BaseEffect)objectType);
        }if(objectType is PlayerData)
        {
            FillPlayerData((PlayerData)objectType);
        }

    }
    public void FillPlayerData(PlayerData playerData)
    {
        text.text = RegexReplace(playerData.description);
    }
    public void FillAbility(BaseAbility ab, bool displayName = false)
    {
        string t = "";
        if (displayName)
        {
            t += ab.abilityData.abilityName.ToUpper() + " : ";
        }
        t += ab.abilityData.description;
        text.text = RegexReplace(t); 

    }
    public void FillEffect(BaseEffect eb, bool displayName = true)
    {
        string finalString = "";
        if (displayName)
        {
            finalString += eb.effectData.name;
            if (eb.effectData.stackable)
            {
                finalString += " "+eb.effectStrength;
            }
            finalString += " - ";
        }
        finalString += eb.effectData.effectDescription;
        text.text = RegexReplace(finalString);
    }
    public void FillCooldown(BaseAbility ab)
    {
        COOLDOWN_TYPE type = ab.abilityData.cooldownType;
        int chargeAmount =ab.abilityData.cooldownDuration - ab.currentCharge;
        string suffix = "";
        switch (type)
        {
            case COOLDOWN_TYPE.FIGHT_AMOUNT:
                suffix = string.Format("{0} cleared", GlobalHelper.PluralOrSingular("room", "rooms", ab.abilityData.cooldownDuration));
                break;
            case COOLDOWN_TYPE.KILL_AMOUNT:
                suffix = string.Format("{0} killed", GlobalHelper.PluralOrSingular("enemy", "enemies", ab.abilityData.cooldownDuration));
                break;
            case COOLDOWN_TYPE.AREA_CLEARED:
                suffix = string.Format("{0} cleared", GlobalHelper.PluralOrSingular("area", "areas", ab.abilityData.cooldownDuration));
                break;
            case COOLDOWN_TYPE.UNIT_LOST_AMOUNT:
                suffix = string.Format("friendly {0} lost", GlobalHelper.PluralOrSingular("unit", "units", ab.abilityData.cooldownDuration));
                break;
            case COOLDOWN_TYPE.TURN_AMOUNT:
                suffix = string.Format("{0} taken", GlobalHelper.PluralOrSingular("turn", "turns", ab.abilityData.cooldownDuration));
                break;
        }
        string s =string.Format("Cooldown : {0}/{1} {2}", chargeAmount,ab.abilityData.cooldownDuration,suffix);
        text.text = s;
    }

    public string RegexReplace(string input)
    {
        string output = input;
        string regexPattern = @"<.+>";
        MatchCollection matches = Regex.Matches(input, regexPattern);
        foreach (Match match in matches)
        {
            string s = match.Value.Trim('<').Trim('>');
            if(GlobalHelper.GetEffectData(s))
            {
                EffectContainer effectContainer = GlobalHelper.UI().effectContainerManager.getNext();
                effectContainer.FillInfos( GlobalHelper.effectLookup(GlobalHelper.GetEffectData(s)));

            }
            else if (GlobalHelper.GetAbilityData(s))
            {
                EffectContainer effectContainer = GlobalHelper.UI().effectContainerManager.getNext();
                effectContainer.FillInfos(GlobalHelper.abilityLookup(GlobalHelper.GetAbilityData(s)));
            }
            output = Regex.Replace(input, regexPattern, s);
        }
        return output;
    }
}
