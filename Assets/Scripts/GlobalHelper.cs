using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalHelper
{
    public static GlobalVariables GlobalVariables;


    public static int gridCount = -1;


    public static Dictionary<string, List<Room>> roomDatas;

    public static List<Area> areaList;

    public static GlobalVariables getGlobal()
    {
        return GlobalVariables;
    }


    public static void SetGlobal(GlobalVariables go)
    {
        GlobalVariables = go;
        
    }


    /// <summary>
    /// Load all the data in a way that is accessible for other classes.
    /// </summary>
    public static void LoadGame()
    {
        roomDatas = new Dictionary<string, List<Room>>();
        areaList = Resources.LoadAll<Area>("Areas").ToList();
        int roomCount = 0;
        foreach(Area area in areaList)
        {
            roomDatas[area.name] = Resources.LoadAll<Room>("Areas/" + area.name).ToList();
            foreach(Room room in roomDatas[area.name])
            {
                room.roomPrefab = GetRoomPrefab(room.name);
                if(room.roomPrefab == null)
                {
                    Debug.LogError("Room " + room.name + " Has no linked prefab");
                }
                else
                {
                    room.roomPrefab.GetComponent<RoomView>().roomData = room;
                }
            }
            roomCount += roomDatas[area.name].Count;
            area.roomList = roomDatas[area.name]; 
        }
        Debug.Log("Sucessfully loaded" + roomDatas.Keys.Count + " Areas");
    }
    public static GameObject GetRoomPrefab(string roomIndex)
    {
        return Resources.Load<GameObject>("Prefabs/Rooms/" + roomIndex);
    }

}
