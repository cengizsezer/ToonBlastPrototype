using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GridGoalController : PocoSingleton<GridGoalController>
{
    public List<Goal> GridGoals { get; private set; }
    private List<GridGoalUI> GridGoalUiElements { get; set; }

    private AudioClip goalCollectAudio;
    private RectTransform gridUiElementsParent;

    public GridGoalController(GridGoalControllerSettings settings, GridGoalControllerReferences references)
    {
        Instance = this;
        this.gridUiElementsParent = references.GoalObjectsParent;
        this.goalCollectAudio = settings.GoalCollectAudio;
        GridGoals = new List<Goal>(settings.GridGoals);
       
        GridGoalUiElements = new List<GridGoalUI>();
        StartAllGoals();
        TryLoadGoalSaveData();
        SpawnUiElements();
    }

    public void OnEntityDestroyed(IGridEntity entity)
    {
        for (int i = 0; i < GridGoals.Count; i++)
        {
            Goal goal = GridGoals[i];
            GridGoalUI goalUI = GridGoalUiElements[i];

            if (goal.IsCompleted) continue;
            if (goal.entityType.GridEntityTypeName == entity.EntityType.GridEntityTypeName)
            {
                goal.DecreaseGoal();
                CreateFlyingSpriteToGoal(entity, goalUI);
                if (goal.IsCompleted) CheckAllGoalsCompleted();
            }
        }
    }

    public void CreateFlyingSpriteToGoal(IGridEntity entity, GridGoalUI goalUI)
    {
        int goalAmount = goalUI.Goal.GoalLeft;
        UIEffectManager.I.CreateCurvyFlyingSprite(
            entity.EntityType.DefaultEntitySprite,
            entity.EntityTransform.GetComponent<RectTransform>().sizeDelta * 1.25f, // create bigger flying image for better visual representation
            entity.EntityTransform.position,
            goalUI.transform.position,
            UIEffectManager.CanvasLayer.OverEverything,
            () => OnFlyingSpriteReachGoal(goalAmount, goalUI));
    }

    private void OnFlyingSpriteReachGoal(int goalAmount, GridGoalUI goalUI)
    {
        //AudioManager.Instance.PlayAudio(goalCollectAudio, AudioManager.PlayMode.Single, 1);
        goalUI.SetGoalAmount(goalAmount);
    }

    private void StartAllGoals()
    {
        foreach (Goal goal in GridGoals) goal.StartGoal();
    }

    private void TryLoadGoalSaveData()
    {
        if (!LevelSaveData.Data.HasLevelSaved) return;
        for (int i = 0; i < GridGoals.Count; i++)
        {

            Goal goal = GridGoals[i];
            goal.LoadGoalAmountLeft(LevelSaveData.Data.GoalAmountsLeft[i]);
            Debug.Log(LevelSaveData.Data.GoalAmountsLeft[i]);
        }
    }

    private void SpawnUiElements()
    {
        int idx = 0;
        foreach (Goal goal in GridGoals)
        {
            GameObject newGo = PoolManager.I.GetObject<GridGoalUI>().gameObject;
            idx++;
            newGo.name = "cengiz" + "-----" + idx;
            newGo.transform.position = gridUiElementsParent.position;
            newGo.transform.SetParent(gridUiElementsParent);
            GridGoalUI goalUi = newGo.GetComponent<GridGoalUI>();
            goalUi.transform.localScale = Vector3.one;
            goalUi.SetupGoalUI(goal);
            GridGoalUiElements.Add(goalUi);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridUiElementsParent);
    }

    private void CheckAllGoalsCompleted()
    {
        foreach (Goal goal in GridGoals) if (!goal.IsCompleted) return;
        GameManager.I.CurrentLevel.LevelCleared();
    }
}
