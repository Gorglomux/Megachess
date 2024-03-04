using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reserve : MonoBehaviour
{
    public GameObject containerPrefab;
    Player playerRef;

    public Transform rootContainers;
    Dictionary<string,ReserveContainer> containers = new Dictionary<string, ReserveContainer>();
    // Start is called before the first frame update
    void Start()
    {

        playerRef = GlobalHelper.GlobalVariables.player;
        playerRef.OnInventoryAdded += AddPiece;
        playerRef.OnNewUnitAdded += AddContainer;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
