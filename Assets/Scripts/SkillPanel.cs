using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 技能面板 - 显示玩家技能信息和冷却状态
/// </summary>
public class SkillPanel : MonoBehaviour
{
    [Header("技能1 - 魔法球")]
    [SerializeField] private Image skill1Icon;                    // 技能1图标
    [SerializeField] private Image skill1CooldownImage;           // 技能1冷却遮罩
    [SerializeField] private TextMeshProUGUI skill1CooldownText;  // 技能1冷却时间文本
    [SerializeField] private TextMeshProUGUI skill1NameText;      // 技能1名称
    [SerializeField] private TextMeshProUGUI skill1DescText;      // 技能1描述
    [SerializeField] private TextMeshProUGUI skill1DamageText;    // 技能1伤害
    [SerializeField] private TextMeshProUGUI skill1CooldownInfoText; // 技能1冷却信息
    
    [Header("技能2 - 时间回溯")]
    [SerializeField] private Image skill2Icon;                    // 技能2图标
    [SerializeField] private Image skill2CooldownImage;           // 技能2冷却遮罩
    [SerializeField] private TextMeshProUGUI skill2CooldownText;  // 技能2冷却时间文本
    [SerializeField] private TextMeshProUGUI skill2NameText;      // 技能2名称
    [SerializeField] private TextMeshProUGUI skill2DescText;      // 技能2描述
    [SerializeField] private TextMeshProUGUI skill2EffectText;    // 技能2效果描述
    [SerializeField] private TextMeshProUGUI skill2CooldownInfoText; // 技能2冷却信息
    
    [Header("技能3 - 炮台召唤")]
    [SerializeField] private Image skill3Icon;                    // 技能3图标
    [SerializeField] private Image skill3CooldownImage;           // 技能3冷却遮罩
    [SerializeField] private TextMeshProUGUI skill3CooldownText;  // 技能3冷却时间文本
    [SerializeField] private TextMeshProUGUI skill3NameText;      // 技能3名称
    [SerializeField] private TextMeshProUGUI skill3DescText;      // 技能3描述
    [SerializeField] private TextMeshProUGUI skill3DurationText;  // 技能3持续时间
    [SerializeField] private TextMeshProUGUI skill3CooldownInfoText; // 技能3冷却信息
    
    [Header("玩家组件引用")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private HainoRecall hainoRecall;
    [SerializeField] private TurretController turretController;
    
    [Header("技能数据")]
    [SerializeField] private string skill1Name = "魔法飞弹";
    [SerializeField] private string skill1Description = "发射一枚魔法飞弹，对敌人造成伤害";
    [SerializeField] private int skill1Damage = 15;
    [SerializeField] private float skill1Cooldown = 0.5f;
    
    [SerializeField] private string skill2Name = "时间回溯";
    [SerializeField] private string skill2Description = "回溯到3秒前的位置和状态，同时触发时停效果";
    [SerializeField] private float skill2Cooldown = 10f;
    
    [SerializeField] private string skill3Name = "炮台召唤";
    [SerializeField] private string skill3Description = "召唤一个自动攻击的魔法炮台";
    [SerializeField] private float skill3Duration = 10f;
    [SerializeField] private float skill3Cooldown = 40f;
    
    [Header("冷却条设置")]
    [SerializeField] private Color cooldownNormalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    [SerializeField] private Color cooldownReadyColor = new Color(1f, 1f, 1f, 0f);
    
    // 私有变量
    private float skill1CurrentCooldown = 0f;
    private float skill2CurrentCooldown = 0f;
    private float skill3CurrentCooldown = 0f;
    
    private void Start()
    {
        // 自动获取玩家组件
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (playerCombat == null)
                playerCombat = player.GetComponent<PlayerCombat>();
            if (hainoRecall == null)
                hainoRecall = player.GetComponent<HainoRecall>();
            if (turretController == null)
                turretController = player.GetComponent<TurretController>();
        }
        
        // 初始化技能信息显示
        InitializeSkillInfo();
        
        // 设置冷却图片为填充模式
        SetupCooldownImages();
    }
    
    private void Update()
    {
        // 更新技能冷却状态
        UpdateSkillCooldowns();
    }
    
    /// <summary>
    /// 初始化技能信息显示
    /// </summary>
    private void InitializeSkillInfo()
    {
        // 技能1信息
        if (skill1NameText != null) skill1NameText.text = skill1Name;
        if (skill1DescText != null) skill1DescText.text = skill1Description;
        if (skill1DamageText != null) skill1DamageText.text = $"伤害: {skill1Damage}";
        if (skill1CooldownInfoText != null) skill1CooldownInfoText.text = $"冷却: {skill1Cooldown}秒";
        
        // 技能2信息
        if (skill2NameText != null) skill2NameText.text = skill2Name;
        if (skill2DescText != null) skill2DescText.text = skill2Description;
        if (skill2EffectText != null) skill2EffectText.text = "效果: 回溯+时停";
        if (skill2CooldownInfoText != null) skill2CooldownInfoText.text = $"冷却: {skill2Cooldown}秒";
        
        // 技能3信息
        if (skill3NameText != null) skill3NameText.text = skill3Name;
        if (skill3DescText != null) skill3DescText.text = skill3Description;
        if (skill3DurationText != null) skill3DurationText.text = $"持续: {skill3Duration}秒";
        if (skill3CooldownInfoText != null) skill3CooldownInfoText.text = $"冷却: {skill3Cooldown}秒";
    }
    
