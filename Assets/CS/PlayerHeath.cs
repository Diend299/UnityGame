using UnityEngine;
using System.Collections;
using UnityEngine.UI; // 必须引用 UI 命名空间

public class PlayerHealth : MonoBehaviour
{
    [Header("生命值设置")]
    public int maxHealth = 10;
    public int currentHealth;
    [Header("UI 关联")]
    public Slider healthSlider; // 将你的 Slider 拖到这里

    [Header("受伤反馈")]
    public float invincibilityDuration = 1.5f; // 受伤后的无敌时间
    private bool isInvincible = false;
    
    private SpriteRenderer sr;
    private Color originalColor;
    private Animator anim;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalColor = sr.color;
                // 初始化血条
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        // 如果正在无敌状态，直接跳过不扣血
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log("海诺受伤！剩余血量：" + currentHealth);
                // 更新 UI
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // 1. 触发受伤动画（如果有的话）
        anim.SetTrigger("Hurt");

        // 2. 进入无敌闪烁状态
        StartCoroutine(BecomeInvincible());

        // 3. 检查游戏结束
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        
        // 闪烁效果：在无敌时间内不停切换透明度
        float timer = 0;
        while (timer < invincibilityDuration)
        {
            sr.color = new Color(1, 1, 1, 0.2f); // 变透明
            yield return new WaitForSeconds(0.15f);
            sr.color = originalColor; // 变回来
            yield return new WaitForSeconds(0.15f);
            timer += 0.3f;
        }

        isInvincible = false;
        sr.color = originalColor;
    }

    void GameOver()
    {
        Debug.Log("海诺倒下了...");
        // 暂时先重启当前关卡
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}