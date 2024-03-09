using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodlustEffect : BaseEffect
{
    public BloodlustEffect(EffectData ed) : base(ed)
    {
    }

    public override void TriggerEffect(object o)
    {
        if(!(o is Unit))
        {
            Debug.LogError("Cannot trigger effect, objefct received is not an unit");
            return;
        }
        Unit target = (Unit)o;
        if(target.health <= 0)
        {
            //Display bloolust trigger message here
            if (UnitHavingEffect.actionsLeft > 0)
            {
                //This is a weird situation where a unit with multiple moves has the bloodlust effect. Might not happen
                Debug.LogWarning("Something weird has happen with " + UnitHavingEffect.unitData + " and the bloodlust effect.");
                UnitHavingEffect.actionsLeft++;
            }
            else
            {
                UnitHavingEffect.actionsLeft = 1;
                GlobalHelper.UI().captureManager.DisplayAtPosition(target.GetWorldPosition(), "Bloodlust active");
            }
            Debug.Log("Bloodlust active");
        }
        else
        {

            Debug.Log("Bloodlust inactive" + target.health);
        }

    }


}
