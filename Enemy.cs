using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    WaveSpawner WS;
    //Way Point
    private Waypoints2 WayPoints;
    protected Transform target;
    public int waypointIndex = 0;
    public Transform TargetToLook;

    //Stats
    protected float baseSpeed;
    protected float speed;
    public GameObject HPBarPrefab;
    public float health;
    public float MAXhealth;
    protected float defenseRate;
    public float mana;
    public float MAXmana;

    public GameObject deathEffect;
    public Material[] lvMaterial = new Material[6];
    public GameObject lvBelt;

    private float debuffTimer;
    private float manaTimer;

    void Start()
    {
        WS = WaveSpawner.Instance();
        mana = 0.0f;
        MAXmana = 100.0f;
        manaTimer = 25.0f;
        speed = WS.speed;
        baseSpeed = speed;
        MAXhealth = WS.Enemy_health_stack_now;
        MAXhealth = MAXhealth * WS.healthRate;
        health = MAXhealth;
        defenseRate = WS.defenseRate;
        Instantiate(HPBarPrefab, transform.position + new Vector3(0.0f, 5.0f, 0.0f), Quaternion.identity);
        GameObject WP = GameObject.FindGameObjectWithTag("WaypointHolder");
        WayPoints = WP.GetComponent<Waypoints2>();
        target = WayPoints.points[0];
        WS.Enemy_Counter++;
        debuffTimer = 0.0f;
        lvBelt.GetComponent<Renderer>().material = lvMaterial[(WS.stageLevel - 1)];
    }

    public void TakeDamage (float amount)
    {
        health -= amount * defenseRate; // 방어력수치적용.
        if(health <= 0.0f)
        {
            Die();
        }
    }
    public void TakeTrueDamage(float amount)
    {
        health -= amount;
        if(health <= 0.0f)
        {
            Die();
        }
    }

    public void SetTargetToLook(Transform tr)
    {
        TargetToLook = tr;
    }

    void Die()
    {
        GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);
        Destroy(gameObject);
    }

    void Update()
    {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        transform.LookAt(target);
        if (Vector3.Distance(transform.position, target.position) <= 0.7f)
        {
            GetNextWaypoint();
        }

        if (mana >= 100.0f)
        {
            SkillCast();
            mana = -0.01f;
        }
        else
        {
            if (WS.Game_difficulty >= 2)
                mana += Time.deltaTime * manaTimer;
        }

        if (debuffTimer >= 0.0f)
        {
            debuffTimer -= (Time.deltaTime * 0.9f);
        }
        else
        {
            speed = baseSpeed;
        }
    }

    public void Debuff(float spd)
    {
        debuffTimer += 1.0f / 60.0f;
        if (baseSpeed * spd >= speed)
            return;
        speed = baseSpeed * spd;
    }

    void GetNextWaypoint()
    {
            if (waypointIndex >= WayPoints.points.Length - 1)
            {

                ReachToPath();
                return;
            }
            waypointIndex++;
            target = WayPoints.points[waypointIndex];
        
    }

    void ReachToPath()
    {
        WS.Enemy_Stacked++;
        if(WS.Enemy_Stacked == 20)
        {
            GameManager.Instance().GameOver();
        }
        Die();
    }

    protected virtual void SkillCast()
    {
    }
}
