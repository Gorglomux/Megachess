using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TutorialRoomBase : MonoBehaviour
{

    public int roomIndex;

    public List<GameObject> fogOfWar = new List<GameObject>();
    public Dictionary<int,List<Vector3Int>> events = new Dictionary<int, List<Vector3Int>>();
    public Tilemap eventTilemap;

    // Start is called before the first frame update
    void Start()
    {
        LoadEvents();
        GlobalHelper.GetRoom().OnBoardUpdate += CheckBoardUpdate;
        Initialize();
    }
    public virtual void Initialize()
    {

    }
    public void LoadEvents()
    {
        foreach (Vector3Int position in eventTilemap.cellBounds.allPositionsWithin)
        {
            if (!eventTilemap.HasTile(position))
            {
                continue;
            }
            //Get the Unit data in the global variables 
            //print(position);
            TileBase entity = eventTilemap.GetTile(position);
            if (entity == null || entity.name == "")
            {
            }
            else
            {
                int identifier =int.Parse( entity.name.Split(" ")[1]);
                if (!events.ContainsKey(identifier))
                {
                    events[identifier] = new List<Vector3Int>();
                }
                events[identifier].Add(GlobalHelper.GetRoom().TilemapToCell( position));
            }
        }
        eventTilemap.gameObject.SetActive(false);
    }
    public void CheckBoardUpdate()
    {
        RoomView room = GlobalHelper.GetRoom();
        foreach (Unit u in room.getAllUnits().ToList())
        {
            if (u != null && !u.isEnemy)
            {
                CheckEvent(u.occupiedCells[0]);
            }
        }
    }

    public void EnableFogOfWar(int index)
    {
        foreach(GameObject go in fogOfWar)
        {
            go.SetActive(false);
        }

        if(fogOfWar.Count > index)
        {
            fogOfWar[index].SetActive(true);

        }

    }

    public void  CheckEvent(Vector3Int position)
    {
        foreach(var e in events)
        {
            int identifier = e.Key;
            foreach (Vector3Int cell in e.Value)
            {
                if(cell == position)
                {
                    TriggerEvent(identifier);
                }
            }

        }
    }
    public List<int> triggeredEvents = new List<int>();
    public virtual void TriggerEvent(int identifier)
    {
        if (triggeredEvents.Contains(identifier)){
            return;
        }
        triggeredEvents.Add(identifier);
        switch (identifier)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;

        }

    }
}
