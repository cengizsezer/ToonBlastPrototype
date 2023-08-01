using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GridGoalUI : PoolObject
{
  
    [SerializeField] private ParticleSystem psGridGoal;
    [SerializeField] private TMPro.TMP_Text goalAmountLeftText;
    [SerializeField] private Image goalImage;
    [SerializeField] private Image goalCompletedImage;

    public Goal Goal { get; private set; }

    public void SetupGoalUI(Goal goal)
    {
        Goal = goal;
        goalImage.sprite = goal.entityType.DefaultEntitySprite;
        SetGoalAmount(goal.GoalLeft, false);
    }

    public void SetGoalAmount(int goalAmount, bool playParticles = true)
    {
        if (playParticles) 
            psGridGoal.Play();
        if (goalAmount == 0)
        {
            goalCompletedImage.enabled = true;
            goalAmountLeftText.text = "";
        }
        else
        {
            goalCompletedImage.enabled = false;
            goalAmountLeftText.text = goalAmount.ToString();
        }
    }

    public override void OnCreated()
    {
        OnDeactive();
    }

    public override void OnDeactive()
    {
        //PoolManager.I.EnqueObject(this);
        transform.SetParent(null);
        gameObject.SetActive(false);
    }

    public override void OnSpawn()
    {
        gameObject.SetActive(true);
    }
}
