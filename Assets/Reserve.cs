using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reserve : MonoBehaviour
{
    public GameObject containerPrefab;
    Player playerRef;

    public Transform rootContainers;
    public Dictionary<string,ReserveContainer> containers = new Dictionary<string, ReserveContainer>();

    // Start is called before the first frame update
    void Start()
    {

        playerRef = GlobalHelper.GlobalVariables.player;
        playerRef.OnInventoryAdded += AddPiece;
        playerRef.OnInventoryRemoved += RemovePiece;
        playerRef.OnNewUnitAdded += AddContainer;
    }
    void RemovePiece(Unit u)
    {
        if (containers.ContainsKey(u.unitData.unitName))
        {
            containers[u.unitData.unitName].RemoveUnit();
            if (containers[u.unitData.unitName].unitCount <= 0)
            {
                RemoveContainer(u.unitData.unitName);
            }
        }

    }
    void AddPiece(Unit u)
    {
        containers[u.unitData.unitName].AddUnit(u);
    }

    void AddContainer(UnitData ud)
    {
        ReserveContainer container = GameObject.Instantiate(containerPrefab, rootContainers).GetComponent<ReserveContainer>();
        container.Initialize(ud);

        containers[ud.unitName]= container;

    }

    void RemoveContainer(string id)
    {
        if (containers.ContainsKey(id))
        {
            Destroy(containers[id].gameObject);
            containers.Remove(id);
        }

    }

    public void DeleteReserve()
    {
        foreach(string id in containers.Keys)
        {
            RemoveContainer(id);
        }
        containers.Clear();
    }

}
