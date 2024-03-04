using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{

    public LayerMask mask;


    public GameInfos infos;
    // Start is called before the first frame update
    void Start()
    {
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
    void Update()
    {

        //Get the mouse position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition = new Vector3(worldPosition.x,worldPosition.y,0);
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector3.forward, 1000, mask);
        foreach (RaycastHit2D hit in hits)
        {

            Collider2D nextCollider = hit.collider;


            IHoverable hovered = nextCollider.GetComponent<IHoverable>();
            if (hovered != null && currentHovered != hovered && hovered != currentSelected)
            {
                if(currentHovered != null)
                {
                    currentHovered.onHoverExit();
                }
                currentHovered = hovered;
                hovered.onHoverEnter();
                infos.hovered = hovered;
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
                    if (!currentSelected.onSelect()) {

                        infos.selected = null;
                        currentSelected = null;
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
                        spritePreview.material = u.spriteRenderer.material;
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
}
