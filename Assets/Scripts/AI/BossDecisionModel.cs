using System;
using System.Collections.Generic;
using UnityEngine;

namespace FogboundMaze
{
    public readonly struct BossDecisionContext
    {
        public readonly BossPhase Phase;
        public readonly float Distance;
        public readonly int ActiveSummons;
        public readonly int SummonCap;
        public readonly BossActionType PreviousAction;
        public readonly Func<BossActionType, bool> IsReady;

        public BossDecisionContext(BossPhase phase, float distance, int activeSummons, int summonCap,
            BossActionType previousAction, Func<BossActionType, bool> isReady)
        {
            Phase = phase;
            Distance = distance;
            ActiveSummons = activeSummons;
            SummonCap = summonCap;
            PreviousAction = previousAction;
            IsReady = isReady;
        }
    }

    public static class BossDecisionModel
    {
        public static BossActionType Choose(BossDecisionContext context)
        {
            if (context.Phase is not (BossPhase.PhaseOne or BossPhase.PhaseTwo or BossPhase.PhaseThree))
                return BossActionType.None;

            var candidates = new List<BossActionType>
            {
                BossActionType.Cleave,
                BossActionType.Charge,
                BossActionType.CorruptionShot
            };
            if (context.Phase is BossPhase.PhaseTwo or BossPhase.PhaseThree)
            {
                candidates.Add(BossActionType.Summon);
                candidates.Add(BossActionType.Shockwave);
            }

            var best = BossActionType.None;
            var bestScore = float.NegativeInfinity;
            foreach (var action in candidates)
            {
                if (context.IsReady != null && !context.IsReady(action)) continue;
                if (action == BossActionType.Summon && context.ActiveSummons >= context.SummonCap) continue;
                var score = Score(action, context);
                if (action == context.PreviousAction) score -= 3.5f;
                if (score > bestScore)
                {
                    bestScore = score;
                    best = action;
                }
            }
            return best;
        }

        public static float Score(BossActionType action, BossDecisionContext context) => action switch
        {
            BossActionType.Cleave => Mathf.Lerp(8f, 0f, Mathf.InverseLerp(2f, 7f, context.Distance)),
            BossActionType.Charge => 5.5f + Mathf.Clamp01((context.Distance - 5f) / 12f) * 3f,
            BossActionType.CorruptionShot => 4f + Mathf.Clamp01((context.Distance - 4f) / 10f) * 4f,
            BossActionType.Summon => 7f - context.ActiveSummons * 1.5f,
            BossActionType.Shockwave => 6.5f - Mathf.Abs(context.Distance - 6f) * .35f,
            _ => float.NegativeInfinity
        };
    }
}
