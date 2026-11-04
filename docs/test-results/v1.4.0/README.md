# Maze Escape v1.4.0 候选版验证记录

首次验证：2026-10-28。移动 HUD 回归：2026-10-31。移动操作与怪物群体稳定性回归：2026-11-01。近距离攻击与刷怪增压回归：2026-11-03。死亡结算装备遮挡回归：2026-11-04。

状态：功能分支发布候选。以下记录证明自动规则、Windows 构建和基础画面通过，不代表真人手感、多平台兼容或正式发布完成。

## 自动测试

- EditMode：`22/22` 通过，0 失败。
- PlayMode：`51/51` 通过，0 失败。
- 覆盖迷宫连通与复杂度、A*、Utility AI 评分、攻击距离迟滞、近身全向感知、静止玩家追击攻击、视觉和遮挡、同伴不遮挡视线、近距离群体分离、声音调查、有限警报、感知记忆重置、默认长刀、狙击枪单发与拉栓、手动换弹、配镜减速、手持/背负互斥、死亡装备隐藏与重开恢复、环境配置、移动端浮动摇杆、点击开镜、冲刺锁定、自动冲刺与按钮布局、原有主循环回归。

## 平台构建

- Unity：`6000.3.18f1`。
- Windows Release：成功，`0 errors / 0 warnings`，构建报告大小 `113,203,942` bytes。
- Android Release：成功，`0 errors / 0 warnings`，APK `40,199,546` bytes。
- APK SHA-256：`89b10adf201cf9ff4f0044ba1d2b3f4f2047df5705903dc7db6b539d73e63972`。

## 运行时冒烟

- 独立 Player 退出码：`0`。
- 版本检查：`FOGBOUND_VERSION_PASS 1.4.0`。
- 运行时检查：`FOGBOUND_RUNTIME_SMOKE_PASS`。
- 默认长刀、狙击枪第三人称、配镜和手动换弹截图成功；移动 HUD 截图确认武器状态与操作按钮分区，冲刺键不遮挡角色。

## 证据文件

- `EditMode.xml`
- `PlayMode.xml`
- `Windows-build-summary.txt`
- `Android-build-summary.txt`
- `native/default-blade.png`
- `native/sniper-third-person.png`
- `native/sniper-scope.png`
- `native/sniper-reload.png`
- `mobile-hud-gameplay.png`

## 尚未完成

- 作者已确认怪物近距离追击正常且暂未再出现鬼畜；死亡结算装备遮挡代码、自动测试和新 APK 已完成，仍需覆盖安装确认结算画面。
- WebGL 构建与浏览器交互验证。
- PR 合并、`v1.4.0` 标签和 GitHub Release。

发布前必须把最新 APK 的多点触控和不同手机宽高比结果追加到本目录。不能把 Windows 强制移动 HUD 截图写成 Android 真机通过。
