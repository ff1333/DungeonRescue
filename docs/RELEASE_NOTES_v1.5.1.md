# Maze Escape v1.5.1

日期：2026-11-14

状态：功能回归、Windows 视觉验收与 Windows/Android 构建均已完成。

## 本次更新

- 玩家、公主、普通怪、精英怪和 Boss 使用五套独立奇幻角色，不再依靠同模型缩放和改色区分。
- 普通怪携带短刃，精英怪携带战斧和盾牌，Boss 使用法杖、弱点核心和阶段色变化。
- 公主使用独立法师模型，牢笼打开后播放欢呼动画。
- 长刀改为完整双手剑模型；狙击枪、拉栓、换弹、开镜和遮挡规则保持不变。
- 迷宫新增火炬、蜡烛、木箱、木桶、碎石、旗帜、宝箱、钥匙和装饰石柱。
- 地下城装饰不参与碰撞或 A*，已有玩法、寻路和 Boss 数值保持不变。

## 验证

- EditMode：`32/32 PASS`。
- PlayMode：`58/58 PASS`。
- Windows Boss 验收构建：`0 errors / 0 warnings`。
- Windows 正式版与 Android 正式版：`0 errors / 0 warnings`。
- Android Boss 直测版：`0 errors / 0 warnings`。
- 实际截图已检查普通/精英怪区分、第一人称狙击枪、独立 Boss 和 Boss 大厅道具。
- 第三方资源均为 CC0，许可证与来源已进入仓库。

## 发布附件

- `MazeEscape-Windows-v1.5.1.zip`：Windows 正式版，`50,418,195` bytes。
- `MazeEscape-Android-v1.5.1.apk`：Android 正式版，`41,209,806` bytes。
- `SHA256SUMS.txt`：两份正式附件的 SHA-256 校验值。

Boss 快速验收包位于 `Builds/Packages/v1.5.1-boss-test/`，不应作为正式商店版本发布。Android 包仍需要在目标真机上完成最终触控、性能和画面验收。
