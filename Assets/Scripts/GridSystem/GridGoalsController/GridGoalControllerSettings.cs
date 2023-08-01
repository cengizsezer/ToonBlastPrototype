using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class GridGoalControllerSettings
{
    [SerializeField] private AudioClip goalCollectAudio;
    [SerializeField] private List<Goal> gridGoals;
    
    public AudioClip GoalCollectAudio => goalCollectAudio;
    public List<Goal> GridGoals => gridGoals;
}
