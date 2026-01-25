using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("攻击设置")]
    public GameObject magicBallPrefab; // 魔法球预制体
    public Transform firePoint;        // 召唤点（海诺的手部位置）
    public float attackRate = 0.5f;    // 攻击间隔（秒）
    
    private float nextAttackTime = 0f;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 检测鼠标左键点击，且满足攻击冷却
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackRate;
        }
    }

    void Attack()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // 拿到身上的 Controller 脚本
        PlayerController controller = GetComponent<PlayerController>();

        if (mousePos.x > transform.position.x)
        {
            controller.facingDirection = 1; // 修改控制器的变量
        }
        else
        {
            controller.facingDirection = -1; // 修改控制器的变量
        }


        // 3. 播放攻击动画
        anim.SetTrigger("Attack");

        // 4. 计算发射方向（同样使用上面定义好的 mousePos）
        Vector2 direction = (mousePos - firePoint.position).normalized;

        // 5. 计算旋转并生成魔法球
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Instantiate(magicBallPrefab, firePoint.position, rotation);
    }
}
