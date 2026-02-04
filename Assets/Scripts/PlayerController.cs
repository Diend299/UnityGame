using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("呼吸设置")]
    public float breathSpeed = 2f;    // 呼吸快慢
    public float breathAmount = 0.01f; // 呼吸幅度（不要太大）

    [Header("检测设置")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private float horizontalInput;
    public int facingDirection = 1; // 记录朝向：1是右，-1是左

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 1. 跳跃逻辑
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // 2. 转向逻辑（只记录方向，不直接改Scale）
        if (horizontalInput > 0) facingDirection = 1;
        else if (horizontalInput < 0) facingDirection = -1;

        // 3. 呼吸逻辑 (使用正弦波)
        float breatheEffect = 0;
        // 只有在站立且在地板上时才呼吸
        if (horizontalInput == 0 && isGrounded)
        {
            breatheEffect = Mathf.Sin(Time.time * breathSpeed) * breathAmount;
        }

        // 4. 最终应用缩放：合并转向 + 呼吸
        // X轴受转向影响，Y轴受呼吸影响
        transform.localScale = new Vector3(facingDirection, 1f + breatheEffect, 1f);

        // 5. 传值给动画机
        anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
    }
    [Header("踩头设置")]
    public float bounceForce = 10f; // 踩怪后的弹跳力度

    // 当玩家碰到东西时触发
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. 判断撞到的是不是敌人
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 2. 关键：判断玩家是否正在“向下掉” (y轴速度小于0)
            // 并且玩家的位置要在敌人的上方（防止从侧面撞也起跳）
            if (rb.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                // 给玩家一个向上的瞬间速度，实现“踩头跳”
                rb.velocity = new Vector2(rb.velocity.x, bounceForce);
                
                // (可选) 如果你想踩一下怪也扣怪物的血，可以在这里调用：
                // collision.gameObject.GetComponent<Enemy>().TakeDamage(1);
            }
        }
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }
}
