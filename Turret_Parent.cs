using UnityEngine;

public class Turret_Parent : MonoBehaviour
{
    protected Transform target;
    protected UpgradeShopManager USM;
    protected SoundManager SDM;
    private Node BaseNode;

    [Header("Related Objects Field")]
    //from Scene
    public GameObject TagTail;
    public GameObject TagObject;
    //from Asset
    public GameObject RangeDisplay;
    public GameObject TurretNextLV;

    [Header("Turret Information Field")]
    public string turretName;
    public string turretDescription;
    public int turretIndex;
    public int synergyCode1;
    public int synergyCode2;

    public int turretLV;
    public float range;
    public float baseDamage;
    public float damage;
    public float baseFireRate;
    public float fireRate;
    public float turnSpeed;
    public float rangeScale;
    public Transform PartToRotate;

    public bool bIsBuffed;              // 버프타워 버프 플래그
    public bool bEnemySkillTriggered;   // Mage enemy skill 플래그 
    public bool bIsLV5;                 // LV5 turret 플래그

    public string enemyTag = "Enemy";   // 버프타워에서만 "BuffTail"

    [Header("Model issue Init Field")]   // 모델에서 틀어진 만큼의 Transform 값을 조정해주는 필드
    public Vector3 InitialMove;           // 생성시 틀어진 위치만큼 이동해줘야할때 사용함.
    public float InitialRotateNumber;     // 생성시 틀어진 위치만큼 움직여줘야하는 각도 > 주로 x축
    public int InitialRotateAxis;         // 생성시 틀어진 위치만큼 움직여줘야하는 축   > 주로 z축, 가끔y축


    void Start()
    {
        bIsLV5 = false;
        bEnemySkillTriggered = false;
        baseFireRate = fireRate;
 
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        transform.position = transform.position + InitialMove;

        rangeScale = range * 0.2f;
        UpgradeConfirm();
    }


    void Awake()
    {
        USM = UpgradeShopManager.Instance();
        SDM = SoundManager.Instance();
        gameObject.AddComponent<AudioSource>().clip = SDM.TurretFire[turretIndex].clip;
        gameObject.GetComponent<AudioSource>().playOnAwake = false;
    }

    public void SetBaseNode(Node node)
    {
        BaseNode = node;
        node.turret = gameObject;
    }
    public Node GetBaseNode()
    {
        return BaseNode;
    }

    public void SetNodeTurretNull() //터렛이 노드에서 없어질 때 후처리
    {
        BaseNode.turret = null;
        BaseNode.BuildableEffect.GetComponent<SpriteRenderer>().sprite = BaseNode.NodeCoverSprites[8];
        BaseNode.BuildableEffect.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
    
    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        if (enemies.Length == 0) return;
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }
 
    void Update()
    {
        if (target != null)
        {
            if (bEnemySkillTriggered)
            {
                StopFire();
                return;
            }
            LockOnTarget();
            Fire();
        }
        else
        {
            StopFire();
        }
    }

    void LockOnTarget()
    {
            Vector3 dir = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(PartToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
            if (InitialRotateAxis == 3)     // 회전해야하는 모델이 회전할 때 z축이 돌아가는 경우
                PartToRotate.rotation = Quaternion.Euler(InitialRotateNumber, 0f, rotation.y);
            else if (InitialRotateAxis == 2)// y축이 돌아가는 경우
                PartToRotate.rotation = Quaternion.Euler(InitialRotateNumber, rotation.y, 0f);
            else if (InitialRotateAxis == 1)// x축이 돌아가는 경우
                PartToRotate.rotation = Quaternion.Euler(rotation.y, 0f, 0f);
    }

    public void Buff(int lev)
    {
        if (bIsBuffed) return;
        bIsBuffed = true;
        switch (lev)
        {
            case 1:
                damage = damage * 1.1f;
                break;
            case 2:
                damage = damage * 1.2f;
                break;
            case 3:
                damage = damage * 1.3f;
                break;
            case 4:
                damage = damage * 1.4f;
                break;
            case 5:
                damage = damage * 1.5f;
                break;
            default:
                break;
        }
    }
    public void ResetBuff()
    {
        damage = baseDamage;
        fireRate = baseFireRate;
    }

    protected virtual void UpgradeConfirm(){ }

    protected virtual void StopFire()
    {
        GetComponent<AudioSource>().Stop();
    }

    protected virtual void Fire() { }


}
