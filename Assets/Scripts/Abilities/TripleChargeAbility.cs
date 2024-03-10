using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleChargeAbility : BaseAbility
{
    public TripleChargeAbility(AbilityData ab) : base(ab)
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
        Debug.Log("Triple charge activates on " + target);
        GlobalHelper.UI().captureManager.DisplayAtPosition(u.GetWorldPosition(), "+ Triple Charge");
        GlobalHelper.UI().HideHoverInfos();
        BaseEffect chargedEffect = GlobalHelper.effectLookup(GlobalHelper.GetEffectData("Charged"));
        chargedEffect.effectStrength = 3;
        u.AddEffect(chargedEffect);
        GlobalHelper.getCamMovement().ShakeCamera(0.3f, 0.2f);

        currentCharge = abilityData.cooldownDuration;

    }
}
