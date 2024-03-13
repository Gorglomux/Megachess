using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Action<object> OnUnitPlayed = delegate { };


    public Room roomToDebug;

    public RoomView currentRoom;

    public Queue<Room> roomQueue = new Queue<Room>();
    
    //Debug for now, linear exploration 
    public Queue<Area> areaQueue = new Queue<Area>();

    public  Coroutine paletteCoroutine;

    public GameObject roomRoot;

    public bool firstAreaLoaded = false;
    public IState currentState;
    public int roomIndex = 0;
    public int extraTurns = 0;
    public int areaBeaten = 0;
    public int roomToClearAmount = 0;
    public int roomToClearBonus = 0;


    public bool firstAttackThisArea = false;
    public bool playerTurn = false;

    public bool wasInShop = false;
    public int canCaptureThisFight = 1;

    public int currentResetCost = 0;
    public int resetReduction = 0;

    public bool isActionInProgress = false;

    public bool isPillage = false;
    public int pillageMoney = 0;
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
            if (!CheckTutorial())
            {
                print("Skipping tutorial !");
                areaQueue.Dequeue();
                ChangeState(new TitleScreenState());
            }
            else
            {
                ChangeState(new ChangeRoomState());
            }
        }

        OnRoomLoaded += StartSave;
        //else
        //{
        //    LoadArea(GlobalHelper.areaList[0]);

        //}
        SetVictoryAmount();
        roomToClearAmount = GlobalHelper.GlobalVariables.gameInfos.roomToClearBaseAmount;
    }

    public void SetVictoryAmount()
    {
        if (PlayerPrefs.GetInt("ClassesInitialized", 0) == 1)
        {
            return;
        }
        foreach(PlayerData playerData in GlobalHelper.playerDataList)
        {

            PlayerPrefs.SetInt(playerData.name, 0);

        }
        foreach(PlayerData data in GlobalHelper.GlobalVariables.gameInfos.PlayerDatasUnlockedAtStart)
        {
            PlayerPrefs.SetInt(data.name, 1);
            print(PlayerPrefs.GetInt(data.name));
        }
        PlayerPrefs.SetInt("ClassesInitialized", 1);
        PlayerPrefs.Save();
    }

    public bool shouldGetBackToTitle = false;
    public void BackToTitle()
    {
        GlobalHelper.GlobalVariables.bloodSplatManager.Cleanup();
        shouldGetBackToTitle = true;
        ChangeState(new ChangeRoomState());
    }
    public void StartSave()
    {
        StartCoroutine(DelayedSave());

    }
    public IEnumerator DelayedSave()
    {
        yield return null;
        yield return null;
        print("Saving Backup !");
        GlobalHelper.GlobalVariables.player.BackupInventory();
    }
    public bool CheckTutorial()
    {

        return PlayerPrefs.GetInt("PlayTutorial", 1) == 1? true:false ;
    }

    public void StartGame()
    {
        StartCoroutine(corStartGame());
    }
    public IEnumerator corStartGame()
    {
        yield return new WaitForSeconds(1f);
        ChangeState(new ChangeRoomState()); // SelectAreaState? 
    }
    public Tween LoadArea(Area a)
    {
        currentResetCost = 0;
        if(a.overrideRoomCount > 0)
        {
            roomToClearAmount = a.overrideRoomCount;
        }
        else
        {
            roomToClearAmount = /*areaBeaten+*/  roomToClearBonus + GlobalHelper.GlobalVariables.gameInfos.roomToClearBaseAmount;

        }
        firstAttackThisArea = false;
        GlobalHelper.GlobalVariables.gameInfos.currentArea = a;
        print("Loading area " + a.name);

        List<Room> roomsToAdd = a.GetRooms();
        roomsToAdd = roomsToAdd.Take(roomToClearAmount).ToList();
        roomsToAdd.ForEach(r => roomQueue.Enqueue(r));
        
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
        if(r.roomPrefab == null)
        {
            Debug.LogError("Error :" + r.name + " Does not have an associated room prefab");
        }
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
            currentState?.OnExit(this);
        }

        currentState = s;
        GlobalHelper.GlobalVariables.gameInfos.gameState = currentState;
        currentState?.OnEntry(this);
    }
    private void Update()
    {
        if(currentState != null)
        {
           currentState?.OnUpdate(this);
        }

        if (GlobalHelper.GlobalVariables.gameInfos.shouldClearPlayerPrefs)
        {
            GlobalHelper.GlobalVariables.gameInfos.shouldClearPlayerPrefs = false;
            PlayerPrefs.DeleteAll();
        }
    }

    public Unit CreateUnit(UnitData ud, bool isEnemy)
    {
        //Instantiate the unit 
        GameObject unitGo = GameObject.Instantiate(GlobalHelper.GlobalVariables.unitPrefab);
        Unit u = unitGo.GetComponent<Unit>();
        u.transform.localScale = Vector3.one;
        u.transform.position = Vector3.zero;

        u.Initialize(ud, isEnemy);
        return u;
    }
}
