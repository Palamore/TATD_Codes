using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Base : Turret_Parent
{
    [Header("Base Turret Fields")]
    public Transform FirePoint;
    public GameObject BulletPrefab;
    public GameObject UniqueSkillEffect;
    private float fireCountdown = 0.0f;


    protected override void UpgradeConfirm()
    {
        baseDamage = baseDamage * USM.GetDmgRateData()[0];
        damage = baseDamage;
        baseFireRate = baseFireRate * USM.GetAtkSpdRateData()[0];
        fireRate = baseFireRate;
    }

    protected override void Fire()
    {
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
            if (bIsLV5)
            {
                int r = Random.Range(0, 2);
                if(r == 1)
                {
                    Invoke("Shoot", 0.1f);
                    Invoke("Effect", 0.1f);
                }
            }
        }

        fireCountdown -= Time.deltaTime;
    }

    protected override void StopFire()
    {
        
    }
    private void Effect()
    {
        GameObject effect = Instantiate(UniqueSkillEffect, FirePoint.position, Quaternion.identity);
        Destroy(effect, 1.0f);
    }

    private void Shoot()
    {
        GameObject bulletGO = (GameObject)Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);
        if (bulletGO == null) return;
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        GetComponent<AudioSource>().Play();
        bullet.SetTarget(target);
        bullet.SetDamage(damage);
    }

}