    /// <summary>
    /// 设置冷却图片
    /// </summary>
    private void SetupCooldownImages()
    {
        // 设置所有冷却图片为Radial360填充模式
        SetupCooldownImage(skill1CooldownImage);
        SetupCooldownImage(skill2CooldownImage);
        SetupCooldownImage(skill3CooldownImage);
    }
    
    /// <summary>
    /// 设置单个冷却图片
    /// </summary>
    private void SetupCooldownImage(Image cooldownImage)
    {
        if (cooldownImage != null)
        {
            cooldownImage.type = Image.Type.Filled;
            cooldownImage.fillMethod = Image.FillMethod.Radial360;
            cooldownImage.fillClockwise = true;
            cooldownImage.fillOrigin = (int)Image.Origin360.Top;
            cooldownImage.fillAmount = 0f;
            cooldownImage.color = cooldownReadyColor;
        }
    }
    
    /// <summary>
    /// 更新技能冷却状态
    /// </summary>
    private void UpdateSkillCooldowns()
    {
        // 这里需要通过反射或其他方式获取实际技能的冷却状态
        // 暂时使用模拟数据展示效果
        
        // 技能1冷却（普通攻击）
        if (playerCombat != null)
        {
            // 假设PlayerCombat中有获取冷却的方法
            UpdateCooldownDisplay(skill1CooldownImage, skill1CooldownText, 
                GetSkill1CooldownProgress(), GetSkill1RemainingCooldown());
        }
        
        // 技能2冷却（时间回溯）
        if (hainoRecall != null)
        {
            UpdateCooldownDisplay(skill2CooldownImage, skill2CooldownText,
                GetSkill2CooldownProgress(), GetSkill2RemainingCooldown());
        }
        
        // 技能3冷却（炮台召唤）
        if (turretController != null)
        {
            UpdateCooldownDisplay(skill3CooldownImage, skill3CooldownText,
                GetSkill3CooldownProgress(), GetSkill3RemainingCooldown());
        }
    }
    
    /// <summary>
    /// 更新冷却显示
    /// </summary>
    private void UpdateCooldownDisplay(Image cooldownImage, TextMeshProUGUI cooldownText, 
        float progress, float remainingTime)
    {
        if (cooldownImage != null)
        {
            // progress: 0 = 冷却中, 1 = 冷却完成
            cooldownImage.fillAmount = 1f - progress;
            
            // 根据冷却状态改变颜色
            if (progress >= 1f)
            {
                cooldownImage.color = cooldownReadyColor;  // 技能就绪，隐藏遮罩
            }
            else
            {
                cooldownImage.color = cooldownNormalColor; // 冷却中，显示遮罩
            }
        }
        
        if (cooldownText != null)
        {
            if (remainingTime > 0f)
            {
                cooldownText.text = remainingTime.ToString("F1");
                cooldownText.gameObject.SetActive(true);
            }
            else
            {
                cooldownText.gameObject.SetActive(false);
            }
        }
    }
    
    // 模拟获取技能冷却进度的方法
    // 实际使用时需要与PlayerCombat、HainoRecall、TurretController中的实际冷却系统对接
    
    private float GetSkill1CooldownProgress()
    {
        // 返回0-1的值，1表示冷却完成
        // 这里需要与PlayerCombat的实际冷却对接
        return 1f; // 默认可用
    }
    
    private float GetSkill1RemainingCooldown()
    {
        return 0f;
    }
    
    private float GetSkill2CooldownProgress()
    {
        // 需要与HainoRecall的实际冷却对接
        // 可以通过反射获取私有字段或使用公共方法
        if (hainoRecall != null)
        {
            // 暂时返回满进度（可用）
            return 1f;
        }
        return 1f;
    }
    
    private float GetSkill2RemainingCooldown()
    {
        return 0f;
    }
    
    private float GetSkill3CooldownProgress()
    {
        // 需要与TurretController的实际冷却对接
        if (turretController != null)
        {
            return 1f;
        }
        return 1f;
    }
    
    private float GetSkill3RemainingCooldown()
    {
        return 0f;
    }
    
    /// <summary>
    /// 触发技能冷却显示（外部调用）
    /// </summary>
    public void TriggerSkillCooldown(int skillIndex, float cooldownDuration)
    {
        switch (skillIndex)
        {
            case 1:
                StartCoroutine(CooldownAnimation(skill1CooldownImage, skill1CooldownText, cooldownDuration));
                break;
            case 2:
                StartCoroutine(CooldownAnimation(skill2CooldownImage, skill2CooldownText, cooldownDuration));
                break;
            case 3:
                StartCoroutine(CooldownAnimation(skill3CooldownImage, skill3CooldownText, cooldownDuration));
                break;
        }
    }
    
    /// <summary>
    /// 冷却动画协程
    /// </summary>
    private IEnumerator CooldownAnimation(Image cooldownImage, TextMeshProUGUI cooldownText, float duration)
    {
        if (cooldownImage == null) yield break;
        
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f;
        cooldownImage.color = cooldownNormalColor;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            cooldownImage.fillAmount = 1f - progress;
            
            if (cooldownText != null)
            {
                float remaining = duration - elapsed;
                cooldownText.text = remaining.ToString("F1");
                cooldownText.gameObject.SetActive(remaining > 0);
            }
            
            yield return null;
        }
        
        cooldownImage.fillAmount = 0f;
        cooldownImage.color = cooldownReadyColor;
        
        if (cooldownText != null)
            cooldownText.gameObject.SetActive(false);
    }
}
