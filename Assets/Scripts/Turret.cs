using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Turret : MonoBehaviour
{
    [Header("炮台设置")]
    public GameObject magicBallPrefab;     // 子弹预制体
    public Transform firePoint;            // 发射点
    public float attackRate = 1.0f;        // 攻击间隔（秒）
    public float attackRange = 10f;        // 攻击范围
    public float duration = 15f;           // 持续时间
    public LayerMask enemyLayer;           // 敌人图层

    private float nextAttackTime = 0f;
    private float lifeTimer = 0f;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lifeTimer = duration;
    }

    void Update()
    {
        // 更新生存计时器
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        // 自动攻击
        if (Time.time >= nextAttackTime)
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        // 寻找最近的敌人
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        
        if (enemies.Length > 0)
        {
            // 找到最近的敌人
            Transform target = null;
            float minDistance = Mathf.Infinity;
            
            foreach (var enemy in enemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    target = enemy.transform;
                }
            }

            if (target != null)
            {
                Attack(target);
                nextAttackTime = Time.time + attackRate;
            }
        }
    }

    void Attack(Transform target)
    {
        // 播放攻击动画
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        // 计算发射方向
        Vector2 direction = (target.position - firePoint.position).normalized;
        
        // 计算旋转
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        // 生成子弹
        Instantiate(magicBallPrefab, firePoint.position, rotation);
    }

    void OnDrawGizmosSelected()
    {
        // 绘制攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
