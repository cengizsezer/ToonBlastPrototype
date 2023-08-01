﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridEntityTypeDefinition
{
    public bool EntityIncludedInShuffle { get; }
    public GameObject GridEntityPrefab { get; }
    public string GridEntityTypeName { get; }
    public Guid ID { get; set; }
    public Sprite DefaultEntitySprite { get; }
    public GameObject OnDestroyParticle { get; }
    public AudioClip OnDestroyAudio { get; }
    public List<EntityDestroyTypes> ImmuneToDestroyTypes { get; }
}
