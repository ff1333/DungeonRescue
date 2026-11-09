using NUnit.Framework;

namespace FogboundMaze.Tests
{
    public sealed class BossSystemTests
    {
        [Test]
        public void Campaign_HasContinuousStoryAndOnlyLevelTenIsBossLevel()
        {
            var levels = LevelCatalog.CreateDefault();
            for (var i = 0; i < levels.Count; i++)
            {
                Assert.That(levels[i].storyTitle, Is.Not.Empty);
                Assert.That(levels[i].storyObjective, Is.Not.Empty);
                Assert.That(levels[i].bossLevel, Is.EqualTo(i == 9));
            }
        }

        [TestCase(1f, BossPhase.PhaseOne)]
        [TestCase(.70f, BossPhase.PhaseOne)]
        [TestCase(.699f, BossPhase.PhaseTwo)]
        [TestCase(.35f, BossPhase.PhaseTwo)]
        [TestCase(.349f, BossPhase.PhaseThree)]
        [TestCase(0f, BossPhase.Defeated)]
        public void BossConfig_MapsHealthToExpectedPhase(float health, BossPhase expected)
            => Assert.That(BossConfig.Default.PhaseFor(health), Is.EqualTo(expected));

        [Test]
        public void DecisionModel_RespectsCooldownAndDoesNotRepeatWhenAlternativeExists()
        {
            var chosen = BossDecisionModel.Choose(new BossDecisionContext(
                BossPhase.PhaseOne, 2.5f, 0, 4, BossActionType.Cleave,
                action => action != BossActionType.Charge));
            Assert.That(chosen, Is.EqualTo(BossActionType.CorruptionShot));
        }

        [Test]
        public void DecisionModel_DoesNotSummonBeyondCap()
        {
            var chosen = BossDecisionModel.Choose(new BossDecisionContext(
                BossPhase.PhaseTwo, 6f, 4, 4, BossActionType.Shockwave,
                action => action is BossActionType.Summon or BossActionType.Cleave));
            Assert.That(chosen, Is.EqualTo(BossActionType.Cleave));
        }

        [Test]
        public void DecisionModel_DisablesActionsOutsideCombatPhases()
        {
            foreach (var phase in new[] { BossPhase.Dormant, BossPhase.Intro, BossPhase.Stunned, BossPhase.Defeated })
                Assert.That(BossDecisionModel.Choose(new BossDecisionContext(
                    phase, 5f, 0, 4, BossActionType.None, _ => true)), Is.EqualTo(BossActionType.None));
        }
    }
}
