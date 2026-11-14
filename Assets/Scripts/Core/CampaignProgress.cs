using UnityEngine;

namespace FogboundMaze
{
    public sealed class CampaignProgress
    {
        public const string CompletionKey = "Fogbound.CompletedLevels";
#if FOGBOUND_BOSS_TEST && !UNITY_EDITOR
        private const string BossTestUnlockSuffix = ".BossTestUnlockApplied";
#endif
        private readonly string key;
        private readonly bool useLegacy;

        public CampaignProgress(bool isolated = false)
        {
            key = isolated ? "Fogbound.Smoke.CompletedLevels" : CompletionKey;
            useLegacy = !isolated;
#if FOGBOUND_BOSS_TEST && !UNITY_EDITOR
            var bossTestUnlockKey = key + BossTestUnlockSuffix;
            if (PlayerPrefs.GetInt(bossTestUnlockKey, 0) == 0)
            {
                var completedMask = PlayerPrefs.GetInt(key, 0) | ((1 << 9) - 1);
                PlayerPrefs.SetInt(key, completedMask);
                PlayerPrefs.SetInt(bossTestUnlockKey, 1);
                PlayerPrefs.Save();
            }
#endif
        }

        public int CompletedCount
        {
            get
            {
                // Only a continuous prefix counts; a stray selected-level value never unlocks a stage.
                var mask = PlayerPrefs.GetInt(key, 0);
                if (!PlayerPrefs.HasKey(key) && useLegacy)
                    return Mathf.Clamp(PlayerPrefs.GetInt("Fogbound.UnlockedLevel", 1) - 1, 0, 9);
                var count = 0;
                while (count < 10 && (mask & (1 << count)) != 0) count++;
                return count;
            }
        }

        public bool IsUnlocked(int level) => level >= 1 && level <= Mathf.Min(10, CompletedCount + 1);
        public bool IsCompleted(int level) => level >= 1 && level <= CompletedCount;
        public bool CanReplayBoss
        {
            get
            {
#if FOGBOUND_BOSS_TEST && !UNITY_EDITOR
                return IsUnlocked(10);
#else
                return IsCompleted(10);
#endif
            }
        }

        public void Complete(int level)
        {
            if (!IsUnlocked(level)) return;
            var count = Mathf.Max(CompletedCount, level);
            PlayerPrefs.SetInt(key, (1 << count) - 1);
            PlayerPrefs.Save();
        }
    }
}
