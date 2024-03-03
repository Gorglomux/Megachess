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

    private int basePaletteIndex = -1;

    public List<EffectData> currentEffects;
    public bool isEnemy = false;

    public int health;

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
    public void Initialize(UnitData ud, bool isEnemy)
    {
        unitData = ud;
        spriteRenderer.sprite = ud.sprite;
        currentEffects = ud.effectDatas;
        LoadPalette();
        this.isEnemy = isEnemy;
        health = occupiedCells.Count;
        UID = GlobalHelper.GetUID();
    }
    void LoadPalette(bool megaTransform = false)
    {
        if(basePaletteIndex == -1 || megaTransform)
        {
            spriteRenderer.material = GlobalHelper.GlobalVariables.areaMaterial;
        }
        else
        {
            //This is for debug 
            spriteRenderer.material = new Material(GlobalHelper.GlobalVariables.paletteMaterial);
            spriteRenderer.material.SetFloat("_PaletteIndex", basePaletteIndex);
        }

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
    
    public void Attack(List<Vector3Int> positions)
    {
        if (roomRef.isValidAttack(this,positions))
        {
            List<Unit> targeted = roomRef.GetUnitsAt(positions);
            if(targeted != null && targeted.Count > 0)
            {
                foreach (Unit targetedUnit in targeted)
                {
                    targetedUnit.TakeDamage(occupiedCells.Count);
                }
            }
            if (unitData.moveAfterAttack)
            {
                Move(positions);
            }

        }

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

        List<Vector3Int> result = roomRef.CheckMega(this, occupiedCells[0]);
        if (result != null && result.Count > 0)
        {
            roomRef.MakeMega(this, result);
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
    public void TakeDamage(int damageCount)
    {
        health -= damageCount;
        if(health == 0)
        {
            //Destroy me 
            roomRef.DestroyUnit(this);
        }
    }
}
