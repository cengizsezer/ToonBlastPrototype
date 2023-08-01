using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] LevelSettings[] levelList;
    [Group] [SerializeField] LevelSceneReferences levelSceneReferences;

    public LevelController CurrentLevel { get; private set; }

    public LevelSettings CurrentLevelConfig
    {
        get
        {
            if (GameManagerSaveData.Data.CurrentLevelIndex == levelList.Length)
            {
                Debug.Log("All Levels Completed Restarting");
                GameManagerSaveData.Data.ResetLevelIndex();
            }
            return levelList[GameManagerSaveData.Data.CurrentLevelIndex];
        }
    }
    private LevelSettings chosenLevelConfig;

    private void Start()
    {
        
    }
    public void CreateCurrentLevel()
    {
        CurrentLevel = new LevelController(CurrentLevelConfig, levelSceneReferences);
    }



#if UNITY_EDITOR

    [EasyButtons.Button(Mode = EasyButtons.ButtonMode.EnabledInPlayMode)]
    private void WinLevel()
    {
        CurrentLevel.LevelCleared();
    }

    [EasyButtons.Button(Mode = EasyButtons.ButtonMode.EnabledInPlayMode)]
    private void LoseLevel()
    {
        CurrentLevel.LevelFailed();
    }

    [EasyButtons.Button(Mode = EasyButtons.ButtonMode.DisabledInPlayMode)]
    private void DeleteAllSaves()
    {
        DeleteAllData.Delete();
    }

    private void OnDrawGizmos()
    {
        DrawGridEntityNames();
    }

    private void DrawGridEntityNames()
    {
        if (CurrentLevel == null) return;
        foreach (IGridEntity entity in CurrentLevel.GridController.arrEntityGrids)
        {
            if (entity == null || entity.EntityType == null) continue;
            Gizmos.color = Color.red;
            Extensions.drawString(entity.EntityType.GridEntityTypeName, entity.EntityTransform.position, Color.black);
        }
    }
#endif
}
