using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Missile : Turret_Parent
{
    [Header("Missile Turret Fields")]
    public Transform[] FirePoints = new Transform[8];
    public GameObject MissilePrefab;
    private float fireCountdown = 0.0f;


    protected override void UpgradeConfirm()
    {
        baseDamage = baseDamage * USM.GetDmgRate()[7];
        damage = baseDamage;
        baseFireRate = baseFireRate * USM.GetAtkSpdRate()[7];
        fireRate = baseFireRate;
    }

    protected override void Fire()
    {
        if (fireCountdown <= 0f)
        {
            Launch();
            fireCountdown = 1.5f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    protected override void StopFire()
    {

    }

    private void Launch()
    {
        GameObject[] missileGO = new GameObject[FirePoints.Length];
        for (int i = 0; i < FirePoints.Length; i++)
        {
            missileGO[i] = (GameObject)Instantiate(MissilePrefab, FirePoints[i].position, FirePoints[i].rotation);
            Missile missile = missileGO[i].GetComponent<Missile>();
            if (missile != null)
            {
                missile.Seek(target);
                missile.Set_Damage(damage);
                if (bIsLV5)
                    missile.explosionRadius = 10.0f;
            }
        }
    }

}
