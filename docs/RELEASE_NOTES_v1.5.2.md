# Dungeon Rescue v1.5.2

日期：2026-11-16

类型：v1.5.1 之后的场景碰撞与摆放 bug 修复补丁。历史版本与附件保留，不覆盖旧包。

## 修复内容

- 为碎石、木桶、木箱、桌子和宝箱增加简化实体碰撞，并约束生成位置，避免玩家和怪物穿入道具、视觉上被遮住，同时保持迷宫路线可通行。Boss 大厅石柱沿用原有的独立掩体碰撞。
- 壁挂火炬只生成在可依附的实体墙面上，调整朝向和贴墙距离，避免悬在通道上方或空中。
- 未修改战斗规则、Boss 数值、关卡解锁和存档格式。

## 验证与边界

- EditMode `32/32`、PlayMode `58/58`；十关实体通路、火炬贴墙和通道碰撞检查通过。
- Windows、WebGL、Android Release 构建均为 `0 errors / 0 warnings`；Android `versionName=1.5.2`、`versionCode=14`。
- 作者已试玩包含修复的 Windows Development 候选包并确认问题消失；本轮新 Windows 正式包仅做主页可见窗口检查。
- 本轮 WebGL 未进行浏览器游玩，Android 新 APK 未进行真机复测；构建通过不等于上述人工验收通过。

## 发布附件

- `DungeonRescue-Windows-v1.5.2.zip`：`50,419,545` bytes，SHA-256 `3bf5833406ceda690d5aeabe7c429eb08163a966e5731f7d32a6a21c9c1f1fd4`。
- `DungeonRescue-WebGL-v1.5.2.zip`：`26,944,221` bytes，SHA-256 `95081be21bd84d72f94f7ad08f2dc15075cd2e33e268ceaaa38174b64f83bb3b`。
- `DungeonRescue-v1.5.2.apk`：`41,212,906` bytes，SHA-256 `dfc776eacdfabbf9fe23e14452f695182672f6010f7f81d24aa48543325e9381`。
- `SHA256SUMS.txt`：三份包体的校验清单。

详细记录见 `test-results/v1.5.2/README.md`，历史 v1.5.1 Release 保留供版本对照。
