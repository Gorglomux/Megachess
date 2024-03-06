using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReserveContainer : MonoBehaviour
{
    public Sprite spriteNormal;
    public Sprite spriteHovered;
    public Image containerImage;
    public Image unitImage;
    public TextMeshProUGUI unitCountText;

    private int _unitCount;
    public int unitCount { get { return _unitCount; } set { if (value != _unitCount) { UpdateText(_unitCount,value); _unitCount = value;  } } }

    UnitData unitData;
    public Queue units = new Queue();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize(UnitData ud)
    {
        unitData = ud;
        unitImage.sprite = ud.sprite;

        unitCount = 1;
    }

    public void RemoveUnit()
    {
        unitCount--;
    }
    public void UpdateText(int oldValue, int newValue)
    {
        unitCountText.text = newValue.ToString();
        if (newValue > oldValue)
        {

            unitCountText.transform.DOPunchScale(Vector3.one * 1.2f, 0.2f, 2).SetEase(Ease.OutQuint).onComplete += () =>
            {
                unitCountText.transform.DOScale(Vector3.one, 0.2f);
            };
        }

    }

    public void AddUnit(Unit u)
    {
        units.Enqueue(u);
        u.transform.position = transform.position;
        u.gameObject.SetActive(false);
        unitCount = units.Count;
    }

    public void OnPointerEnter(BaseEventData data)
    {
        containerImage.sprite = spriteHovered;
    }

    public void OnPointerExit(BaseEventData data)
    {
        containerImage.sprite = spriteNormal;
    }

    public void OnBeginDrag(BaseEventData data)
    {
        if (canSelect())
        {
            EventSystem.current.SetSelectedGameObject(gameObject);

        }
        //Dequeue unit 

        //Put it as the current selected ? 
    }

    public void OnEndDrag(BaseEventData data)
    { 
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnPointerDown(BaseEventData data)
    {
        if (canSelect())
        {
            EventSystem.current.SetSelectedGameObject(gameObject);

        }
    }
    public void OnSelect(BaseEventData data)
    {
        RoomView room = GlobalHelper.GetRoom();
        if (room.CheckUnitsLeft()>0)
        {
            selected = true;
            SpriteRenderer spritePreview = GlobalHelper.GlobalVariables.inputManager.spritePreview;
            spritePreview.gameObject.SetActive(true);
            spritePreview.sprite = unitImage.sprite;
            spritePreview.transform.localScale = Vector3.one;
            Unit u = (Unit)units.Peek();
            if (u != null)
            {
                spritePreview.material.SetFloat("_PaletteIndex", u.basePaletteIndex);

            }


            unitImage.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            GlobalHelper.UI().ShakeButtonBottomRightText();
            GlobalHelper.UI().SetBottomText("Press the Fight button to start the fight!"); 
        }

    }

    public void OnDeselect(BaseEventData data)
    {
        RoomView r = GlobalHelper.GetRoom();
        if (r.CheckUnitsLeft()<=0)
        {
            return;
        }
        selected = false;
        //Place the object here 
        if(units.Count > 0)
        {
            Unit u = (Unit)units.Peek();
            List<Vector3Int> evaluated = u.EvaluateAroundPosition(GlobalHelper.GetMouseWorldPosition());
            if(evaluated.Count > 0)
            {
                foreach(Vector3Int point in evaluated)
                {
                    if (r.SpawnableCells.Contains(r.CellToTilemap(point)) && r.GetUnitAt(point) == null)
                    {
                        RemoveUnit();
                        Unit unit = (Unit)units.Dequeue();
                        unit.gameObject.SetActive(true);
                        //This will break why did you write this 

                        r.PlaceUnitOnMap(unit, r.CellToTilemap(point));
                    }
                    else
                    {
                        //Display error and lerp back to the reserve 
                    }
                   
                }
            }
        }
        GlobalHelper.getGlobal().indicatorManager.HideSpawnableCells();
        GlobalHelper.getGlobal().indicatorManager.ShowSpawnableCells();


        GlobalHelper.GlobalVariables.inputManager.spritePreview.gameObject.SetActive(false);
        GlobalHelper.GlobalVariables.inputManager.spritePreview.sprite = null;
        unitImage.color = new Color(1, 1, 1, 1f);
    }
    public bool selected = false;
    private void Update()
    {
        if (selected)
        {
            if (units.Count > 0)
            {
               
                Unit u = (Unit)units.Peek();
                List<Vector3Int> evaluated = u.EvaluateAroundPosition(GlobalHelper.GetMouseWorldPosition());
                GlobalHelper.GlobalVariables.indicatorManager.HoverOnSpawnableCells(evaluated);

            }
        }
    }

    public bool canSelect()
    {

        return units.Count > 0 && GlobalHelper.GlobalVariables.gameInfos.gameState is UnitPlaceState;
    }
}
