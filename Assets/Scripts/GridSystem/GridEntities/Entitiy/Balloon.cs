using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Balloon : GridEntitiyBase
{
    public override void OnGridChange(Vector2Int changeCoordinate, GridChangeEventType gridChangeEventType, IGridEntityTypeDefinition entityType)
    {
        base.OnGridChange(changeCoordinate, gridChangeEventType, entityType);
        if (gridChangeEventType == GridChangeEventType.EntityMatched && entityType is BlockTypeDefinition)
        {
            if ((GridCoordinates - changeCoordinate).magnitude <= 1)
            {
                DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent(EntityDestroyTypes.DestroyedByNearbyMatch);
                destroyEvent.OnStart(gridController, new List<Balloon>() { this });
            }
        }

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

    private void OnEntityDestroy()
    {
        OnEntityDestroyed.Invoke(this);
        GoPool();
    }
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
    }


}
