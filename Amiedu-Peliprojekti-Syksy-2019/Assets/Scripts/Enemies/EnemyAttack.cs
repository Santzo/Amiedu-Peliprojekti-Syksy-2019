using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyAttack
{
    public float attackRange;
    public float forwardForce;
    public float attackInterval;
    public float animationMultiplier;
    public float minPhysical;
    public float maxPhysical;
    public float minFire;
    public float maxFire;
    public float minSpectral;
    public float maxSpectral;
    public AnimationClip animation;
    public AudioClip audio;
}


