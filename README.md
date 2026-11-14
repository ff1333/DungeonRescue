# 勇闯地下城 / Dungeon Rescue

使用 Unity 6 与 C# 开发的 3D 地下城营救迷宫作品。玩家同时携带栓动狙击枪和长刀进入程序化生成的地下设施，在局部雾区、断电、污染和具备感知记忆的怪物压力下寻找被囚禁的公主。普通怪物为骷髅仆从，精英怪为重甲骷髅守卫，最终 Boss 为独立骷髅法师。游戏支持第一/第三人称切换、十关顺序解锁、本地进度保存，以及第十关程序化迷宫后连接固定 Boss 大厅的最终营救战。

**当前正式版为 v1.4.0。** 本版新增默认长刀开局、栓动狙击枪随时切换、5 发弹匣与手动换弹、右键配镜和短时专注减速、自适应武器 HUD、声音感知、视线遮挡、目标记忆、搜索、有限群体警报、Utility AI，以及替代短局昼夜循环的固定地下城环境状态；同时完成怪物近身追击、死亡装备遮挡、贴墙武器后收、武器比例和终点/出生灯光收敛。

**当前开发分支为 v1.5.1 发布候选版。** v1.5.0 的营救主线、三阶段深渊狱长和 Boss Replay 保持不变；本轮把玩家、公主、普通怪、精英怪和 Boss 拆成五套独立奇幻角色，并加入火炬、旗帜、木箱、碎石、宝箱、钥匙和装饰石柱。换模未修改碰撞根节点、对象池、A* 或 Boss 数值。当前 EditMode `32/32`、PlayMode `58/58`，Windows Boss 验收构建为 `0 errors / 0 warnings`。

