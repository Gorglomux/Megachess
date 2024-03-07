using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface ISelectable
{
    bool onSelect();
    void onSelectTick(Vector3 mousePosition);
    void onDeselect(Vector3 mousePosition);
}
public interface IHoverable
{
    void onHoverEnter();
    void onHoverExit();
}

public interface IDraggable
{
    Sprite onDragBegin(Vector3 mousePosition);
    void onDragEnd(Vector3 mousePosition);
    void onDragTick(Vector3 mousePosition);
}

public class Unit : MonoBehaviour , ISelectable, IHoverable, IDraggable
{
    public UnitData unitData;
    public int UID;
    public List<Vector3Int> occupiedCells = new List<Vector3Int>();
    
    public SpriteRenderer spriteRenderer;
    public SendSpriteAtlasPosition sendAtlasPosition;

    public List<Func<List<Vector3Int>, Tilemap>> movementMethods;
    public List<Func<List<Vector3Int>, Tilemap>> attackMethods;

    public  int basePaletteIndex = -1;

    public List<EffectData> currentEffects;
    public bool isEnemy = false;

    public int health;
    public int megaSize = 1;

    public int maxActions = 1;
    public int actionsLeft = 1;

    public Sequence idleSequence;
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

    public GameObject bloodEffectPrefab;
    public List<Texture2D> bloodEffectTextures;

    //Drag related variables 
    public Color ColorDragged;

    // Start is called before the first frame update
    void Start()
    {

    }
    public void Initialize(UnitData ud, bool isEnemy, int palette = -1)
    {
        unitData = ud;

        currentEffects = ud.effectDatas;
        this.isEnemy = isEnemy;
        LoadPalette(palette);
        health = occupiedCells.Count;
        UID = GlobalHelper.GetUID();
        megaSize = (int)Mathf.Sqrt(occupiedCells.Count);
        if(megaSize == 0)
        {
            megaSize = 1;
        }

    }
    public Sequence s = null;
    public Tween tweenIdle = null;
    public void StartIdle()
    {
        if(s != null)
        {
            s.Kill();
        }
        if(tweenIdle != null)
        {
            tweenIdle.Kill();
        }
        tweenIdle = spriteRenderer.transform.DORotate(Vector3.zero,0.1f);
        tweenIdle.onComplete += () =>
        {
            s = DOTween.Sequence();
            float duration = 0.5f;
            s.PrependInterval(0.3f);
            Tween t = spriteRenderer.transform.DOShakeRotation(1f, new Vector3(0, 0, 9), 5, 90);
            s.Append(t);
            s.AppendInterval(duration);
            s.SetLoops(-1);
        };

    }
    public void EndIdle()
    {
        if(s != null)
        {
            s.Kill();
            spriteRenderer.transform.DORotate(Vector3.zero, 0.2f);
        }if(tweenIdle != null)
        { 
            tweenIdle.Kill();
        }

    }

