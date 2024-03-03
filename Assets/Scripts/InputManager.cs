using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Update is called once per frame
    void Update()
    {
        //Get the mouse position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector3.forward, 1000, mask);
        foreach (RaycastHit2D hit in hits)
        {
            Collider2D nextCollider = hit.collider;
            IHoverable hovered = nextCollider.GetComponent<IHoverable>();
            if (currentHovered != hovered)
            {
                if(currentHovered != null)
                {
                    currentHovered.onHoverExit();
                }
                currentHovered = hovered;
                hovered.onHoverEnter();
            }

        }
        if(hits.Length == 0)
        {
            if(currentHovered != null)
            {
                currentHovered.onHoverExit();
                currentHovered = null;
            }
        }
    }
}
