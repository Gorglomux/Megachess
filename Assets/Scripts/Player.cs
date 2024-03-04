using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event Action<UnitData> OnNewUnitAdded = delegate { };
    public event Action<Unit> OnInventoryAdded = delegate { };
    public event Action<Unit> OnInventoryRemoved = delegate { };

    public Dictionary<UnitData, List<Unit>> inventory = new Dictionary<UnitData,List<Unit>>();
    // Start is called before the first frame update
    void Start()
    {
        testInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void testInventory()
    {
        // Maybe delay them ? 
        AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Knight"), false ));
        AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Knight"), false ));
        AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Rook"), false ));
        AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Rook"), false ));
        AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Queen"), false ));

        //TODO : REMOVE THIS AND PUT IT AT THE START OF THE SELECTION PHASE
        GlobalHelper.GlobalVariables.indicatorManager.ShowSpawnableCells();
    }

    public void AddUnit(Unit unit)
    {
        if (!inventory.ContainsKey(unit.unitData))
        {
            inventory[unit.unitData] = new List<Unit>(); 
            OnNewUnitAdded(unit.unitData);
            print("Category created");
        }
        inventory[unit.unitData].Add(unit);
        OnInventoryAdded(unit);
        print("Unit added");
    }


}
