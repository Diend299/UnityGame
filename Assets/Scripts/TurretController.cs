using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class TurretController : MonoBehaviour
{
    [Header("炮台设置")]
    public GameObject turretPrefab;        // 炮台预制体
    public float spawnDistance = 2f;       // 生成距离
    public float cooldownDuration = 40f;   // 冷却时间（秒）
    public KeyCode summonKey = KeyCode.Q;  // 召唤按键

    [Header("UI 引用")]
    public Image turretCooldownImage;      // 技能冷却圆形冷却条

    private float _cooldownRemaining;      // 当前剩余冷却时间
    private Animator _playerAnim;

    void Start()
    {
        _playerAnim = GetComponent<Animator>();
        
        // 配置冷却背景图为圆形填充模式
        if (turretCooldownImage != null)
        {
            turretCooldownImage.type = Image.Type.Filled;
            turretCooldownImage.fillMethod = Image.FillMethod.Radial360;
            turretCooldownImage.fillClockwise = true;
            turretCooldownImage.fillOrigin = (int)Image.Origin360.Top;
            turretCooldownImage.fillAmount = 1f;
        }
    }

    void Update()
    {
        // 冷却计时
        if (_cooldownRemaining > 0f)
        {
            _cooldownRemaining -= Time.deltaTime;
            if (_cooldownRemaining < 0f) _cooldownRemaining = 0f;
        }

        // 更新冷却 UI
        if (turretCooldownImage != null && cooldownDuration > 0f)
        {
            float progress = 1f - (_cooldownRemaining / cooldownDuration);
            turretCooldownImage.fillAmount = Mathf.Clamp01(progress);
        }

        // 检测按键
        if (Input.GetKeyDown(summonKey) && _cooldownRemaining <= 0f)
        {
            SummonTurret();
        }
    }

    void SummonTurret()
    {
        if (turretPrefab == null) return;

        // 计算生成位置
        Vector3 spawnPosition = transform.position;
        Vector3 forward = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        spawnPosition += forward * spawnDistance;

        // 生成炮台
        GameObject turret = Instantiate(turretPrefab, spawnPosition, Quaternion.identity);
        
        // 设置炮台的朝向
        turret.transform.localScale = new Vector3(transform.localScale.x, 1, 1);

        // 进入冷却
        _cooldownRemaining = cooldownDuration;
        
        // 播放召唤动画
        if (_playerAnim != null)
        {
            _playerAnim.SetTrigger("Summon");
        }
    }
}