    public void LoadPalette(int palette = -1, bool megaTransform = false)
    {
        string spriteName = "";
        if (isEnemy)
        {
            spriteName = unitData.sprite_NB.name;
            sendAtlasPosition.referenceSprite = unitData.sprite_NB;
        }
        else
        {
            spriteName = unitData.sprite.name;
            sendAtlasPosition.referenceSprite = unitData.sprite;
        }
        sendAtlasPosition.m = new Material(GlobalHelper.GlobalVariables.unitMaterial);
        if (basePaletteIndex == -1 || megaTransform)
        {
            basePaletteIndex = GlobalHelper.GlobalVariables.gameInfos.currentArea.paletteIndex;
            unitData.paletteIndex = basePaletteIndex;
            sendAtlasPosition.m.SetFloat("_PaletteIndex", basePaletteIndex);
        }
        else
        {
            //This is for debug 
            sendAtlasPosition.m.SetFloat("_PaletteIndex", basePaletteIndex);
        }

        //Load the palette according to the time created;

        sendAtlasPosition.m.SetFloat("_Dither", 0);
        sendAtlasPosition.SendPosition();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public Sequence Attack(List<Vector3Int> positions)
    {
        Sequence attackSequence = DOTween.Sequence();
        if (roomRef.isValidAttack(this,positions))
        {
            bool hasMoved = false;
            List<Unit> targeted = roomRef.GetUnitsAt(positions);
            if(targeted != null && targeted.Count > 0)
            {

                bool onlySelf = true;
                foreach (Unit targetedUnit in targeted)
                {
                    if(targetedUnit.UID != UID)
                    {
                        onlySelf = false;
                        hasMoved = true;
                        Tween t = transform.DOLocalMove(GetProjectedWorldPosition(positions), GlobalHelper.TWEEN_DURATION_MOVE).SetEase(Ease.InBack, GlobalHelper.TWEEN_OVERSHOOT_MOVE);
                        t.onComplete += () =>
                        {
                            MakeBloodAt(targetedUnit.GetWorldPosition(), (targetedUnit.GetWorldPosition() - GetWorldPosition()).normalized);

                            if (!targetedUnit.TakeDamage(occupiedCells.Count))
                            {
                                Tween t = transform.DOLocalMove(GetWorldPosition(), GlobalHelper.TWEEN_DURATION_MOVE*0.5f).SetEase(Ease.OutCubic);
                            } 
                            GlobalHelper.getCamMovement().ShakeCamera(megaSize * 0.2f * GlobalHelper.CAM_SHAKE_ATTACK);
                        };
                        attackSequence.Join(t);
                    }
                }
                if (!onlySelf && !isEnemy && !roomRef.firstAttackThisRound)
                {
                    roomRef.firstAttackThisRound = true;
                    attackSequence.Prepend(GlobalHelper.getCamMovement().ZoomToPosition(GetWorldPosition() + (targeted[0].GetWorldPosition() - GetWorldPosition()) / 2));
                }
                else if (isEnemy)
                {
                    attackSequence.Prepend(GlobalHelper.getCamMovement().ZoomToPosition(GetWorldPosition() + (targeted[0].GetWorldPosition() - GetWorldPosition()) / 2));

                }
            }
            attackSequence.onComplete += () =>
            {

                if (unitData.moveAfterAttack)
                {
                    Move(positions, !hasMoved);
                    GlobalHelper.getCamMovement().ResetCameraPosition();
                    GlobalHelper.GlobalVariables.indicatorManager.HideAll();
                    if (actionsLeft > 0 && !isEnemy)
                    {
                        StartIdle();
                    }
                    else if (!isEnemy)
                    {
                        EndIdle();
                    }
                }
            };
        }
        roomRef.OnBoardUpdate();
        attackSequence.Play();
        return attackSequence;
    }
    public void MakeBloodAt(Vector3 position, Vector3 direction)
    {
        GameObject randomBlood = GameObject.Instantiate(bloodEffectPrefab);
        ParticleSystem ps = randomBlood.GetComponentInChildren<ParticleSystem>();
        Renderer psRenderer= ps.GetComponentInChildren<Renderer>();
        psRenderer.material = new Material(GlobalHelper.GlobalVariables.paletteMaterial);
        psRenderer.material.SetTexture("_MainTex",bloodEffectTextures[UnityEngine.Random.Range(0, bloodEffectTextures.Count)]);
        psRenderer.material.SetFloat("_PaletteIndex", basePaletteIndex);
        psRenderer.material.SetFloat("_Lerp", 0);

        var module = ps.main;
        GlobalHelper.GlobalVariables.bloodSplatManager.splats.Add(randomBlood);
        // Calculate the angle in radians and convert it to degrees
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        ps.Play();
        randomBlood.transform.position = position;
        randomBlood.transform.eulerAngles = new Vector3(0,0,- angle);

    }

    public bool Move(List<Vector3Int> positions, bool animate = true)
    {
        bool MOVE_SUCCESS = false;

        if(roomRef.isValidMove(this, positions)){
            //Animate here
            //On complete : MoveUnit 
            roomRef.MoveUnit(this, positions);
            if (animate)
            {
                Tween t = transform.DOLocalMove(GetProjectedWorldPosition(positions), GlobalHelper.TWEEN_DURATION_MOVE*0.4f).SetEase(Ease.OutQuint);
            }
            else
            {
                transform.localPosition = GetWorldPosition();

            }
            MOVE_SUCCESS = true;

        }

        List<Vector3Int> result = roomRef.CheckMega(this, occupiedCells[0]);
        if (result != null && result.Count > 0)
        {
            roomRef.MakeMega(this, result);
        }

        GlobalHelper.GlobalVariables.indicatorManager.HideAll();
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
    public Vector3 GetProjectedWorldPosition(List<Vector3Int> positions)
    {
        RoomView rv = GlobalHelper.GetRoom();
        Vector3 meanPosition = Vector3.zero;
        foreach (Vector3Int position in positions)
        {

            meanPosition += rv.tilemapFloorWalls.CellToLocal(rv.CellToTilemap(position));
        }
        meanPosition /= occupiedCells.Count;
        meanPosition += rv.tilemapFloorWalls.cellSize / 2;

        return meanPosition;
    }

    public bool TakeDamage(int damageCount)
    {
        health -= damageCount;
        if(health <= 0)
        {
            //Destroy me 
            roomRef.DestroyUnit(this);
            return true;
        }
        else
        {
            float f = Mathf.Lerp(5, 16, 1- ((float)health / (float)occupiedCells.Count));
            spriteRenderer.material.SetFloat("_Dither",f);
            return false;
        }
    }


    public bool blinking = false;
    public Coroutine tweenBlink;
    public void ToggleBlink(bool enabled, float blinkspeed = 1)
    {
        if (blinking && !enabled)
        {
            blinking = false;
            spriteRenderer.color = new Color(1, 1, 1, 1);
            StopCoroutine(tweenBlink);
        }
        else if (!blinking && enabled)
        {
            tweenBlink = StartCoroutine(corBlink(frameOutDelaySelected / blinkspeed, frameInDelaySelected/blinkspeed));
            //tweenBlink = spriteRenderer.DOColor(new Color(1, 1, 1, 0.7f), 0.5f).SetEase(Ease.Flash,20,0).SetLoops(-1, LoopType.Restart);
            blinking = true;
        }

    }


    [Range(0,1)]public float frameOutDelaySelected = 0.17f;
    [Range(0, 1)] public float frameInDelaySelected = 0.44f;
    public IEnumerator corBlink( float outDelay, float inDelay)
    {

        while (true)
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(inDelay);
            spriteRenderer.color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(outDelay);

        }


    }


    public List<Vector3Int> EvaluateAroundPosition(Vector3 position)
    {


        List<Vector3Int> output = new List<Vector3Int>();
        //Get closest tile index from mousePosision - megasize 

        Vector3Int cellCoords = roomRef.tilemapEntities.WorldToCell(position) - new Vector3Int(megaSize / 2, megaSize / 2);
        for (int y = 0; y < megaSize; y++)
        {
            for (int x = 0; x < megaSize; x++)
            {
                Vector3Int tilemapCoords = roomRef.TilemapToCell(cellCoords) + new Vector3Int(x, y);
                TileBase tb = roomRef.GetTileAt(tilemapCoords);
                if (tb != null)
                {
                    output.Add(tilemapCoords);
                }
            }
        }
        //regarding position + megasize * cellSize / 2 

        bool hasMoved = false;
        foreach(Vector3Int cell in output)
        {
            if (!occupiedCells.Contains(cell))
            {
                hasMoved = true;
                break;
            }
        }
        if (!hasMoved )
        {
           
            output.Clear();
        }
        return output;
    }
    public List<Vector3Int> EvaluateAroundCellPosition(Vector3Int cellPosition)
    {

        List<Vector3Int> output = new List<Vector3Int>();
        //Get closest tile index from mousePosision - megasize 

        for (int y = 0; y < megaSize; y++)
        {
            for (int x = 0; x < megaSize; x++)
            {
                Vector3Int tilemapCoords = cellPosition + new Vector3Int(x, y);
                TileBase tb = roomRef.GetTileAt(tilemapCoords);
                if (tb != null)
                {
                    output.Add(tilemapCoords);
                }
            }
        }
        return output;

    }

    public bool TryMoveOrAttackAtPosition(Vector3 position)
    {
        bool success = false;
        position = new Vector3(position.x,position.y,0);
        //else
        List<Vector3Int> evaluated = EvaluateAroundPosition(position);
        if (Mathf.Sqrt(evaluated.Count) == megaSize)
        {
            List<Vector3Int> validPositions = MovementMethods.GetMovementMethod(unitData.unitName).Invoke(roomRef, this, -1 );
            bool evaluatedCorrect = true;
            foreach (Vector3Int cell in evaluated)
            {
                if (!validPositions.Contains(cell))
                {
                    evaluatedCorrect = false;
                    break;
                }
            }
            if (evaluatedCorrect)
            {
                if (ConsumeAction())
                {
                    Attack(evaluated);
                    success = true;
                }
                else
                {
                    print("No more actions ! ");
                }
            }

        }
        return success;
    }
    #region interfaces
    public Tween growTween;
    public void onHoverEnter()
    {
        if(growTween != null)
        {
            growTween.Kill();
        }
        print(UID + " is hovered !");
        //DoScale if ally? 
        GlobalHelper.GlobalVariables.indicatorManager.DisplayMovement(this);
        growTween = spriteRenderer.transform.DOScale(1.2f, 0.5f).SetEase(Ease.OutQuint);
        GlobalHelper.UI().ShowHoverInfos(this);

    }

    public void onHoverExit()
    {
        if(spriteRenderer== null)
        {
            return;
        }
        if (growTween != null)
        {
            growTween.Kill();
        }
        print(UID + " exit hover!");
        GlobalHelper.GlobalVariables.indicatorManager.HideAll();
        growTween = spriteRenderer.transform.DOScale(1, 0.2f).SetEase(Ease.OutQuint);
        GlobalHelper.UI().HideHoverInfos();

    }

    public bool onSelect()
    {

        GlobalHelper.UI().ShowHoverInfos(this);
        bool success = true;
        if (isEnemy || actionsLeft <= 0)
        {
            success = false;
        }
        else if(actionsLeft > 0)
        {
            EndIdle();
            print(UID + " is selected");
            GlobalHelper.GlobalVariables.indicatorManager.DisplayMovement(this);
            spriteRenderer.transform.DOScale(1.3f, 0.5f).SetEase(Ease.OutBack);
        }
        return success;
    }

    public void onDeselect(Vector3 mousePosition)
    {
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0);


        print(UID + " is deselected");

        if (GlobalHelper.GlobalVariables.gameInfos.gameState is UnitPlaceState)
        {
            MoveInUnitPlace();
        }
        else
        {
            if (!TryMoveOrAttackAtPosition(mousePosition))
            {
                StartIdle();
            }
           
        }
        spriteRenderer.transform.DOScale(1, 0.5f).SetEase(Ease.OutQuint);

        GlobalHelper.GlobalVariables.indicatorManager.HideAll();
    }
    public void onSelectTick(Vector3 mousePosition)
    {
        if(actionsLeft > 0)
        {
        mousePosition = new Vector3(mousePosition.x, mousePosition.y,0);
        List<Vector3Int> evaluated = EvaluateAroundPosition(mousePosition);
        GlobalHelper.GlobalVariables.indicatorManager.ShowPossibleUnitMove(this, evaluated); 
        }


    }


