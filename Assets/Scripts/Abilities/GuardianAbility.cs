using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianAbility : BaseAbility
{
    public GuardianAbility(AbilityData ab) : base(ab)
    {
    }
    public override void ExecuteAbility(object target)
    {
        if(!(target is Unit))
        {
            Debug.LogError("Invalid target for ability " + abilityData.abilityName);
        }
        Unit u = (Unit)target;

        if (!isCharged())
        {
            return;
        }
        Debug.Log("Guardian activates on " + target);

        GlobalHelper.UI().captureManager.DisplayAtPosition(u.GetWorldPosition(), "+ Shielded");
        GlobalHelper.UI().HideHoverInfos();
        u.AddEffect(GlobalHelper.effectLookup(GlobalHelper.GetEffectData("Shielded")));
        u.AddEffect(GlobalHelper.effectLookup(GlobalHelper.GetEffectData("Shielded")));
        u.AddEffect(GlobalHelper.effectLookup(GlobalHelper.GetEffectData("Shielded")));
        GlobalHelper.getCamMovement().ShakeCamera(0.3f, 0.2f);

        currentCharge = abilityData.cooldownDuration;

    }
}
