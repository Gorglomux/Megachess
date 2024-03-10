using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedEffect : BaseEffect
{
    public ChargedEffect(EffectData ed) : base(ed)
    {
    }

    public override void TriggerEffect(object o)
    {

        Debug.Log("Activating Charge effect");
        Unit opponent = (Unit)o;
        if (effectStrength > 0)
        {
            UnitHavingEffect.actionsLeft+= effectStrength;
            effectStrength = 0;
            UnitHavingEffect.RemoveEffect(this);
        }

        

    }


}
