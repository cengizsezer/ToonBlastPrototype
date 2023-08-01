﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Base Entity Type Definition")]
public class BaseEntitiyTypeDefinition : ScriptableObject, IGridEntityTypeDefinition
{
    [BHeader("Base Grid Entity Settings")]
    [SerializeField] protected GameObject gridEntityPrefab;
    [SerializeField] protected string gridEntityTypeName;
    [SerializeField] protected Sprite defaultSprite;
    [SerializeField] protected GameObject onDestroyParticle;
    [SerializeField] protected AudioClip onDestroyAudio;
    [SerializeField] protected bool entityIncludedInShuffle = true;
    [SerializeField] protected List<EntityDestroyTypes> immuneToDestroyTypes = new List<EntityDestroyTypes>();

    public string GridEntityTypeName => gridEntityTypeName;

    public Sprite DefaultEntitySprite => defaultSprite;

    public GameObject OnDestroyParticle => onDestroyParticle;

    public AudioClip OnDestroyAudio => onDestroyAudio;

    public GameObject GridEntityPrefab => gridEntityPrefab;

    public bool EntityIncludedInShuffle => entityIncludedInShuffle;
    public List<EntityDestroyTypes> ImmuneToDestroyTypes => immuneToDestroyTypes;

    public Guid ID { get; set;}
}
