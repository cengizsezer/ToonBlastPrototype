using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class Goal
{
    public BaseEntitiyTypeDefinition entityType;

    [SerializeField] private int goalAmount;
    [NonSerialized] private int goalLeft;

    public bool IsCompleted => goalLeft <= 0;
    public int GoalLeft => goalLeft;

    public void StartGoal()
    {
        goalLeft = goalAmount;
    }

    public void DecreaseGoal()
    {
        goalLeft--;
        if (goalLeft < 0) goalLeft = 0;
    }

    public void LoadGoalAmountLeft(int loadedGoalLeft)
    {
        goalLeft = loadedGoalLeft;
    }
}
