using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PreviewMove : MonoBehaviour
{
    public Tilemap tilemapPreview;

    public TileBase tileUnit;
    public TileBase tileEmpty;
    public TileBase tileMove;
    public TileBase tileAttack;

    public int spritesToShow = 3;
    public Transform tilemapBottomLeft;
    public Grid grid;
    public float tilemapSize = 1;

    public float bigFactor;
    public Vector3 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        bigFactor = (spritesToShow * 2 +1) * grid.cellSize.x;
    }
    public float smallFactor;
    public float ratio;
    public void ShowPreview(Unit u)
    {
        HidePreview();
        print("Showing preview  of unit "+u.unitData.unitName);
        int squareSize = spritesToShow*2 + u.megaSize;
        Vector3Int bottomLeft = tilemapPreview.WorldToCell(tilemapBottomLeft.transform.position);


        List<Vector3Int> cells = MovementMethods.GetMovementMethod(u.unitData.unitName).Invoke(GlobalHelper.GetRoom(), u, spritesToShow);
        int spread = 0;
        int minCellX = 99999;
        int maxCellX = -999999;
        foreach (Vector3Int cell in cells)
        {
            if(cell.x < minCellX)
            {
                minCellX = cell.x;
            }
            if(cell.x > maxCellX)
            {
                maxCellX = cell.x;  
            }
        }
        spread = maxCellX - minCellX+1;


        smallFactor = spread * grid.cellSize.x ;
        ratio = bigFactor / smallFactor;
        grid.transform.localScale = Vector3.one * ratio;
        //Fill an empty square of desired width 
        for (int y = 0; y < spread; y++)
        {
            for(int x = 0;x< spread; x++)
            {
                tilemapPreview.SetTile(bottomLeft + new Vector3Int(x, y), tileEmpty);
            }
        }
        Vector3Int center = bottomLeft + new Vector3Int(spread / 2, spread / 2);

        foreach (Vector3Int cell in cells)
        {

            tilemapPreview.SetTile(cell + center, tileAttack);
        }
        int min = u.megaSize / 2;
        int max = u.megaSize - min;
        for (int y = -min; y < max; y++)
        {
            for (int x = -min; x < max; x++)
            {
                Vector3Int v = center + new Vector3Int(x, y);
                print(" V " + v);
                tilemapPreview.SetTile(v, tileUnit);
            }
        }

    }

    public void HidePreview()
    {

        tilemapPreview.ClearAllTiles();
    }


}
