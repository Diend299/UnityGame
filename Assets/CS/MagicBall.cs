using UnityEngine;

public class MagicBall : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 2f;
    public int damage = 1; // 子弹伤害值
    public GameObject hitEffectPrefab; // 拖入刚才做的粒子预制体

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 尝试获取撞到的物体身上是否有 Enemy 脚本
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null)
        {
            // 如果撞到的是怪兽
            // 1. 在撞击位置生成爆炸粒子
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            enemy.TakeDamage(damage); // 让怪兽扣血
            
            Destroy(gameObject);      // 子弹消失
        }
        
        // 如果撞到的是墙壁（假设以后你给墙加个标签）
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}