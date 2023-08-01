using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Block Type Definition")]
public class BlockTypeDefinition : BaseEntitiyTypeDefinition
{
    [SerializeField] private List<BlockMatchCondition> blockMatchConditions = new List<BlockMatchCondition>();
    public List<BlockMatchCondition> BlockMatchConditions => blockMatchConditions;
}
