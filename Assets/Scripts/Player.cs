using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Player : MonoBehaviour
{
    public event Action<UnitData> OnNewUnitAdded = delegate { };
    public event Action<Unit> OnInventoryAdded = delegate { };
    public event Action<Unit> OnInventoryRemoved = delegate { };
    public event Action OnAbilityChange = delegate { };

    public Dictionary<UnitData, List<Unit>> inventory = new Dictionary<UnitData,List<Unit>>();
    public List<UnitData> inventoryBackup = new List<UnitData>();

    public BaseAbility ability;
    private int _money;
    public int money { get { return _money; } set{
            _money = value;
            GlobalHelper.UI().UpdateMoneyCount();
        } }

    // Start is called before the first frame update
    void Start()
    {
        money = 3;
        //testInventory();
        testMegaInventory();
        GlobalHelper.UI().UpdateMoneyCount();
        //ChangeAbility(GlobalHelper.GetAbilityData("ExtraTurn"));
        ChangeAbility(GlobalHelper.GetAbilityData("Thirst"));
    }
    public void ChangeAbility(AbilityData ab)
    {
        ability = GlobalHelper.abilityLookup(ab);
        OnAbilityChange();
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
    void testMegaInventory()
    {
        for(int i=0; i<5; i++)
        {
            foreach(UnitData ud in GlobalHelper.unitDataList)
            {
                AddUnit(GlobalHelper.GetRoom().CreateUnit(ud, false));
            }
        }


    }

    public void BackupInventory()
    {
        inventoryBackup.Clear();
        foreach (var units in inventory.Values)
        {
            foreach(Unit u in units)
            {
                inventoryBackup.Add(u.unitData);
            }
        }

    }
    public void RestoreBackup()
    {
        foreach(ReserveContainer rc  in GlobalHelper.UI().reserve.containers.Values)
        {
            Destroy(rc.gameObject);
        }
        GlobalHelper.UI().reserve.containers.Clear();
        inventory.Clear();
        foreach (UnitData ud in inventoryBackup.ToList())
        {
            AddUnit(GlobalHelper.GetRoom().CreateUnit(ud, false));
        }
        inventoryBackup.Clear();
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
            u.LoadPalette(unit.unitData.paletteIndex);
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
    public void RemoveUnit(Unit u)
    {
        inventory[u.unitData].Remove(u);
    }

    public void RestoreData(object data)
    {
        throw new NotImplementedException();
    }
}
