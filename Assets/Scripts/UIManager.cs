using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI管理器 - 控制玩家信息面板的显示/隐藏和面板切换
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("面板引用")]
    [SerializeField] private GameObject playerInfoPanel;  // 玩家信息主面板
    [SerializeField] private GameObject characterInfoContent;  // 角色信息内容
    [SerializeField] private GameObject skillInfoContent;      // 技能信息内容
    
    [Header("按钮引用")]
    [SerializeField] private Button characterTabButton;   // 角色信息标签按钮
    [SerializeField] private Button skillTabButton;       // 技能标签按钮
    [SerializeField] private Button closeButton;          // 关闭按钮
    [SerializeField] private Button openButton;           // 打开面板按钮
    
    [Header("动画设置")]
    [SerializeField] private float panelAnimationDuration = 0.3f;
    [SerializeField] private AnimationCurve panelAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("按钮颜色")]
    [SerializeField] private Color selectedTabColor = new Color(1f, 0.8f, 0.4f, 1f);  // 选中标签颜色（金色）
    [SerializeField] private Color normalTabColor = new Color(0.7f, 0.7f, 0.7f, 0.8f); // 普通标签颜色
    
    private bool isPanelOpen = false;
    private RectTransform panelRectTransform;
    private CanvasGroup panelCanvasGroup;
    private Coroutine currentAnimation;
    
    private void Start()
    {
        // 获取组件引用
        panelRectTransform = playerInfoPanel.GetComponent<RectTransform>();
        panelCanvasGroup = playerInfoPanel.GetComponent<CanvasGroup>();
        
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = playerInfoPanel.AddComponent<CanvasGroup>();
        }
        
        // 初始状态：面板隐藏
        playerInfoPanel.SetActive(false);
        panelCanvasGroup.alpha = 0f;
        panelRectTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        
        // 绑定按钮事件
        if (characterTabButton != null)
            characterTabButton.onClick.AddListener(ShowCharacterInfo);
        
        if (skillTabButton != null)
            skillTabButton.onClick.AddListener(ShowSkillInfo);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
        
        if (openButton != null)
            openButton.onClick.AddListener(OpenPanel);
        
        // 默认显示角色信息
        ShowCharacterInfo();
    }
    
    private void Update()
    {
        // 按 Tab 键切换面板显示
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePanel();
        }
        
        // 按 ESC 键关闭面板
        if (Input.GetKeyDown(KeyCode.Escape) && isPanelOpen)
        {
            ClosePanel();
        }
    }
    
    /// <summary>
    /// 切换面板显示状态
    /// </summary>
    public void TogglePanel()
    {
        if (isPanelOpen)
            ClosePanel();
        else
            OpenPanel();
    }
    
    /// <summary>
    /// 打开面板
    /// </summary>
    public void OpenPanel()
    {
        if (isPanelOpen) return;
        
        isPanelOpen = true;
        playerInfoPanel.SetActive(true);
        
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        
        currentAnimation = StartCoroutine(AnimatePanelOpen());
    }
    
    /// <summary>
    /// 关闭面板
    /// </summary>
    public void ClosePanel()
    {
        if (!isPanelOpen) return;
        
        isPanelOpen = false;
        
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        
        currentAnimation = StartCoroutine(AnimatePanelClose());
    }
    
    /// <summary>
    /// 面板打开动画
    /// </summary>
    private IEnumerator AnimatePanelOpen()
    {
        float elapsed = 0f;
        Vector3 startScale = new Vector3(0.8f, 0.8f, 0.8f);
        Vector3 endScale = Vector3.one;
        
        while (elapsed < panelAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = panelAnimationCurve.Evaluate(elapsed / panelAnimationDuration);
            
            panelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            panelRectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            yield return null;
        }
        
        panelCanvasGroup.alpha = 1f;
        panelRectTransform.localScale = endScale;
    }
    
    /// <summary>
    /// 面板关闭动画
    /// </summary>
    private IEnumerator AnimatePanelClose()
    {
        float elapsed = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = new Vector3(0.8f, 0.8f, 0.8f);
        
        while (elapsed < panelAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = panelAnimationCurve.Evaluate(elapsed / panelAnimationDuration);
            
            panelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            panelRectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            yield return null;
        }
        
        panelCanvasGroup.alpha = 0f;
        panelRectTransform.localScale = endScale;
        playerInfoPanel.SetActive(false);
    }
    
    /// <summary>
    /// 显示角色信息面板
    /// </summary>
    public void ShowCharacterInfo()
    {
        characterInfoContent.SetActive(true);
        skillInfoContent.SetActive(false);
        
        // 更新按钮颜色
        UpdateTabButtonColors(true);
        
        // 播放切换动画
        PlayContentSwitchAnimation(characterInfoContent);
    }
    
    /// <summary>
    /// 显示技能信息面板
    /// </summary>
    public void ShowSkillInfo()
    {
        characterInfoContent.SetActive(false);
        skillInfoContent.SetActive(true);
        
        // 更新按钮颜色
        UpdateTabButtonColors(false);
        
        // 播放切换动画
        PlayContentSwitchAnimation(skillInfoContent);
    }
    
    /// <summary>
    /// 更新标签按钮颜色
    /// </summary>
    private void UpdateTabButtonColors(bool isCharacterSelected)
    {
        if (characterTabButton != null)
        {
            var colors = characterTabButton.colors;
            colors.normalColor = isCharacterSelected ? selectedTabColor : normalTabColor;
            characterTabButton.colors = colors;
        }
        
        if (skillTabButton != null)
        {
            var colors = skillTabButton.colors;
            colors.normalColor = isCharacterSelected ? normalTabColor : selectedTabColor;
            skillTabButton.colors = colors;
        }
    }
    
    /// <summary>
    /// 播放内容切换动画
    /// </summary>
    private void PlayContentSwitchAnimation(GameObject content)
    {
        // 简单的缩放动画
        RectTransform rect = content.GetComponent<RectTransform>();
        if (rect != null)
        {
            StopAllCoroutines();
            StartCoroutine(ContentSwitchAnimationCoroutine(rect));
        }
    }
    
    private IEnumerator ContentSwitchAnimationCoroutine(RectTransform rect)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 startScale = new Vector3(0.95f, 0.95f, 0.95f);
        Vector3 endScale = Vector3.one;
        
        rect.localScale = startScale;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rect.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        
        rect.localScale = endScale;
    }
    
    private void OnDestroy()
    {
        // 移除按钮事件监听
        if (characterTabButton != null)
            characterTabButton.onClick.RemoveListener(ShowCharacterInfo);
        
        if (skillTabButton != null)
            skillTabButton.onClick.RemoveListener(ShowSkillInfo);
        
        if (closeButton != null)
            closeButton.onClick.RemoveListener(ClosePanel);
        
        if (openButton != null)
            openButton.onClick.RemoveListener(OpenPanel);
    }
}
