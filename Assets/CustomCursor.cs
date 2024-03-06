using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{

    public Sprite textureNormal;
    public Sprite textureClicked;
    public SpriteRenderer spriteRenderer;
    public Vector2 offset;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.sprite = textureNormal;
    }

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
        transform.position = worldPosition + new Vector3( offset.x,offset.y,-worldPosition.z);
    }
}
