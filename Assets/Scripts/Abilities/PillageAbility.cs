using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillageAbility : BaseAbility
{
    GameManager gm ;
    public PillageAbility(AbilityData ab) : base(ab)
    {
        gm = GlobalHelper.GetGameManager();
        gm.OnStartFight += ResetPlunderMoney;
    }
    public override void ExecuteAbility(object target)
    {


        if (!isCharged())
        {
            return;
        }
        GameManager gm = GlobalHelper.GetGameManager();
        Debug.Log("Pillage is starting charge activates on " + target);
        GlobalHelper.UI().captureManager.DisplayAtPosition(Vector3.zero, "Pillage Active");
        GlobalHelper.UI().HideHoverInfos();
        gm.isPillage = true;
        GlobalHelper.getCamMovement().ShakeCamera(0.3f, 0.2f);

        gm.OnStartFight += endPillage;
        gm.OnPlayerEndTurn += endPillage;
        gm.OnRoomCleared += endPillage;

        gm.OnKillUnit += onKillPillage;
        currentCharge = abilityData.cooldownDuration;


    }

    public void onKillPillage(object o)
    {
        if(!(o is Unit))
        {
            return;
        }
        Unit u = (Unit)o;
        if (u.isEnemy)
        {
            GlobalHelper.GetGameManager().pillageMoney++;
            GlobalHelper.UI().captureManager.DisplayAtPosition(u.GetWorldPosition() - new Vector3(0,0.55f,0), "Plundered !");
        }

    }

    public override void onDestroy()
    {
        base.onDestroy();

        UnbindPillageEvents();
        gm.OnStartFight -= ResetPlunderMoney;
    }

    public void UnbindPillageEvents()
    {
        gm.OnStartFight -= endPillage;
        gm.OnPlayerEndTurn -= endPillage;
        gm.OnRoomCleared -= endPillage;

        gm.OnKillUnit -= onKillPillage;
    }
    public void endPillage(object o)
    {
        gm.isPillage = false;

        GlobalHelper.UI().captureManager.DisplayAtPosition(Vector3.zero, "Pillage Disabled");
        UnbindPillageEvents();
    }
    public void ResetPlunderMoney(object o)
    {
        GlobalHelper.GetGameManager().pillageMoney = 0;

    }
}
