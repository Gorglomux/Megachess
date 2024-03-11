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

    private Vector3Int inactiveVector = new Vector3Int(9999,9999);
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
            indicators[i].SetState(INDICATOR_STATE.INACTIVE, false, false,inactiveVector);
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
        if (GlobalHelper.GlobalVariables.gameInfos.selected != null && GlobalHelper.GlobalVariables.gameInfos.selected is Unit)
        {
            selectedUnit = GlobalHelper.GlobalVariables.gameInfos.selected as Unit;
        }
        RoomView r = GlobalHelper.GetRoom();

        print("Displaying movement of " + u.unitData.unitName);
        List<Vector3Int> positions = MovementMethods.GetMovementMethod(u.unitData.unitName).Invoke(r,u, -1 );
        DisplayPositions(positions, u);
        if (MovementMethods.HasAttackMethod(u.unitData.unitName))
        {
            List<Vector3Int> attacks = MovementMethods.GetAttackMethod(u.unitData.unitName).Invoke(r, u, -1);
            DisplayPositions(attacks, u, INDICATOR_STATE.ATTACK);

        }else if (MovementMethods.HasSpreadMethod(u.unitData.unitName))
        {
            List<List<Vector3Int>> spreadCells = MovementMethods.GetSpreadMethod(u.unitData.unitName).Invoke(GlobalHelper.GetRoom(), u, -1, new Vector3Int(9999, 9999, 0));
            foreach(List<Vector3Int> spreadList in spreadCells)
            {
                DisplayPositions(spreadList, u, INDICATOR_STATE.ATTACK);
            }

        }

    }
    public void DisplayPositions(List<Vector3Int> positions, Unit u, INDICATOR_STATE overrideState = INDICATOR_STATE.INACTIVE)
    {
        bool isAttack = (MovementMethods.HasAttackMethod(u.unitData.unitName) && overrideState == INDICATOR_STATE.ATTACK) || !MovementMethods.HasAttackMethod(u.unitData.unitName);
        RoomView r = GlobalHelper.GetRoom();
        foreach (Vector3Int position in positions)
        {
            bool isSelected = (selectedUnit != null && selectedUnit.UID == u.UID);

            //Pool an indicator
            Indicator indicator = getNext();
            //Set it at the position
            indicator.transform.position = transform.position + GetIndicatorPosition(position);
            //Show
            indicator.gameObject.SetActive(true);

            indicator.SetState(INDICATOR_STATE.MOVE, u.isEnemy, isSelected, position);

            if ((selectedUnit == null || isSelected) && isAttack)
            {
                //If there is a unit at this position
                Unit uToBlink = r.GetUnitAt(position);
                if (uToBlink != null && u.UID != uToBlink.UID && u.isEnemy != uToBlink.isEnemy)
                {
                    float blinkSpeed = 1;
                    if (selectedUnit != null && u.actionsLeft > 0)
                    {
                        blinkSpeed = 3;
                    }
                    uToBlink.ToggleBlink(true, blinkSpeed);
                    blinkingUnits.Add(uToBlink);
                    indicator.SetState(INDICATOR_STATE.ATTACK, u.isEnemy, isSelected, position);
                }
                //If ally do nothing
                //If enemy make the sprite blink? 
            }

            if(overrideState != INDICATOR_STATE.INACTIVE)
            {

                indicator.SetState(overrideState, u.isEnemy, isSelected, position);
            }
        }
    }

    List<Unit> blinkingUnits = new List<Unit>();
    public void HideAll()
    {
        foreach(Indicator go in activeIndicators)
        {
            go.SetState(INDICATOR_STATE.INACTIVE,false,false,inactiveVector);
        }


        foreach(Unit u in blinkingUnits)
        {
            u.ToggleBlink(false);
        }
        blinkingUnits.Clear();
        activeIndicators.Clear();

        if (GlobalHelper.GlobalVariables.gameInfos.selected != null && GlobalHelper.GlobalVariables.gameInfos.selected is Unit)
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
            indicator.SetState(INDICATOR_STATE.PLACE_FROM_RESERVE,false,false, cell);
        }

    }


    public void HoverOnSpawnableCells(List<Vector3Int> positions)
    {
        RoomView r = GlobalHelper.GetRoom();

        foreach (Vector3Int position in r.SpawnableCells)
        {
            Indicator indicator = spawnableCellIndicators.Find((x) => x.targetedCell == position);
            if (positions.Contains(r.TilemapToCell( position)))
            {
                if (indicator != null && indicator.currentState == INDICATOR_STATE.PLACE_FROM_RESERVE)
                {
                    indicator.SetState(INDICATOR_STATE.TARGETED,false,true, position);
                }
            }
            else if(indicator != null && indicator.currentState == INDICATOR_STATE.TARGETED)
            {
                indicator.SetState((INDICATOR_STATE.PLACE_FROM_RESERVE),false,false, position);
            }
        }
    }


    public void HideSpawnableCells()
    {
        foreach (Indicator go in spawnableCellIndicators)
        {
            go.SetState(INDICATOR_STATE.INACTIVE,false,false,inactiveVector);
        }
        spawnableCellIndicators.Clear();
    }

    //Refaire ça pour inclure les attaques et les targeted

    public void ShowPossibleUnitMove(Unit u, List<Vector3Int> positions)
    {
        bool hasSpread = MovementMethods.HasSpreadMethod(u.unitData.unitName);
        RoomView r = GlobalHelper.GetRoom();
        bool hasSetSpread = false;
        if (hasSpread)
        {
            List<Vector3Int> finalCells = new List<Vector3Int>();
            List<List<Vector3Int>> spreadCells = MovementMethods.GetSpreadMethod(u.unitData.unitName).Invoke(GlobalHelper.GetRoom(), u, -1, new Vector3Int(9999, 9999, 0));
            foreach (List<Vector3Int> spreadList in spreadCells)
            {
                bool correct = true;
                if (!hasSetSpread)
                {
                    if (positions.Count == 0)
                    {
                        correct = false;
                    }
                    foreach (Vector3Int cell in positions)
                    {

                        if (u.occupiedCells.Contains(cell) || !spreadList.Contains(cell))
                        {
                            correct = false;
                            break;
                        }
                    }
                    foreach (Vector3Int cell in spreadList)
                    {
                        Indicator indicator = activeIndicators.Find((x) => x.transform.position == transform.position + GetIndicatorPosition(cell));
                        if (correct)
                        {

                            if (indicator != null)
                            {
                                finalCells.Add(cell);
                                indicator.SetState(INDICATOR_STATE.TARGETED, false, indicator.isSelected, cell);
                                hasSetSpread = true;
                            }

                        }
                        else if (indicator != null)
                        {

                            indicator.SetState(INDICATOR_STATE.ATTACK, false, indicator.isSelected, cell);

                        }
                    }
                }
                else
                {
                    correct = false;
                    foreach (Vector3Int cell in spreadList)
                    {
                        Indicator indicator = activeIndicators.Find((x) => x.transform.position == transform.position + GetIndicatorPosition(cell));
                        if (indicator  != null && finalCells.Contains(cell))
                        {

                            if (indicator != null)
                            {
                                indicator.SetState(INDICATOR_STATE.TARGETED, false, indicator.isSelected, cell);
                                hasSetSpread = true;
                            }

                        }
                        else if (indicator != null)
                        {

                            indicator.SetState(INDICATOR_STATE.ATTACK, false, indicator.isSelected, cell);

                        }
                    }
                }
                
               
            }

        }
        if((hasSpread && !hasSetSpread)||!hasSpread)
        {

            List<Vector3Int> validPositions = MovementMethods.GetMovementMethod(u.unitData.unitName).Invoke(r, u, -1);
            foreach (Vector3Int position in validPositions)
            {
                Indicator indicator = activeIndicators.Find((x) => x.transform.position == transform.position + GetIndicatorPosition(position));
                if (positions.Contains(position))
                {

                    if (indicator != null)
                    {
                        indicator.SetState(INDICATOR_STATE.TARGETED, false, indicator.isSelected, position);
                    }
                }
                else if (indicator != null)
                {
                    Unit enemy = r.GetUnitAt(position);
                    if (enemy != null && enemy.UID != u.UID && enemy.isEnemy)
                    {
                        indicator.SetState(INDICATOR_STATE.ATTACK, false, indicator.isSelected, position);
                    }
                    else
                    {
                        indicator.SetState(INDICATOR_STATE.MOVE, false, indicator.isSelected, position);
                    }
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
