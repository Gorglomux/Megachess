using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassiveShop : MonoBehaviour
{
    public Transform gridTransform;
    public Transform gridTransformUnits;
    public GameObject prefabPassiveContainer;
    public GameObject prefabUnitContainer;

    public List<ShopContainer> passiveContainers = new List<ShopContainer>();
    public List<ShopUnitContainer> unitContainers = new List<ShopUnitContainer>();

    public ShopContainer hoveredPlayerContainer;
    public Button nextButton;
    // Start is called before the first frame update 
    void Start()
    {


    }
    public void FillPassives()
    {
        GlobalHelper.UI().SetBottomText("Click on an item to buy it!");
        GlobalHelper.UI().DisableButton(GlobalHelper.UI().endTurnButton);
        GlobalHelper.UI().DisableButton(GlobalHelper.UI().abilityButton.button);
        GlobalHelper.UI().DisableButton(GlobalHelper.UI().resetFightButton);

        nextButton.gameObject.SetActive(false);
        StartCoroutine(corFillPassives());
    }
    public IEnumerator corFillPassives()
    {
        List<PassiveData> passives = GlobalHelper.passiveDataList.OrderBy(x=> GlobalHelper.rand.Next()).ToList();
        passives.RemoveAll(x => GlobalHelper.GlobalVariables.player.passiveDatas.Contains(x));
        passives = passives.Take(GlobalHelper.GlobalVariables.gameInfos.shopPassiveChoiceAmount).ToList();

        foreach (PassiveData passiveData in passives)
        {
            GameObject go = GameObject.Instantiate(prefabPassiveContainer, gridTransform);
            ShopContainer pc = go.GetComponent<ShopContainer>();

            EventTrigger et = pc.GetComponent<EventTrigger>();
            // Add PointerEnter event listener
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerClick;
            entry2.callback.AddListener((data) => OnSelectPassiveContainer((PointerEventData)data, pc));
            et.triggers.Add(entry2);

            //EventTrigger.Entry entryDeselect = new EventTrigger.Entry();
            //entryDeselect.eventID = EventTriggerType.Deselect;
            //entryDeselect.callback.AddListener((data) => OnDeselectPlayerContainer((PointerEventData)data));
            //et.triggers.Add(entryDeselect);

            EventTrigger.Entry entryHover = new EventTrigger.Entry();
            entryHover.eventID = EventTriggerType.PointerEnter;
            entryHover.callback.AddListener((data) => OnHoverEnter((PointerEventData)data, pc));
            et.triggers.Add(entryHover);

            EventTrigger.Entry entryExitHover = new EventTrigger.Entry();
            entryExitHover.eventID = EventTriggerType.PointerExit;
            entryExitHover.callback.AddListener((data) => OnHoverExit((PointerEventData)data));
            et.triggers.Add(entryExitHover);

            pc.FillPassiveData(passiveData);
            passiveContainers.Add(pc);

            GlobalHelper.getCamMovement().ShakeCamera(1);
            pc.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        StartCoroutine(corFillUnits());
    }
    public IEnumerator corFillUnits()
    {
        List<UnitData> units = GlobalHelper.unitDataList.OrderBy(x => GlobalHelper.rand.Next()).ToList();
        units.RemoveAll(x => x.baseCost<=0);;
        units = units.Take(GlobalHelper.GlobalVariables.gameInfos.shopUnitChoiceAmount).ToList();
        foreach (UnitData ud in units)
        {
            GameObject go = GameObject.Instantiate(prefabUnitContainer, gridTransformUnits);
            ShopUnitContainer pc = go.GetComponent<ShopUnitContainer>();

            EventTrigger et = pc.GetComponent<EventTrigger>();
            // Add PointerEnter event listener
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerClick;
            entry2.callback.AddListener((data) => OnSelectShopUnitContainer((PointerEventData)data, pc));
            et.triggers.Add(entry2);

            //EventTrigger.Entry entryDeselect = new EventTrigger.Entry();
            //entryDeselect.eventID = EventTriggerType.Deselect;
            //entryDeselect.callback.AddListener((data) => OnDeselectPlayerContainer((PointerEventData)data));
            //et.triggers.Add(entryDeselect);

            EventTrigger.Entry entryHover = new EventTrigger.Entry();
            entryHover.eventID = EventTriggerType.PointerEnter;
            entryHover.callback.AddListener((data) => OnHoverEnterUnit((PointerEventData)data, pc));
            et.triggers.Add(entryHover);

            EventTrigger.Entry entryExitHover = new EventTrigger.Entry();
            entryExitHover.eventID = EventTriggerType.PointerExit;
            entryExitHover.callback.AddListener((data) => OnHoverExit((PointerEventData)data));
            et.triggers.Add(entryExitHover);

            pc.FillUnitData(ud);
            unitContainers.Add(pc);

            GlobalHelper.getCamMovement().ShakeCamera(1);
            pc.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }

        nextButton.gameObject.SetActive(true);
    }

    public void DisplayItems()
    {


    }



    public void OnNext()
    {
        nextButton.gameObject.SetActive(false);   
        StartCoroutine(goNext());
    }

    public IEnumerator goNext()
    {
        foreach (ShopContainer sc in passiveContainers)
        {
            AudioManager.instance.PlaySound("dialogue", 1, UnityEngine.Random.Range(0.8f, 0.9f));
            Destroy(sc.gameObject);
            GlobalHelper.getCamMovement().ShakeCamera(2);
            yield return new WaitForSeconds(0.2f);
        }

        foreach (ShopUnitContainer suc in unitContainers)
        {
            AudioManager.instance.PlaySound("dialogue", 1, UnityEngine.Random.Range(0.8f, 0.9f));
            Destroy(suc.gameObject);
        }

        GlobalHelper.getCamMovement().ShakeCamera(2);
        yield return new WaitForSeconds(0.5f);

        passiveContainers.Clear();
        GlobalHelper.getCamMovement().ShakeCamera(2);
        GlobalHelper.UI().HideHoverInfos();
        AudioManager.instance.PlaySound("dialogue", 1, UnityEngine.Random.Range(0.8f, 0.9f));
        GlobalHelper.GetGameManager().ChangeState(new ChangeRoomState());

    }
    public void OnHoverEnter(PointerEventData data, ShopContainer container)
    {
        AudioManager.instance.PlaySound("sfx_tap", 1, UnityEngine.Random.Range(0.8f, 0.9f));

        GlobalHelper.UI().ShowHoverInfos(container.passiveData);
    }
    public void OnHoverEnterUnit(PointerEventData data, ShopUnitContainer container)
    {
        AudioManager.instance.PlaySound("sfx_tap", 1, UnityEngine.Random.Range(0.8f, 0.9f));

        GlobalHelper.UI().ShowHoverInfos(container.unitWithin);
    }
    public void OnHoverExit(PointerEventData data)
    {
        GlobalHelper.UI().HideHoverInfos();
    }
    public void OnSelectPassiveContainer(PointerEventData data, ShopContainer container)
    {

        if (GlobalHelper.GlobalVariables.player.CanBuy(container.passiveData.passiveCost)){
            GlobalHelper.GlobalVariables.player.Buy(container.passiveData.passiveCost);
            GlobalHelper.GlobalVariables.player.AddPassive(container.passiveData);
            //Buy attempt here 
            AudioManager.instance.PlaySound("dialogue", 1, UnityEngine.Random.Range(0.8f, 0.9f));
            GlobalHelper.getCamMovement().ShakeCamera(0.5f, 0.2f);
            passiveContainers.Remove(container);
            Destroy(container.gameObject);
        }
        else
        {
            GlobalHelper.UI().SetBottomText("Not enough money ! ", 3);
            GlobalHelper.getCamMovement().ShakeCamera(0.5f, 0.2f);
        }

    }

    public void OnSelectShopUnitContainer(PointerEventData data, ShopUnitContainer container)
    {

        if (GlobalHelper.GlobalVariables.player.CanBuy(container.unitData.baseCost))
        {   
            GlobalHelper.GlobalVariables.player.Buy(container.unitData.baseCost);
            GlobalHelper.GlobalVariables.player.AddUnitData(container.unitData);
            //Buy attempt here 
            AudioManager.instance.PlaySound("dialogue", 1, UnityEngine.Random.Range(0.8f, 0.9f));
            GlobalHelper.getCamMovement().ShakeCamera(0.5f, 0.2f);
            unitContainers.Remove(container);
            Destroy(container.gameObject);
        }
        else
        {
            GlobalHelper.UI().SetBottomText("Not enough money ! ", 3);
            GlobalHelper.getCamMovement().ShakeCamera(0.5f, 0.2f);
        }

    }


}
