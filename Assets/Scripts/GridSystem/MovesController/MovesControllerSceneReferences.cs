using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class MovesControllerSceneReferences
{
    [SerializeField] private TMPro.TMP_Text movesLeftText;
    public TMPro.TMP_Text MovesLeftText => movesLeftText;
}
