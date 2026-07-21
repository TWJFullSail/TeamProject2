using UnityEngine;

[CreateAssetMenu]

public class weaponStats : ScriptableObject
{
    enum damageType
    {
        bullet,
        stationary,
        AOE,
        Rotate,
        DOT
    }

    public GameObject weaponModel;

    [SerializeField] damageType type;
    [Range(1,100)] public int damage;
    [Range(1, 1000)] public int detectDist;
    [Range(1, 100)] public int damageDist;
    [Range(0.1f, 2)] public float rate;

    [Range(1, 100)] public int level;
    [Range(0.1f, 2)] public float lvlBonus;

    public ParticleSystem hitEffect;
    public AudioClip[] sound;
    [Range(0, 1)] public float soundVol;
}
