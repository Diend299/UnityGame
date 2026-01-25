using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float lifeTime = 2f;

    void Start()
    {
        // 确保它有速度。transform.right 会根据生成时的旋转角度自动朝向
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.right * speed;
        }
        
        // 2秒后自动消失
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果撞到了玩家
        if (collision.CompareTag("Player"))
        {
            // 尝试获取玩家的血量脚本并扣血
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            
            // 撞到玩家后子弹消失（也可以不消失，做成穿透伤害）
            //Destroy(gameObject);
        }
        
        // 撞到地面也消失
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}