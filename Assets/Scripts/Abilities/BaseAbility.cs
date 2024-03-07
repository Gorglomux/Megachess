using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility
{
    public AbilityData abilityData;

    public int currentCharge = 0 ;
    
    public BaseAbility(AbilityData ab)
    {
        abilityData = ab;
    }
    public virtual void ExecuteAbility(Object target)
    {
        if (!isCharged())
        {
            return;
        }
        currentCharge = abilityData.cooldownDuration;
    }

    public bool isCharged()
    {
        return currentCharge == 0;
    }


    /// <summary>
    /// On lie ça a des events pour recharger 
    /// </summary>
    public void ChargeAbility()
    {
        abilityData.cooldownDuration = Mathf.Clamp(--currentCharge ,0, 999);
    }
}
