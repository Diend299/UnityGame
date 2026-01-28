using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerHealth))]
public class HainoRecall : MonoBehaviour
{
    private class Snapshot
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float Health;
        public float Time;
    }

    [Header("回溯参数")]
    [SerializeField] private float recordDuration = 3f;      // 记录持续时间（秒）
    [SerializeField] private float recallDuration = 0.5f;    // 回溯动画时间（秒）
    [SerializeField] private KeyCode recallKey = KeyCode.E;  // 触发按键

    [Header("冷却设置")]
    [SerializeField] private float cooldownDuration = 10f;   // 冷却时间（秒）

    [Header("UI 引用")]
    [SerializeField] private Image recallCooldownImage;      // 技能冷却圆形冷却条（放在血量滑块下方，只用 Background 图像）

    private readonly List<Snapshot> _history = new List<Snapshot>();
    private PlayerHealth _playerHealth;
    private bool _isRecalling;
    private float _recallTimer;

    private float _cooldownRemaining;   // 当前剩余冷却时间（>0 表示正在冷却）

    private Snapshot _startSnapshot;   // 回溯开始时的状态
    private Snapshot _targetSnapshot;  // 要回溯到的目标状态（3 秒前）

    private void Awake()
    {
        _playerHealth = GetComponent<PlayerHealth>();

        // 配置冷却背景图为圆形填充模式
        if (recallCooldownImage != null)
        {
            recallCooldownImage.type = Image.Type.Filled;
            recallCooldownImage.fillMethod = Image.FillMethod.Radial360;
            recallCooldownImage.fillClockwise = true;  // 顺时针
            // 起点可以根据你的 UI 需要调整，这里用顶部为起点
            recallCooldownImage.fillOrigin = (int)Image.Origin360.Top;
            // 初始状态：技能未冷却，显示满圈表示“就绪”
            recallCooldownImage.fillAmount = 1f;
        }
    }

    private void Update()
    {
        float now = Time.time;

        // 冷却计时
        if (_cooldownRemaining > 0f)
        {
            _cooldownRemaining -= Time.deltaTime;
            if (_cooldownRemaining < 0f) _cooldownRemaining = 0f;
        }

        // 更新冷却 UI：使用 Background 图像做圆形顺时针加载
        if (recallCooldownImage != null && cooldownDuration > 0f)
        {
            // progress: 0 -> 冷却刚开始（空），1 -> 冷却结束（满圈，技能就绪）
            float progress = 1f - (_cooldownRemaining / cooldownDuration);
            recallCooldownImage.fillAmount = Mathf.Clamp01(progress);
        }

        // 记录历史（即使在回溯中也记录当前状态，方便后续继续使用）
        RecordSnapshot(now);
        TrimHistory(now);

        if (_isRecalling)
        {
            UpdateRecall(now);
        }
        else
        {
            // 技能只有在不处于回溯中且不在冷却中时才能释放
            if (Input.GetKeyDown(recallKey) && _cooldownRemaining <= 0f)
            {
                TryStartRecall(now);
            }
        }
    }

    private void RecordSnapshot(float now)
    {
        if (_playerHealth == null) return;

        var snap = new Snapshot
        {
            Position = transform.position,
            Rotation = transform.rotation,
            // 使用 PlayerHealth 的 int currentHealth 字段
            Health = _playerHealth.currentHealth,
            Time = now
        };

        _history.Add(snap);
    }

    private void TrimHistory(float now)
    {
        float cutoff = now - recordDuration;
        // 移除超过记录时间窗口的旧数据
        while (_history.Count > 0 && _history[0].Time < cutoff)
        {
            _history.RemoveAt(0);
        }
    }

    private void TryStartRecall(float now)
    {
        if (_history.Count == 0 || _playerHealth == null) return;

        float targetTime = now - recordDuration;

        // 在历史列表中找到最接近 targetTime 的快照
        Snapshot best = _history[0];
        float bestDiff = Mathf.Abs(best.Time - targetTime);

        for (int i = 1; i < _history.Count; i++)
        {
            float diff = Mathf.Abs(_history[i].Time - targetTime);
            if (diff < bestDiff)
            {
                bestDiff = diff;
                best = _history[i];
            }
        }

        _startSnapshot = new Snapshot
        {
            Position = transform.position,
            Rotation = transform.rotation,
            Health = _playerHealth.currentHealth,
            Time = now
        };

        _targetSnapshot = best;
        _isRecalling = true;
        _recallTimer = 0f;

        // 开始回溯时进入冷却（10s）
        _cooldownRemaining = cooldownDuration;
    }

    private void UpdateRecall(float now)
    {
        if (_startSnapshot == null || _targetSnapshot == null)
        {
            _isRecalling = false;
            return;
        }

        _recallTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_recallTimer / recallDuration);

        // 位置和旋转平滑插值
        transform.position = Vector3.Lerp(_startSnapshot.Position, _targetSnapshot.Position, t);
        transform.rotation = Quaternion.Slerp(_startSnapshot.Rotation, _targetSnapshot.Rotation, t);

        // 血量平滑插值（PlayerHealth 使用 int currentHealth，无专门的 SetHealth 方法）
        float newHealth = Mathf.Lerp(_startSnapshot.Health, _targetSnapshot.Health, t);
        int roundedHealth = Mathf.RoundToInt(newHealth);
        _playerHealth.currentHealth = Mathf.Clamp(roundedHealth, 0, _playerHealth.maxHealth);

        // 同步更新血条 UI（如果已经绑定了 Slider）
        if (_playerHealth.healthSlider != null)
        {
            _playerHealth.healthSlider.value = _playerHealth.currentHealth;
        }

        if (t >= 1f)
        {
            _isRecalling = false;
        }
    }
}
