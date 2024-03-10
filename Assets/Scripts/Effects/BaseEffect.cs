using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect
{
    public Action<object> OnApply = delegate { };
    public EffectData effectData;

    public int effectStrength;

    public Unit UnitHavingEffect;
    public BaseEffect(EffectData ed)
    {
        effectData = ed;
        SetUpTargetting();
        effectStrength = ed.defaultValue;
    }
    public void AddStrength(int value)
    {
        if (effectData.stackable)
        {
            effectStrength += value;
        }
    }
    public void BindEffect(Unit u)
    {
        UnitHavingEffect = u;
        SetUpTargetting();
    }
    public void SetUpTargetting()
    {
        DisableTargetting();
        switch (effectData.effectActivationTime)
        {
            case EFFECT_ACTIVATION_TIME.ON_APPLY:
                OnApply += TriggerEffect;
                //GlobalHelper.GetGameManager().OnStartFight += TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.BEFORE_ATTACK:
                if(UnitHavingEffect != null)
                {
                    UnitHavingEffect.OnBeforeAttack += TriggerEffect;
                }
                break;
            case EFFECT_ACTIVATION_TIME.AFTER_ATTACK:
                if (UnitHavingEffect != null)
                {
                    UnitHavingEffect.OnAfterAttack += TriggerEffect;
                }
                break;
            case EFFECT_ACTIVATION_TIME.ON_END_TURN:
                GlobalHelper.GetGameManager().OnPlayerEndTurn += TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.ON_START_TURN:
                 GlobalHelper.GetGameManager().OnStartTurn += TriggerEffect;
                break;
        };
    }
    public virtual void TriggerEffect(object o)
    {
        Debug.Log("Base method for triggering effect on " + effectData.effectName);
    }

    public void onDestroy()
    {
        Debug.Log("Effect destroyed");
        DisableTargetting();
    }
    public void DisableTargetting()
    {
        switch (effectData.effectActivationTime)
        {
            case EFFECT_ACTIVATION_TIME.ON_APPLY:
                //GlobalHelper.GetGameManager().OnStartFight -= TriggerEffect;
                OnApply -= TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.BEFORE_ATTACK:
                if (UnitHavingEffect != null)
                {
                    UnitHavingEffect.OnBeforeAttack -= TriggerEffect;
                }
                break;
            case EFFECT_ACTIVATION_TIME.AFTER_ATTACK:
                if (UnitHavingEffect != null)
                {
                    UnitHavingEffect.OnAfterAttack -= TriggerEffect;
                }
                break;
            case EFFECT_ACTIVATION_TIME.ON_END_TURN:
                GlobalHelper.GetGameManager().OnPlayerEndTurn -= TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.ON_START_TURN:
                GlobalHelper.GetGameManager().OnStartTurn -= TriggerEffect;
                break;
        };
    }
    /// <summary>
    /// Returns true if the effect is negative
    /// </summary>
    public bool RemoveStrength(int value = 1)
    {
        effectStrength--;
        if(effectStrength <= 0)
        {
            return true;
        }
        return false;
    }

    //If opponent is attacking unit
    public bool isAttackingSelf(Unit opponent)
    {
        return (GlobalHelper.isPlayerTurn() && !opponent.isEnemy )||(!GlobalHelper.isPlayerTurn() && opponent.isEnemy);

    }
}
