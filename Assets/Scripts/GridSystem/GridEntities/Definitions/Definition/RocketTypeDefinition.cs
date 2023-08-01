using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Rocket Type Definition")]
public class RocketTypeDefinition : BaseEntitiyTypeDefinition
{
    [SerializeField] private GameObject rocketExplodeAnimPrefab;

    public GameObject RocketExplodeAnimPrefab => rocketExplodeAnimPrefab;
}
