using System.Collections;
using System.Collections.Generic;
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
        }

    }
    public void FillAbility(BaseAbility ab)
    {
        text.text = ab.abilityData.description; 

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
}
