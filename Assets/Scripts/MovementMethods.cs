using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementMethods
{

    public static Func<RoomView, Unit, List<Vector3Int>> GetMovementMethod(string indice)
    {
        return movementMethods[indice];
    }


    public static Dictionary<string, Func<RoomView,Unit,  List<Vector3Int>>> movementMethods = new Dictionary<string, Func<RoomView, Unit, List<Vector3Int>>>()
    {
        {"Rook", (room,unit)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> directions = new List<Vector3Int>
            {
                new Vector3Int(0,1),
                new Vector3Int(0,-1),
                new Vector3Int(1,0),
                new Vector3Int(-1,0)
            };
            foreach (Vector3Int direction in directions)
            {
                positions.AddRange(GetCellAlongDirection(unit,room,direction));
            }
            //Filter unique position 
            positions = positions.Distinct().ToList();

            return positions; 
        
        } },
        {"Knight", (room,unit)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> staticMovements = new List<Vector3Int>
            {
                new Vector3Int(2,1),
                new Vector3Int(2,-1),
                new Vector3Int(-2,1),
                new Vector3Int(-2,-1),
                new Vector3Int(1,2),
                new Vector3Int(1,-2),
                new Vector3Int(-1,2),
                new Vector3Int(-1,-2)
            };
            foreach (Vector3Int staticMovement in staticMovements)
            {
                positions.AddRange(GetCellFromFixedMovement(unit,room,staticMovement));
            }

            return positions;

        } },
        {"Bishop", (room,unit)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> directions = new List<Vector3Int>
            {
                new Vector3Int(1,1),
                new Vector3Int(-1,-1),
                new Vector3Int(1,-1),
                new Vector3Int(-1,1)
            };
            foreach (Vector3Int direction in directions)
            {
                positions.AddRange(GetCellAlongDirection(unit,room,direction));
            }
            //Filter unique position 
            positions = positions.Distinct().ToList();

            return positions;

        } },
        {"Queen", (room,unit)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> directions = new List<Vector3Int>
            {
                new Vector3Int(1,1),
                new Vector3Int(-1,-1),
                new Vector3Int(1,-1),
                new Vector3Int(-1,1),
                new Vector3Int(0,1),
                new Vector3Int(0,-1),
                new Vector3Int(1,0),
                new Vector3Int(-1,0)
            };
            foreach (Vector3Int direction in directions)
            {
                positions.AddRange(GetCellAlongDirection(unit,room,direction));
            }
            //Filter unique position 
            positions = positions.Distinct().ToList();

            return positions;

        } },
    };

    #region Movement method
    public static List<Vector3Int> GetCellFromFixedMovement(Unit unit, RoomView room, Vector3Int staticMovement)
    {
        List<Vector3Int> output = new List<Vector3Int>();
        bool obstructed = false;
        List<Vector3Int> temp = new List<Vector3Int>();
        foreach (Vector3Int cell in unit.occupiedCells)
        {

            Vector3Int movement = cell + staticMovement * unit.megaSize;
            if (room.InBounds(movement) && cell != movement && room.GetTileAt(movement)?.name == GlobalHelper.GlobalVariables.TILE_GROUND)
            {
                Unit unitAt = room.GetUnitAt(movement);
                if (unitAt != null && unitAt.UID == unit.UID)
                {
                }
                else
                {
                    temp.Add(movement);
                }
            }
            else
            {
                obstructed = true;
                break;
            }
        }
        if (!obstructed)
        {
            output.AddRange(temp);
        }
        return output;
    }
    public static List<Vector3Int> GetCellAlongDirection(Unit unit, RoomView room, Vector3Int direction)
    {
        List<Vector3Int> output = new List<Vector3Int>();
        int maxDistance = 999;
        List<List<Vector3Int>> temp = new List<List<Vector3Int>>();
        int tempIndex = 0;
        foreach (Vector3Int cell in unit.occupiedCells)
        {
            temp.Add(new List<Vector3Int>());
            Vector3Int movement = cell;
            int i = 0;
            while (room.InBounds(movement) && i <= maxDistance)
            {
                if (room.InBounds(movement) && cell != movement)
                {
                    if (room.GetTileAt(movement)?.name == GlobalHelper.GlobalVariables.TILE_GROUND)
                    {
                        Unit u = room.GetUnitAt(movement);
                        if(u != null && (u.isEnemy != unit.isEnemy || u.UID == unit.UID))
                        {
                            temp[tempIndex].Add(movement);

                        }else if(u != null &&  (u.isEnemy == unit.isEnemy))
                        {

                            maxDistance = i;
                            break;
                        }
                        else if (u == null)
                        {
                            temp[tempIndex].Add(movement);

                        }


                        if (u != null && u.UID != unit.UID)
                        {
                            maxDistance = i;
                            if (u.megaSize > 1)
                            {

                                foreach (Vector3Int c in u.occupiedCells)
                                {
                                    Vector3 a = c - cell;
                                    Vector3 b = direction;
                                    if (Mathf.Abs(Vector3.Dot(a.normalized, b.normalized)) == 1)
                                    {
                                        temp[tempIndex].Add(c);
                                        maxDistance++;
                                    }
                                }
                            }
                            break;
                        }
                        else
                        {

                        }
                    }
                    else if (room.GetTileAt(movement)?.name == GlobalHelper.GlobalVariables.TILE_WALL)
                    {
                        maxDistance = i;
                        break;
                    }
                    i++;
                }
                movement += direction;
            }
            tempIndex++;
        }
        //Shorten the positions according to the break distance
        foreach (List<Vector3Int> list in temp)
        {
            output.AddRange(list.Take(maxDistance + 1));

        }
        return output;
    }
    #endregion;
}
