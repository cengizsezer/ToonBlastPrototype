﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class GridGoalControllerReferences
{
    [SerializeField] private RectTransform goalObjectsParent;
    public RectTransform GoalObjectsParent => goalObjectsParent;
}
