using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FogboundMaze.Tests
{
    public sealed class BossEncounterTests
    {
        private int? savedProgress;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            savedProgress = PlayerPrefs.HasKey(CampaignProgress.CompletionKey)
                ? PlayerPrefs.GetInt(CampaignProgress.CompletionKey) : null;
            PlayerPrefs.SetInt(CampaignProgress.CompletionKey, (1 << 9) - 1);
            SceneManager.LoadScene("Main");
            yield return null;
            yield return null;
            var game = GameDirector.Instance;
            game.ShowLevelSelection();
            Assert.That(game.SelectLevel(10), Is.True);
            game.BeginRun();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Time.timeScale = 1f;
            if (savedProgress.HasValue) PlayerPrefs.SetInt(CampaignProgress.CompletionKey, savedProgress.Value);
            else PlayerPrefs.DeleteKey(CampaignProgress.CompletionKey);
            PlayerPrefs.Save();
            yield return null;
        }

        [UnityTest]
        public IEnumerator LevelTenExit_StartsBossAndDoesNotCompleteCampaign()
        {
            var game = GameDirector.Instance;
            game.ReachExit();
            Assert.That(game.Phase, Is.EqualTo(GamePhase.BossIntro));
            Assert.That(game.Progress.IsCompleted(10), Is.False);
            Assert.That(game.World.BossGate.activeSelf, Is.True);
            Assert.That(game.BossEncounter.Boss.CanAttack, Is.False);
            var before = game.BossEncounter.Boss.Health.Current;
            game.BossEncounter.Boss.TakeDamage(500f, false);
            Assert.That(game.BossEncounter.Boss.Health.Current, Is.EqualTo(before));
            yield return new WaitForSeconds(1.9f);
            Assert.That(game.Phase, Is.EqualTo(GamePhase.BossFight));
        }

        [UnityTest]
        public IEnumerator BossDeathRequiresPrincessRescueBeforeCompletion()
        {
            var game = GameDirector.Instance;
            game.ReachExit();
            yield return new WaitForSeconds(1.9f);
            game.BossEncounter.Boss.TakeDamage(10000f, false);
            yield return null;
            Assert.That(game.Phase, Is.EqualTo(GamePhase.Rescue));
            Assert.That(game.Progress.IsCompleted(10), Is.False);
            Assert.That(game.World.BossGate.activeSelf, Is.False);

            game.Player.Teleport(game.World.PrincessPosition + Vector3.up * .08f);
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            Assert.That(game.Phase, Is.EqualTo(GamePhase.Won));
            Assert.That(game.Progress.IsCompleted(10), Is.True);
        }

        [UnityTest]
        public IEnumerator BossSummonsAreCappedAndRetryResetsEncounter()
        {
            var game = GameDirector.Instance;
            game.ReachExit();
            yield return new WaitForSeconds(1.9f);
            game.BossEncounter.SummonAdds(12);
            yield return new WaitForSeconds(1.7f);
            Assert.That(game.ActiveEnemyCount, Is.LessThanOrEqualTo(BossConfig.Default.summonCap));
            game.Retry();
            yield return null;
            Assert.That(game.Phase, Is.EqualTo(GamePhase.Staging));
            Assert.That(game.ActiveEnemyCount, Is.Zero);
            Assert.That(game.World.BossGate.activeSelf, Is.False);
            Assert.That(game.BossEncounter.Boss.Phase, Is.EqualTo(BossPhase.Dormant));
        }

        [UnityTest]
        public IEnumerator BladeAttackDamagesBossThroughWeaponController()
        {
            var game = GameDirector.Instance;
            game.ReachExit();
            yield return new WaitForSeconds(1.9f);
            var boss = game.BossEncounter.Boss;
            var forward = game.CameraRig.Camera.transform.forward;
            forward.y = 0f;
            forward.Normalize();
            game.Player.Teleport(boss.transform.position - forward * 2.2f + Vector3.up * .08f);
            game.Player.Weapon.Equip(WeaponType.LongBlade);
            Physics.SyncTransforms();
            var before = boss.Health.Current;
            game.Player.Weapon.Tick(true, true, false);
            yield return new WaitForSeconds(.22f);
            Assert.That(boss.Health.Current, Is.LessThan(before));
        }
    }
}
