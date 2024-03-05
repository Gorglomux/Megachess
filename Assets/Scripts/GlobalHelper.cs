using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GlobalHelper
{
    public static GlobalVariables GlobalVariables;


    public static int gridCount = -1;


    public static Dictionary<string, List<Room>> roomDatas;

    public static List<Area> areaList;
    public static System.Random rand;
    public static List<UnitData> unitDataList;

    public static int NEXT_UID = 50;

    #region tween variables
    public static float TWEEN_DURATION_MEGA = 0.5f;
    public static float TWEEN_DURATION_MOVE = 0.5f;
    public static float TWEEN_OVERSHOOT_MOVE = 4f;
    public static float TWEEN_OVERSHOOT_MEGA = 8f;
    public static float DEFAULT_CAMERA_SHAKE_DURATION = 1f;
    public static float DEFAULT_CAMERA_MOVE_DURATION = 2f;
    public static float DEFAULT_CAMERA_FLASH_BRIGHTNESS = 1.2f;
    public static float DEFAULT_CAMERA_FLASH_DURATION = 0.12f;

    public static float CAM_SHAKE_ATTACK = 5f;
    public static float CAM_SHAKE_MEGA = 6f;
    public static float DURATION_SHAKE_MEGA = 2f;
    public static float DEFAULT_CAMERA_ZOOM_DURATION = 0.5f;
    public static float DEFAULT_CAMERA_ZOOM_STRENGTH = 0.6f;

    
    #endregion
    public static int GetUID()
    {
        NEXT_UID++;
        return NEXT_UID;
    }
    public static GlobalVariables getGlobal()
    {
        return GlobalVariables;
    }

    public static CameraMovementManager getCamMovement()
    {
        return GlobalVariables.cameraMovement;
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
        if(GlobalVariables.GenerateSeed)
        {
            GlobalVariables.seed = Environment.TickCount;

        }
        rand = new System.Random(GlobalVariables.seed);
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
        unitDataList = Resources.LoadAll<UnitData>("Data/Units").ToList();
    }
    public static GameObject GetRoomPrefab(string roomIndex)
    {
        return Resources.Load<GameObject>("Prefabs/Rooms/" + roomIndex);
    }

    public static UnitData GetUnitData(string identifier)
    {
        return unitDataList.FirstOrDefault((x)=> x.unitName == identifier);
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0);
        return worldPosition;
    }

    public static UIManager UI()
    {
        return GlobalVariables.UIManager;
    }
    #region ROOM_MANIPULATION

    public static RoomView GetRoom()
    {
        return GlobalVariables.gameInfos.currentRoom;
    }
    
    #endregion

    public static IState GetGameState()
    {
        return GlobalVariables.gameInfos.gameState;
    }
}
