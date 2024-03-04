using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    public event Action OnAreaLoaded = delegate { };
    public event Action OnRoomLoaded = delegate { };
    public Room roomToDebug;

    public RoomView currentRoom;

    public Queue<Room> roomQueue = new Queue<Room>();

    private Coroutine paletteCoroutine;

    public GameObject roomRoot;


    public IState currentState;
    // Start is called before the first frame update
    void Start()
    {
        OnRoomLoaded += GlobalHelper.GlobalVariables.UIManager.OnRoomChange;
        OnAreaLoaded += GlobalHelper.GlobalVariables.UIManager.OnAreaChange;
        if (roomToDebug != null)
        {
            print("Debug Initiative launched");
            LoadRoom(roomToDebug);
        }
        else
        {
            LoadArea(GlobalHelper.areaList[0]);

        }
        ChangeState(new UnitPlaceState());
    }
    void test() { print("testetste"); }
    public void LoadArea(Area a)
    {
        GlobalHelper.GlobalVariables.gameInfos.currentArea = a;
        print("Loading area " + a.name);
        a.GetRooms().ForEach(r => roomQueue.Enqueue(r));

        if (paletteCoroutine != null)
        {
            StopCoroutine(paletteCoroutine);
        }
        paletteCoroutine = StartCoroutine(LoadPalette(a.paletteIndex, a.colorText));
        LoadRoom(roomQueue.Dequeue());
        OnAreaLoaded.Invoke();

    }
    public IEnumerator LoadPalette(int paletteIndex, Color textColor)
    {
        Material globalMaterial = GlobalHelper.GlobalVariables.paletteMaterial; ;
        GlobalHelper.GlobalVariables.areaMaterial = new Material(globalMaterial);
        GlobalHelper.GlobalVariables.areaMaterial.SetFloat("_PaletteIndex", paletteIndex);
        GlobalHelper.GlobalVariables.areaMaterial.SetFloat("_IndexToLerpTo", 0);
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
    public void LoadRoom(Room r)
    {
        currentRoom = GameObject.Instantiate(r.roomPrefab,roomRoot.transform).GetComponent<RoomView>();
        GlobalHelper.GlobalVariables.gameInfos.currentRoom = currentRoom;
        currentRoom.LoadRoom();
        OnRoomLoaded();
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
        currentState.OnUpdate(this);
    }
}
