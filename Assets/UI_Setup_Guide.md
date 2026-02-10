# UI设置指南

## 已完成的设置

### 1. 生成的UI资产
已使用ComfyUI生成以下UI资产并放置在 `Assets/UI/` 目录：
- `ui_character_panel.png` - 角色信息面板背景
- `ui_skill_panel.png` - 技能面板背景
- `ui_tab_buttons.png` - 标签按钮
- `ui_close_button.png` - 关闭按钮

### 2. 创建的脚本
- `UIManager.cs` - UI管理器，控制面板显示/隐藏和切换
- `CharacterInfoPanel.cs` - 角色信息面板逻辑
- `SkillPanel.cs` - 技能面板逻辑，显示技能冷却

### 3. 场景对象结构
```
Canvas
├── PlayerInfoPanel (主面板)
│   ├── TabButtons (标签按钮容器)
│   │   ├── CharacterTabButton (角色信息标签)
│   │   └── SkillTabButton (技能标签)
│   ├── CharacterInfoContent (角色信息内容)
│   ├── SkillInfoContent (技能信息内容)
│   └── CloseButton (关闭按钮)
└── OpenButton (打开面板按钮)
```

## 需要在Unity编辑器中完成的配置

### 1. 设置面板背景图片
1. 选中 `PlayerInfoPanel` 对象
2. 在Image组件中设置Source Image为 `ui_character_panel.png`
3. 调整RectTransform大小为 500x400

### 2. 配置UIManager组件
选中 `PlayerInfoPanel`，在UIManager组件中设置：
- **Player Info Panel**: 拖拽PlayerInfoPanel自身
- **Character Info Content**: 拖拽CharacterInfoContent
- **Skill Info Content**: 拖拽SkillInfoContent
- **Character Tab Button**: 拖拽CharacterTabButton
- **Skill Tab Button**: 拖拽SkillTabButton
- **Close Button**: 拖拽CloseButton
- **Open Button**: 拖拽OpenButton (在Canvas下)

### 3. 配置标签按钮
- 为 `CharacterTabButton` 和 `SkillTabButton` 设置按钮图片 (ui_tab_buttons.png)
- 在按钮下添加Text子对象显示"角色信息"和"技能"

### 4. 配置关闭按钮
- 为 `CloseButton` 设置图片 (ui_close_button.png)

### 5. 配置打开按钮
- 为 `OpenButton` 设置合适的图标
- 放置在屏幕右上角

### 6. 配置角色信息内容
在 `CharacterInfoContent` 下创建：
- **角色头像**: Image (使用现有的ComfyUI_00128__clean.png)
- **角色名称**: TextMeshPro Text - "海诺"
- **等级**: TextMeshPro Text - "Lv.1"
- **生命值**: Slider + Text
- **经验值**: Slider
- **属性文本**: 攻击力、防御力、暴击率

### 7. 配置技能信息内容
在 `SkillInfoContent` 下创建三个技能槽：

#### 技能1 - 魔法飞弹
- **图标**: Image (可以使用att.png或att2.png)
- **冷却遮罩**: Image (Filled类型，Radial360)
- **名称**: TextMeshPro Text - "魔法飞弹"
- **描述**: TextMeshPro Text - "发射一枚魔法飞弹，对敌人造成伤害"
- **伤害**: TextMeshPro Text - "伤害: 15"
- **冷却信息**: TextMeshPro Text - "冷却: 0.5秒"
- **冷却时间显示**: TextMeshPro Text (用于显示剩余冷却时间)

#### 技能2 - 时间回溯
- **图标**: Image (可以使用现有的UI资产)
- **冷却遮罩**: Image (Filled类型，Radial360)
- **名称**: TextMeshPro Text - "时间回溯"
- **描述**: TextMeshPro Text - "回溯到3秒前的位置和状态"
- **效果**: TextMeshPro Text - "效果: 回溯+时停"
- **冷却信息**: TextMeshPro Text - "冷却: 10秒"

#### 技能3 - 炮台召唤
- **图标**: Image
- **冷却遮罩**: Image (Filled类型，Radial360)
- **名称**: TextMeshPro Text - "炮台召唤"
- **描述**: TextMeshPro Text - "召唤一个自动攻击的魔法炮台"
- **持续时间**: TextMeshPro Text - "持续: 10秒"
- **冷却信息**: TextMeshPro Text - "冷却: 40秒"

### 8. 配置SkillPanel组件引用
选中 `SkillInfoContent`，在SkillPanel组件中设置所有引用字段。

### 9. 配置CharacterInfoPanel组件引用
选中 `CharacterInfoContent`，在CharacterInfoPanel组件中设置：
- Player Health: 拖拽Player对象
- Player Controller: 拖拽Player对象
- 各种UI文本和滑块的引用

## 操作说明

### 键盘快捷键
- **Tab键**: 打开/关闭玩家信息面板
- **ESC键**: 关闭面板

### 鼠标操作
- 点击标签按钮切换"角色信息"和"技能"面板
- 点击关闭按钮关闭面板
- 点击打开按钮显示面板

## 技能冷却系统

技能面板会自动显示三个技能的冷却状态：
1. **技能1** (魔法飞弹): 0.5秒冷却
2. **技能2** (时间回溯): 10秒冷却
3. **技能3** (炮台召唤): 40秒冷却

冷却显示方式：
- 使用圆形填充遮罩显示冷却进度
- 显示剩余冷却时间数字
- 技能就绪时遮罩消失

## 注意事项

1. 确保Player对象有Tag设置为"Player"
2. 确保安装了TextMeshPro包
3. 面板默认隐藏在屏幕外，按Tab键或点击打开按钮显示
4. 所有动画使用Unity原生AnimationCurve，无需DOTween
