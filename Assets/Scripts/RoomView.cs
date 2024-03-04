using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomView : MonoBehaviour
{
    public Action OnBoardUpdate = delegate {}; 
    public Action<Unit> OnKillUnit = delegate {}; 
    public Room roomData;


    public GameObject unitsParent;
    public Tilemap tilemapFloorWalls;
    public Tilemap tilemapEntities;
    public Tilemap tilemapSpawnableCells;

    public List<Vector3Int> SpawnableCells = new List<Vector3Int>();
    public Unit[,] arrayUnits;  
    // Start is called before the first frame update
    void Start()
    {    
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Tween LoadRoom()
    {
        transform.localScale = Vector3.zero;
        GlobalHelper.GlobalVariables.gameInfos.currentRoom = this;
        arrayUnits = new Unit[tilemapFloorWalls.size.x, tilemapFloorWalls.size.y];
        LoadEntities();
        LoadSpawnableCells();
        //StartCoroutine(testMoveUnitsRight());
        //StartCoroutine(testInstantiateAllyUnitAndFight());
        return AnimateLevel();
    }
    public void LoadSpawnableCells()
    {
        foreach (Vector3Int position in tilemapSpawnableCells.cellBounds.allPositionsWithin)
        {
            if (!tilemapSpawnableCells.HasTile(position))
            {
                continue;
            }
            if(tilemapSpawnableCells.GetTile(position).name == "SpawnableCell")
            {
                SpawnableCells.Add(position);
            }
        }
        tilemapSpawnableCells.gameObject.SetActive(false);
    }
    #region testMethods
    public IEnumerator testMoveUnitsRight()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.65f);
            foreach (Unit u in getAllUnits())
            {
                //print("Moving " + u.name + " " + u.transform.localPosition + " " + u.gameObject + " To the right");
                List<Vector3Int> newPositions = new List<Vector3Int>();
                Vector3Int randomDirection = new Vector3Int(UnityEngine.Random.Range(-1,2), UnityEngine.Random.Range(-1, 2));
                foreach (Vector3Int position in u.occupiedCells)
                {
                    newPositions.Add(position + Vector3Int.left);
                }
                u.Move(newPositions);
            }

        }


    }
    public IEnumerator testInstantiateAllyUnitAndFight()
    {
        yield return null;
        //Instantiate a unit at position
        UnitData ud = GlobalHelper.GetUnitData("Rook");
        Vector3Int p = new Vector3Int(6,0,0);
        //Instantiate the unit 
        GameObject unitGo = GameObject.Instantiate(GlobalHelper.GlobalVariables.unitPrefab);
        Unit u = unitGo.GetComponent<Unit>();
        u.transform.parent = unitsParent.transform;
        u.transform.localScale = Vector3.one;
        u.transform.position = Vector3.zero;
        //Put it in the cell to world position 
        u.transform.localPosition = GetCenter(CellToTilemap(p)) + new Vector3(0.01f, 0, 0);
        //Set a different palette, then initialize
        //u.basePaletteIndex = 5;
        //Assign the tile in the array 
        arrayUnits[p.x, p.y] = u;
        u.occupiedCells.Add(p);

        u.Initialize(ud, true); 

    }
    #endregion

    //doscale to 1, screen shake , pop the name in the UI (typewriter?), after 1 second load the text
    public Tween AnimateLevel()
    {

        transform.localScale = Vector3.one;
        return null;
    }
    public Vector3 GetCenter(Vector3Int tilePosition)
    {
        return tilemapEntities.CellToLocal(tilePosition) + (tilemapEntities.cellSize / 2);
    }
    public void LoadEntities()
    {
        //For in entities
        foreach (Vector3Int position in tilemapEntities.cellBounds.allPositionsWithin)
        {
            if (!tilemapEntities.HasTile(position))
            {
                continue;
            }
            //Get the Unit data in the global variables 
            //print(position);
            TileBase entity = tilemapEntities.GetTile(position);
            UnitData ud = GlobalHelper.GetUnitData(entity.name);
            if (ud == null)
            {
                Debug.LogError("Invalid Unit data for name : " + entity.name);
            }
            else
            {
                Unit u = CreateUnit(ud, true);
                PlaceUnitOnMap(u, position);
            }
        }

        //Form megas when possible without animation 
        foreach(Unit u in getAllUnits().ToList())
        {
            if (u != null)
            {
                List<Vector3Int> result = CheckMega(u, u.occupiedCells[0]);
                if (result != null && result.Count>0)
                {
                    MakeMega(u,result);
                }
            }

        }
        //Hide the tilemap 
        tilemapEntities.gameObject.SetActive(false);
    }


    public List<Unit> getAllUnits()
    {
        List<Unit> units = new List<Unit> ();
        foreach(Unit u in arrayUnits)
        {
            if(u != null)
            {
                //Check if mega 
                if (units.Find((x) => u.UID == x.UID))
                {
                    //print("Mega detected, not adding unit");
                    continue;
                }
                else
                {
                    units.Add(u);
                }
            }
        }
        return units;
    }

    public Vector3Int TilemapToCell(Vector3Int position)
    {
        return position + Abs(tilemapEntities.cellBounds.position);
    }

    public Vector3Int CellToTilemap(Vector3Int position)
    {
        return position - Abs(tilemapEntities.cellBounds.position);
    }

    public Vector3Int Abs(Vector3Int v)
    {
        return new Vector3Int(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }


    public bool isValidMove(Unit u, List<Vector3Int> targetDestination)
    {
        bool isValid = true;
        if (targetDestination.Count == u.occupiedCells.Count)
        {

            //Movement stops if there is a unit 
            foreach (Vector3Int position in targetDestination)
            {
                if (!InBounds(position))
                {
                    isValid = false;
                    break;
                }
                Unit unitAtPosition = GetUnitAt(position);
                if (unitAtPosition != null && unitAtPosition.UID != u.UID)
                {
                    isValid = false;
                    break;
                }
                TileBase tileBase = GetTileAt(position);
                if (tileBase == null || tileBase.name != GlobalHelper.GlobalVariables.TILE_GROUND)
                {
                    isValid = false;
                    break;
                }
            }

        }
        else
        {
            isValid = false;
        }
        return isValid;
    }
    public bool isValidAttack(Unit u, List<Vector3Int> targetDestination)
    {
        bool isValid = true;
        if (targetDestination.Count == u.occupiedCells.Count)
        {
            //Movement stops if there is a unit 
            foreach (Vector3Int position in targetDestination)
            {
                if (!InBounds(position))
                {
                    isValid = false;
                    break;
                }
                Unit target = GetUnitAt(position);
                if (target != null)
                {
                    if(u.isEnemy != target.isEnemy || u.unitData.canTargetAllies ||u.UID == target.UID)
                    {

                    }
                    else
                    {
                        isValid = false;
                        break;
                    }
                }
            }

        }
        else
        {
            isValid = false;
        }
        return isValid;
    }

    //IsValidAttack
    public Unit GetUnitAt(Vector3Int position)
    {
        return arrayUnits[position.x, position.y];
    }
    public List<Unit> GetUnitsAt(List<Vector3Int> positions)
    {
        List<Unit> units = new List<Unit>();
        foreach (Vector3Int position in positions)
        {
            Unit u = GetUnitAt(position);
            if(u != null)
            {
                //Check if mega 
                if (units.Find((x) => u.UID == x.UID))
                {
                    print("Mega detected, not adding unit");
                    continue;
                }
                else
                {
                    units.Add(u);
                }
            }
            
        }
        return units;
    }

    public TileBase GetTileAt(Vector3Int position)
    {
        Vector3Int offset = CellToTilemap(position);
        return tilemapFloorWalls.GetTile(offset); ;
    }

    public void MoveUnit(Unit u, List<Vector3Int> positions)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3Int oldCell = u.occupiedCells[i];
            arrayUnits[oldCell.x, oldCell.y] = null;

            Vector3Int offset = positions[i];
            arrayUnits[offset.x, offset.y] = u;
            u.occupiedCells[i] = offset;
        }
        OnBoardUpdate();
    }

    public bool InBounds(Vector3 position)
    {

        return (position.x >= 0 && position.x < arrayUnits.GetLength(0)) && (position.y >= 0 && position.y < arrayUnits.GetLength(1));
    }

    public void DestroyUnit(Unit u)
    {
        //Remove the unit from the grid
        foreach(Vector3Int position in u.occupiedCells)
        {
            arrayUnits[position.x, position.y] = null;
        }
        OnKillUnit(u);
        u.gameObject.SetActive(false);
        //Destroy(u.gameObject);
    }
    public List<Unit> destroyedUnitsThisFight = new List<Unit>();

    public void CleanUpFight()
    {
        foreach(Unit u in destroyedUnitsThisFight)
        {
            Destroy(u.gameObject);
        }
        destroyedUnitsThisFight.Clear();
    }
    public List<Vector3Int> CheckMega(Unit baseUnit, Vector3Int baseCell)
    {
        List<Vector3Int> result = new List<Vector3Int> ();
        int megaToCheck = 0;

        List<Vector3Int> mega = new List<Vector3Int>() { baseCell };
        while (mega.Count > 0)
        {
            megaToCheck++;
            mega.Clear();
            for (int y = -megaToCheck; y <= megaToCheck; y++)
            {
                for (int x = -megaToCheck; x <= megaToCheck; x++)
                {
                    List<Vector3Int> temp =  CheckBox(baseUnit, baseCell + new Vector3Int(x, y, 0), megaToCheck + 1);
                    if (temp != null)
                    {
                        mega = temp;
                    }
                }
            }
            if(mega.Count != 0)
            {
                result = new List<Vector3Int>(mega);
            }
        }

        print(" Mega size is " + megaToCheck);
        //Foreach dans les unités
        //Si l'unité est déjà dans un mega avec une taille similaire on le fait pas 

        foreach (Vector3Int v in result)
        {
            Unit u = GetUnitAt(v);
            if (u.occupiedCells.Count >= megaToCheck * megaToCheck)
            {
                print("Mega is already bigger than what is attempted to create, aborting;");
                return null;
            }
            else
            {
                foreach(Vector3Int cell in u.occupiedCells)
                {
                    if (!result.Contains(cell))
                    {
                        return null;
                    }
                }
            }
        }
        return result;
    }
    public List<Vector3Int> CheckBox(Unit baseUnit, Vector3Int start, int megaSize)
    {
        List<Vector3Int> result = new List<Vector3Int>(); 
        for(int y  = 0; y < megaSize; y++)
        {
            for(int x = 0; x < megaSize; x++)
            {
                Vector3Int toCheck = start + new Vector3Int(x, y);
                if (InBounds(toCheck))
                {
                    Unit u = arrayUnits[toCheck.x, toCheck.y];

                    if (u != null&& u.unitData ==baseUnit.unitData && u.isEnemy == baseUnit.isEnemy)
                    {
                        result.Add(toCheck);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        if (result.Count == megaSize * megaSize)
        {
            return result;
        }
        return null;
    }
    public void MakeMega(Unit template, List<Vector3Int> positions)
    {
        //Instantiate the unit 
        GameObject unitGo = GameObject.Instantiate(GlobalHelper.GlobalVariables.unitPrefab);
        Unit u = unitGo.GetComponent<Unit>();
        u.transform.parent = unitsParent.transform;
        u.transform.localScale = Vector3.one * Mathf.Sqrt(positions.Count);
        u.transform.position = Vector3.zero;
        //Put it in the cell to world position 
        foreach(Vector3Int position in positions)
        {
            Unit unit = GetUnitAt(position);
            Destroy(unit.gameObject);
            //Assign the tile in the array 
            arrayUnits[position.x, position.y] = u;
        }
        u.occupiedCells.AddRange(positions);
        u.Initialize(template.unitData, template.isEnemy );
        u.transform.localPosition = u.GetWorldPosition();
    }

    public Unit CreateUnit(UnitData ud, bool isEnemy)
    {
        //Instantiate the unit 
        GameObject unitGo = GameObject.Instantiate(GlobalHelper.GlobalVariables.unitPrefab);
        Unit u = unitGo.GetComponent<Unit>();
        u.transform.parent = unitsParent.transform;
        u.transform.localScale = Vector3.one;
        u.transform.position = Vector3.zero;

        u.Initialize(ud, isEnemy);
        return u;
    }
    public void PlaceUnitOnMap(Unit u, Vector3Int position)
    {
        //Put it in the cell to world position 
        u.transform.localPosition = tilemapEntities.CellToLocal(position) + (tilemapEntities.cellSize / 2) + new Vector3(0.01f, 0, 0);
        Vector3Int p = TilemapToCell(position);

        //Assign the tile in the array 
        arrayUnits[p.x, p.y] = u;
        u.occupiedCells.Add(p);
        OnBoardUpdate();

    }

    public List<Unit> GetEnemies()
    {
        List<Unit> enemies = new List<Unit>();
        foreach(Unit u in getAllUnits())
        {
            if (u.isEnemy)
            {
                enemies.Add(u);
            }
        }
        return enemies;
    }
    public List<Unit> GetAllies()
    {
        List<Unit> allies = new List<Unit>();
        foreach (Unit u in getAllUnits())
        {
            if (!u.isEnemy)
            {
                allies.Add(u);
            }
        }
        return allies;
    }

}
