# 壁挂火炬贴墙摆放

日期：2026-11-16
状态：作者确认 Windows Development 试玩无问题；修复已合并推送 `main`，尚未更新正式 Release 或 APK

代码提交：`ea7ed25`（本地分支 `fix/solid-dungeon-props`）
合并提交：`89a7344`（`origin/main`）

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
- 作者随后确认当前试玩没有问题；该确认针对本次 Windows 候选包，不代表已逐关验证或完成 Android 真机回归。GitHub 正式 Release 和 Android APK 尚未更新。
