using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GridController
{
    private static Vector2Int[] arrCoordinateMatrises = new Vector2Int[]
     {
        new Vector2Int( 0,-1),
        new Vector2Int( 1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int( 0, 1),
     };

    private CanvasScaler canvasScaler;
    private RectTransform gridCenterTransform;
    private RectTransform gridOverlayTransform;
    public RectTransform GridOverlay => gridOverlayTransform;
    private RectTransform gridFrame;

    private uint gridRowCount;
    private uint gridColumnCount;
    private float gridCellSpacing;
    private List<(Vector2Int, GridChangeEventType, IGridEntityTypeDefinition)> lsCachedGridChanges = new();
    private UnityEvent<Vector2Int, GridChangeEventType, IGridEntityTypeDefinition> OnGridChange = new();
    public int RowCount { get { return (int)gridRowCount; } }
    public int ColumnCount { get { return (int)gridColumnCount; } }
    public float GridCellSpacing { get { return gridCellSpacing * canvasScaler.transform.localScale.x; } }
   
    public UnityEvent OnGridInterractable = new UnityEvent();
    public Vector2[,] arrGridPositions;
    public IGridEntity[,] arrEntityGrids { get; private set; }
    private bool[,] arrControlledGridCoordinates;
    public bool GridInterractable { get { return gridEventsInProgress == 0 && entitiesInProcess == 0 && gridDestroyed == false; } }
    private int entitiesInProcess = 0;
    private int gridEventsInProgress = 0;
    private bool gridDestroyed = false;
    
    private GridEntitySpawnController entitySpawner;
    private GridGoalController goalController;

    public GridController(GridControllerSettings settings, GridControllerSceneReferences references)
    {
        this.canvasScaler = references.CanvasScaler;
        this.gridCenterTransform = references.GridRect;
        this.gridOverlayTransform = references.GridOverlay;
        this.gridFrame = references.GridFrame;
        gridRowCount = settings.RowCount;
        gridColumnCount = settings.ColumnCount;

        CalculateCellSpacing(settings, references);
        CreateGridAndCalculatePositions();
        ResizeGridFrame(settings);
    }

    public void StartGrid(GridEntitySpawnController entitySpawner, GridGoalController goalController)
    {
     
        this.entitySpawner = entitySpawner;
        this.goalController = goalController;
        this.entitySpawner.FillAllGridWithStartLayout();
        UpdateAllEntities();
    }

    #region Private Methods
    private void CalculateCellSpacing(GridControllerSettings settings, GridControllerSceneReferences references)
    {
        float rowCellSpacing = references.GridRect.rect.width / (settings.MaxEntitiesPerSide);
        float columnCellSpacing = references.GridRect.rect.height / (settings.MaxEntitiesPerSide);
        gridCellSpacing = Mathf.Min(rowCellSpacing, columnCellSpacing);
    }

    private void WriteEntityMovementToGrid(Vector2Int newCoordinates, IGridEntity entity)
    {
        Vector2Int oldEntityCoordinates = entity.GridCoordinates;
        arrEntityGrids[entity.GridCoordinates.x, entity.GridCoordinates.y] = null;
        arrEntityGrids[newCoordinates.x, newCoordinates.y] = entity;
        entity.OnMoveEntity(newCoordinates, MovementMode.Linear);
        CacheGridChange(newCoordinates, GridChangeEventType.EntityMoved, entity.EntityType);
        CacheGridChange(oldEntityCoordinates, GridChangeEventType.EntityMoved, entity.EntityType);
    }

    private void CacheGridChange(Vector2Int changeCords, GridChangeEventType gridChangeEventType, IGridEntityTypeDefinition entityType)
    {
        lsCachedGridChanges.Add((changeCords, gridChangeEventType, entityType));
    }

   
  

    private void CreateGridAndCalculatePositions()
    {
        arrEntityGrids = new IGridEntity[gridRowCount, gridColumnCount];
        arrGridPositions = new Vector2[gridRowCount, gridColumnCount];
        arrControlledGridCoordinates = new bool[gridRowCount, gridColumnCount];

       
        float bottomLeftCornerX = gridCenterTransform.position.x - ((gridColumnCount / 2f) - .5f) * GridCellSpacing;
        float bottomLeftCornerY = gridCenterTransform.position.y - ((gridRowCount / 2f) - .5f) * GridCellSpacing;

        Vector2 bottomLeftCorner = new Vector2(bottomLeftCornerX, bottomLeftCornerY);
        Vector2 cursorPoint = bottomLeftCorner;
        for (int i = 0; i < arrGridPositions.GetLength(0); i++)
        {
            for (int j = 0; j < arrGridPositions.GetLength(1); j++)
            {
                arrGridPositions[i, j] = cursorPoint;
                cursorPoint.x += GridCellSpacing;
            }
            cursorPoint.y += GridCellSpacing;
            cursorPoint.x = bottomLeftCorner.x;
        }
    }

    private void ResizeGridFrame(GridControllerSettings settings)
    {
        Vector2 frameSizeAdd = new Vector2(settings.GridFrameWidthAdd, settings.GridFrameBottomAdd + settings.GridFrameTopAdd);
        gridFrame.sizeDelta = new Vector2(ColumnCount * gridCellSpacing, RowCount * gridCellSpacing) + frameSizeAdd * GridCellSpacing;
        gridFrame.transform.position += new Vector3(0, settings.GridFrameTopAdd - settings.GridFrameBottomAdd, 0) * GridCellSpacing;
    }

    private void CollectMatchingSurroundingEntitiesRecursive<T>(T entity, ref List<T> entityListToCollect) where T : IGridEntity
    {
        foreach (Vector2Int surroundingCoordinateAdd in arrCoordinateMatrises)
        {
            Vector2Int surroundingCoordinate = entity.GridCoordinates + surroundingCoordinateAdd;
           
            if (surroundingCoordinate.x < 0 || surroundingCoordinate.x == gridRowCount) continue;
            if (surroundingCoordinate.y < 0 || surroundingCoordinate.y == gridColumnCount) continue;
            if (arrControlledGridCoordinates[surroundingCoordinate.x, surroundingCoordinate.y]) continue;

            IGridEntity surroundingMatchingEntity = arrEntityGrids[surroundingCoordinate.x, surroundingCoordinate.y];
          
            if (surroundingMatchingEntity == null || surroundingMatchingEntity.EntityType != entity.EntityType) continue;

            
            arrControlledGridCoordinates[surroundingCoordinate.x, surroundingCoordinate.y] = true;
            T castEntity = (T)surroundingMatchingEntity;
            entityListToCollect.Add(castEntity);
            CollectMatchingSurroundingEntitiesRecursive(castEntity, ref entityListToCollect);
        }
    }
    #endregion

    #region Public Methods

    public void UpdateAllEntities()
    {
        Array.Clear(arrControlledGridCoordinates, 0, arrControlledGridCoordinates.Length);
        foreach (IGridEntity entity in arrEntityGrids)
        {

            if (entity == null) continue;

            entity.OnUpdateEntity();
        }
    }
    public void GridDestroyOnLevelClear()
    {
        gridFrame.localPosition = Vector2.zero;
        gridDestroyed = true;
        foreach (IGridEntity entity in arrEntityGrids)
        {
            if (entity == null) continue;
            entity.DestoryEntity(EntityDestroyTypes.DestroyedByLevelEnd);
        }
    }
    

    public List<IGridEntity> GetEntitiesTowardsRight(Vector2Int entityCords)
    {
        List<IGridEntity> entitiesInRow = new List<IGridEntity>();
        for (int i = entityCords.y; i < gridColumnCount; i++)
        {
            IGridEntity entity = arrEntityGrids[entityCords.x, i];
            if (entity != null)
            {
                entitiesInRow.Add(entity);
            }
        }
        return entitiesInRow;
    }

    public List<IGridEntity> GetEntitiesTowardsLeft(Vector2Int entityCords)
    {
        List<IGridEntity> entitiesInRow = new List<IGridEntity>();
        for (int i = entityCords.y; i >= 0; i--)
        {
            IGridEntity entity = arrEntityGrids[entityCords.x, i];
            if (entity != null)
            {
                entitiesInRow.Add(entity);
            }
        }
        return entitiesInRow;
    }

    public List<IGridEntity> GetEntitiesTowardsUp(Vector2Int entityCords)
    {
        List<IGridEntity> entitiesInRow = new List<IGridEntity>();
        for (int i = entityCords.x; i < gridRowCount; i++)
        {
            IGridEntity entity = arrEntityGrids[i, entityCords.y];
            if (entity != null)
            {
                entitiesInRow.Add(entity);
            }
        }
        return entitiesInRow;
    }

    public List<IGridEntity> GetEntitiesTowardsDown(Vector2Int entityCords)
    {
        List<IGridEntity> entitiesInRow = new List<IGridEntity>();
        for (int i = entityCords.x; i >= 0; i--)
        {
            IGridEntity entity = arrEntityGrids[i, entityCords.y];
            if (entity != null)
            {
                entitiesInRow.Add(entity);
            }
        }
        return entitiesInRow;
    }

   

    public void RegisterGridEntityToPosition(IGridEntity entity, int columnIndex, int rowIndex)
    {
        entity.OnEntityDestroyed.AddListener(goalController.OnEntityDestroyed); // subscribe GridGoalController to new entity to keep track of destroyed entities
        arrEntityGrids[columnIndex, rowIndex] = entity;
        CacheGridChange(new Vector2Int(columnIndex, rowIndex), GridChangeEventType.EntityMoved, entity.EntityType);
        OnGridChange.AddListener(entity.OnGridChange);
    }

    public void OnGridEventStart(IGridEvent gridEvent)
    {
       
        gridEventsInProgress++;
    }

    public void OnGridEventEnd(IGridEvent gridEvent)
    {
        if (gridDestroyed) return;
        gridEventsInProgress--;
      
        // call all changes made if grid events are completed
        if (gridEventsInProgress == 0)
        {
           
            CallCachedChanges();
            
            entitySpawner.SummonRequestedEntities();
        }
    }

    public void RemoveEntitiesFromGridArray<T>(List<T> entitiesToRemove, GridChangeEventType gridChangeEventType) where T : IGridEntity
    {
        foreach (T entityToRemove in entitiesToRemove) OnGridChange.RemoveListener(entityToRemove.OnGridChange);
        foreach (T entityToRemove in entitiesToRemove)
        {
            arrEntityGrids[entityToRemove.GridCoordinates.x, entityToRemove.GridCoordinates.y] = null;
            CacheGridChange(entityToRemove.GridCoordinates, gridChangeEventType, entityToRemove.EntityType);
        }
    }

    public void WriteEntityFall(IGridEntity gridEntity)
    {
        int columnIndex = gridEntity.GridCoordinates.y;
        int coordinateToFallTo = gridEntity.GridCoordinates.x;
        for (int i = gridEntity.GridCoordinates.x - 1; i >= 0; i--)
        {
            if (arrEntityGrids[i, columnIndex] == null) coordinateToFallTo = i;
            else break;
        }
        WriteEntityMovementToGrid(new Vector2Int(coordinateToFallTo, columnIndex), gridEntity);
    }

    public void CallCachedChanges()
    {
        while (lsCachedGridChanges.Count > 0)
        {
            
            OnGridChange.Invoke(lsCachedGridChanges[0].Item1, lsCachedGridChanges[0].Item2, lsCachedGridChanges[0].Item3);
            lsCachedGridChanges.RemoveAt(0);
        }
      
        UpdateAllEntities();
       
    }

    public void CallEntitySpawn(int columnIndex)
    {
        entitySpawner.AddEntitySpawnReqeust(columnIndex);
    }

    public void EntityStartProcess()
    {
        entitiesInProcess++;
    }

    public void EntityEndProcess()
    {
        entitiesInProcess--;
        if (GridInterractable && !gridDestroyed) { OnGridInterractable.Invoke(); }
    }

    public void CollectMatchingSurroundingEntities<T>(T entity, ref List<T> entityListToCollect) where T : IGridEntity
    {
        entityListToCollect.Add(entity);
        arrControlledGridCoordinates[entity.GridCoordinates.x, entity.GridCoordinates.y] = true;
        CollectMatchingSurroundingEntitiesRecursive(entity, ref entityListToCollect);
    }
    #endregion
}
