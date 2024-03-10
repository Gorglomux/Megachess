using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodShieldPassive : BasePassive
{
    public BloodShieldPassive(PassiveData ed) : base(ed)
    {
    }

    public override void TriggerEffect(object o)
    {
        if (!(o is Unit))
        {
            Debug.LogError("o is not unit what do I do boss");
        }
        Unit u = (Unit)o;
        if(u.lastAttacker != null && usedThisFight ==0 && !u.lastAttacker.isEnemy)
        {
            GlobalHelper.UI().captureManager.DisplayAtPosition(u.GetWorldPosition()+ new Vector3(0,0.25f,0), "+ Blood Shield");
            u.lastAttacker.AddEffect(GlobalHelper.effectLookup(GlobalHelper.GetEffectData("Shielded")));
            usedThisFight++;

        }

    }

}

