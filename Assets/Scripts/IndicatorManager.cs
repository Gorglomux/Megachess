using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public int pooledIndicators = 300;
    public int currentIndicatorIndex = 0;
    public Indicator[] indicators;

    public List<Indicator> activeIndicators = new List<Indicator>();
    public List<Indicator> spawnableCellIndicators = new List<Indicator>();

    public Unit selectedUnit;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }
    void Initialize()
    {
        print("Start pooling");
        indicators = new Indicator[pooledIndicators];
        for(int i = 0; i< pooledIndicators; i++)
        {
            indicators[i] = GameObject.Instantiate(indicatorPrefab, this.transform).GetComponent<Indicator>();
            indicators[i].SetState(INDICATOR_STATE.INACTIVE);
            indicators[i].gameObject.SetActive(false);
        }
        print("Pooling done !");
    }


    // Update is called once per frame
    void Update()
    { 
    }

    public Indicator getNext(List<Indicator> indicatorList = null)
    {
        if (indicatorList == null)
        {
            indicatorList = activeIndicators;
        }
        bool added = false;
        int maxIter = 100;
        int i = 0;
        do
        {
            currentIndicatorIndex = (currentIndicatorIndex + 1) % pooledIndicators;
            Indicator indicator = indicators[currentIndicatorIndex];

            if (indicator.currentState == INDICATOR_STATE.INACTIVE)
            {
                indicatorList.Add(indicators[currentIndicatorIndex]);
                added = true;
            }
            i++;

        } while (i < maxIter && !added);

        return indicators[currentIndicatorIndex];
    }
    public void DisplayMovement(Unit u)
    {
        if (GlobalHelper.GlobalVariables.gameInfos.selected != null)
        {
            selectedUnit = GlobalHelper.GlobalVariables.gameInfos.selected as Unit;
        }

        print("Displaying movement of " + u.unitData.unitName);
        RoomView r = GlobalHelper.GetRoom();
        List<Vector3Int> positions = MovementMethods.GetMovementMethod(u.unitData.unitName).Invoke(r,u);

        foreach (Vector3Int position in positions)
        {
            //Pool an indicator
            Indicator indicator = getNext();
            //Set it at the position
            indicator.transform.position = transform.position + GetIndicatorPosition(position);
            //Show
            indicator.gameObject.SetActive(true);
            if (u.isEnemy)
            {
                indicator.SetState(INDICATOR_STATE.ENEMY_ACTIVE);

            }
            else
            {
                indicator.SetState(INDICATOR_STATE.ALLY_INACTIVE);

            }
            if (selectedUnit == null ||( selectedUnit != null && selectedUnit.UID == u.UID) )
            {
                //If there is a unit at this position
                Unit uToBlink = r.GetUnitAt(position);
                if (uToBlink != null && u.UID != uToBlink.UID && u.isEnemy != uToBlink.isEnemy)
                {
                    float blinkSpeed = 1;
                    if(selectedUnit != null && u.actionsLeft > 0 )
                    {
                        blinkSpeed = 3;
                    }
                    uToBlink.ToggleBlink(true, blinkSpeed);
                    blinkingUnits.Add(uToBlink);
                }
                //If ally do nothing
                //If enemy make the sprite blink? 
            }

        }
    }
    List<Unit> blinkingUnits = new List<Unit>();
    public void HideAll()
    {
        foreach(Indicator go in activeIndicators)
        {
            go.SetState(INDICATOR_STATE.INACTIVE);
        }


        foreach(Unit u in blinkingUnits)
        {
            u.ToggleBlink(false);
        }
        blinkingUnits.Clear();
        activeIndicators.Clear();

        if (GlobalHelper.GlobalVariables.gameInfos.selected != null)
        {
            selectedUnit = GlobalHelper.GlobalVariables.gameInfos.selected as Unit;
            //Toggle back the indicators 
            DisplayMovement(selectedUnit);
        }
        else
        {
            selectedUnit = null;
        }
    }
    public void ShowSpawnableCells()
    {
        HideSpawnableCells();
        RoomView r = GlobalHelper.GetRoom();
        foreach(Vector3Int cell in r.SpawnableCells)
        {

            //Pool an indicator
            Indicator indicator = getNext(spawnableCellIndicators);
            indicator.gameObject.SetActive(true);
            //Set it at the position
            indicator.transform.position = transform.position + GetIndicatorPosition(r.TilemapToCell(cell));
            indicator.SetState(INDICATOR_STATE.PLACE_FROM_RESERVE);
        }

    }


    public void HoverOnSpawnableCells(List<Vector3Int> positions)
    {
        RoomView r = GlobalHelper.GetRoom();

        foreach (Vector3Int position in r.SpawnableCells)
        {
            Indicator indicator = spawnableCellIndicators.Find((x) => x.transform.position == transform.position + GetIndicatorPosition(r.TilemapToCell(position)));
            if (positions.Contains(r.TilemapToCell( position)))
            {
                if (indicator != null && indicator.currentState == INDICATOR_STATE.PLACE_FROM_RESERVE)
                {
                    indicator.SetState(INDICATOR_STATE.ALLY_TARGETED);
                }
            }
            else if(indicator != null && indicator.currentState == INDICATOR_STATE.ALLY_TARGETED)
            {
                indicator.SetState((INDICATOR_STATE.PLACE_FROM_RESERVE));
            }
        }
    }


    public void HideSpawnableCells()
    {
        foreach (Indicator go in spawnableCellIndicators)
        {
            go.SetState(INDICATOR_STATE.INACTIVE);
        }
        spawnableCellIndicators.Clear();
    }

    public void ShowPossibleUnitMove(Unit u, List<Vector3Int> positions)
    {
        RoomView r = GlobalHelper.GetRoom();

        List<Vector3Int> validPositions = MovementMethods.GetMovementMethod(u.unitData.unitName).Invoke(r, u);
        foreach (Vector3Int position in validPositions)
        {
            Indicator indicator = activeIndicators.Find((x) => x.transform.position ==  transform.position + GetIndicatorPosition(position));
            if (positions.Contains(position))
            {
                
                if (indicator != null)
                {
                    indicator.SetState(INDICATOR_STATE.ALLY_TARGETED);
                }
            }
            else if(indicator != null)
            {
                if (GlobalHelper.GetGameState() is FightState)
                {
                    indicator.SetState((INDICATOR_STATE.ALLY_ACTIVE));
                }
                else
                {
                    indicator.SetState((INDICATOR_STATE.ALLY_INACTIVE));
                }
            }
        }

    }
    Vector3 GetIndicatorPosition(Vector3Int position)
    {
        RoomView room = GlobalHelper.GetRoom();
        return  room.GetCenter(room.CellToTilemap(position));
    }
}
