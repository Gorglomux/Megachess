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
    public static bool HasSpreadMethod(string indice)
    {
        return spreadMethods.ContainsKey(indice);
    }
    public static Func<RoomView, Unit, int, List<Vector3Int>> GetAttackMethod(string indice)
    {
        if (HasAttackMethod(indice))
        {

            return attackMethods[indice];
        }
        else
        {
            return movementMethods[indice];
        }
    }
    public static Func<RoomView, Unit, int, Vector3Int, List<List<Vector3Int>>> GetSpreadMethod(string indice)
    {
        if (HasSpreadMethod(indice))
        {
            return spreadMethods[indice];
        }
        else
        {
            return null;
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
        {"Bow", (room,unit, preview)=>{
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
            int startOffset = 2;
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
            
            Vector3Int origin = new Vector3Int(0,0);
            int ratio = 1;
            if(preview < 0)
            {
                origin = unit.occupiedCells[unit.megaSize-1];
                ratio = unit.megaSize;
            }           
            
            List<Vector3Int> positionsOffset = new List<Vector3Int>();
            foreach(Vector3Int cell in positions)
            {
                Debug.Log(" Distance betweeen" + origin + "and" +cell +" = " +Vector3.Distance(origin,cell));
                if(Vector3.Distance(origin,cell) > 2 * ratio)
                {
                    positionsOffset.Add(cell);
                }

            }

            return positionsOffset;

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
    public static Dictionary<string, Func<RoomView, Unit, int, Vector3Int, List<List<Vector3Int>>>> spreadMethods = new Dictionary<string, Func<RoomView, Unit, int, Vector3Int, List<List<Vector3Int>>>>()
    {
       {"Knife", (room,unit, preview, targetedPosition)=>{

            bool returnAll = false;
            if(targetedPosition.x == 9999)
            {
                returnAll = true;
            }

            List<List<Vector3Int>> positions = new List<List<Vector3Int>>();
            List<Vector3Int> directions = new List<Vector3Int>
            {
                new Vector3Int(0,1),
                new Vector3Int(0,-1),
                new Vector3Int(1,0),
                new Vector3Int(-1,0)
            };
            int attackSize =(int) 2*unit.megaSize;
            if(preview > 1)
            {
               List<Vector3Int> pPreview = new List<Vector3Int>();
                for(int i =1; i<= attackSize ; i++)
                {
                    pPreview.AddRange(GetCellAlongStaticPreview(unit,directions[0] * i));
                }
                positions.Add(pPreview);
            }
            else
            {
               foreach (Vector3Int direction in directions)
                {
                    List<Vector3Int> p= new List<Vector3Int>();
                    for(int i =1; i<= attackSize; i++)
                    {
                        p.AddRange(GetCellFromFixedMovement(unit,room,direction * i,true,false,false));
                    }
                    if (returnAll)
                    {
                        positions.Add(p);
                    }
                    else
                    {
                       if (p.Contains(targetedPosition))
                       {
                           positions.Add(p);
                       }
                   }

                }

            }

            return positions;

        } },
       {"Fork", (room,unit, preview, targetedPosition)=>{

            bool returnAll = false;
            if(targetedPosition.x == 9999)
            {
                returnAll = true;
            }

            List<List<Vector3Int>> positions = new List<List<Vector3Int>>();
            List<Vector3Int> directions = new List<Vector3Int>
            {
                new Vector3Int(0,1),
                new Vector3Int(0,-1),
                new Vector3Int(1,0),
                new Vector3Int(-1,0)
            };

            int attackSize =(int) 2*unit.megaSize;
            if(preview > 1)
            {
               List<Vector3Int> pPreview = new List<Vector3Int>();
                Vector3Int dir = directions[0];
               Vector3Int startFork = dir;
               //pPreview.Add(startFork);
               for(int i =0; i< attackSize ; i++)
                {
                   if(dir.x == 0)
                   {
                        pPreview.AddRange(GetCellAlongStaticPreview(unit,startFork+ directions[0] * i + new Vector3Int(1,0) * (i+1)  ));
                        pPreview.AddRange(GetCellAlongStaticPreview(unit,startFork+ directions[0] * i+ new Vector3Int(-1,0) * (i+1) ) );

                   }
                   else if(dir.y == 0)
                   {
                        pPreview.AddRange(GetCellAlongStaticPreview(unit,startFork+ directions[0] * i + new Vector3Int(0,1) *  (i+1)  ));
                        pPreview.AddRange(GetCellAlongStaticPreview(unit,startFork+ directions[0] * i+ new Vector3Int(0,-1) *  (i+1)  ) );
                   }
                    pPreview.AddRange(GetCellAlongStaticPreview(unit,startFork+ directions[0] * i));

                }
                positions.Add(pPreview);
            }
            else
            {
               foreach (Vector3Int direction in directions)
                {
                    List<Vector3Int> pPreview = new List<Vector3Int>();
                    Vector3Int dir = direction;
                    Vector3Int startFork = dir;
                    //pPreview.AddRange(GetCellFromFixedMovement(unit,room, startFork));
                    for(int i = 0; i< attackSize ; i++)
                    {
                        if(dir.x == 0)
                        {
                            pPreview.AddRange(GetCellFromFixedMovement(unit,room,startFork+ dir * i + new Vector3Int(1,0) * (i+1) ,true,false,false ));
                            pPreview.AddRange(GetCellFromFixedMovement(unit,room, startFork+ dir * i+ new Vector3Int(-1,0) * (i+1) ,true,false,false ) );

                        }
                        if(dir.y == 0)
                        {
                            pPreview.AddRange(GetCellFromFixedMovement(unit,room, startFork+ dir * i + new Vector3Int(0,1) * (i+1),true,false,false  ));
                            pPreview.AddRange(GetCellFromFixedMovement(unit,room, startFork+ dir * i+ new Vector3Int(0,-1) * (i+1) ,true,false,false ) );
                        }
                        pPreview.AddRange(GetCellFromFixedMovement(unit,room, startFork+ dir * i));

                    }
                    if (returnAll)
                    {
                        positions.Add(pPreview);
                    }
                    else
                    {
                       if (pPreview.Contains(targetedPosition))
                       {
                           positions.Add(pPreview);
                       }
                   }

                }

            }

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
        {"Fork", (room,unit, preview)=>{
                List<Vector3Int> positions = new List<Vector3Int>();
                List<Vector3Int> staticMovements = new List<Vector3Int>
                {
                    new Vector3Int(3,0),
                    new Vector3Int(0,-3),
                    new Vector3Int(0,3),
                    new Vector3Int(-3,0),
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
                        positions.AddRange(GetCellFromFixedMovement(unit,room,staticMovement,false));
                    }

                }
            //Filter unique position 
            positions = positions.Distinct().ToList();

            return positions;

        } },
           {"Bow", (room,unit, preview)=>{
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
                    new Vector3Int(-1,0),
                    new Vector3Int(-2,0),
                    new Vector3Int(0,2),
                    new Vector3Int(2,0),
                    new Vector3Int(0,-2)
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
                        positions.AddRange(GetCellFromFixedMovement(unit,room,staticMovement,false));
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
                    positions.AddRange(GetCellFromFixedMovement(unit,room,staticMovement,false));
                }

            }

            return positions;

        } },
         {"Knife", (room,unit, preview)=>{
            List<Vector3Int> positions = new List<Vector3Int>();
            List<Vector3Int> staticMovements = new List<Vector3Int>
            {
            };
             int squareHoleSizeA = -2;
             int squareHoleSizeB = 3;
            for(int y=squareHoleSizeA;y<squareHoleSizeB; y++)
            {
                 for(int x= squareHoleSizeA; x<squareHoleSizeB; x++)
                 {

                    if(x != 0 &&y != 0 && y != squareHoleSizeA && y != squareHoleSizeB - 1)
                    {
                        staticMovements.Add(new Vector3Int(x,y));
                    }

                    if(y != 0 && x != 0 && x != squareHoleSizeA && x != squareHoleSizeB - 1)
                    {
                        staticMovements.Add(new Vector3Int(x,y));
                    }
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
                    positions.AddRange(GetCellFromFixedMovement(unit,room,staticMovement,false));
                }

            }

            return positions;

        } },
        {"Pawn", (room,unit, preview)=>{
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
                    positions.AddRange(GetCellFromFixedMovement(unit,room,staticMovement));
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
    public static List<Vector3Int> GetCellFromFixedMovement(Unit unit, RoomView room, Vector3Int staticMovement, bool canAttack = true, bool onlyReturnAttack = false, bool megaSizeMovement = true)
    {
        List<Vector3Int> output = new List<Vector3Int>();
        bool obstructed = false;
        List<Vector3Int> temp = new List<Vector3Int>();
        bool isAttackingEnemy = false;
        foreach (Vector3Int cell in unit.occupiedCells)
        {
            Vector3Int movement;
            if (megaSizeMovement)
            {
                movement = cell + staticMovement * unit.megaSize;
            }
            else
            {
                movement = cell + staticMovement;
            }
            if (room.InBounds(movement) && cell != movement && room.GetTileAt(movement)?.name == GlobalHelper.GlobalVariables.TILE_GROUND)
            {
                Unit unitAt = room.GetUnitAt(movement);

                if (onlyReturnAttack)
                {
                    if (unitAt != null && unitAt.isEnemy != unit.isEnemy)
                    {
                        isAttackingEnemy = true;
                    }
                }
                //temp.Add(movement);
                if (unitAt != null && unitAt.UID == unit.UID && unitAt.isEnemy == unit.isEnemy)
                {
                }
                else
                {
                    if (canAttack || (!canAttack && unitAt == null) || (!canAttack && unitAt != null && unitAt.UID == unit.UID))
                    {
                        temp.Add(movement);
                    }
                    else
                    {

                        obstructed = true;
                        break;
                    }
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
            if (onlyReturnAttack && isAttackingEnemy)
            {

                output.AddRange(temp);
            }
            else if (!onlyReturnAttack)
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
                for (int i = 0; i <= previewDepth; i++)
                {
                    output.Add(direction * i + new Vector3Int(x, y));
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

                        }
                        else if (!canAttack && u != null && (u.UID == unit.UID))
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


                        if (u != null && u.UID != unit.UID && canAttack)
                        {
                            maxDistance = i;
                            Debug.Log("Unit detected at " + movement + "From " + cell + " Detecting if next move would fit");
                            //Evaluate the next set of cells
                            List<Vector3Int> extraMove = new List<Vector3Int>();
                            foreach (Vector3Int cell2 in unit.occupiedCells)
                            {
                                Vector3Int nextCell = cell2 + direction * (i + 1) + direction;
                                Debug.Log("Evaluating original cell" + cell2 + " at " + nextCell);
                                if (room.InBounds(nextCell))
                                {
                                    if (room.GetTileAt(nextCell)?.name == GlobalHelper.GlobalVariables.TILE_GROUND)
                                    {
                                        Unit u2 = room.GetUnitAt(nextCell);
                                        if ((u2 != null && (u2.isEnemy != unit.isEnemy || u2.UID == unit.UID)) || u2 == null)
                                        {
                                            extraMove.Add(nextCell);
                                            Debug.Log("Adding extra cell" + cell2 + " at " + nextCell);
                                        }

                                    }
                                }
                            }
                            //If it fits then bravo 
                            if (extraMove.Count == unit.occupiedCells.Count)
                            {
                                if (extraMove.Contains(movement))
                                {
                                    List<Unit> targets = new List<Unit>();
                                    foreach (Vector3Int cellExtraMovement in extraMove)
                                    {
                                        Unit unitTarget = room.GetUnitAt(cellExtraMovement);

                                        if (unitTarget != null)
                                        {
                                            targets.Add(unitTarget);
                                        }
                                    }
                                    bool valid = true;
                                    for (int j = 0; j < extraMove.Count; j++)
                                    {
                                        if (isPathBlocked(unit.occupiedCells[j], extraMove[j], targets, unit, room))
                                        {
                                            valid = false;
                                            Debug.Log("INVALID");
                                            break;
                                        }

                                    }
                                    if (valid)
                                    {
                                        Debug.Log("Can fit one more dude in there !");
                                        maxDistance = i + 1;
                                        output.AddRange(extraMove);
                                    }



                                }

                            }


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

    public static bool isPathBlocked(Vector3Int start, Vector3Int destination, List<Unit> targets, Unit u, RoomView r)
    {

        Vector3Int direction = destination - start;
        int x = direction.x == 0 ? 0 : (int)Mathf.Sign(direction.x);
        int y = direction.y == 0 ? 0 : (int)Mathf.Sign(direction.y);

        Vector3Int unit = new Vector3Int(x, y);
        Vector3Int movement = start;
        bool blocked = false;
        int i = 0;
        while (movement != destination)
        {
            if (i > 1000)
            {
                Debug.LogError("The calculations are wrong chief");
                break;
            }
            i++;
            movement = start + unit * i;
            if (r.InBounds(movement) && r.GetTileAt(movement)?.name != GlobalHelper.GlobalVariables.TILE_WALL)
            {
                Unit unitAt = r.GetUnitAt(movement);

                if (unitAt != null && unitAt.UID != u.UID && !targets.Contains(unitAt))
                {
                    blocked = true;
                    break;
                }
            }
            else
            {
                blocked = true;
            }
        }
        return blocked;

    }
    public static bool isGroundCell(Vector3Int cell, RoomView room)
    {
        if (room.InBounds(cell))
        {
            if (room.GetTileAt(cell)?.name == GlobalHelper.GlobalVariables.TILE_GROUND)
            {
                return true;
            }
        }
        return false;
    }
    #endregion;
    public static int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }
    }
}
