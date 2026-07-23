using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;
    public GameObject projectile;

    [Range(1,10)] public int shootDamage;
    [Range(5, 1000)] public int shootDist;
    [Range(0.1f, 2)] public float shootRate;

    public int ammoCur;
    public int ammoTotal;
    [Range(5, 50)] public int clipSize;
    [Range(1, 5)] public int clipMax;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootSoundVol;
    public bool isLaser;
}
