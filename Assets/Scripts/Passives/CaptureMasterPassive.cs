using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaptureMasterPassive : BasePassive
{
    public CaptureMasterPassive(PassiveData ed) : base(ed)
    {
    }

    public override void TriggerEffect(object o)
    {
        GlobalHelper.GetGameManager().canCaptureThisFight += 1;
        GlobalHelper.UI().captureManager.DisplayAtPosition(Vector3.zero, "+ 1 maximum captured unit");
        usedThisFight++;

    }

}

public class EarlyStepPassive : BasePassive
{
    public EarlyStepPassive(PassiveData ed) : base(ed)
    {
    }

    public override void TriggerEffect(object o)
    {
        if (!(o is Unit))
        {
            Debug.LogError("o is not unit what do I do boss");
        }
        Unit u = (Unit)o;
        if(u.placedOrder == 0)
        {
            Debug.Log("Unit is the first ally unit placed this round !");
            BaseEffect chargedEffect = GlobalHelper.effectLookup(GlobalHelper.GetEffectData("Charged"));
            u.AddEffect(chargedEffect);
            chargedEffect.UnitHavingEffect = u;
            usedThisFight++;

            GlobalHelper.UI().captureManager.DisplayAtPosition(u.GetWorldPosition(), "+ Charged");
        }

    }

}

public class EasygoingPassive : BasePassive
{
    public EasygoingPassive(PassiveData ed) : base(ed)
    {
    }

    public override void TriggerEffect(object o)
    {
        GlobalHelper.GetRoom().parAfterEffects++;
        GlobalHelper.UI().ParCount.text = "Par "+GlobalHelper.GlobalVariables.gameInfos.currentRoom.parAfterEffects.ToString();
        usedThisFight++;

    }

}
public class MartyrPassive : BasePassive
{
    public MartyrPassive(PassiveData ed) : base(ed)
    {
    }

    public override void TriggerEffect(object o)
    {
        if(!(o is Unit))
        {
            Debug.LogError("o is not unit in martyrPassive");
        }
        Unit u = (Unit)o;
        if (u.isEnemy)
        {
            Debug.Log("Martyr : Enemy died, not activating");
            return;
        }
        GlobalHelper.GetGameManager().StartCoroutine(corMartyr());




    }
    public IEnumerator corMartyr()
    {
        yield return null;
        yield return null;
        yield return null;
        List<Unit> allies = GlobalHelper.GetRoom().GetAllies();
        if (allies.Count > 0 && usedThisFight == 0)
        {

            Unit martyr = allies[GlobalHelper.rand.Next(allies.Count)];
            if (martyr.health == 0)
            {
                Debug.LogError("Martyr : Did you just give me a dead body to shield");
            }
            else
            {
                usedThisFight++;
                BaseEffect shieldedEffect = GlobalHelper.effectLookup(GlobalHelper.GetEffectData("Shielded"));
                GlobalHelper.UI().captureManager.DisplayAtPosition(martyr.GetWorldPosition(), "+ Martyr");

                martyr.AddEffect(shieldedEffect);
                Debug.Log("Martyr : activated on" + martyr.UID);
            }
        }
        else
        {
            Debug.Log("Martyr :All allies are dead what do you want me to do");
        }
        if (usedThisFight > 0)
        {
            Debug.Log("Martyr : will not activate because usedthisfight is " + usedThisFight);
        }
    }
}

public class PassiveIncomePassive : BasePassive
{
    public PassiveIncomePassive(PassiveData ed) : base(ed)
    {
    }

    public override void TriggerEffect(object o)
    {
        GlobalHelper.GlobalVariables.player.money++;
        GlobalHelper.UI().captureManager.DisplayAtPosition(Vector3.zero, "+ 1 Passive Income");
        usedThisFight++;
    }

}

public class PawnceptionPassive : BasePassive
{
    public PawnceptionPassive(PassiveData ed) : base(ed)
    {
    }

    public override void TriggerEffect(object o)
    {
        if (!(o is Unit))
        {
            Debug.LogError("o is not unit in martyrPassive");
        }
        Unit u = (Unit)o;
        if (u.isEnemy)
        {
            Debug.Log("Pawnception : Enemy died, not activating");
            return;
        }
        if(usedThisFight == 0)
        {
            GlobalHelper.GlobalVariables.player.AddUnit(GlobalHelper.GetGameManager().CreateUnit(GlobalHelper.GetUnitData("Pawn"), false));
            GlobalHelper.UI().captureManager.DisplayAtPosition(Vector3.zero, "+ 1 Pawn");
            usedThisFight++;

        }

    }

}

public class ShortcutPassive : BasePassive
{
    public ShortcutPassive(PassiveData ed) : base(ed)
    {
    }

    public override void TriggerEffect(object o)
    {
        usedThisFight++;
        GlobalHelper.GetGameManager().roomToClearBonus--;
        GlobalHelper.UI().captureManager.DisplayAtPosition(Vector3.zero, "-1 room per area");
    }
}
