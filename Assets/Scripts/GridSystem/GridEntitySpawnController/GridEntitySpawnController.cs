using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GridEntitySpawnController:PocoSingleton<GridEntitySpawnController>
{
    private int[] BlockSpawnRequests { get; set; }

    private Vector2[] _spawnPositionRow;
    private IGridEntityTypeDefinition[] _gridEntityTypesToSpawnFrom;
    private RectTransform _gridParentTransform;
    private int _collumnCount;

    private GridController _gridController;
    private GridStartLayout _startLayout;

    public GridEntitySpawnController(GridController gridController, GridEntitySpawnerSettings settings, GridEntitySpawnerSceneReferences references)
    {
        Instance = this;
        this._gridEntityTypesToSpawnFrom = settings.EntityTypes;
        this._gridParentTransform = references.GridParentTransform;
        this._gridController = gridController;
        this._collumnCount = gridController.ColumnCount;
        this._startLayout = settings.gridStartLayout;
        BlockSpawnRequests = new int[_collumnCount];
        CalculateSpawnPositionRow(settings.SpawnHeight);
    }

    private void CalculateSpawnPositionRow(int spawnHeight)
    {
        _spawnPositionRow = new Vector2[_collumnCount];
        for (int i = 0; i < _collumnCount; i++)
        {
            _spawnPositionRow[i] = _gridController.arrGridPositions[_gridController.RowCount - 1, i] + new Vector2(0, spawnHeight);
        }
    }
    public void FillAllGridWithStartLayout()
    {
        TryLoadGridLayoutFromSavedData();
        StartFillBoardRequest();
        SummonRequestedEntities(_startLayout);
    }

    private void TryLoadGridLayoutFromSavedData()
    {
        if (!LevelSaveData.Data.HasLevelSaved) return;

        GridStartLayout gridLayout = new GridStartLayout(_gridController.RowCount, _gridController.ColumnCount);

        BaseEntitiyTypeDefinition[] entityTypes = new BaseEntitiyTypeDefinition[_gridController.arrEntityGrids.Length];
        for (int i = 0; i < entityTypes.Length; i++)
        {
            string entityTypeName = LevelSaveData.Data.SavedGrid[i];
            entityTypes[i] = AllGridEntities.GetEntityTypeByName(entityTypeName);
        }

        //isim olarak kaydediyor
        _startLayout = GridStartLayout.FromArray(_gridController.RowCount, _gridController.ColumnCount, entityTypes);
        //burada scriptable objeden gelen referansları atıyor
    }

    

    private void StartFillBoardRequest()
    {
        for (int i = 0; i < BlockSpawnRequests.Length; i++)
        {
            BlockSpawnRequests[i] = _gridController.RowCount;
        }
    }

    private void ClearRequests()
    {
        for (int i = 0; i < BlockSpawnRequests.Length; i++) BlockSpawnRequests[i] = 0;
    }

    // registers a spawn request to collumn index
    public void AddEntitySpawnReqeust(int collumnIndex)
    {
        BlockSpawnRequests[collumnIndex]++;
    }

    // unregisters a spawn request to collumn index
    public void RemoveEntitySpawnReqeust(int collumnIndex)
    {
        BlockSpawnRequests[collumnIndex]--;
    }

    // spawns grid entities at the requested collumns
    public void SummonRequestedEntities(GridStartLayout layout = null)
    {
       
        int[] summonRequestsCopy = (int[])BlockSpawnRequests.Clone();
        int blocksSummoned = 0;
        ClearRequests();

        for (int i = 0; i < _collumnCount; i++)
        {
            for (int j = summonRequestsCopy[i] - 1; j >= 0; j--)
            {
                blocksSummoned++;
              
                IGridEntityTypeDefinition randomEntityType = null;

                if (layout == null || layout.rows[j].row[i] == null)
                {
                    randomEntityType = _gridEntityTypesToSpawnFrom[UnityEngine.Random.Range(0, _gridEntityTypesToSpawnFrom.Length)];
                }
                else
                {
                    randomEntityType = layout.rows[j].row[i];
                }

                Vector2 spawnPos = _spawnPositionRow[i] - j * new Vector2(0, _gridController.GridCellSpacing);
                IGridEntity newEntity = PoolManager.I.ResolveFunc<IGridEntity, Block>(Configs.ObjectName.Block);
               
                if (newEntity is null)
                {
                    Debug.Log("Block NULL");
                }
                
                GameObject newEntityGO = (newEntity as PoolObject).gameObject;
                newEntityGO.transform.position = spawnPos;
                newEntityGO.transform.SetParent(_gridParentTransform);
                newEntityGO.gameObject.name = $"{randomEntityType.GridEntityTypeName} {i}_{j}";
                newEntityGO.GetComponent<RectTransform>().sizeDelta = new Vector2(_gridController.GridCellSpacing, _gridController.GridCellSpacing);

                newEntity.SetupEntity(_gridController, randomEntityType);
                Vector2Int gridCoordinates = new Vector2Int(_gridController.RowCount - j - 1, i);
                _gridController.RegisterGridEntityToPosition(newEntity, gridCoordinates.x, gridCoordinates.y);
                newEntity.OnMoveEntity(gridCoordinates, MovementMode.Linear);
            }
        }
        if (blocksSummoned > 0) _gridController.CallCachedChanges();
    }



    public void SpawnEntity(IGridEntityTypeDefinition entityType, Vector2Int gridCoordinates)
    {
        Vector2 spawnPos = _gridController.arrGridPositions[gridCoordinates.x, gridCoordinates.y];
        //IGridEntity newEntity = PoolManager.I.Resolve<IGridEntity>("Horizontal(Clone)");
        IGridEntity newEntity = PoolManager.I.ResolveFunc<IGridEntity, Rocket>(entityType.GridEntityPrefab.name);
        Debug.Log(newEntity);
        newEntity.SetupEntity(_gridController, entityType);

        GameObject newEntityGO = (newEntity as PoolObject).gameObject;
        newEntityGO.GetComponent<RectTransform>().sizeDelta = new Vector2(_gridController.GridCellSpacing, _gridController.GridCellSpacing);
        newEntityGO.transform.position = spawnPos;
        newEntityGO.transform.SetParent(_gridParentTransform);
        newEntityGO.gameObject.name = $"{entityType.GridEntityTypeName} {gridCoordinates.x}_{gridCoordinates.y}";

        _gridController.RegisterGridEntityToPosition(newEntity, gridCoordinates.x, gridCoordinates.y);
        newEntity.OnMoveEntity(gridCoordinates, MovementMode.Linear);
    }




}