- [下载 GitHub Release v1.4.0](https://github.com/ff1333/DungeonRescue/releases/tag/v1.4.0)
- [作品集演示视频（v1.3.1 录制）](https://github.com/ff1333/DungeonRescue/releases/download/v1.3.1/MazeEscape-v1.3.1-portfolio-720p.mp4)（中文旁白与字幕，录制时仍使用旧名称，核心流程仍适用）
- [v1.4.0 发布说明](docs/RELEASE_NOTES_v1.4.0.md)
- [v1.4.0 自动验证证据与发布边界](docs/test-results/v1.4.0/README.md)
- [v1.5.0 候选版发布说明](docs/RELEASE_NOTES_v1.5.0.md)
- [v1.5.0 候选版验证边界](docs/test-results/v1.5.0/README.md)
- [v1.5.1 奇幻美术候选版说明](docs/RELEASE_NOTES_v1.5.1.md)
- [v1.5.1 验证记录](docs/test-results/v1.5.1/README.md)
- [v1.3.1 历史发布说明](docs/RELEASE_NOTES_v1.3.1.md)
- [v1.3.0 设置与开发记录](docs/13_V1_3_0_SETTINGS.md)
- [v1.3.0 自动测试、平台构建与验收边界](docs/test-results/v1.3.0/README.md)
- [文档阅读入口](docs/README.md)

![v1.3.1 中文开始界面](docs/test-results/v1.3.1/native/title-zh.png)

![中英文与声音设置](docs/test-results/v1.3.0/native/v1.3.0-settings-en.png)

## 核心玩法

- 十关确定性迷宫，每关使用不同固定种子、尺寸和难度参数
- 十关连续营救叙事，第十关连接固定监牢大厅、三阶段深渊狱长和公主营救结局
- 开始页、十关选择、机制图鉴、默认长刀准备和本地顺序解锁
- Growing Tree 程序化生成，并筛选路线长度、转弯和分岔质量
- 探索小地图记录走过的区域，不直接暴露完整出口路线
- 第一人称与第三人称切换，共用输入、瞄准和伤害逻辑
- 默认同时携带栓动狙击枪与长刀，支持随时切换、单发拉栓、手动换弹、配镜瞄准、备用弹药、命中反馈和遮挡判断
- 骷髅仆从与重甲骷髅守卫通过视觉锥、墙体遮挡、听觉、目标记忆、Utility AI 和网格 A* 行动
- 对象池按关卡敌人上限预热，复用怪物实例
- 后期关卡逐步加入局部雾区、应急红灯、断电、污染、封锁和全图避难灯
- Windows、WebGL、Android 操作与构建支持
- 默认中文，可切换英文；声音开关和分组音量本地保存
- 完成战役后在主菜单解锁 Boss Replay，可直接重战最终首领

## 操作方式

### Windows / WebGL

- `WASD`：移动
- 鼠标：转向和瞄准
- 鼠标左键：长刀攻击；狙击枪单击发射一枪，每枪后自动拉栓
- 鼠标右键：狙击枪配镜；刚开镜 3 秒内游戏速度为 0.5 倍
- `1` / `2` 或鼠标滚轮：切换狙击枪与长刀
- `Shift`：冲刺
- `R`：手动换弹；空弹不会自动装填
- `V`：切换第一/第三人称
- `Esc`：暂停并释放鼠标

### Android

- 左侧浮动摇杆：移动；向上拖过冲刺标记后松手可保持自动冲刺
- 滑动屏幕：转动视角
- 点击配镜：切换开镜/退镜，开镜后可独立点击攻击
- 点击冲刺：锁定或解除冲刺
- 狙击枪、长刀、攻击、装弹和视角按钮：执行对应动作

## 工程设计

### 程序化迷宫

关卡使用不同固定种子生成，确保每关布局不同，同时让同一问题能够复现。Growing Tree 算法先保证逻辑连通，再通过候选筛选控制路线长度、早期选择点、转弯和分岔密度。PlayMode 还会驱动实体角色沿路线移动，用来发现“逻辑可达但碰撞体挡住入口”的问题。

### 寻路与敌人 AI

怪物复用迷宫逻辑网格执行 A*，不需要在每次生成后重新烘焙 NavMesh。`EnemyPerception` 负责视野角、距离、遮挡、声音和目标记忆，`EnemyDecisionModel` 为巡逻、调查、搜索、追击和攻击评分，`EnemyAgent` 只执行最终意图。攻击采用进入/退出距离迟滞，近距离怪物会互相分离，其他怪物也不会被误判为视线墙体。狙击枪声传播 42 米，长刀和冲刺脚步声范围更小；警报只共享可疑位置并受半径和冷却限制。对象复用时重置生命、路径、感知和状态，避免池化污染。

### 相机与战斗

两种视角共享战斗规则。远程攻击先从相机准心确定瞄准点，再从枪口检查真实遮挡，减少第三人称越墙命中。第一人称眼位保持在角色碰撞胶囊内，武器遮挡检测改为跟随相机位置与朝向；靠近正面或侧面墙体时第一人称挂点后收，第三人称同步进入低持姿态。狙击枪为 5 发弹匣、20 发备弹、高伤害单发武器，每枪后必须完成拉栓，换弹只能由玩家明确触发；右键配镜会缩小 FOV，并在每次刚开镜的 3 秒内提供 0.5 倍时间流速。HUD 显示拉栓、装弹和专注状态，角色通过骨骼支撑手 IK 双手持械，未使用武器背在背后。

### 环境难度

短局昼夜循环被移除。每关使用固定的供电、低照明、应急红灯、断电、污染、封锁或地下深层状态。局部雾区通过场景粒子和 HUD 后层雾幕明确表现，并同步降低该区域内怪物视野。污染关卡仍要求玩家在避难灯之间规划路线；灯柱分布不等同于出口指示。

### 本地化与设置

`PortfolioSettings` 管理默认值和 PlayerPrefs，`LocalizedLabel` 负责中英文刷新及中文字体，设置面板提供静音、总音量、音乐与音效音量。静音不会清空原滑块值，重新开启声音后恢复此前音量。

## 当前验证结果

- v1.5.1 EditMode：`32/32 PASS`
- v1.5.1 PlayMode：`58/58 PASS`
- v1.5.1 Windows Release：构建成功，`0 errors / 0 warnings`，ZIP `50,418,195` bytes
- v1.5.1 Android Release：构建成功，`0 errors / 0 warnings`，APK `41,209,806` bytes，`versionName=1.5.1`、`versionCode=13`
- v1.5.1 Boss 直测版：Windows/Android 均构建成功，`0 errors / 0 warnings`；仅开放快速最终战入口，不改变正式版解锁规则
- v1.5.1 Windows 独立 Player：玩家、普通怪、精英怪、Boss、公主、第一人称武器和 Boss 大厅截图均通过视觉检查
- v1.5.1 人工边界：Android 正式 APK 和 Boss 直测 APK 尚未完成本轮真机视觉、性能与触控复核

- v1.5.0 候选版：运行时、EditMode 测试程序集、PlayMode 测试程序集静态编译通过，`0 errors`
- v1.5.0 EditMode：连续三轮 `32/32 PASS`，每轮 0 失败、0 跳过
- v1.5.0 PlayMode：连续三轮 `58/58 PASS`，每轮 0 失败、0 跳过
- v1.5.0 Windows Release：构建成功，`0 errors / 0 warnings`，完整目录 `113,233,622` bytes
- v1.5.0 Android Release：构建成功，`0 errors / 0 warnings`，APK `40,236,166` bytes，`versionName=1.5.0`、`versionCode=12`
- v1.5.0 人工边界：尚未完成“第十关迷宫 -> 深渊狱长 -> 靠近公主结算”的真人完整试玩，也未在 Android 真机安装本次 APK

- v1.4.0 EditMode：`22/22 PASS`
- v1.4.0 PlayMode：`54/54 PASS`
- v1.4.0 Windows Release：构建成功，0 errors，0 warnings
- v1.4.0 Android Release：构建成功，0 errors，0 warnings
- Windows 独立 Player：默认长刀、双手持械与背负武器、配镜画面、拉栓/换弹状态和地下环境截图通过检查
- Android 真机已确认怪物近距离追击、死亡结算遮挡、第一人称侧墙检测和武器比例修正正常；最后一轮仅收敛终点与出生灯光，未再重复真机测试
- WebGL：此前版本曾在 Edge 完成设置、图鉴、选关、移动、攻击、装弹和暂停交互；v1.4.0 本轮未重建 WebGL，不属于本次正式附件
- Android：作者已完成 v1.4.0 候选包真机验收；最后一轮仅修改信号灯参数，未重复进行该 APK 的观感复测

v1.5.1 自动证据与视觉截图见 [v1.5.1 验证记录](docs/test-results/v1.5.1/README.md)，v1.5.0 Boss 功能证据见 [v1.5.0 验证记录](docs/test-results/v1.5.0/README.md)，已发布版本证据见 [v1.4.0 验证记录](docs/test-results/v1.4.0/README.md)。自动测试和平台构建不能替代真人手感、Android 真机、长时间压力或商店审核。

## 项目结构

```text
Assets/
  Art/             CC0 角色、怪物、武器和项目视觉资源
  Editor/          平台构建脚本
  Scenes/          主场景 Main.unity
  Scripts/         生成、寻路、战斗、相机、UI、设置和存档代码
  Tests/           EditMode 与 PlayMode 自动测试
docs/
  devlogs/         各轮需求、修复和验证记录
  images/          历史版本实际运行截图
  test-results/    各版本测试 XML、截图和构建摘要
```

## 本地运行

1. 安装 Unity `6000.3.18f1` 及需要的平台模块。
2. 使用 Unity Hub 打开仓库根目录。
3. 等待资源导入和脚本编译完成。
4. 打开 `Assets/Scenes/Main.unity`。
5. Console 没有红色错误后，点击 Unity 顶部三角形 Play。

正式包位于 GitHub Releases。最新版 Windows ZIP 需要完整解压后运行 `DungeonRescue.exe`；WebGL ZIP 需要 HTTP 静态托管；Android APK 为作品集测试分发包。v1.4.0 及更早历史附件仍保留原文件名。

## 素材与能力边界

- 角色、两类怪物、武器模型、共享色板和角色动画来自 Quaternius Zombie Apocalypse Kit，采用 CC0 1.0；中文字体采用 SIL OFL 1.1。素材包名称按原许可证保留。详情见 [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)。
- 本项目负责素材集成、玩法、生成、寻路、战斗、UI、测试和交付，不将外部模型宣称为本人建模。
- 当前是单机作品集项目，没有联网同步、服务器、热更新和完整商业化系统。
- v1.2.2、v1.2.1、v1.2.0 和 v1.1.0 文档保留用于展示迭代历史，不代表当前版本。

## 历史版本

- [从主菜单到十关迷宫的完整代码与开发解释](docs/PROJECT_DEVELOPMENT_EXPLAINED.md)

- [v1.2.2：避难灯与装弹反馈](docs/12_V1_2_2_LIGHTS_AND_RELOAD.md)
- [v1.2.1：右手武器、命中与遮挡修复](docs/11_V1_2_1_WEAPONS.md)
- [v1.2.0：关卡图鉴与战斗表现](docs/10_V1_2_0_POLISH.md)
- [v1.1.0：开始页、十关选择和动画角色](docs/09_V1_1_0_MENU_AND_CHARACTERS.md)
