using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasePassive
{
    public Action<object> OnApply = delegate { };
    public PassiveData passiveData;
    public int usedThisFight = 0;
    public BasePassive(PassiveData ed)
    {
        passiveData = ed;
        SetUpTargetting();
        GlobalHelper.GetGameManager().OnStartFight += ResetUse;
        OnApply(null);
    }
    public void ResetUse(object o)
    {
        usedThisFight = 0;
    }

    public void SetUpTargetting()
    {
        DisableTargetting();
        switch (passiveData.passiveActivationTime)
        {

            case EFFECT_ACTIVATION_TIME.ON_APPLY:
                OnApply += TriggerEffect;
                //GlobalHelper.GetGameManager().OnStartFight += TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.BEFORE_ATTACK:

                break;
            case EFFECT_ACTIVATION_TIME.AFTER_ATTACK:

                break;
            case EFFECT_ACTIVATION_TIME.ON_END_TURN:
                GlobalHelper.GetGameManager().OnPlayerEndTurn += TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.ON_START_TURN:
                GlobalHelper.GetGameManager().OnStartTurn += TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.ON_BEGIN_FIGHT:
                GlobalHelper.GetGameManager().OnStartFight += TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.ON_END_FIGHT:
                GlobalHelper.GetGameManager().OnRoomCleared += TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.ON_UNIT_PLAYED:
                GlobalHelper.GetGameManager().OnUnitPlayed += TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.ON_UNIT_KILLED:
                GlobalHelper.GetGameManager().OnKillUnit += TriggerEffect;
                break;
        };
    }
    public virtual void TriggerEffect(object o)
    {
        Debug.Log("Base method for triggering effect on " + passiveData.passiveName);
    }

    public void onDestroy()
    {
        Debug.Log("Effect destroyed");
        DisableTargetting(); ;
        GlobalHelper.GetGameManager().OnStartFight -= ResetUse;
    }
    public void DisableTargetting()
    {
        switch (passiveData.passiveActivationTime)
        {

            case EFFECT_ACTIVATION_TIME.ON_APPLY:
                OnApply -= TriggerEffect;
                //GlobalHelper.GetGameManager().OnStartFight += TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.BEFORE_ATTACK:

                break;
            case EFFECT_ACTIVATION_TIME.AFTER_ATTACK:

                break;
            case EFFECT_ACTIVATION_TIME.ON_END_TURN:
                GlobalHelper.GetGameManager().OnPlayerEndTurn -= TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.ON_START_TURN:
                GlobalHelper.GetGameManager().OnStartTurn -= TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.ON_BEGIN_FIGHT:
                GlobalHelper.GetGameManager().OnStartFight -= TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.ON_END_FIGHT:
                GlobalHelper.GetGameManager().OnRoomCleared -= TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.ON_UNIT_PLAYED:
                GlobalHelper.GetGameManager().OnUnitPlayed -= TriggerEffect;
                break;
            case EFFECT_ACTIVATION_TIME.ON_UNIT_KILLED:
                GlobalHelper.GetGameManager().OnKillUnit -= TriggerEffect;
                break;
        };
    }
}

