using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 角色信息面板 - 显示玩家角色信息
/// </summary>
public class CharacterInfoPanel : MonoBehaviour
{
    [Header("玩家引用")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator playerAnimator;
    
    [Header("UI 引用")]
    [SerializeField] private Image characterPortrait;      // 角色头像
    [SerializeField] private TextMeshProUGUI characterNameText;  // 角色名称
    [SerializeField] private TextMeshProUGUI levelText;    // 等级文本
    [SerializeField] private TextMeshProUGUI healthText;   // 生命值文本
    [SerializeField] private TextMeshProUGUI moveSpeedText; // 移动速度文本
    [SerializeField] private Slider healthSlider;          // 生命值滑块
    [SerializeField] private Slider expSlider;             // 经验值滑块
    
    [Header("属性显示")]
    [SerializeField] private TextMeshProUGUI attackPowerText;  // 攻击力
    [SerializeField] private TextMeshProUGUI defenseText;      // 防御力
    [SerializeField] private TextMeshProUGUI critRateText;     // 暴击率
    
    [Header("待机动画")]
    [SerializeField] private bool playIdleAnimation = true;
    [SerializeField] private string idleAnimationName = "Idle";
    
    [Header("数据")]
    [SerializeField] private string characterName = "海诺";
    [SerializeField] private int characterLevel = 1;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int maxExp = 100;
    [SerializeField] private int attackPower = 15;
    [SerializeField] private int defense = 8;
    [SerializeField] private float critRate = 5f;
    
    private void Start()
    {
        // 如果未手动指定，尝试自动获取
        if (playerHealth == null)
            playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerHealth>();
        
        if (playerController == null && playerHealth != null)
            playerController = playerHealth.GetComponent<PlayerController>();
        
        if (playerAnimator == null && playerHealth != null)
            playerAnimator = playerHealth.GetComponent<Animator>();
        
        // 初始化显示
        UpdateCharacterInfo();
    }
    
    private void Update()
    {
        // 实时更新生命值
        UpdateHealthDisplay();
        
        // 播放待机动画
        if (playIdleAnimation && playerAnimator != null)
        {
            // 保持播放待机动画
            playerAnimator.SetBool("IsIdle", true);
        }
    }
    
    /// <summary>
    /// 更新角色信息显示
    /// </summary>
    public void UpdateCharacterInfo()
    {
        // 更新名称和等级
        if (characterNameText != null)
            characterNameText.text = characterName;
        
        if (levelText != null)
            levelText.text = $"Lv.{characterLevel}";
        
        // 更新属性
        if (attackPowerText != null)
            attackPowerText.text = attackPower.ToString();
        
        if (defenseText != null)
            defenseText.text = defense.ToString();
        
        if (critRateText != null)
            critRateText.text = $"{critRate}%";
        
        // 更新经验条
        if (expSlider != null)
        {
            expSlider.maxValue = maxExp;
            expSlider.value = currentExp;
        }
        
        UpdateHealthDisplay();
    }
    
    /// <summary>
    /// 更新生命值显示
    /// </summary>
    private void UpdateHealthDisplay()
    {
        if (playerHealth != null)
        {
            // 更新生命值文本
            if (healthText != null)
                healthText.text = $"{playerHealth.currentHealth} / {playerHealth.maxHealth}";
            
            // 同步生命值滑块
            if (healthSlider != null && playerHealth.healthSlider != null)
            {
                healthSlider.maxValue = playerHealth.maxHealth;
                healthSlider.value = playerHealth.currentHealth;
            }
        }
    }
    
    /// <summary>
    /// 增加经验值
    /// </summary>
    public void AddExp(int amount)
    {
        currentExp += amount;
        
        if (currentExp >= maxExp)
        {
            LevelUp();
        }
        
        if (expSlider != null)
            expSlider.value = currentExp;
    }
    
    /// <summary>
    /// 升级
    /// </summary>
    private void LevelUp()
    {
        characterLevel++;
        currentExp -= maxExp;
        maxExp = Mathf.RoundToInt(maxExp * 1.2f);  // 经验需求增加20%
        
        // 提升属性
        attackPower += 2;
        defense += 1;
        critRate += 0.5f;
        
        // 恢复满血
        if (playerHealth != null)
            playerHealth.currentHealth = playerHealth.maxHealth;
        
        UpdateCharacterInfo();
        
        Debug.Log($"恭喜！{characterName} 升到了 {characterLevel} 级！");
    }
    
    /// <summary>
    /// 设置角色头像
    /// </summary>
    public void SetCharacterPortrait(Sprite portrait)
    {
        if (characterPortrait != null)
            characterPortrait.sprite = portrait;
    }
    
    /// <summary>
    /// 更新属性值（用于装备或buff变化时）
    /// </summary>
    public void UpdateStats(int newAttack, int newDefense, float newCritRate)
    {
        attackPower = newAttack;
        defense = newDefense;
        critRate = newCritRate;
        
        UpdateCharacterInfo();
    }
}
