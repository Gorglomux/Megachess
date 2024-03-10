using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassivesMenu : MonoBehaviour
{
    public GameObject containerImagePrefab;
    public Transform layoutTransform;
    public Transform startPosition;
    public Transform targetPosition;
    public List<GameObject> images = new List<GameObject>();    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPosition.transform.position;
    }
    bool extended = false;

    public void ExtendOrRetract()
    {
        if (!extended)
        {
            Extend();
        }
        else
        {
            Retract();
        }
    }
    public void Extend()
    {
        transform.DOMove(targetPosition.position, 0.5f).SetEase(Ease.OutQuint);
        extended = true;
    }
    public void Retract()
    {
        transform.DOMove(startPosition.position, 0.5f).SetEase(Ease.OutQuint);
        extended = false;
    }

    public void AddPassive(PassiveData passiveData)
    {
        GameObject go = GameObject.Instantiate(containerImagePrefab,layoutTransform);
        go.GetComponentInChildren<Image>().sprite = passiveData.passiveSprite;

        //Do the hover here 


        EventTrigger et = go.AddComponent<EventTrigger>();
        // Add PointerEnter event listener
        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;
        entry2.callback.AddListener((data) => OnHoverExitContainer((PointerEventData)data));
        et.triggers.Add(entry2);


        EventTrigger.Entry entryHover = new EventTrigger.Entry();
        entryHover.eventID = EventTriggerType.PointerEnter;
        entryHover.callback.AddListener((data) => OnHoverContainer((PointerEventData)data, passiveData));
        et.triggers.Add(entryHover);
        images.Add(go);
        go.SetActive(true);
    }

    public void OnHoverContainer(PointerEventData data, PassiveData pasData)
    {
        GlobalHelper.UI().ShowHoverInfos(pasData);
    }
    public void OnHoverExitContainer(PointerEventData data)
    {
        GlobalHelper.UI().HideHoverInfos();
    }


}
