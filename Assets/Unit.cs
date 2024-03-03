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
    public int UID;
    public List<Vector3Int> occupiedCells = new List<Vector3Int>();
    
    public SpriteRenderer spriteRenderer;

    public List<Func<List<Vector3Int>, Tilemap>> movementMethods;
    public List<Func<List<Vector3Int>, Tilemap>> attackMethods;

    public int basePaletteIndex;

    public List<EffectData> currentEffects;


    public RoomView _room;
    public RoomView roomRef { get
        {
            if(_room == null)
            {
                _room = GlobalHelper.GetRoom();
            }
            return _room;
        }
        private set
        {
        }
    }
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
        UID = GlobalHelper.GetUID();
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
    

    public bool Move(List<Vector3Int> positions)
    {
        bool MOVE_SUCCESS = false;

        if(roomRef.isValidMove(this, positions)){
            //Animate here
            //On complete : MoveUnit 
            roomRef.MoveUnit(this, positions);
            transform.localPosition = GetWorldPosition();
            MOVE_SUCCESS = true;

        }





        return MOVE_SUCCESS;
    }

    public Vector3 GetWorldPosition()
    {
        RoomView rv = GlobalHelper.GetRoom();
        Vector3 meanPosition = Vector3.zero;
        foreach(Vector3Int position in occupiedCells)
        {
            
            meanPosition += rv.tilemapFloorWalls.CellToLocal(rv.CellToTilemap(position));
        }
        meanPosition /= occupiedCells.Count;
        meanPosition += rv.tilemapFloorWalls.cellSize / 2;

        return meanPosition;
    }
}
