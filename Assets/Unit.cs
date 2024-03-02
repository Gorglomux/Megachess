using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IClickable
{

}
public interface IHoverable
{

}

public class Unit : MonoBehaviour , IClickable, IHoverable
{
    public UnitData unitData;
    public List<Vector3Int> occupiedCells;
    
    public SpriteRenderer spriteRenderer;

    public List<Func<List<Vector3Int>, Tilemap>> movementMethods;
    public List<Func<List<Vector3Int>, Tilemap>> attackMethods;

    public int basePaletteIndex;

    public List<EffectData> currentEffects;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Initialize(UnitData ud)
    {
        unitData = ud;
        spriteRenderer.sprite = ud.sprite;
        currentEffects = ud.effectDatas;
        LoadPalette();
    }
    void LoadPalette(bool megaTransform = false)
    {
        spriteRenderer.material = GlobalHelper.GlobalVariables.areaMaterial;

        //Load the palette according to the time created;

        if (!megaTransform)
        {
            basePaletteIndex =(int) GlobalHelper.GlobalVariables.areaMaterial.GetFloat("_PaletteIndex");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
