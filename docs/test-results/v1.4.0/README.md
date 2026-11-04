# Maze Escape v1.4.0 验证记录

首次验证：2026-10-28。移动 HUD 回归：2026-10-31。移动操作与怪物群体稳定性回归：2026-11-01。近距离攻击与刷怪增压回归：2026-11-03。死亡结算装备遮挡与靠墙收枪回归：2026-11-04。

状态：v1.4.0 最终发布证据。以下记录证明自动规则与 Windows/Android 构建通过；最后一轮仅调整灯光，作者决定不再重复真机测试，因此不能把最新灯光观感写成 Android 真机已验证。

## 自动测试

- EditMode：`22/22` 通过，0 失败。
- PlayMode：`54/54` 通过，0 失败。
- 覆盖迷宫连通与复杂度、A*、Utility AI 评分、攻击距离迟滞、近身全向感知、静止玩家追击攻击、视觉和遮挡、同伴不遮挡视线、近距离群体分离、声音调查、有限警报、感知记忆重置、默认长刀、狙击枪单发与拉栓、手动换弹、配镜减速、手持/背负互斥、死亡装备隐藏与重开恢复、第一人称相机侧墙检测、靠墙收枪与离墙复位、终点/出生灯光范围和阴影、环境配置、移动端浮动摇杆、点击开镜、冲刺锁定、自动冲刺与按钮布局、原有主循环回归。

## 平台构建

- Unity：`6000.3.18f1`。
- Windows Release：成功，`0 errors / 0 warnings`，构建报告大小 `113,205,990` bytes。
- Android Release：成功，`0 errors / 0 warnings`，APK `40,203,338` bytes。
- APK SHA-256：`1a9d19391a0e288f769338af54994d35a2e59f6839e3ac9680a343db0ea27c84`。

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

## 验收边界

- 作者已确认怪物近距离追击、死亡结算遮挡、第一人称侧墙检测和武器比例修正正常。
- 最后一轮只收敛终点与出生灯光；作者选择直接发布，没有对该 APK 再做真机观感复测。
- WebGL 沿用此前浏览器交互验证，本轮未重建。
- Windows 截图和自动测试不能替代 Android 真机结果，文档与简历均不得声称最后一版灯光已真机验收。
