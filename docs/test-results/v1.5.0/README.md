# Maze Escape v1.5.0 候选版验证记录

日期：2026-11-09

## 已完成

- `FogboundMaze.Runtime.csproj`：`dotnet build` 成功，0 errors。
- `FogboundMaze.EditModeTests.csproj`：`dotnet build` 成功，0 errors。
- `FogboundMaze.PlayModeTests.csproj`：`dotnet build` 成功，0 errors。
- `git diff --check`：通过。
- 本地化 JSON 可解析；本轮没有新增重复英文键。
- 三轮静态检查摘要见 `static-compile.txt`。

## 新增测试源码

- EditMode：剧情元数据、Boss 关标记、阶段阈值、冷却、防重复、召唤上限、非战斗阶段禁用决策。
- PlayMode：第十关出口只进入 Boss、Intro 不可受伤、Boss 死亡后等待营救、营救写入进度、召唤上限、重试清理、武器伤害数值。
- 旧回归：前九关出口仍直接完成；原十关迷宫、移动、相机、AI、武器、HUD 与存档测试源码继续保留。

## 未完成及原因

Unity `6000.3.18f1` 批处理返回：`No valid Unity Editor license found. Please activate your license.` 图形编辑器同样无法进入工程，Unity Hub 停留在 Sign in。因此没有生成 v1.5.0 EditMode/PlayMode XML，没有运行物理与生命周期测试，也没有构建 Windows/Android。

正式发布前必须：登录并激活 Unity Personal；连续运行三轮 EditMode 和三轮 PlayMode；保存 XML；重建 Windows/Android；检查 BuildReport；进行 Windows 最终 Boss 流程试玩和 Android 基本操作回归。旧版 v1.4.0 证据不用于替代本轮测试。
