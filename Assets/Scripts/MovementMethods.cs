using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementMethods
{

    public static bool HasAttackMethod(string indice)
    {
        return attackMethods.ContainsKey(indice);
    }
    public static Func<RoomView, Unit, int, List<Vector3Int>> GetAttackMethod(string indice)
    {
        if(HasAttackMethod(indice)){

           return attackMethods[indice];
        }
        else
        {

            return movementMethods[indice];
        }
    }


    public static Func<RoomView, Unit, int, List<Vector3Int>> GetMovementMethod(string indice)
    {
        return movementMethods[indice];
    }

    public static Dictionary<string, Func<RoomView, Unit, int, List<Vector3Int>>> attackMethods = new Dictionary<string, Func<RoomView, Unit, int, List<Vector3Int>>>()
    {
        {"Pawn", (room,unit, preview)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> staticMovements = new List<Vector3Int>
            {
                new Vector3Int(1,1),
                new Vector3Int(1,-1),
                new Vector3Int(-1,-1),
                new Vector3Int(-1,1),
            };
            if(preview > 1)
            {
                foreach(Vector3Int staticMovement in staticMovements)
                {
                    positions.AddRange(GetCellAlongStaticPreview(unit,staticMovement));
                }
            }
            else
            {
                foreach (Vector3Int staticMovement in staticMovements)
                {
                    positions.AddRange(GetCellFromFixedMovement(unit,room,staticMovement,true,true));
                }

            }

            return positions;

        } },
            {"Assassin", (room,unit, preview)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> directionsMovement = new List<Vector3Int>
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
            if(preview > 1)
            {
                foreach(Vector3Int staticMovement in directionsMovement)
                {
                    positions.AddRange(GetCellAlongStaticPreview(unit,staticMovement));
                }
            }
            else
            {
                foreach (Vector3Int staticMovement in directionsMovement)
                {
                    positions.AddRange(GetCellFromFixedMovement(unit,room,staticMovement,true,true));
                }

            }
            //Filter unique position 
            positions = positions.Distinct().ToList();

            return positions;

        } },

    };


    public static Dictionary<string, Func<RoomView, Unit, int, List<Vector3Int>>> movementMethods = new Dictionary<string, Func<RoomView, Unit, int, List<Vector3Int>>>()
    {
        {"Rook", (room,unit, preview)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> directions = new List<Vector3Int>
            {
                new Vector3Int(0,1),
                new Vector3Int(0,-1),
                new Vector3Int(1,0),
                new Vector3Int(-1,0)
            };
            if(preview > 1)
            {
                foreach(Vector3Int direction in directions)
                {
                    positions.AddRange(GetCellAlongDirectionPreview(unit,preview,direction));
                }
            }
            else
            {

                foreach (Vector3Int direction in directions)
                {
                    positions.AddRange(GetCellAlongDirection(unit,room,direction));
                }
            }
            //Filter unique position 
            positions = positions.Distinct().ToList();

            return positions;

        } },
        {"Knight", (room,unit, preview)=>{
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
            if(preview > 1)
            {
                foreach(Vector3Int staticMovement in staticMovements)
                {
                    positions.AddRange(GetCellAlongStaticPreview(unit,staticMovement));
                }
            }
            else
            {
                foreach (Vector3Int staticMovement in staticMovements)
                {
                    positions.AddRange(GetCellFromFixedMovement(unit,room,staticMovement));
                }

            }

            return positions;

        } }, {"Pawn", (room,unit, preview)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> staticMovements = new List<Vector3Int>
            {
                new Vector3Int(0,1),
                new Vector3Int(1,0),
                new Vector3Int(0,-1),
                new Vector3Int(-1,0),
            };
            if(preview > 1)
            {
                foreach(Vector3Int staticMovement in staticMovements)
                {
                    positions.AddRange(GetCellAlongStaticPreview(unit,staticMovement));
                }
            }
            else
            {
                foreach (Vector3Int staticMovement in staticMovements)
                {
                    positions.AddRange(GetCellFromFixedMovement(unit,room,staticMovement, false));
                }

            }

            return positions;

        } }, {"King", (room,unit, preview)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> staticMovements = new List<Vector3Int>
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
            if(preview > 1)
            {
                foreach(Vector3Int staticMovement in staticMovements)
                {
                    positions.AddRange(GetCellAlongStaticPreview(unit,staticMovement));
                }
            }
            else
            {
                foreach (Vector3Int staticMovement in staticMovements)
                {
                    positions.AddRange(GetCellFromFixedMovement(unit,room,staticMovement, false));
                }

            }

            return positions;

        } },
        {"Bishop", (room,unit, preview)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> directions = new List<Vector3Int>
            {
                new Vector3Int(1,1),
                new Vector3Int(-1,-1),
                new Vector3Int(1,-1),
                new Vector3Int(-1,1)
            };
            if(preview > 1)
            {
                foreach(Vector3Int direction in directions)
                {
                    positions.AddRange(GetCellAlongDirectionPreview(unit,preview,direction));
                }
            }
            else
            {

                foreach (Vector3Int direction in directions)
                {
                    positions.AddRange(GetCellAlongDirection(unit,room,direction));
                }
            }
            //Filter unique position 
            positions = positions.Distinct().ToList();

            return positions;

        } },
        {"Queen", (room,unit, preview)=>{
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
            if(preview > 1)
            {
                foreach(Vector3Int direction in directions)
                {
                    positions.AddRange(GetCellAlongDirectionPreview(unit,preview,direction));
                }
            }
            else
            {

                foreach (Vector3Int direction in directions)
                {
                    positions.AddRange(GetCellAlongDirection(unit,room,direction));
                }
            }
            //Filter unique position 
            positions = positions.Distinct().ToList();

            return positions;

        } },
        {"Vampire", (room,unit, preview)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> staticMovements = new List<Vector3Int>
            {
            };

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
            foreach(Vector3Int direction in directions)
            {
                for(int i=0; i<8; i++)
                {
                    staticMovements.Add(2*direction*i);
                }
            }
            if(preview > 1)
            {
                foreach(Vector3Int staticMovement in staticMovements)
                {
                    positions.AddRange(GetCellAlongStaticPreview(unit,staticMovement));
                }
            }
            else
            {
                foreach (Vector3Int staticMovement in staticMovements)
                {
                    positions.AddRange(GetCellFromFixedMovement(unit,room,staticMovement));
                }

            }

            return positions;

        } }, 
        {"Assassin", (room,unit, preview)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> directionsMovement = new List<Vector3Int>
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
            if(preview > 1)
            {
                foreach(Vector3Int direction in directionsMovement)
                {
                    positions.AddRange(GetCellAlongDirectionPreview(unit,preview,direction));
                }
               
            }
            else
            {
                foreach (Vector3Int direction in directionsMovement)
                {
                    positions.AddRange(GetCellAlongDirection(unit,room,direction,false));
                }

            }
            //Filter unique position 
            positions = positions.Distinct().ToList();

            return positions;

        } },

    };

    #region Movement method
    public static List<Vector3Int> GetCellFromFixedMovement(Unit unit, RoomView room, Vector3Int staticMovement, bool canAttack = true, bool onlyReturnAttack = false)
    {
        List<Vector3Int> output = new List<Vector3Int>();
        bool obstructed = false;
        List<Vector3Int> temp = new List<Vector3Int>();
        bool isAttackingEnemy = false;
        foreach (Vector3Int cell in unit.occupiedCells)
        {
            Vector3Int movement = cell + staticMovement * unit.megaSize;
            if (room.InBounds(movement) && cell != movement && room.GetTileAt(movement)?.name == GlobalHelper.GlobalVariables.TILE_GROUND)
            {
                Unit unitAt = room.GetUnitAt(movement);

                if (onlyReturnAttack)
                {
                    if(unitAt != null && unitAt.isEnemy != unit.isEnemy)
                    {
                        isAttackingEnemy = true;
                    }
                }
                temp.Add(movement);
                //if (unitAt != null && unitAt.UID == unit.UID && unitAt.isEnemy == unit.isEnemy)
                //{
                //}
                //else 
                //{
                //    if (canAttack || (!canAttack && unitAt == null)|| (!canAttack && unitAt != null && unitAt.UID == unit.UID))
                //    {
                //        temp.Add(movement);
                //    }
                //    else
                //    {
                //        obstructed = true;
                //        break;
                //    }
                //}
            }
            else
            {
                obstructed = true;
                break;
            }
        }
        if (!obstructed )
        {
            if (onlyReturnAttack && isAttackingEnemy)
            {

                output.AddRange(temp);
            }
            else if(!onlyReturnAttack)
            {

                output.AddRange(temp);
            }
        }
        return output;
    }
    public static List<Vector3Int> GetCellAlongDirectionPreview(Unit unit, int previewDepth, Vector3Int direction)
    {
        List<Vector3Int> output = new List<Vector3Int>();
        int min = unit.megaSize / 2;
        int max = unit.megaSize - min;
        for (int y = -min; y < max; y++)
        {
            for (int x = -min; x < max; x++)
            {
                for(int i=0; i<= previewDepth; i++)
                {
                    output.Add(direction * i + new Vector3Int(x,y) );
                }
            }
        }
        output = output.Distinct().ToList();
        return output;
    }
    public static List<Vector3Int> GetCellAlongStaticPreview(Unit unit, Vector3Int staticMovement)
    {
        List<Vector3Int> output = new List<Vector3Int>();
        int min = unit.megaSize / 2;
        int max = unit.megaSize - min;
        for (int y = -min; y < max; y++)
        {
            for (int x = -min; x < max; x++)
            {
                output.Add(staticMovement * unit.megaSize + new Vector3Int(x, y));
            }
        }
        output = output.Distinct().ToList();
        return output;
    }
    public static List<Vector3Int> GetCellAlongDirection(Unit unit, RoomView room, Vector3Int direction, bool canAttack = true)
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
                        if (u != null && (u.isEnemy != unit.isEnemy || u.UID == unit.UID) && canAttack)
                        {
                            temp[tempIndex].Add(movement);

                        }else if(!canAttack && u != null && ( u.UID == unit.UID))
                        {

                            temp[tempIndex].Add(movement);
                        }
                        else if (u != null && (u.isEnemy == unit.isEnemy))
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
        output = output.Distinct().ToList();
        List<Vector3Int> outputCleaned = new List<Vector3Int>();
        foreach (Vector3Int cell in output)
        {
            List<Vector3Int> toAdd = new List<Vector3Int>();
            bool correctCell = true;
            for (int y = 0; y < unit.megaSize; y++)
            {
                for (int x = 0; x < unit.megaSize; x++)
                {
                    Vector3Int toCheck = cell + new Vector3Int(x, y);
                    if (output.Contains(toCheck) && room.InBounds(toCheck) && room.GetTileAt(toCheck)?.name == GlobalHelper.GlobalVariables.TILE_GROUND)
                    {

                    }
                    else
                    {
                        correctCell = false;
                        break;
                    }
                }
            }
            if (correctCell)
            {
                for (int y = 0; y < unit.megaSize; y++)
                {
                    for (int x = 0; x < unit.megaSize; x++)
                    {
                        Vector3Int add = cell + new Vector3Int(x, y);
                        outputCleaned.Add(add);
                    }
                }
            }
        }
        outputCleaned = outputCleaned.Distinct().ToList();

        return outputCleaned;
    }
    #endregion;
}
