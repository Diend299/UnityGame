using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [Header("攻击参数")]
    public GameObject breathPrefab;   // 吐息子弹预制体
    public Transform firePoint;       // 嘴部位置
    public float attackRange = 5f;    // 进入这个范围就开始喷
    public float attackCooldown = 2f; // 攻击冷却

    private float nextAttackTime = 0f;
    private Transform player;
    private Animator anim;
    private Enemy enemyMovement;      // 引用之前的移动脚本

    void Start()
    {
        anim = GetComponent<Animator>();
        enemyMovement = GetComponent<Enemy>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 如果在攻击范围内且冷却好了
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            StartCoroutine(PerformAttack());
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    IEnumerator PerformAttack()
    {
        enemyMovement.moveSpeed = 0; // 停止移动

        // 1. 触发前摇动画
        anim.SetTrigger("Attack"); // 这里触发 attackM0

        // 2. 等待前摇结束 (假设 attackM0 长度为 0.5秒)
        yield return new WaitForSeconds(0.5f);

        // 此时动画机因为 Exit Time 已经自动切到了 tuxi0（持续吐息）

        // 3. 开启持续伤害/生成光束（这里可以循环生成小火球，或者开启一个长条光束物体）
        float duration = 2.0f; // 持续吐息 2 秒
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 这里可以每隔 0.1 秒生成一个微型子弹，或者控制激光显示
            SpawnBreathEffect(); 
            elapsed += 0.2f;
            yield return new WaitForSeconds(0.2f);
        }

        // 4. 停止攻击，回到待机
        anim.SetTrigger("FinishAttack"); // 在 Animator 里连线回 Idle，条件设为这个 Trigger
        enemyMovement.moveSpeed = 2f; 
    }
    // 在这里真正生成吐息物体
    void SpawnBreathEffect()
    {
        if (player != null && breathPrefab != null)
        {
            // 计算从嘴巴指向玩家的方向
            Vector2 direction = (player.position - firePoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // 生成胶囊体子弹
            GameObject bullet = Instantiate(breathPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
            
            // 给它一个初始速度（如果你想让它像喷泉一样喷出来）
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if(rb != null) {
                rb.velocity = bullet.transform.right * 10f; // 10是飞行速度
            }
        }
    }
}