    public void onDragTick(Vector3 mousePosition)
    {
    }

    public Sprite onDragBegin(Vector3 mousePosition)
    {
        EndIdle();
        spriteRenderer.color = ColorDragged;
        return unitData.sprite;
    }

    public void onDragEnd(Vector3 mousePosition)
    {
        spriteRenderer.color = Color.white;
        if(GlobalHelper.GlobalVariables.gameInfos.gameState is UnitPlaceState)
        {
            MoveInUnitPlace();
        }
        else
        {
            /*
            if (!TryMoveOrAttackAtPosition(mousePosition))
            {
                StartIdle();
            }
            else
            {
                EndIdle();
            }*/
        }


        print("Attack?");
        //Attack or move here
    }
    #endregion
    public void MoveInUnitPlace()
    {
        List<Vector3Int> evaluated = EvaluateAroundPosition(GlobalHelper.GetMouseWorldPosition());
        if (evaluated.Count > 0)
        {
            bool allCorrect = true;
            foreach (Vector3Int point in evaluated)
            {
                if (!roomRef.SpawnableCells.Contains(roomRef.CellToTilemap(point)))
                {
                    allCorrect = false;
                    break;
                }
            }
            if (allCorrect)
            {
                Move(evaluated);
            }
        }
    }

    public void RefreshActions()
    {
        actionsLeft = maxActions;
    }
    public bool ConsumeAction()
    {
        if(actionsLeft > 0)
        {
            actionsLeft--;
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator EnemyAttack()
    {
        //Foreach cells in attackPattern
        //Evaluate around position 
        List<Vector3Int> validPositions = MovementMethods.GetMovementMethod(unitData.unitName).Invoke(roomRef, this, -1);
        foreach (Vector3Int validPosition in validPositions)
        {
            List<Vector3Int> evaluated = EvaluateAroundCellPosition(validPosition);
            //If all cells in attack pattern
            if (Mathf.Sqrt(evaluated.Count) == megaSize)
            {
                Unit unitToBeat = null; 
                //Loop through the cells}
                foreach(Vector3Int point in evaluated)
                {
                    if (!validPositions.Contains(point))
                    {
                        break;
                    }
                    if(unitToBeat == null)
                    {
                        Unit u = roomRef.GetUnitAt(point);
                        if(u != null && u.isEnemy != isEnemy)
                        {
                            unitToBeat = u;

                        }

                    }

                }
                if(unitToBeat != null && unitToBeat.isEnemy != isEnemy)
                {
                    if (ConsumeAction())
                    {

                        yield return Attack(evaluated).WaitForCompletion();
                        if (unitData.moveAfterAttack)
                        {
                            Move(evaluated, false);
                        }
                    }
                    else
                    {
                        yield break;
                    }
                }

            }

        }



    }
}
