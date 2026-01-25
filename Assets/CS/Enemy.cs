using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("数值设置")]
    public int maxHealth = 3;
    public int currentHealth = 3;
    public float moveSpeed = 2f;      // 移动速度
    public float detectionRange = 8f; // 发现玩家的距离
    public float stopDistance = 1.2f; // 离玩家多近时停下（防止重叠）

    [Header("视觉反馈")]
    public Color hurtColor = Color.red;
    private Color originalColor;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Transform player; // 玩家的坐标

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = sr.color;
        
        // 自动寻找带有 "Player" 标签的物体
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            // 如果玩家进入范围且不在“贴脸”距离，就移动
            if (distance < detectionRange && distance > stopDistance)
            {
                MoveTowardsPlayer();
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y); // 停下
            }
        }
    }

    void MoveTowardsPlayer()
    {
        // 计算方向
        float direction = player.position.x > transform.position.x ? 1 : -1;
        
        // 执行移动
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // 怪物转向
        transform.localScale = new Vector3(direction, 1, 1);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(HitStop(0.05f)); 
        StartCoroutine(FlashHurt());
        if (currentHealth <= 0) Die();
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    IEnumerator FlashHurt()
    {
        sr.color = hurtColor;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    void Die()
{
    // 1. 恢复时间（防止死在顿帧里导致全场静止）
    Time.timeScale = 1f;
    
    // 2. 可以在这里播放死亡特效
    Debug.Log("怪兽死亡");
    
    // 3. 立即关闭碰撞体和渲染，防止它“诈尸”或者挡路
    GetComponent<Collider2D>().enabled = false;
    if(GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().enabled = false;

    // 4. 彻底销毁
    Destroy(gameObject, 0.1f); 
}
}