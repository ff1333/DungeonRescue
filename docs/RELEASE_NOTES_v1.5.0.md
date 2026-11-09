# Maze Escape v1.5.0 候选版

日期：2026-11-09

状态：功能代码与测试源码已完成，尚未满足正式发布门槛。

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
- Unity Test Runner：未执行，Unity Hub 未登录且 Editor 无有效许可证。
- Windows / Android Release：未构建，不能生成或上传 v1.5.0 正式附件。

正式发布前必须登录 Unity Hub，实际运行三轮 EditMode、三轮 PlayMode，并重建 Windows 与 Android。旧版 `v1.4.0` Release 和标签保持不变。
