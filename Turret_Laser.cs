using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Laser : Turret_Parent
{ 

    [Header("Laser Turret Fields")]
    public Transform FirePoint;
    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;
    public float debuffSpeed;


    protected override void UpgradeConfirm()
    {
        baseDamage = baseDamage * USM.GetDmgRate()[3];
        damage = baseDamage;
    }

    protected override void Fire()
    {
        Laser();
    }

    protected override void StopFire()
    {
        if (lineRenderer.enabled)
            lineRenderer.enabled = false;
            impactEffect.Stop();
            impactLight.enabled = false;

    }

    private void Laser()
    {
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }

        lineRenderer.SetPosition(0, FirePoint.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 dir = FirePoint.position - target.position;

        impactEffect.transform.position = target.position + dir.normalized * 0.5f;
        impactEffect.transform.rotation = Quaternion.LookRotation(dir);

        Enemy e = target.GetComponent<Enemy>();
        if(e!= null)
        {
            e.TakeDamage(damage / (60.0f / fireRate));
            e.debuff(debuffSpeed);
        }
    }
    
}
