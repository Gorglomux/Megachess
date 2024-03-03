using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public int pooledIndicators = 300;
    public int currentIndicatorIndex = 0;
    public GameObject[] indicators;

    public List<GameObject> activeIndicators = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }
    void Initialize()
    {
        print("Start pooling");
        indicators = new GameObject[pooledIndicators];
        for(int i = 0; i< pooledIndicators; i++)
        {
            indicators[i] = GameObject.Instantiate(indicatorPrefab, this.transform);
            indicators[i].gameObject.SetActive(false);
        }
        print("Pooling done !");
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject getNext()
    {
        currentIndicatorIndex = (currentIndicatorIndex +1)% pooledIndicators;
        activeIndicators.Add(indicators[currentIndicatorIndex]);
        return indicators[currentIndicatorIndex];   
    }

    public void DisplayMovement(Unit u)
    {
        HideAll();
        RoomView r = GlobalHelper.GetRoom();
        List<Vector3Int> positions = MovementMethods.GetMovementMethod(u.unitData.unitName).Invoke(r,u);

        foreach (Vector3Int position in positions)
        {
            //Pool an indicator
            GameObject indicator = getNext();
            //Set it at the position
            indicator.transform.position = r.GetCenter( r.CellToTilemap(position));
            //Show
            indicator.gameObject.SetActive(true);
            //If there is a unit at this position
            Unit uToBlink = r.GetUnitAt(position);
            if (uToBlink != null && u.UID != uToBlink.UID)
            {
                uToBlink.ToggleBlink(true);
                blinkingUnits.Add(uToBlink);
            }
                //If ally do nothing
                //If enemy make the sprite blink? 

        }
    }
    List<Unit> blinkingUnits = new List<Unit>();
    public void HideAll()
    {
        foreach(GameObject go in activeIndicators)
        {
            go.SetActive(false);
        }
        foreach(Unit u in blinkingUnits)
        {
            u.ToggleBlink(false);
        }
        blinkingUnits.Clear();
        activeIndicators.Clear();
    }
}
