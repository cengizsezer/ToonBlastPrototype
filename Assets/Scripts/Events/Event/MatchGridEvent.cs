using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MatchGridEvent : IGridEvent, IDisposable
{
    private GridController gridController;
    private Vector3 matchMergePosition;
    private Vector2Int matchMergeCoordinates;
    private BlockMatchCondition? activeMatchCondition;
    private int _entitiesToDestroy;
    private int _entitiesDestroyed;

    public MatchGridEvent(Vector3 matchMergePosition, Vector2Int matchMergeCoordinates, BlockMatchCondition? activeMatchCondition)
    {
        this.matchMergeCoordinates = matchMergeCoordinates;
        this.activeMatchCondition = activeMatchCondition;
        this.matchMergePosition = matchMergePosition;
    }

    public void OnComplate()
    {
        if (activeMatchCondition != null)
        {
          
            GridEntitySpawnController.Instance.SpawnEntity(activeMatchCondition.Value.GetRandomEntityToSpawn(), matchMergeCoordinates);
            GridEntitySpawnController.Instance.RemoveEntitySpawnReqeust(matchMergeCoordinates.y);
        }
        gridController.OnGridEventEnd(this);
    }

    public void OnStart<T>(GridController grid, List<T> effectedEntities) where T : IGridEntity
    {
        // remove entity from list if entity is immune to destroy type
        for (int i = 0; i < effectedEntities.Count; i++)
        {
            if (effectedEntities[i].IsDestroyableBy(EntityDestroyTypes.DestroyedByMatch))
            {
                _entitiesToDestroy++;
              
            }
            else
            {
                effectedEntities.RemoveAt(i);
                i--;
            }
        }

        if (effectedEntities.Count == 0) return;
        gridController = grid;

        gridController.OnGridEventStart(this);
        gridController.RemoveEntitiesFromGridArray(effectedEntities, GridChangeEventType.EntityMatched);

        foreach (IGridEntity entityObject in effectedEntities)
        {
            gridController.CallEntitySpawn(entityObject.GridCoordinates.y);
            entityObject.OnEntityDestroyed.AddListener(OnEntityDestroyed);

            if (activeMatchCondition == null)
            {
                // normal destroy
                entityObject.DestoryEntity(EntityDestroyTypes.DestroyedByMatch);
            }
            else
            {
                // on condition block merge destroy
                Block block = entityObject as Block;
                if (!block) Debug.LogError("Matched an object with match conditions but the object is not a block");
                block.MoveToPointThanDestroy(matchMergePosition);
            }
        }
    }

    private void OnEntityDestroyed(IGridEntity entityDestroyed)
    {
        _entitiesDestroyed++;
       
        if (_entitiesDestroyed == _entitiesToDestroy) OnComplate();
    }

    public void Dispose()
    {
        Debug.Log("Dispose");
    }
}
