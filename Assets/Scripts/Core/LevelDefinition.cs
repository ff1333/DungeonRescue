using System;
using System.Collections.Generic;
using UnityEngine;

namespace FogboundMaze
{
    [Serializable]
    public sealed class LevelDefinition
    {
        [Min(1)] public int levelNumber;
        [Min(4)] public int width;
        [Min(4)] public int height;
        public int seed;
        [Range(1, 512)] public int mazeCandidateCount;
        [Min(4)] public int targetRouteLength;
        [Range(0f, 1f)] public float routeWeight;
        [Range(0f, 1f)] public float deadEndWeight;
        [Range(0f, 1f)] public float turnWeight;
        [Min(1)] public int maxEnemies;
        [Min(0.5f)] public float spawnInterval;
        [Range(0f, 1f)] public float eliteChance;
        [Range(0f, 0.08f)] public float fogDensity;
        public DungeonCondition environment;
        [Range(0, 12)] public int localFogZones;
        public bool miasma;
        [Min(0f)] public float miasmaDamagePerSecond;
        [Min(2f)] public float safeLightRadius;
        public string storyTitle;
        public string storyObjective;
        public bool bossLevel;

        public void Validate()
        {
            if (levelNumber < 1 || width < 4 || height < 4 || maxEnemies < 1)
            {
                throw new InvalidOperationException("Level dimensions and enemy limits must be positive.");
            }

            if (spawnInterval <= 0f || eliteChance < 0f || eliteChance > 1f)
            {
                throw new InvalidOperationException("Level combat values are outside the supported range.");
            }

            if (miasma && (miasmaDamagePerSecond <= 0f || safeLightRadius < 2f))
            {
                throw new InvalidOperationException("Miasma levels require damage and a usable safe-light radius.");
            }
        }
    }

    public static class LevelCatalog
    {
        public static IReadOnlyList<LevelDefinition> CreateDefault()
        {
            return new[]
            {
                Make(1, 7, 7, 1101, 96, 17, 6, 4.2f, 0.00f, 0.002f, DungeonCondition.Powered, 0, false),
                Make(2, 8, 8, 2203, 128, 21, 7, 3.8f, 0.00f, 0.003f, DungeonCondition.Powered, 0, false),
                Make(3, 9, 9, 3307, 160, 25, 8, 3.5f, 0.00f, 0.005f, DungeonCondition.Dim, 1, false),
                Make(4, 10, 10, 4409, 192, 29, 9, 3.2f, 0.00f, 0.007f, DungeonCondition.Dim, 3, false),
                Make(5, 11, 10, 5513, 240, 32, 10, 2.9f, 0.14f, 0.009f, DungeonCondition.Emergency, 3, false),
                Make(6, 11, 11, 6619, 288, 35, 11, 2.65f, 0.18f, 0.010f, DungeonCondition.Blackout, 4, false),
                Make(7, 12, 11, 7723, 320, 38, 12, 2.4f, 0.22f, 0.012f, DungeonCondition.Contaminated, 4, true),
                Make(8, 13, 12, 8837, 384, 42, 14, 2.2f, 0.26f, 0.014f, DungeonCondition.Contaminated, 6, true),
                Make(9, 14, 13, 9941, 448, 46, 16, 2.0f, 0.31f, 0.016f, DungeonCondition.Lockdown, 6, true),
                Make(10, 15, 14, 10151, 512, 50, 18, 1.8f, 0.38f, 0.018f, DungeonCondition.DeepCore, 8, true)
            };
        }

        private static LevelDefinition Make(
            int number,
            int width,
            int height,
            int seed,
            int mazeCandidateCount,
            int targetRouteLength,
            int maxEnemies,
            float spawnInterval,
            float eliteChance,
            float fogDensity,
            DungeonCondition environment,
            int localFogZones,
            bool miasma)
        {
            var story = StoryFor(number);
            return new LevelDefinition
            {
                levelNumber = number,
                width = width,
                height = height,
                seed = seed,
                mazeCandidateCount = mazeCandidateCount,
                targetRouteLength = targetRouteLength,
                routeWeight = Mathf.Lerp(0.50f, 0.68f, (number - 1) / 9f),
                deadEndWeight = Mathf.Lerp(0.18f, 0.12f, (number - 1) / 9f),
                turnWeight = Mathf.Lerp(0.32f, 0.20f, (number - 1) / 9f),
                maxEnemies = maxEnemies,
                spawnInterval = spawnInterval,
                eliteChance = eliteChance,
                fogDensity = fogDensity,
                environment = environment,
                localFogZones = localFogZones,
                miasma = miasma,
                miasmaDamagePerSecond = miasma ? Mathf.Lerp(4f, 8f, (number - 7) / 3f) : 0f,
                safeLightRadius = miasma ? Mathf.Lerp(7f, 6.25f, (number - 7) / 3f) : 7f,
                storyTitle = story.title,
                storyObjective = story.objective,
                bossLevel = number == 10
            };
        }

        private static (string title, string objective) StoryFor(int level) => level switch
        {
            1 => ("坠入地牢", "穿过入口层，寻找公主留下的第一处痕迹。"),
            2 => ("破碎徽记", "沿着破损的王室徽记继续深入。"),
            3 => ("回声走廊", "追踪牢车经过时留下的金属回声。"),
            4 => ("雾中车辙", "在迷雾中辨认押送队伍的车辙。"),
            5 => ("狱卒警戒", "突破精英狱卒的警戒线。"),
            6 => ("熄灯区", "穿过断电区域，找到通往深层监牢的升降井。"),
            7 => ("污染裂隙", "借助安全灯穿越深渊污染。"),
            8 => ("仪式残响", "追踪仪式能量，确认公主仍被囚禁。"),
            9 => ("最终封锁", "解除监牢前的最后一道封锁。"),
            10 => ("深渊营救", "穿过最终迷宫，击败深渊狱长并营救公主。"),
            _ => ("未知区域", "继续探索地牢。")
        };
    }
}
