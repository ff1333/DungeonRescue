# v1.5.0 Boss 验收包

Android APK 和 Windows ZIP 只用于快速验收第十关最终战，不作为 GitHub Release 正式附件。

本地附件：

- `Builds/Packages/v1.5.0-boss-test/MazeEscape-Windows-v1.5.0-boss-test.zip`，`49,477,936` bytes。
- `Builds/Packages/v1.5.0-boss-test/MazeEscape-Android-v1.5.0-boss-test.apk`，`40,238,978` bytes。
- Windows SHA-256：`4d94786181a9a253fd67edc282bf92981e11b31ed4c0bc52659fdde35f795626`。
- Android SHA-256：`d0dbbf2a6f668b0410dc8c9378d3e72fdba348481caf20dc19df8c96df38d8dd`。

## 与正式包的区别

- 游戏代码、场景、Boss 数值和正式 v1.5.0 相同。
- 第一次启动时，把第 1 至第 9 关写为已完成，因此第 10 关可以直接选择。
- 第 10 关仍为未完成状态，必须真实击败深渊狱长并靠近公主才会写入通关记录。
- 解锁只执行一次，不会在以后启动时把已经完成的第 10 关重置掉。
- 主页面提前显示 `BOSS REPLAY`，点击后可以跳过第十关迷宫，直接进入 Boss 大厅。
- 应用显示名称为 `Maze Escape Boss Test`，包名和签名与本机正式测试包一致，可以覆盖安装并保留设置。

## 自动验证

- Android Boss Test 构建：`Succeeded`，`0 errors / 0 warnings`。
- Windows Boss Test 构建：`Succeeded`，`0 errors / 0 warnings`。
- Windows Player 自动检查：退出码 0。
- 进度断言：`FOGBOUND_BOSS_UNLOCK_PASS level10Unlocked=true level10Completed=false bossReplay=true`。
- 包内版本：`1.5.0`；Android 包名 `com.ff1333.fogboundmaze`，versionCode `12`。
- 辅助功能加入后重新运行 EditMode：`32/32 PASS`。
- 辅助功能加入后重新运行 PlayMode：`58/58 PASS`。

## 使用方法

1. 把 `MazeEscape-v1.5.0-boss-test.apk` 发到 Android 手机。
2. 如果手机允许覆盖安装，直接安装；如果提示版本冲突，先卸载旧测试包再安装。
3. 打开 `Maze Escape Boss Test`。
4. 点击开始并进入选关，第 10 关应当可以直接选择。
5. 要测试完整流程，选择第 10 关，走完迷宫后从出口进入 Boss 大厅。
6. 要直接测试 Boss，在主页面点击 `BOSS REPLAY`，会直接进入 Boss 大厅。
7. 验证三阶段 Boss、召唤、弱点、污染安全灯、牢笼开启和公主营救。

## Windows 使用方法

1. 完整解压 `MazeEscape-Windows-v1.5.0-boss-test.zip`，不要只拿 exe。
2. 双击 `MazeEscapeBossTest.exe`。
3. 点击开始并进入选关，第 10 关应当可以直接选择。
4. 返回主页面点击 `BOSS REPLAY`，可以直接进入 Boss 大厅。
5. 要测试完整链路时，再从第 10 关入口走完迷宫进入大厅。

## 边界

它没有无敌、无限弹药或自动击杀。`BOSS REPLAY` 只用于直接进入最终战，选关入口仍可验证完整第十关。正式发布时使用 `MazeEscape-Android-v1.5.0.apk` 和 `MazeEscape-Windows-v1.5.0.zip`，不会携带 `FOGBOUND_BOSS_TEST` 编译标记；正式包仍需完成战役后才显示 Boss Replay。
