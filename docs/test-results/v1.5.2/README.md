# v1.5.2 验证记录

本补丁只修复大块落地道具穿模与壁挂火炬悬空。自动测试 XML 保存在同目录的 `EditMode.xml` 和 `PlayMode.xml`。

| 检查 | 结果 |
|---|---|
| EditMode | 32/32 PASS |
| PlayMode | 58/58 PASS |
| 发布边界检查 | PASS |
| Windows Release | 0 errors / 0 warnings；主页显示 v1.5.2 |
| WebGL Release | 0 errors / 0 warnings；未做浏览器游玩 |
| Android Release | 0 errors / 0 warnings；v1.5.2 / code 14；未做本轮真机复测 |
| 人工试玩 | 作者已验收修复后的 Windows Development 候选包；不等同于正式包全流程复测 |

`Builds/Packages/v1.5.2/SHA256SUMS.txt` 记录 Windows ZIP、WebGL ZIP 和 APK 的哈希。三份正式附件均由同一 v1.5.2 代码基线构建；Windows ZIP 含 `DungeonRescue.exe`，WebGL ZIP 含 `index.html`。WebGL 需要经 HTTP 静态服务器运行，不能直接双击 `index.html`。旧版测试和附件不应重命名为本补丁的验证证据。
