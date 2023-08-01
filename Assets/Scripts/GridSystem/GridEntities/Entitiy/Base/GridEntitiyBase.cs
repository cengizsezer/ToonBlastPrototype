using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GridEntitiyBase : PoolObject, IGridEntity
{
    [SerializeField] protected Image entityImage;
    protected GridController gridController;
    public IGridEntityTypeDefinition EntityType { get; protected set; }
    public Vector2Int GridCoordinates { get; protected set; }
    public Transform EntityTransform => transform;

    public UnityEvent<IGridEntity> OnEntityDestroyed { get; private set; } = new UnityEvent<IGridEntity>();

    public virtual bool IsDestroyableBy(EntityDestroyTypes destroyType) => !EntityType.ImmuneToDestroyTypes.Contains(destroyType);
    public virtual void OnGridChange(Vector2Int changeCoordinate, GridChangeEventType gridChangeEventType, IGridEntityTypeDefinition entityType)
    {
        if (GridCoordinates - changeCoordinate != new Vector2Int(1, 0)) return;
        if (gridController.arrEntityGrids[changeCoordinate.x, changeCoordinate.y] != null) return;
        gridController.WriteEntityFall(this);
    }

    public virtual void OnUpdateEntity()
    {
        //
    }

    public virtual void OnMoveEnded()
    {
        //
    }

    #region SetPropertyEntitiy
    public void SetupEntity(GridController grid, IGridEntityTypeDefinition blockType)
    {
        gridController = grid;
        EntityType = blockType;
        SetBlockImage(blockType.DefaultEntitySprite);
    }

    public void SetBlockImage(Sprite sprite)
    {
        if (sprite == null || entityImage == null) return;
        entityImage.sprite = sprite;
    }

    public virtual void OnMoveEntity(Vector2Int newCoordinates, MovementMode movementMode)
    {
        MoveToCoordinate(newCoordinates, movementMode);
    }

    #endregion

    #region Destroy Entities

    protected bool _inProcess;
    protected void MoveToCoordinate(Vector2Int newCoordinates, MovementMode movementMode)
    {
        KillLastTween();
        ProcessStarted();
        GridCoordinates = newCoordinates;
        Vector2 targetPos = gridController.arrGridPositions[newCoordinates.x, newCoordinates.y];
        Tween moveTween = null;
        switch (movementMode)
        {
            case MovementMode.Linear:
                moveTween = TweenHelper.BouncyMoveTo(transform, targetPos);
                break;
            case MovementMode.Curvy:
                moveTween = TweenHelper.CurvingMoveTo(transform, targetPos);
                break;
            default:
                break;
        }
        moveTween.onComplete += () => { ProcessEnded(); OnMoveEnded(); };
        CacheTween(moveTween);
    }

    private void ProcessStarted()
    {
        gridController.EntityStartProcess();
        _inProcess = true;
    }

    private void ProcessEnded()
    {
        gridController.EntityEndProcess();
        _inProcess = false;
    }

    public virtual void DestoryEntity(EntityDestroyTypes destroyType)
    {
        OnEntityDestroyed.Invoke(this);
        GoPool();
    }

    protected virtual void PlayOnDestroyAudio()
    {
        if (EntityType.OnDestroyAudio) AudioManager.Instance.PlayAudio(EntityType.OnDestroyAudio, AudioManager.PlayMode.Single, 1);
    }
    private void SummonOnDestroyParticle()
    {
        if (!EntityType.OnDestroyParticle) return; // dont do anyting if no death particles are provided
        //particle Play
       
    }

    #endregion

    #region TWEEN

    protected Tween _lastTween = null;

    public void CacheTween(Tween tween)
    {
        _lastTween = tween;
    }

    protected void CompleteLastTween()
    {
        if (_lastTween != null) _lastTween.Complete(true);
    }

    protected void KillLastTween()
    {
        if (_lastTween == null) return;
        _lastTween.Kill(true);
    }
    #endregion

    #region POOLING METHODS

    public override void OnCreated()
    {
        
        OnDeactive();
    }

    public override void OnDeactive()
    {
        
        transform.SetParent(null);
        gameObject.SetActive(false);

       
    }

    protected void GoPool()
    {
        if (_inProcess) ProcessEnded();
        PlayOnDestroyAudio();
        SummonOnDestroyParticle();
        KillLastTween();
        OnEntityDestroyed.RemoveAllListeners();
        OnDeactive();
    }

    public override void OnSpawn()
    {
        EntityType = null;
        gameObject.SetActive(true);
    }

    #endregion
}
