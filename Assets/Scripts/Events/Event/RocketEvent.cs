using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RocketEvent : IGridEvent
{
    private GridController gridController;

    private EntityDestroyTypes entitityDestroyTypes;
    private int entitiesToDestroy;
    private int entitiesDestroyed;

    public RocketEvent(EntityDestroyTypes destroyType)
    {

        this.entitityDestroyTypes = destroyType;
    }

    public void OnComplate()
    {
        gridController.OnGridEventEnd(this);
    }

    public void OnStart<T>(GridController grid, List<T> effectedEntities) where T : IGridEntity
    {
        // remove entity from list if entity is immune to destroy type
        for (int i = 0; i < effectedEntities.Count; i++)
        {
            if (effectedEntities[i].IsDestroyableBy(entitityDestroyTypes)) entitiesToDestroy++;
            else
            {
                effectedEntities.RemoveAt(i);
                i--;
            }
        }
        if (effectedEntities.Count == 0) return;
        gridController = grid;

        gridController.OnGridEventStart(this);
        gridController.RemoveEntitiesFromGridArray(effectedEntities, GridChangeEventType.EntityDestroyed);

        Debug.Log(CoroutineMonoParent.I);
        CoroutineMonoParent.I.StartCoroutine(DestroyOneByOneRoutine(effectedEntities));
        
    }

    private IEnumerator DestroyOneByOneRoutine<T>(List<T> effectedEntities) where T : IGridEntity
    {
        foreach (IGridEntity entityObject in effectedEntities)
        {
            yield return new WaitForSeconds(.08f);
            gridController.CallEntitySpawn(entityObject.GridCoordinates.y);
            entityObject.OnEntityDestroyed.AddListener(OnEntityDestroyed);
            entityObject.DestoryEntity(entitityDestroyTypes);
        }
    }

    private void OnEntityDestroyed(IGridEntity entityDestroyed)
    {
        entitiesDestroyed++;
        if (entitiesDestroyed == entitiesToDestroy) OnComplate();
    }
}
