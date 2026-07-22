using UnityEngine;

[CreateAssetMenu]

public class weaponStats : ScriptableObject
{
    public enum weaponType
    {
        aura = 0,
        arm = 1,
        range = 2,
        passive = 3
    }

    public enum stanceType
    {
        nature = 0,
        fantasy = 1,
        tech = 2,
        holy = 3
    }

    public GameObject weaponModel;

    public weaponType type;
    public stanceType stance;
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