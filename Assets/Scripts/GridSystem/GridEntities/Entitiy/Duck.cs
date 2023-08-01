using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Duck:GridEntitiyBase
{
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

    public override void OnMoveEnded()
    {
        base.OnMoveEnded();
        if (GridCoordinates.x == 0)
        {
            DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent(EntityDestroyTypes.DestroyedByFallOff);
            destroyEvent.OnStart(gridController, new List<Duck>() { this });
        }
    }

    public override void DestoryEntity(EntityDestroyTypes destroyType)
    {
        if (destroyType != EntityDestroyTypes.DestroyedByFallOff && destroyType != EntityDestroyTypes.DestroyedByLevelEnd) return;
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
}
