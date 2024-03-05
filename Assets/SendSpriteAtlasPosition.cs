using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class SendSpriteAtlasPosition : MonoBehaviour
{
    public Material m;
    public SpriteRenderer spriteRenderer;
    public Sprite referenceSprite;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer= GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SendPosition()
    {
        Sprite s = referenceSprite;
        Vector2 texSize = new Vector2(s.texture.width, s.texture.height);
        Vector2 positionInAtlas = new Vector2(s.rect.position.x / texSize.x, s.rect.position.y / texSize.y);
        Vector2 textureSize = s.rect.size / texSize;

        print(positionInAtlas.x + " " + positionInAtlas.y + " " + textureSize);
        //Recalculate and send the index to the material 
        m.SetVector("_SpritePosition", positionInAtlas);
        m.SetVector("_SpriteSize", textureSize);
        //Send the size of the sprite as well
        spriteRenderer.material = m;
    }
}
