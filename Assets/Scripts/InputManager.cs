using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{

    public Texture2D cursorTexture;
    public Texture2D clickedCursorTexture;

    public Vector2 cursorHotspot;

    public LayerMask mask;


    public GameInfos infos;
    // Start is called before the first frame update
    void Start()
    {
        currentHoverDelay = maxHoverDelay;
           infos = GlobalHelper.GlobalVariables.gameInfos;
    }
    IHoverable currentHovered;

    ISelectable currentSelected;
    IDraggable currentDraggable;

    public bool dragStarted = false;
    public SpriteRenderer spritePreview;
    public float minDistanceToStartDrag = 0.15f;

    private Vector3 startDragPosition = Vector3.positiveInfinity;
    // Update is called once per frame

    public float maxHoverDelay = 0.2f;
    public float currentHoverDelay = 0f;
    void Update()
    {
        if(currentHovered != null)
        {
            currentHoverDelay += Time.deltaTime;

        }

        //Get the mouse position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition = new Vector3(worldPosition.x,worldPosition.y,0);
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector3.forward, 1000, mask);
        foreach (RaycastHit2D hit in hits)
        {

            Collider2D nextCollider = hit.collider;


            IHoverable hovered = nextCollider.GetComponent<IHoverable>();
            if (hovered != null && currentHovered != hovered && hovered != currentSelected &&(currentHovered == null || currentHoverDelay > maxHoverDelay))
            {
                if(currentHovered != null)
                {
                    currentHovered.onHoverExit();
                }
                currentHovered = hovered;
                hovered.onHoverEnter();
                infos.hovered = hovered;
                currentHoverDelay = 0;
            }


           
        }
        if (Input.GetMouseButtonDown(0))
        {
            if(hits.Length > 0)
            {
                ISelectable selectable = hits[0].collider.GetComponent<ISelectable>();
                if (selectable != null && currentSelected != selectable)
                {
                    //Check valid target here ?  
                    if (currentSelected != null)
                    {
                        infos.selected = null;
                        currentSelected.onDeselect(new Vector3(worldPosition.x,worldPosition.y,0));
                    }
                    currentSelected = selectable;
                    currentHovered.onHoverExit();
                    currentHovered = null;
                    infos.hovered = currentHovered;
                    infos.selected = selectable;
                    if (!currentSelected.onSelect()) {

                        infos.selected = selectable;
                        StartCoroutine(corDeselectAfterAFrame());

                    }
                    else
                    {
                        infos.selected = selectable;
                    }
                }
            }
            else if (currentSelected != null)
            {
                Deselect(worldPosition);
            }

        }
        if(currentSelected != null)
        {
            currentSelected.onSelectTick(worldPosition);
        }


        if (Input.GetMouseButton(0))
        {

            if(currentSelected is IDraggable)
            {
                if(startDragPosition.x == float.PositiveInfinity)
                {
                    startDragPosition = worldPosition;
                }
                MonoBehaviour selected = currentSelected as MonoBehaviour;
                float distanceSelected = Vector3.Distance(startDragPosition, worldPosition);
                if (distanceSelected >= minDistanceToStartDrag && !dragStarted)
                {
                    currentDraggable = currentSelected as IDraggable;
                    Sprite spriteToShow = currentDraggable.onDragBegin(worldPosition);
                    dragStarted = true;
                    spritePreview.gameObject.SetActive(true);
                    spritePreview.sprite = spriteToShow;
                    Unit u = currentDraggable as Unit;
                    if (u != null)
                    {
                        spritePreview.transform.localScale =Vector3.one * u.megaSize;
                        spritePreview.material.SetFloat("_PaletteIndex", u.spriteRenderer.material.GetFloat("_PaletteIndex"));
                    }
                    else
                    {
                        spritePreview.transform.localScale = Vector3.one;

                    }
                }

                if (dragStarted)
                {
                    currentDraggable.onDragTick(worldPosition);
                    //Tilt the sprite a little bit 
                    //Put it at the center of the mouse 
                }

            }
        }
        else
        {
            if(dragStarted)
            {
                dragStarted = false;
                currentDraggable.onDragEnd(worldPosition);
                spritePreview.sprite = null;
                spritePreview.gameObject.SetActive(false);
                Deselect(worldPosition);

            }
            startDragPosition = Vector3.positiveInfinity;
        }
        if (spritePreview.enabled)
        {
            spritePreview.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);

        }

        if (hits.Length == 0)
        {
            if(currentHovered != null)
            {
                currentHovered.onHoverExit();
                currentHovered = null;
                infos.hovered = null;
            }
        }
        //REMOVE ME
        if (Input.GetKeyUp(KeyCode.KeypadMinus))
        {
            Time.timeScale = 0.2f;
            GlobalHelper.UI().SetBottomText("Timescale " + Time.timeScale);
        }
        if (Input.GetKeyUp(KeyCode.KeypadPlus))
        {
            Time.timeScale = 1;
            GlobalHelper.UI().SetBottomText("Timescale " + Time.timeScale);
        }
        if (Input.GetKeyUp(KeyCode.Keypad1))
        {
            Time.timeScale = 5;
            GlobalHelper.UI().SetBottomText("Timescale " + Time.timeScale);
        }
        if (Input.GetKeyUp(KeyCode.Keypad0))
        {
            Time.timeScale = 0;
            GlobalHelper.UI().SetBottomText("Timescale " + Time.timeScale);
        }
    }


    void Deselect(Vector3 position)
    {
        if (currentSelected != null)
        {
            infos.selected = null;
            currentSelected.onDeselect(new Vector3(position.x, position.y, 0));
            currentSelected = null;
        }
    }
    public IEnumerator corDeselectAfterAFrame()
    {
        yield return null;
        infos.selected = null;
        currentSelected = null;
    }
}
