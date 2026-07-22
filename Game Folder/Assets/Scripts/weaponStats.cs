using UnityEngine;

[CreateAssetMenu]

public class weaponStats : ScriptableObject
{  
    public enum stanceType
    {
        nature = 0,
        fantasy = 1,
        tech = 2,
        holy = 3
    }

    public enum weaponType
    {
        aura = 0,
        arm = 1,
        range = 2,
        passive = 3
    }
    public enum dmgType
    {
        bullet,
        stationary,
        AOE,
        melee,
        DOT,
        rangedWeapon
    }

    public GameObject weaponModel;

    public stanceType stance = stanceType.nature;
    public weaponType type = weaponType.aura;
    public dmgType dType = dmgType.DOT;

    [Range(1, 100)] public int dmgAmt;
    [Range(0.1f, 2)] public float dmgRate;
    public bool destroyedOnHit;
    public bool canPierce;
    public string targetTag = "Enemy";

    [Range(1, 100)] public int projectileSpd;
    [Range(1, 10)] public int bulletDestroyTime;

    public Transform pivot;
    public float rotationSpeed;
    public float recoilAngle;
    public float meleeHitAngle;
    public float recoilDuration;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public bool targetAtPosition;
    public float detectionRadius;
    public float aoeBlastRadius;


    [Range(1, 1000)] public int detectDist;
    [Range(1, 100)] public int damageDist;
    [Range(1, 100)] public int level;
    [Range(0.1f, 2)] public float lvlBonus;

    public ParticleSystem hitEffect;
    [Range(0.1f, 2)] public AudioClip[] sound;
    [Range(0, 1)] public float soundVol;
}