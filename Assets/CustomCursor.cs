using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{

    public Sprite textureNormal;
    public Sprite textureClicked;
    public SpriteRenderer spriteRenderer;
    public Vector2 offset;

    public bool hideMouse = false;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.sprite = textureNormal;
    }

    private bool isMouseHidden = false;
    // Update is called once per frame
    void Update()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            spriteRenderer.sprite = textureClicked;
        }
        if (Input.GetMouseButtonUp(0))
        {
            spriteRenderer.sprite = textureNormal;
        }

        if (hideMouse && !isMouseHidden)
        {
            isMouseHidden = true;
            spriteRenderer.gameObject.SetActive(false);
        }else if(!hideMouse && isMouseHidden)
        {
         
            isMouseHidden = false;
            spriteRenderer.gameObject.SetActive(true);

        }
        transform.position = worldPosition + new Vector3( offset.x,offset.y,-worldPosition.z);
    }
}
