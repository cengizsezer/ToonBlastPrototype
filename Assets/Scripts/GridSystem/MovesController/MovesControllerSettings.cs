using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class MovesControllerSettings
{
    [SerializeField] private int moveCount;
    public int MoveCount => moveCount;
}
