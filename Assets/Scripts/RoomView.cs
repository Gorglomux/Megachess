using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomView : MonoBehaviour
{
    public Room roomData;


    public GameObject unitsParent;
    public Tilemap tilemapFloorWalls;
    public Tilemap tilemapEntities;

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
        StartCoroutine(testMoveUnitsRight());
        return AnimateLevel();
    }
    #region testMethods
    public IEnumerator testMoveUnitsRight()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(1f);
            foreach (Unit u in getAllUnits())
            {
                print("Moving " + u.name + " " + u.transform.localPosition + " " + u.gameObject + " To the right");
                List<Vector3Int> newPositions = new List<Vector3Int>();
                foreach (Vector3Int position in u.occupiedCells)
                {
                    newPositions.Add(position + Vector3Int.up);
                }
                u.Move(newPositions);
            }

        }


    }
    #endregion

    //doscale to 1, screen shake , pop the name in the UI (typewriter?), after 1 second load the text
    public Tween AnimateLevel()
    {

        transform.localScale = Vector3.one;
        return null;
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
                //Instantiate the unit 
                GameObject unitGo = GameObject.Instantiate(GlobalHelper.GlobalVariables.unitPrefab);
                Unit u = unitGo.GetComponent<Unit>();
                u.Initialize(ud);
                u.transform.parent = unitsParent.transform;
                u.transform.localScale = Vector3.one;
                u.transform.position = Vector3.zero;
                //Put it in the cell to world position 
                u.transform.localPosition = tilemapEntities.CellToLocal(position) +  (tilemapEntities.cellSize / 2) + new Vector3(0.01f,0,0);
                //u.transform.localPosition = tilemapEntities.CellToLocal(position) + tilemapEntities.GetLayoutCellCenter() + new Vector3(0.01f,0,0);

                Vector3Int p = TilemapToCell(position);
                //Assign the tile in the array 
                arrayUnits[p.x,p.y] = u;
                u.occupiedCells.Add(p);
                print(u.transform.localPosition + " " + p);
            }
        }

        //Form megas when possible without animation 
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
                if (GetUnitAt(position) != null)
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
    //IsValidAttack
    public Unit GetUnitAt(Vector3Int position)
    {
        return arrayUnits[position.x, position.y];
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
    }

    public bool InBounds(Vector3 position)
    {

        return (position.x >= 0 && position.x < arrayUnits.GetLength(0)) && (position.y >= 0 && position.y < arrayUnits.GetLength(1));
    }


}
