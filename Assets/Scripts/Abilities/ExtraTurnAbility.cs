using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraTurnAbility : BaseAbility
{
    public ExtraTurnAbility(AbilityData ab) : base(ab)
    {
    }

    public override void ExecuteAbility(object target)
    {
        if (!isCharged())
        {
            return;
        }
        Debug.Log("Za warudo");
        GlobalHelper.UI().captureManager.DisplayAtPosition(Vector3.zero, abilityData.abilityName + " Activated");
        GlobalHelper.UI().HideHoverInfos();
        GlobalHelper.GetGameManager().extraTurns++;
        GlobalHelper.getCamMovement().ShakeCamera(0.8f, 0.5f);
        currentCharge = abilityData.cooldownDuration;

    }

}
