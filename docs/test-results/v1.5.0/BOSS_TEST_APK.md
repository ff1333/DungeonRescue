# v1.5.0 Boss 验收包

Android APK 和 Windows ZIP 只用于快速验收第十关最终战，不作为 GitHub Release 正式附件。

本地附件：

- `Builds/Packages/v1.5.0-boss-test/MazeEscape-Windows-v1.5.0-boss-test.zip`，`49,477,906` bytes。
- `Builds/Packages/v1.5.0-boss-test/MazeEscape-Android-v1.5.0-boss-test.apk`，`40,236,854` bytes。
- Windows SHA-256：`cdc456fa0f6accc43b5ce4262bab19848db86ae51241a10f9a5f5354b19646c6`。
- Android SHA-256：`b8a8d2d53a8bbcf6053c16f43dafcb54bdc9fa9e867963b3feda9122f17f2d0e`。

## 与正式包的区别

- 游戏代码、场景、Boss 数值和正式 v1.5.0 相同。
- 第一次启动时，把第 1 至第 9 关写为已完成，因此第 10 关可以直接选择。
- 第 10 关仍为未完成状态，必须真实击败深渊狱长并靠近公主才会写入通关记录。
- 解锁只执行一次，不会在以后启动时把已经完成的第 10 关重置掉。
- 应用显示名称为 `Maze Escape Boss Test`，包名和签名与本机正式测试包一致，可以覆盖安装并保留设置。

## 自动验证

- Android Boss Test 构建：`Succeeded`，`0 errors / 0 warnings`。
- Windows Boss Test 构建：`Succeeded`，`0 errors / 0 warnings`。
- Windows Player 自动检查：退出码 0。
- 进度断言：`FOGBOUND_BOSS_UNLOCK_PASS level10Unlocked=true level10Completed=false`。
- 包内版本：`1.5.0`；Android 包名 `com.ff1333.fogboundmaze`，versionCode `12`。
- 辅助功能加入后重新运行 EditMode：`32/32 PASS`。
- 辅助功能加入后重新运行 PlayMode：`58/58 PASS`。

## 使用方法

1. 把 `MazeEscape-v1.5.0-boss-test.apk` 发到 Android 手机。
2. 如果手机允许覆盖安装，直接安装；如果提示版本冲突，先卸载旧测试包再安装。
3. 打开 `Maze Escape Boss Test`。
4. 点击开始并进入选关，第 10 关应当可以直接选择。
5. 进入第 10 关后仍需先走完本关迷宫，再从出口进入 Boss 大厅。
6. 验证三阶段 Boss、召唤、弱点、污染安全灯、牢笼开启、公主营救和 Boss Replay。

## Windows 使用方法

1. 完整解压 `MazeEscape-Windows-v1.5.0-boss-test.zip`，不要只拿 exe。
2. 双击 `MazeEscapeBossTest.exe`。
3. 点击开始并进入选关，第 10 关应当可以直接选择。
4. 其余流程和 Android 相同：走完第十关迷宫，再进入 Boss 大厅完成营救。

## 边界

它不是跳过第十关迷宫或无敌作弊包，只省略前九关的重复通关时间。正式发布时仍使用 `MazeEscape-Android-v1.5.0.apk` 和 `MazeEscape-Windows-v1.5.0.zip`，不会携带 `FOGBOUND_BOSS_TEST` 编译标记。
