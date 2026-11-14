# Dungeon Rescue / 勇闯地下城文档入口

当前 `v1.5.1` 深渊营救、最终 Boss 与奇幻美术统一发布候选说明：

- `RELEASE_NOTES_v1.5.1.md`：五套角色拆分、地下城道具和当前验证结果；
- `devlogs/23-v1.5.1-fantasy-art-pass.md`：资源决策、换模接入问题和修复记录；
- `test-results/v1.5.1/README.md`：EditMode、PlayMode、平台构建与实际 Player 截图；

- `RELEASE_NOTES_v1.5.0.md`：候选版范围、玩法、自动验证结果与尚未完成的人工门槛；
- `devlogs/22-v1.5.0-princess-rescue-boss.md`：需求、架构、实现与调研记录；
- `test-results/v1.5.0/README.md`：三轮 EditMode、三轮 PlayMode、Windows/Android 构建证据与人工边界。

**当前公开正式版为 v1.4.0，v1.5.1 尚未合并或发布。** v1.5.1 继承 v1.5.0 最终战，并完成玩家、公主、怪物、Boss 与场景道具的统一奇幻美术迭代。

- [v1.3.0 Release 下载](https://github.com/ff1333/DungeonRescue/releases/tag/v1.3.0)
- [v1.3.0 自动测试、平台构建和真机验收边界](test-results/v1.3.0/README.md)
- [v1.2.2 避难灯和装弹反馈](12_V1_2_2_LIGHTS_AND_RELOAD.md)（历史）
- [v1.2.1 右手武器与命中反馈](11_V1_2_1_WEAPONS.md)（历史）
- [v1.2.0 图鉴和表现优化](10_V1_2_0_POLISH.md)（历史）

历史版本 **v1.1.0** 见 [09_V1_1_0_MENU_AND_CHARACTERS.md](09_V1_1_0_MENU_AND_CHARACTERS.md)：开始界面、十关选择、本地通关记录、新角色，以及当时的复测和发布步骤。
当时的开发日志见 [devlogs/07-campaign-menu-and-character-art.md](devlogs/07-campaign-menu-and-character-art.md)。
`08_V1_0_3_HUD_AND_BRANCHES.md` 是上一版血条、死亡显示和迷宫分岔的历史记录。
上一版入口/防掉落的历史记录见 `07_V1_0_2_ENTRY_FIX.md`，本次继续保留并通过实体通路回归。

建议按下面顺序阅读：

1. `00_GAME_DESIGN.md`：玩法目标、十关范围和难度解锁。
2. `01_ARCHITECTURE.md`：代码模块、运行顺序和关键取舍。
3. `02_CAMPAIGN_AND_BALANCE.md`：每关迷宫与压力参数。
4. `03_TESTING.md`：自动测试、实际 Player 烟测和人工测试边界。
5. `13_V1_3_0_SETTINGS.md` 和 `test-results/v1.3.0/README.md`：当前版本实现、验证和下载边界；`04_BUILD_AND_RELEASE.md` 仅保留首次发布的历史流程。
6. `05_INTERVIEW_TALK.md`：面试时如何用 3-5 分钟讲清项目。
7. `devlogs/`：从算法基线到发布版本的开发留痕。
8. `devlogs/04-first-playtest-bugfix.md`：首次试玩反馈、原因、修复和复测证据。

`04_BUILD_AND_RELEASE.md` 保留 `v1.0.0` 的首次发布流程；首次试玩补丁的交付状态和
步骤见 `06_V1_0_1_PATCH_RELEASE.md`。学习时先运行项目，再按架构文档逐个定位脚本。
