using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Min Group Size Condition")]
public class MinBlockGroupSizeCondition : ACondition
{
    [Tooltip("Inclusive")]
    [SerializeField] private int minGroupSize;

    public override bool IsConditionMet(object obj)
    {
        Block block = obj as Block;
        if (block == null) return false;
        return block.GroupSize >= minGroupSize;
    }
}
