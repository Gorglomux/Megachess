using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneAbility : BaseAbility
{
    public CloneAbility(AbilityData ab) : base(ab)
    {
    }
    public override void ExecuteAbility(object target)
    {
        if(!(target is ReserveContainer))
        {
            Debug.LogError("Invalid target for ability " + abilityData.abilityName);
        }
        ReserveContainer rc = (ReserveContainer)target;

        if (!isCharged())
        {
            return;
        }
        UnitData ud = rc.unitData;
        if(ud == null)
        {
            Debug.LogError("Couldn't clone unit ???? on container " + rc);
        } 
        Debug.Log("Clone activates on " + target);
        GlobalHelper.UI().captureManager.DisplayAtPosition(Vector3.zero, "+ "+ud.unitName);
        GlobalHelper.UI().HideHoverInfos();
        GlobalHelper.GlobalVariables.player.AddUnit(GlobalHelper.GetRoom().CreateUnit(ud,false));

        GlobalHelper.getCamMovement().ShakeCamera(0.3f, 0.2f);

        currentCharge = abilityData.cooldownDuration;

    }
}
