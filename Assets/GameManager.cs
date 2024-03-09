using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    public Action OnAreaLoaded = delegate { };
    public Action OnRoomLoaded = delegate { };


    public Action<object> OnRoomCleared = delegate { };
    public Action<object> OnAreaCleared = delegate { };



    //public Action<object> OnBeforeAttack = delegate { };
    //public Action<object> OnAfterAttack = delegate { };
    public Action<object> OnPlayerEndTurn = delegate { };
    public Action<object> OnStartTurn = delegate { };
    public Action<object> OnStartFight= delegate { };
    public Action<object> OnKillUnit = delegate { };


    public Room roomToDebug;

    public RoomView currentRoom;

    public Queue<Room> roomQueue = new Queue<Room>();
    
    //Debug for now, linear exploration 
    public Queue<Area> areaQueue = new Queue<Area>();

    private Coroutine paletteCoroutine;

    public GameObject roomRoot;

    public bool firstAreaLoaded = false;
    public IState currentState;
    public int roomIndex = 0;
    public int extraTurns = 0;
    // Start is called before the first frame update
    void Start()
    {
        GlobalHelper.areaList.ForEach(area => areaQueue.Enqueue(area));
        OnRoomLoaded += GlobalHelper.GlobalVariables.UIManager.OnRoomChange;
        OnAreaLoaded += GlobalHelper.GlobalVariables.UIManager.OnAreaChange;
        if (roomToDebug != null)
        {
            GlobalHelper.GlobalVariables.gameInfos.currentArea = GlobalHelper.areaList[0];
            print("Debug Initiative launched");
            LoadRoom(roomToDebug).onComplete += () =>
            {

                ChangeState(new UnitPlaceState());
            };
        }
        else
        {
            ChangeState(new ChangeRoomState());
        }
        //else
        //{
        //    LoadArea(GlobalHelper.areaList[0]);

        //}
    }

    public Tween LoadArea(Area a)
    {
        GlobalHelper.GlobalVariables.gameInfos.currentArea = a;
        print("Loading area " + a.name);
        a.GetRooms().ForEach(r => roomQueue.Enqueue(r));

        if (paletteCoroutine != null)
        {
            StopCoroutine(paletteCoroutine);
        }
        paletteCoroutine = StartCoroutine(LoadPalette(a.paletteIndex, a.colorText));
        OnAreaLoaded.Invoke();
        GlobalHelper.GlobalVariables.gameInfos.areaSize = roomQueue.Count;
        GlobalHelper.UI().UpdateRoomCount(roomIndex + 1, GlobalHelper.GlobalVariables.gameInfos.areaSize);
        return LoadRoom(roomQueue.Dequeue());

    }
    public void OnEndTurn()
    {
        OnPlayerEndTurn(null);
    }
    public Tween LoadNextArea()
    {
        roomIndex = 0;
        if (!firstAreaLoaded)
        {
            firstAreaLoaded = true;
        }
        return LoadArea(areaQueue.Dequeue());
    }
    public Tween LoadNextRoom()
    {
        if (roomQueue.Count > 0)
        {
            CleanPreviousRoom();
            roomIndex++;
            return LoadRoom(roomQueue.Dequeue()); ;
        }
        else
        {
            return null;
        }
    }

    public void CleanPreviousRoom()
    {
        if(currentRoom != null)
        {
            Destroy(currentRoom.gameObject);

        }

    }

    public IEnumerator LoadPalette(int paletteIndex, Color textColor)
    {
        Material globalMaterial = GlobalHelper.GlobalVariables.paletteMaterial; ;
        print("Setting Palette ! ");
        //Set the palette to the room
        globalMaterial.SetFloat("_IndexToLerpTo", paletteIndex);
        float lerpValue = 0f;
        Tween t = DOTween.To(() => lerpValue, x => lerpValue = x, 1, 3);
        t.onUpdate += () =>
        {
            globalMaterial.SetFloat("_Lerp",lerpValue);
        };
        yield return t.WaitForCompletion();

        globalMaterial.SetFloat("_Lerp", 0);
        globalMaterial.SetFloat("_IndexToLerpTo", 0);
        globalMaterial.SetFloat("_PaletteIndex", paletteIndex);
        //Set the material to the room and the floor in the room prefab
        paletteCoroutine = null;
    }
    public Tween LoadRoom(Room r)
    {
        CleanPreviousRoom();
        currentRoom = GameObject.Instantiate(r.roomPrefab,roomRoot.transform).GetComponent<RoomView>();
        GlobalHelper.GlobalVariables.gameInfos.currentRoom = currentRoom;
        Tween t = currentRoom.LoadRoom();
        t.onComplete += () =>
        {
            OnRoomLoaded();
        };
        return t;
    }

    public void ChangeState(IState s)
    {
        if(currentState != null)
        {
            currentState.OnExit(this);
        }

        currentState = s;
        GlobalHelper.GlobalVariables.gameInfos.gameState = currentState;
        currentState.OnEntry(this);
    }

    private void Update()
    {
        if(currentState != null)
        {
           currentState.OnUpdate(this);
        }
    }
     
}
