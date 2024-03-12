using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardedEffect : BaseEffect
{
    public GuardedEffect(EffectData ed) : base(ed)
    {
    }

    public override void TriggerEffect(object o)
    {
        if (!(o is Unit))
        {
            Debug.LogError("Cannot trigger effect, objefct received is not an unit");
            return;
        }
        Unit target = (Unit)o;
        if (isAttackingSelf(target))
        {
            return;
        }


        if (target.health <= 0)
        {
            GlobalHelper.UI().captureManager.DisplayAtPosition(UnitHavingEffect.GetWorldPosition() + new Vector3(0, 0.25f, 0), "+ Shielded");
            UnitHavingEffect.AddEffect(GlobalHelper.effectLookup(GlobalHelper.GetEffectData("Shielded")));

            //GlobalHelper.UI().captureManager.DisplayAtPosition(target.GetWorldPosition(), "Bloodlust active");
            Debug.Log("Bloodlust active");
        }

    }


}
