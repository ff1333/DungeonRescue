# Maze Escape v1.5.0 候选版

日期：2026-11-12

状态：自动测试与 Windows/Android 构建已通过，等待最终人工试玩、合并与公开发布。

## 本次更新

- 十关统一为“进入地下城营救公主”的连续剧情。
- 第十关程序化迷宫出口连接固定监牢大厅，不再直接结算胜利。
- 新增深渊狱长：三阶段状态机、五类招式、冷却、防重复 Utility 选择、弱点窗口和召唤上限。
- 第二阶段加入哥布林/深渊蛮兵增援，第三阶段启用污染与三处安全灯。
- Boss 死亡后打开牢笼；玩家靠近公主后才保存第十关完成状态。
- 战役完成后主菜单和结算页提供 Boss Replay。
- 进入 Boss 战时提供有限生命和弹药补给，避免长迷宫后的无解状态。
- 中英文词条同步增加最终战、阶段、营救与重战首领文案。

## 工程变化

- `BossEncounterController`：管理大厅生命周期、门、补给、召唤、污染、牢笼和营救。
- `DungeonBossController`：执行 Boss 移动、受击、阶段、弱点与招式。
- `BossDecisionModel`：纯评分逻辑，可独立测试距离、冷却、防重复和召唤上限。
- `BossConfig`：集中保存生命、阈值、伤害、速度和召唤上限。
- `MazeWorld`：为第十关追加固定 Boss 大厅、掩体、封印、安全灯与牢笼。
- `GameHud.Boss`：Boss 血条、阶段提示和营救目标。

## 当前验证

- C# Runtime 程序集：编译通过，0 errors。
- EditMode 测试程序集：编译通过，0 errors。
- PlayMode 测试程序集：编译通过，0 errors。
- `git diff --check`：通过。
- Unity EditMode：连续三轮 `32/32 PASS`，每轮 0 失败、0 跳过。
- Unity PlayMode：连续三轮 `58/58 PASS`，每轮 0 失败、0 跳过。
- Windows Release：`Succeeded`，`0 errors / 0 warnings`，完整目录 `113,233,622` bytes。
- Android Release：`Succeeded`，`0 errors / 0 warnings`，APK `40,236,166` bytes。
- APK 元数据：`com.ff1333.fogboundmaze`，`versionName=1.5.0`，`versionCode=12`，minSdk 25，targetSdk 36。
- Windows 正式 Player：第 10 关 gameplay 自动烟测退出码 0，版本检查与 `FOGBOUND_RUNTIME_SMOKE_PASS` 通过。

正式发布前仍需真人完成“第十关迷宫 -> 进入 Boss 房 -> 击败深渊狱长 -> 靠近公主结算 -> Boss Replay”完整流程，并在 Android 真机安装本次 APK 做基本输入回归。旧版 `v1.4.0` Release 和标签保持不变。
