using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Block : GridEntitiyBase
{
    public List<Block> CurrentMatchGroup;
    public int GroupSize { get { if (CurrentMatchGroup == null) return 0; return CurrentMatchGroup.Count; } }

    private bool MatchGroupCalculated = false;

    public override void OnMoveEntity(Vector2Int newCoordinates, MovementMode movementMode)
    {
        // if this block moved, we need to recalculate the match group
        MatchGroupCalculated = false;
        base.OnMoveEntity(newCoordinates, movementMode);
    }
    public override void OnGridChange(Vector2Int changeCoordinate, GridChangeEventType gridChangeEventType, IGridEntityTypeDefinition entityType)
    {
        base.OnGridChange(changeCoordinate, gridChangeEventType, entityType);

        // if a surrounding block is changed, we need to recalculate the match group
        if ((GridCoordinates - changeCoordinate).magnitude == 1)
            MatchGroupCalculated = false;
    }


    public void OnClickBlock()
    {

        if (this is not Block) return;

        TryMatch();
    }

    public override void OnUpdateEntity()
    {
        base.OnUpdateEntity();
        CheckMatchGroup();
    }

    private void TryMatch()
    {
        if (!gridController.GridInterractable) return;
        bool matchSuccess = GameManager.I.CurrentLevel.MovesController.TryMakeMatchMove(this);
        if (!matchSuccess) MatchFail();
    }

    private void MatchFail()
    {
        AnimateShake();
    }

    public void AnimateShake()
    {
        CompleteLastTween();
        _lastTween = TweenHelper.Shake(transform);
    }

    public override void DestoryEntity(EntityDestroyTypes destroyType)
    {
        AnimateDestroy();
    }

    public void AnimateDestroy()
    {
        CompleteLastTween();
        _lastTween = TweenHelper.PunchScale(transform, OnEntityDestroy);
    }

    public void MoveToPointThanDestroy(Vector3 position)
    {
        _lastTween = transform.DOMove(position, .3f).SetEase(Ease.InBack);
        _lastTween.onComplete += () => OnEntityDestroy();
    }


    private void OnEntityDestroy()
    {
        OnEntityDestroyed.Invoke(this);
        GoPool();
    }

    #region MATCH METHODS

    private void CheckMatchGroup()
    {
        if (MatchGroupCalculated) return;

        List<Block> blockGroup = new List<Block>();
        gridController.CollectMatchingSurroundingEntities<Block>(this, ref blockGroup);

        AssignMatchGroup(blockGroup);
        BlockMatchCondition? condition = ActiveBlockCondition();

        
        foreach (Block block in blockGroup)
        {
            block.AssignMatchGroup(blockGroup);
            block.SetSpriteOfCondition(condition);
        }
    }

    public BlockMatchCondition? ActiveBlockCondition()
    {
        BlockTypeDefinition blockTypeDefinition = EntityType as BlockTypeDefinition;
        if (blockTypeDefinition != null)
        {
            foreach (var blockMatchCondition in blockTypeDefinition.BlockMatchConditions)
            {
                if (blockMatchCondition.Condition.IsConditionMet(this))
                {
                    return blockMatchCondition;
                }
            }
        }
        return null;
    }

    private void SetSpriteOfCondition(BlockMatchCondition? condition)
    {
        if (condition != null) entityImage.sprite = condition.Value.Sprite;
        else entityImage.sprite = EntityType.DefaultEntitySprite;
    }

    public void AssignMatchGroup(List<Block> group)
    {
        MatchGroupCalculated = true;
        CurrentMatchGroup = group;
    }

    #endregion

    #region POOLING METHOD
    public override void OnCreated()
    {
        OnDeactive();
    }

    public override void OnDeactive()
    {
       
        transform.SetParent(null);
        gameObject.SetActive(false);
    }
    public override void OnSpawn()
    {
        gameObject.SetActive(true);
        CurrentMatchGroup = null;
        //CurrentMatchGroup.Clear();




    }

    #endregion
}
