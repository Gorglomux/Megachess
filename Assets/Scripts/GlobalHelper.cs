using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum EFFECT_ACTIVATION_TIME
{
    ON_BEGIN_FIGHT,
    ON_END_FIGHT,
    ON_UNIT_PLAYED,
    ON_UNIT_KILLED,
    ON_APPLY,
    BEFORE_ATTACK,
    AFTER_ATTACK,
    ON_END_TURN,
    ON_START_TURN,

}

public class GlobalHelper
{

    public static GlobalVariables GlobalVariables;
    
    public Transform root;

    public static int gridCount = -1;

    public static Dictionary<string, List<Room>> roomDatas;

    public static List<Area> areaList;
    public static List<AbilityData> abilityList;
    public static System.Random rand;
    public static List<UnitData> unitDataList;
    public static List<EffectData> effectDataList;
    public static List<PassiveData> passiveDataList;
    public static List<PlayerData> playerDataList;

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
    public static float DEFAULT_CAMERA_ZOOM_STRENGTH = 1f;


    #endregion
    public static float ScreenShakeMultiplier = 1;
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

    public static GameManager GetGameManager()
    {
        return GlobalVariables.gameManager;
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
        abilityList = Resources.LoadAll<AbilityData>("Data/Abilities").ToList();
        effectDataList = Resources.LoadAll<EffectData>("Data/Effects").ToList();
        playerDataList = Resources.LoadAll<PlayerData>("Data/Players").ToList();
        passiveDataList = Resources.LoadAll<PassiveData>("Data/Passives").ToList();
    }
    public static GameObject GetRoomPrefab(string roomIndex)
    {
        return Resources.Load<GameObject>("Prefabs/Rooms/" + roomIndex);
    }

    public static UnitData GetUnitData(string identifier)
    {
        return unitDataList.FirstOrDefault((x)=> x.unitName == identifier);
    }

    public static AbilityData GetAbilityData(string identifier)
    {
        return abilityList.FirstOrDefault((x) => x.name == identifier);
    }
    public static PlayerData GetPlayerData(string identifier)
    {
        return playerDataList.FirstOrDefault((x) => x.name == identifier);
    }
    public static EffectData GetEffectData(string identifier)
    {
        return effectDataList.FirstOrDefault((x) => x.name == identifier);
    }

    public static PassiveData GetPassiveData(string identifier)
    {
        return passiveDataList.FirstOrDefault((x) => x.name == identifier);
    }

    public static BaseAbility abilityLookup(AbilityData ad)
    {
        BaseAbility ability = null;
        switch (ad.name)
        {
            case "ExtraTurn":
                ability = new ExtraTurnAbility(ad);
                break;
            case "Thirst":
                ability = new ThirstAbility(ad);
                break;
            case "Guardian":
                ability = new GuardianAbility(ad);
                break;
            case "TripleCharge":
                ability = new TripleChargeAbility(ad);
                break;
            case "Clone":
                ability = new CloneAbility(ad);
                break;
            case "Promote":
                ability = new PromoteAbility(ad);
                break;
            default:
                Debug.LogError("Invalid ability lookup");
                break;
        }
        return ability;
    }
    public static BaseEffect effectLookup(EffectData ed)
    {
        BaseEffect effect = null;
        switch (ed.name)
        {
            case "Bloodlust":
                effect = new BloodlustEffect(ed);
                break;
            case "Shielded":
                effect = new ShieldedEffect(ed);
                break;
            case "Charged":
                effect = new ChargedEffect(ed);
                break;
            case "Stationary":
                effect = new StationaryEffect(ed);
                break;
            case "Cleave":
                effect = new CleaveEffect(ed);
                break;
            case "Guarded":
                effect = new GuardedEffect(ed);
                break;
            default:
                Debug.LogError("Invalid Effect lookup");
                break;
        }
        return effect;
    }
    public static BasePassive passiveLookup(PassiveData passiveData)
    {
        BasePassive bp = null;
        switch (passiveData.name)
        {
            case ("BloodShield"):
                bp = new BloodShieldPassive(passiveData);
                break;
            case ("CaptureMaster"):
                bp = new CaptureMasterPassive(passiveData);
                break;
            case ("EarlyStep"):
                bp = new EarlyStepPassive(passiveData);
                break;
            case ("Easygoing"):
                bp = new EasygoingPassive(passiveData);
                break;
            case ("Martyr"):
                bp = new MartyrPassive(passiveData);
                break;
            case ("PassiveIncome"):
                bp = new PassiveIncomePassive(passiveData);
                break;
            case ("Pawnception"):
                bp = new PawnceptionPassive(passiveData);
                break;
            case ("Shortcut"):
                bp = new ShortcutPassive(passiveData);
                break;
        }
        return bp;


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

    public static string PluralOrSingular(string singular, string plural, int value)
    {
        if (value == 1)
        {
            return singular;
        }
        else
        {
            return plural;
        }
    }
    public static bool isPlayerTurn()
    {


        return GetGameManager().playerTurn;
    }

    public static Action<object> GetEffectActivationEvent(EFFECT_ACTIVATION_TIME activationTime)
    {
        Action<object> action = null;
        switch (activationTime)
        {
            case EFFECT_ACTIVATION_TIME.ON_APPLY:
                action = GetGameManager().OnStartFight;
                break;
            case EFFECT_ACTIVATION_TIME.BEFORE_ATTACK:
                break;
            case EFFECT_ACTIVATION_TIME.AFTER_ATTACK:
                break;
            case EFFECT_ACTIVATION_TIME.ON_END_TURN:
                action = GetGameManager().OnPlayerEndTurn;
                break;
            case EFFECT_ACTIVATION_TIME.ON_START_TURN:
                action = GetGameManager().OnStartTurn;
                break;
        }
        return action;
    }
    public static bool CheckUnitFightState()
    {
        GameManager gm = GetGameManager();
        return gm.currentState is FightState || gm.currentState is UnitPlaceState || gm.currentState is TutorialFightState || gm.currentState is TutorialUnitPlaceState;
    }

    public static bool IsMouseOnReserve(Vector3 position)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(UI().ReserveRectTransform, position);
    }

    public static int GetResetCost()
    {
        return (int)Mathf.Clamp(GlobalVariables.gameInfos.baseResetCost + GetGameManager().currentResetCost - GetGameManager().resetReduction , 1, 10);
    }

}
