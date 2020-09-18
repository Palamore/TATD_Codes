using UnityEngine;
using System.Collections;

public class Enemy_Speed : Enemy
{
    public GameObject SkillEffect;
    protected override void SkillCast()
    {
        health += MAXhealth / 4.0f;
        baseSpeed = baseSpeed * 1.5f;
        GameObject ef = (GameObject)Instantiate(SkillEffect, transform.position, Quaternion.identity);
        Destroy(ef, 2.0f);
        transform.localScale = transform.localScale * 1.2f;
    }
}