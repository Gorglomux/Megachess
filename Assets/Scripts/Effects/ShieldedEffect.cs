using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedEffect : BaseEffect
{
    public ShieldedEffect(EffectData ed) : base(ed)
    {
    }

    public override void TriggerEffect(object o)
    {
        if(!(o is Unit))
        {
            Debug.LogError("Cannot trigger effect, objefct received is not an unit");
            return;
        }
        Debug.Log("Activating Shielded effect");
        Unit opponent = (Unit)o;
        if (isAttackingSelf(opponent))
        {
            Debug.Log("Shielding against opponent");
            if (effectStrength > 0)
            {
                UnitHavingEffect.shieldAmount++;

                UnitHavingEffect.RemoveEffect(this);
            }
        }
        else
        {
            Debug.Log("Attacking opponent, I will not shield");
        }
        

    }


}
