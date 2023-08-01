using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Level Settings")]
public class LevelSettings : ScriptableObject
{
    [Group] [SerializeField] private GridControllerSettings gridControllerSettings;
    [Group] [SerializeField] private GridEntitySpawnerSettings gridEntitySpawnerSettings;
    [Group] [SerializeField] private GridGoalControllerSettings goalsControllerSettings;
    [Group] [SerializeField] private MovesControllerSettings movesController;

    public GridControllerSettings GridControllerSettings => gridControllerSettings;
    public GridEntitySpawnerSettings GridEntitySpawnerSettings => gridEntitySpawnerSettings;
    public GridGoalControllerSettings GridGoalsControllerSettings => goalsControllerSettings;
    public MovesControllerSettings MovesControllerSettings => movesController;
}
