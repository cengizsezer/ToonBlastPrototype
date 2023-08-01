using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AllGridEntities
{
    private static BaseEntitiyTypeDefinition[] allEntityTypeDefinitions;

    public static BaseEntitiyTypeDefinition[] AllEntityTypeDefinition
    {
        get
        {
            if (allEntityTypeDefinitions != null) return allEntityTypeDefinitions;
            allEntityTypeDefinitions = Resources.LoadAll<BaseEntitiyTypeDefinition>("ScriptableObjects/GridEntityDefinitions");
            return allEntityTypeDefinitions;
        }
    }

    public static BaseEntitiyTypeDefinition GetEntityTypeByName(string name)
    {
        foreach (var entityTypeDefinition in AllEntityTypeDefinition)
        {
            if (entityTypeDefinition.GridEntityTypeName == name)
            {
                return entityTypeDefinition;
            }
        }
        Debug.LogError("Could not find entity type with name: " + name);
        return null;
    }
}
