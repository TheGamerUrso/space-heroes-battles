using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDestroyablePart : MonoBehaviour
{
    protected Action<bool> Attacked;
    public Ship shipOwner;
    public GameObject target => gameObject;
    [SerializeField] protected BossEnemy baseBossEnemy;
    [SerializeField] protected GameObject fireEffect;
    [SerializeField] protected GameObject prepareToAttack;
    [SerializeField] protected BoxCollider boxCollider;
    public AudioClip hitSFX;
    [Range(.1f, 1)]
    protected float takeDamageDelay;
}
