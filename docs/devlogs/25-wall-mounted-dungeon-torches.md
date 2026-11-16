# 壁挂火炬贴墙摆放

日期：2026-11-16
状态：Windows Development 候选已构建，待作者试玩确认，尚未发布

代码提交：`ea7ed25`（本地分支 `fix/solid-dungeon-props`）

## 试玩反馈与根因

作者在开发包里看到火炬悬在通道中央上方，附近没有墙可以挂。`MazeWorld.CreateDungeonLights()` 原来随机选迷宫格后直接把 `torch_mounted` 放在格子中心，没有读取该格的墙边信息。

## 修复

- 从候选格的封闭边选墙；四面开放则跳过该格。
- 火炬贴在墙内侧，面向通道，点光源跟随火炬位置。
- 火炬继续无碰撞；落地碎石、木箱、木桶、桌子和宝箱的尺寸与碰撞保持不变。

## 验证与边界

- EditMode `32/32`、PlayMode `58/58`。十关实体通路测试新增火炬封闭墙面、位置、朝向、实体墙射线和无碰撞检查；解法路线及全部开放通道仍可走。
- Windows Development 构建 `0 errors / 0 warnings`，入口：`Builds/Windows/Development/DungeonRescue.exe`。
- 本地可见窗口打开首关入口，看到前方火炬贴在尽头墙面，不再悬于通道中央；只覆盖此处视角，不代表全部关卡的视觉验收。
- 作者仍需在第一/第三人称下检查火炬是否自然贴墙、通道和落地障碍是否正常。GitHub 正式 Release 和 Android APK 尚未更新。
