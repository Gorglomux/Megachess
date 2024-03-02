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

    public List<Unit> units = new List<Unit>();
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
        LoadEntities();

        return AnimateLevel();
    }

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
            print(position);
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

                //Assign the tile in the 
                
            }
        }

        //Form megas when possible without animation 
        //Hide the tilemap 
        tilemapEntities.gameObject.SetActive(false);
    }
}
