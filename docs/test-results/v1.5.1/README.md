# Maze Escape v1.5.1 验证记录

日期：2026-11-14

## 自动回归

| 类型 | 结果 | 通过 | 失败 |
|---|---|---:|---:|
| EditMode | Passed | 32 | 0 |
| PlayMode | Passed | 58 | 0 |

PlayMode 额外验证 KayKit 角色材质、动画片段、玩家/公主不同网格、精英怪/Boss 不同网格，以及原有对象池、武器遮挡和最终战流程。

## Windows 视觉证据

- `fantasy-actors.png`：玩家、骷髅仆从和重甲骷髅守卫。
- `fantasy-first-person.png`：第一人称狙击枪与地下城通道。
- `fantasy-boss.png`：独立骷髅法师 Boss、专属法杖、装饰石柱和大厅道具。
- `fantasy-princess.png`：独立公主角色与营救阶段。

截图由独立 Windows Player 生成，不是 Unity Scene 视图。

## 构建边界

版本为 `1.5.1`，Android `versionCode=13`。

| 构建 | 结果 | 文件大小 | SHA-256 |
|---|---|---:|---|
| Windows 正式版 | `0 errors / 0 warnings` | `50,418,195` bytes | `9dd745f31cc46766d3739da0f7dc523095138a37ff44c7da8cd6f558ea24c503` |
| Android 正式版 | `0 errors / 0 warnings` | `41,209,806` bytes | `dc841effa45123d0cb98086260634092167a436ebf67d56f5f8a8ee71fccde8b` |
| Windows Boss 直测版 | `0 errors / 0 warnings` | `50,420,266` bytes | `e4f3ba052cf3f9519763b8849b42e83b6a98c7ab01a6733a42bfe39396517ae4` |
| Android Boss 直测版 | `0 errors / 0 warnings` | `41,210,474` bytes | `d2ca086279e4c038b6c2bea93a039881aa792887b9eb1d31ac9d859cc5fff509` |

正式附件位于 `Builds/Packages/v1.5.1/`，Boss 快速验收附件位于 `Builds/Packages/v1.5.1-boss-test/`。Boss 直测版只提前开放第十关和 Boss Replay，不包含无敌、无限弹药或自动击杀。自动测试、Windows 截图和成功构建不能替代 Android 真机视觉、性能与触控复核。
