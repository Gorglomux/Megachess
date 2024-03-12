using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromoteAbility : BaseAbility
{
    public PromoteAbility(AbilityData ab) : base(ab)
    {
    }
    public override void ExecuteAbility(object target)
    {
        if(!(target is Unit))
        {
            Debug.LogError("Invalid target for ability " + abilityData.abilityName);
        }
        Unit u = (Unit)target;

        if (!isCharged())
        {
            return;
        }
        if(u.unitData.unitName != "Pawn" || u.megaSize != 1)
        {
            GlobalHelper.getCamMovement().ShakeCamera(0.3f, 0.2f);
            GlobalHelper.UI().SetBottomText("Can only promote non mega pawns ! ", 3);
            return;
        }
        Debug.Log("Promote activates on " + target);
        GlobalHelper.UI().captureManager.DisplayAtPosition(u.GetWorldPosition(), "Promoted");
        GlobalHelper.UI().HideHoverInfos();
        RoomView rv = GlobalHelper.GetRoom();

        Unit queen = rv.CreateUnit(GlobalHelper.GetUnitData("Queen"), false);
        rv.PlaceUnitOnMap(queen,rv.CellToTilemap(u.occupiedCells[0]));
        GlobalHelper.GlobalVariables.gameInfos.selected = null;
        rv.DestroyUnit(u);
        GlobalHelper.getCamMovement().ShakeCamera(0.3f, 0.2f);

        currentCharge = abilityData.cooldownDuration;

    }
}
