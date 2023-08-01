using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController
{
    public LevelSettings LevelSettings { get; private set; }
    public LevelSceneReferences LevelSceneReferences { get; private set; }
    public LevelStates LevelState { get; private set; } = LevelStates.InProgress;

    // Level Components
    public GridController GridController { get; private set; }
    public GridEntitySpawnController GridEntitySpawnController { get; private set; }
    public MovesController MovesController { get; private set; }
    public GridGoalController GridGoalController { get; private set; }


    public LevelController(LevelSettings LevelSettings, LevelSceneReferences levelSceneReferences)
    {
        this.LevelSettings = LevelSettings;
        this.LevelSceneReferences = levelSceneReferences;
        CreateLevelControllers();
    }

    private void CreateLevelControllers()
    {
        GridController = new GridController(LevelSettings.GridControllerSettings, LevelSceneReferences.GridControllerSceneReferences);
        GridEntitySpawnController = new GridEntitySpawnController(GridController, LevelSettings.GridEntitySpawnerSettings, LevelSceneReferences.GridEntitySpawnerSceneReferences);
        MovesController = new MovesController(GridController, GridEntitySpawnController, LevelSettings.MovesControllerSettings, LevelSceneReferences.MovesControllerReferences);
        GridGoalController = new GridGoalController(LevelSettings.GridGoalsControllerSettings, LevelSceneReferences.GridGoalsControllerReferences);
        GridController.StartGrid(GridEntitySpawnController, GridGoalController);
    }

    public void LevelFailed()
    {
        if (LevelState != LevelStates.InProgress) return;
        LevelState = LevelStates.Failed;
        //CreateLevelResultFlyingText("Level Failed");
        //GridController.GridDestroyOnLevelFailed();
        LevelSaveData.Data.ClearSavedLevelState();
    }

    public void LevelCleared()
    {
        if (LevelState != LevelStates.InProgress) return;
        LevelState = LevelStates.Cleared;

        LevelSaveData.Data.ClearSavedLevelState();
        GameManagerSaveData.Data.ProgressLevel();
    }

    private void SaveLevelState()
    {
        LevelSaveData.Data.SaveLevelState(this);
    }

    private void LevelEnded()
    {

        GameManager.I.CreateCurrentLevel();
    }

    public enum LevelStates
    {
        InProgress,
        Cleared,
        Failed
    }
}
