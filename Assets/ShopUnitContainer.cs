using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUnitContainer : MonoBehaviour
{
    public UnitData unitData;
    public TextMeshProUGUI costText;
    public Image spriteImage;

    public Unit unitWithin;
    public void FillUnitData(UnitData ud)
    {
        this.unitData = ud;
        spriteImage.sprite = ud.sprite;
        costText.text = "$" + unitData.baseCost.ToString();
        unitWithin = GlobalHelper.GetGameManager().CreateUnit(ud, false);
        unitWithin.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Destroy(unitWithin.gameObject);
    }

}
