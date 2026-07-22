using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class damage : MonoBehaviour
{
    enum damageType
    {
        bullet,
        stationary,
        AOE,
        melee,
        DOT,
        rangedWeapon
    }

    [SerializeField] weaponStats weapon;
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmt;
    [SerializeField] float damageRate;
    [SerializeField] bool destroyOnHit;
    [SerializeField] float bulletSpeed;
    [SerializeField] float bulletDestroyTime;
    [SerializeField] ParticleSystem hitEffect;

    bool isDamaging;
    Transform pivot;
    float rotationSpeed;
    float recoilAngle;
    float meleeHitAngle;
    float recoilDuration;
    bool canPierce;
    string targetTag = "Enemy";

    public GameObject projectilePrefab;
    Transform firePoint;
    bool targetAtPosition;
    float detectionRadius;
    float aoeBlastRadius;

    float actionTimer;
    bool isRecoiling;
    Quaternion targetRotation;
    HashSet<Collider> targetsInRange = new HashSet<Collider>();
    Dictionary<Collider, float> dotTimers = new Dictionary<Collider, float>();
    SphereCollider detectionSphere;

    bool bounceMode = false;
    float bounceAttackRate;

    public void ApplyUpgrade(string stat, float value)
    {
        switch (stat.ToLower())
        {
            case "damage":
                damageAmt = Mathf.RoundToInt(value);
                break;
            case "rate":
                damageRate = value;
                break;
            case "speed":
                bulletSpeed = Mathf.RoundToInt(value);
                if (rb != null && type == damageType.bullet)
                    rb.linearVelocity = transform.forward * bulletSpeed;
                break;
            case "range":
                detectionRadius = value;
                ApplyDetectionRadius();
                break;
            case "blastradius":
                aoeBlastRadius = value;
                ApplyAOERadius();
                break;
            case "piercing":
                canPierce = value > 0f;
                break;
            case "rotationspeed":
                rotationSpeed = value;
                break;
            case "recoilangle":
                recoilAngle = value;
                break;
        }
    }
    public void EnableBounceMode(float newBounceRate = 0.1f)
    {
        bounceMode = true;
        bounceAttackRate = newBounceRate;
    }

    public void DisableBounceMode() { bounceMode = false; }

    void Start()
    {        
        if(weapon != null)
        {
            type = (damageType)weapon.dType;
            damageAmt = weapon.dmgAmt;
            damageRate = weapon.dmgRate;
            destroyOnHit = weapon.destroyedOnHit;
            bulletSpeed = weapon.projectileSpd;
            bulletDestroyTime = weapon.bulletDestroyTime;
            hitEffect = weapon.hitEffect;                       
            pivot = weapon.pivot;
            rotationSpeed = weapon.rotationSpeed;
            recoilAngle = weapon.recoilAngle;
            meleeHitAngle = weapon.meleeHitAngle;
            recoilDuration = weapon.recoilDuration;
            canPierce = weapon.canPierce;
            targetTag = weapon.targetTag;

            projectilePrefab = weapon.projectilePrefab;
            firePoint = weapon.firePoint;
            targetAtPosition = weapon.targetAtPosition;
            detectionRadius = weapon.detectionRadius;
            aoeBlastRadius = weapon.aoeBlastRadius;
        }
        
        if(type == damageType.bullet)
        {
            if (rb != null) rb.linearVelocity = transform.forward * bulletSpeed;
            if (bulletDestroyTime > 0)
            {
                Destroy(gameObject, bulletDestroyTime);
            }
        }
        else if (type == damageType.AOE)
        {
            ApplyAOERadius();
            Collider[] hits = Physics.OverlapSphere(transform.position, aoeBlastRadius);
            foreach (Collider hit in hits)
            {
                if (hit.isTrigger) continue;
                if (!hit.CompareTag(targetTag)) continue;
                IDamage dmg = hit.GetComponent<IDamage>();
                if (dmg != null) dmg.takeDamage(damageAmt);
            }
            Destroy(gameObject, bulletDestroyTime > 0 ? bulletDestroyTime : 0.5f);
        }

        if (pivot == null) pivot = transform;
        targetRotation = pivot.rotation;

        detectionSphere = GetComponent<SphereCollider>();
        if (detectionSphere != null)
        {
            detectionSphere.radius = detectionRadius;
        }

        if (type == damageType.DOT)
            ApplyDetectionRadius();
    }


    void Update()
    {
        if (gamemanager.instance != null && gamemanager.instance.isPaused) return;

        targetsInRange.RemoveWhere(e => e == null);
        actionTimer += Time.deltaTime;

        if (type == damageType.melee && targetsInRange.Count > 0)
            UpdateMelee();
        else if (type == damageType.rangedWeapon && targetsInRange.Count > 0 && actionTimer >= damageRate)
        {
            actionTimer = 0;
            FireRangedWeapon();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        switch (type)
        {
            case damageType.bullet:
            case damageType.stationary:
            case damageType.AOE:
                IDamage dmg = other.GetComponent<IDamage>();
                if (dmg != null)
                {
                    
                    dmg.takeDamage(damageAmt);
                    if (type == damageType.bullet && !canPierce && destroyOnHit)
                    {
                        Destroy(gameObject);
                    }
                        
                }
                break;

            case damageType.melee:
            case damageType.rangedWeapon:
                if (other.CompareTag(targetTag))
                    targetsInRange.Add(other);
                break;

            case damageType.DOT:                
                break;
        }

        if (hitEffect != null)
        {
            Debug.Log("here");
            Instantiate(hitEffect, other.transform.position + hitEffect.transform.position, hitEffect.transform.rotation);
        }        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;
        
        if (detectionSphere != null)
        {
            float dist = Vector3.Distance(transform.position, other.transform.position);
            float enemyRadius = 0f;
            CapsuleCollider cc = other.GetComponent<CapsuleCollider>();
            if (cc != null) enemyRadius = cc.radius;
            else { SphereCollider sc2 = other.GetComponent<SphereCollider>(); if (sc2 != null) enemyRadius = sc2.radius; }

            float effectiveRange = detectionSphere.radius * transform.lossyScale.x + enemyRadius;
            if (dist > effectiveRange)
            {
                targetsInRange.Remove(other);
                dotTimers.Remove(other);
                return;
            }
        }

        if (type == damageType.melee || type == damageType.rangedWeapon)
        {
            targetsInRange.Add(other);
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmt);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            targetsInRange.Remove(other);
            dotTimers.Remove(other);
        }
    }

    void UpdateMelee()
    {
        if (!isRecoiling)
        {
            Collider closest = GetClosestEnemy();
            if (closest == null) return;

            Vector3 dir = closest.transform.position - pivot.position;
            dir.y = 0;
            if (dir == Vector3.zero) return;

            targetRotation = Quaternion.LookRotation(dir);
            pivot.rotation = Quaternion.Slerp(pivot.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            float angle = Vector3.Angle(pivot.forward, dir);
            if (angle < meleeHitAngle && actionTimer >= damageRate)
            {
                actionTimer = 0;
                IDamage dmg = closest.GetComponent<IDamage>();
                if (dmg != null)
                    dmg.takeDamage(damageAmt);

                StartCoroutine(MeleeRecoilRoutine(bounceMode ? GetNextEnemy(closest) : null));
            }
        }
        else
        {
            pivot.rotation = Quaternion.Slerp(pivot.rotation, targetRotation, rotationSpeed * 2f * Time.deltaTime);
        }
    }

    IEnumerator MeleeRecoilRoutine(Collider bounceTarget)
    {
        isRecoiling = true;

        if (bounceMode && bounceTarget != null)
        {
            Vector3 dir = bounceTarget.transform.position - pivot.position;
            dir.y = 0;
            targetRotation = dir != Vector3.zero ? Quaternion.LookRotation(dir) : pivot.rotation * Quaternion.Euler(0, -recoilAngle, 0);
            actionTimer = damageRate - bounceAttackRate;
        }
        else
        {
            targetRotation = pivot.rotation * Quaternion.Euler(0, -recoilAngle, 0);
        }

        yield return new WaitForSeconds(recoilDuration);
        isRecoiling = false;
    }

    void FireRangedWeapon()
    {
        if (projectilePrefab == null) return;
        Collider target = targetAtPosition ? GetHighestHPEnemy() : GetClosestEnemy();
        if (target == null) return;

        if (targetAtPosition)
        {
            Instantiate(projectilePrefab, target.transform.position, Quaternion.identity);
        }
        else
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            Vector3 dir = target.transform.position - spawnPos;
            dir.y = 0;
            if (dir == Vector3.zero) dir = transform.forward;
            Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir));
        }
    }

    void ApplyDetectionRadius()
    {
        SphereCollider sc = GetComponent<SphereCollider>();
        if (sc != null) sc.radius = detectionRadius;

        Transform ring = transform.Find("CloudRing");
        if (ring != null)
            ring.localScale = new Vector3(detectionRadius * 2f, 0.05f, detectionRadius * 2f);
    }

    void ApplyAOERadius()
    {
        Transform visual = transform.Find("BlastSphere");

        if (visual == null)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "BlastSphere";
            sphere.transform.SetParent(transform, false);
            sphere.transform.localPosition = Vector3.zero;

            Collider sc = sphere.GetComponent<Collider>();
            if (sc != null)
            {
                Destroy(sc);
            }

            Renderer rend = sphere.GetComponent<Renderer>();
            if (rend != null)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                if (mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader")
                    mat = new Material(Shader.Find("Standard"));

                mat.SetFloat("_Surface", 1f);                    
                mat.SetFloat("_Blend", 0f);                      
                mat.SetFloat("_ZWrite", 0f);                     
                mat.SetFloat("_AlphaClip", 0f);                  
                mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                mat.SetColor("_BaseColor", new Color(0.5f, 0.85f, 1f, 0.25f));
                rend.material = mat;
            }

            visual = sphere.transform;
        }

        float d = aoeBlastRadius * 2f;
        visual.localScale = new Vector3(d, d, d);
    }

    Collider GetClosestEnemy()
    {
        Collider closest = null;
        float minD = float.MaxValue;
        foreach (var enemy in targetsInRange)
        {
            if (enemy == null) continue;
            float d = Vector3.Distance(transform.position, enemy.transform.position);
            if (d < minD) { minD = d; closest = enemy; }
        }
        return closest;
    }

    Collider GetNextEnemy(Collider exclude)
    {
        Collider best = null;
        float minD = float.MaxValue;
        foreach (var enemy in targetsInRange)
        {
            if (enemy == null || enemy == exclude) continue;
            float d = Vector3.Distance(transform.position, enemy.transform.position);
            if (d < minD) { minD = d; best = enemy; }
        }
        return best;
    }

    Collider GetHighestHPEnemy()
    {
        Collider best = null;
        float maxHP = float.MinValue;
        foreach (var enemy in targetsInRange)
        {
            if (enemy == null) continue;
            IDamage dmg = enemy.GetComponent<IDamage>();
            float hp = 0;
            if (dmg != null)
            {
                var hpField = dmg.GetType().GetField("hp",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);
                if (hpField != null) hp = System.Convert.ToSingle(hpField.GetValue(dmg));
            }
            if (hp > maxHP) { maxHP = hp; best = enemy; }
        }
        return best ?? GetClosestEnemy();
    }
}
