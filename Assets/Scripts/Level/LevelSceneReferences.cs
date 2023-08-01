using UnityEngine;


[System.Serializable]
public class LevelSceneReferences
{
    [Group] [SerializeField] private GridControllerSceneReferences gridControllerSceneReferences;
    [Group] [SerializeField] private GridEntitySpawnerSceneReferences gridEntitySpawnerSceneReferences;
    [Group] [SerializeField] private GridGoalControllerReferences goalsControllerReferences;
    [Group] [SerializeField] private MovesControllerSceneReferences movesControllerReferences;
    public GridControllerSceneReferences GridControllerSceneReferences => gridControllerSceneReferences;
    public GridEntitySpawnerSceneReferences GridEntitySpawnerSceneReferences => gridEntitySpawnerSceneReferences;
    public GridGoalControllerReferences GridGoalsControllerReferences => goalsControllerReferences;
    public MovesControllerSceneReferences MovesControllerReferences => movesControllerReferences;
}
