using System;
using UnityEngine;

[Serializable]
public class GridEntitySpawnerSettings
{
    [BHeader("Grid Start Layout")]
    public GridStartLayout gridStartLayout = new GridStartLayout(9, 9);

    [BHeader("Grid Entity Spawner Settings")]
    [SerializeField] private BaseEntitiyTypeDefinition[] entityTypes;
    [SerializeField] private int spawnHeight;

    public BaseEntitiyTypeDefinition[] EntityTypes => entityTypes;
    public int SpawnHeight => spawnHeight;
}
