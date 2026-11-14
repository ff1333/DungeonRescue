using System;
using UnityEngine;

namespace FogboundMaze
{
    [Serializable]
    public sealed class BossConfig
    {
        public float maximumHealth = 1800f;
        public float phaseTwoThreshold = .70f;
        public float phaseThreeThreshold = .35f;
        public float moveSpeed = 2.8f;
        public float cleaveDamage = 24f;
        public float chargeDamage = 34f;
        public float projectileDamage = 20f;
        public float shockwaveDamage = 28f;
        public float decisionInterval = .45f;
        public int summonCap = 4;

        public static BossConfig Default => new();

        public BossPhase PhaseFor(float normalizedHealth)
        {
            if (normalizedHealth <= 0f) return BossPhase.Defeated;
            if (normalizedHealth < phaseThreeThreshold) return BossPhase.PhaseThree;
            if (normalizedHealth < phaseTwoThreshold) return BossPhase.PhaseTwo;
            return BossPhase.PhaseOne;
        }
    }
}
