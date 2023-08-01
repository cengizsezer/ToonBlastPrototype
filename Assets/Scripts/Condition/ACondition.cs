using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ACondition: ScriptableObject, ICondition
{
    public abstract bool IsConditionMet(object obj);
}

[System.Serializable]
public struct BlockMatchCondition
{
    [SerializeField] private ACondition condition;
    [SerializeField] private Sprite sprite;
    [SerializeField] private BaseEntitiyTypeDefinition[] entitiesToSpawnOnMatch; 

    public BaseEntitiyTypeDefinition GetRandomEntityToSpawn()
    {
        if (entitiesToSpawnOnMatch == null || entitiesToSpawnOnMatch.Length == 0) return null;
        return entitiesToSpawnOnMatch[UnityEngine.Random.Range(0, entitiesToSpawnOnMatch.Length)];
    }

    public Sprite Sprite => sprite;

    public ACondition Condition => condition;
}
