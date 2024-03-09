using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirstAbility : BaseAbility
{
    public ThirstAbility(AbilityData ab) : base(ab)
    {
    }
    public override void ExecuteAbility(object target)
    {
        if(!(target is Unit))
        {
            Debug.LogError("Invalide target for ability " + abilityData.abilityName);
        }
        Unit u = (Unit)target;

        if (!isCharged())
        {
            return;
        }
        Debug.Log("Thirst activates on " + target);
        GlobalHelper.UI().captureManager.DisplayAtPosition(u.GetWorldPosition(), "+ Bloodlust");
        GlobalHelper.UI().HideHoverInfos();
        u.AddEffect(GlobalHelper.effectLookup(GlobalHelper.GetEffectData("Bloodlust")));
        GlobalHelper.getCamMovement().ShakeCamera(0.3f, 0.2f);

        currentCharge = abilityData.cooldownDuration;

    }
}
