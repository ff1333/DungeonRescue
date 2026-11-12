# Maze Escape v1.5.0 候选版验证记录

日期：2026-11-12

## 自动测试

Unity `6000.3.18f1` 在已激活的 Unity Personal 环境中串行运行六轮 Test Runner：

| 模式 | 轮次 | 结果 | 通过 | 失败 | 跳过 |
|---|---:|---|---:|---:|---:|
| EditMode | 1 | Passed | 32 | 0 | 0 |
| EditMode | 2 | Passed | 32 | 0 | 0 |
| EditMode | 3 | Passed | 32 | 0 | 0 |
| PlayMode | 1 | Passed | 58 | 0 | 0 |
| PlayMode | 2 | Passed | 58 | 0 | 0 |
| PlayMode | 3 | Passed | 58 | 0 | 0 |

EditMode 包含原有 22 项回归与新增 10 项 Boss/剧情规则测试。PlayMode 包含原有 54 项回归与新增 4 项最终战流程测试。三轮结果分别保存在 `EditMode-round1.xml` 至 `EditMode-round3.xml`、`PlayMode-round1.xml` 至 `PlayMode-round3.xml`。

## 构建结果

- Windows Release：`Succeeded`，`0 errors / 0 warnings`，完整目录 `113,233,622` bytes。
- Android Release：`Succeeded`，`0 errors / 0 warnings`，APK `40,236,166` bytes。
- APK：`com.ff1333.fogboundmaze`，`versionName=1.5.0`，`versionCode=12`，minSdk 25，targetSdk 36。
- Windows 发布 ZIP：`48,426,657` bytes，SHA-256 `460ae81686c3826312fe393e9562f830e4e54e628ecf18cffab446f4ecf784e2`。
- Android 发布 APK：SHA-256 `08f465a8ae4beefe8f89b35a0fe9ba1a466306ef6ff9f5c6f82968d3b6c5d22e`。
- Windows 正式 Player：第 10 关 gameplay 自动烟测退出码 0，版本为 `1.5.0`，日志命中 `FOGBOUND_RUNTIME_SMOKE_PASS`。

构建摘要见 `Windows-build-summary.txt`、`Android-build-summary.txt` 和 `Windows-player-smoke-summary.txt`。本地附件位于 `Builds/Packages/v1.5.0/`，该目录被 `.gitignore` 排除，不进入源码提交。

## 静态检查

- `FogboundMaze.Runtime.csproj`：`dotnet build` 成功，0 errors。
- `FogboundMaze.EditModeTests.csproj`：`dotnet build` 成功，0 errors。
- `FogboundMaze.PlayModeTests.csproj`：`dotnet build` 成功，0 errors。
- `git diff --check`：通过。
- 本地化 JSON 可解析，本轮没有新增重复英文键。
- 静态检查摘要见 `static-compile.txt`。

## 人工验证边界

自动测试已经验证状态流转、Boss 规则、营救存档、重试清理和历史回归，构建结果证明两个目标平台可以产出附件；它们不能替代真人手感和设备兼容性。

公开发布前仍需：

1. Windows 真人完成“第十关迷宫 -> 进入 Boss 房 -> 击败深渊狱长 -> 靠近公主结算 -> Boss Replay”。
2. Android 真机安装本次 v1.5.0 APK，复测移动、视角、攻击、开镜、换弹、切刀、暂停和最终战基本流程。
3. 人工确认 Boss 血条、阶段提示、安全灯、牢笼和公主触发没有明显遮挡或穿模。

旧版 v1.4.0 的人工结果不替代本轮新增 Boss 内容。当前可以准确陈述“六轮自动测试和 Windows/Android 构建通过”，不能陈述“v1.5.0 已真人完整通关或已正式发布”。
