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
        AddUnit(GlobalHelper.GetRoom().CreateUnit(GlobalHelper.GetUnitData("Rook"), false ));


    }

    public void AddUnit(Unit unit)
    {
        int toAdd = 1;
        if (unit.isEnemy)
        {

        }
        else
        {
            toAdd = (int)Mathf.Pow(unit.megaSize, 2);
        }
        for(int i= 0; i < toAdd; i++)
        {
            Unit u = GlobalHelper.GetRoom().CreateUnit(unit.unitData, false);
            u.LoadPalette(unit.basePaletteIndex);
            if (!inventory.ContainsKey(u.unitData))
            {
                inventory[u.unitData] = new List<Unit>();
                OnNewUnitAdded(u.unitData);
                print("Category created");
            }
            inventory[u.unitData].Add(u);
            OnInventoryAdded(u);
            u.transform.parent = transform;
            print("Unit added");
            Destroy(unit.gameObject);
        }

    }


}
