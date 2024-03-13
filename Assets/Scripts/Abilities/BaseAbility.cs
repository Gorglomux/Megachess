using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility
{
    public AbilityData abilityData;

    private int _currentCharge = 0 ;
    public int currentCharge { get { return _currentCharge; } set { if (_currentCharge + value <= 0) { _currentCharge = 0; } else { _currentCharge = value; } } }
    
    public BaseAbility(AbilityData ab)
    {
        abilityData = ab;
        SetUpRecharge();
    }
    public virtual void ExecuteAbility(object target)
    {
        if (!isCharged())
        {
            return;
        }

        currentCharge = abilityData.cooldownDuration;
    }

    public bool isCharged()
    {
        return currentCharge == 0;
    }

    public void SetUpRecharge()
    {
        
        RoomView rv = GlobalHelper.GetRoom();
        GameManager gm = GlobalHelper.GetGameManager();
        switch (abilityData.cooldownType)
        {
            case COOLDOWN_TYPE.FIGHT_AMOUNT:
                gm.OnRoomCleared += Discharge;
                break;
            case COOLDOWN_TYPE.KILL_AMOUNT:
                gm.OnKillUnit += DischargeEnemyKill;
                break;
            case COOLDOWN_TYPE.AREA_CLEARED:
                gm.OnAreaCleared += Discharge;
                break;
            case COOLDOWN_TYPE.UNIT_LOST_AMOUNT:
                gm.OnKillUnit += DischargeAllyKill;
                break;
            case COOLDOWN_TYPE.TURN_AMOUNT:
                gm.OnPlayerEndTurn += Discharge;
                break;
        }

    }
    private void Discharge(object o)
    {
        currentCharge --;
    }
    private void DischargeEnemyKill(object o)
    {
        Unit u = (Unit)o;
        if (u.isEnemy)
        {
            currentCharge -= u.megaSize;
        }
    }
    private void DischargeAllyKill(object o)
    {
        Unit u = (Unit)o;
        if (!u.isEnemy)
        {
            currentCharge -= u.megaSize;
        }
    }

    public virtual void onDestroy()
    {
        RoomView rv = GlobalHelper.GetRoom();
        GameManager gm = GlobalHelper.GetGameManager();
        switch (abilityData.cooldownType)
        {
            case COOLDOWN_TYPE.FIGHT_AMOUNT:
                gm.OnRoomCleared -= Discharge;
                break;
            case COOLDOWN_TYPE.KILL_AMOUNT:
                gm.OnKillUnit -= DischargeEnemyKill;
                break;
            case COOLDOWN_TYPE.AREA_CLEARED:
                gm.OnAreaCleared -= Discharge;
                break;
            case COOLDOWN_TYPE.UNIT_LOST_AMOUNT:
                gm.OnKillUnit -= DischargeAllyKill;
                break;
            case COOLDOWN_TYPE.TURN_AMOUNT:
                gm.OnPlayerEndTurn -= Discharge;
                break;
        }

    }

    /// <summary>
    /// On lie ça a des events pour recharger 
    /// </summary>
    public void ChargeAbility()
    {
        abilityData.cooldownDuration = Mathf.Clamp(--currentCharge ,0, 999);
    }

}
