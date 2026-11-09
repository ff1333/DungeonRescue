using System.Collections;
using UnityEngine;

namespace FogboundMaze
{
    public sealed class BossEncounterController : MonoBehaviour
    {
        private GameDirector director;
        private EnemyPool pool;
        private MazeWorld world;
        private DungeonBossController boss;
        private GameObject princess;
        private int generation;

        public DungeonBossController Boss => boss;
        public bool IsPrepared => boss != null;
        public bool IsActive => director != null && director.Phase is GamePhase.BossIntro or GamePhase.BossFight or GamePhase.Rescue;

        public void Initialize(GameDirector gameDirector, EnemyPool enemyPool)
        {
            director = gameDirector;
            pool = enemyPool;
        }

        public void Prepare(LevelDefinition level, MazeWorld mazeWorld)
        {
            generation++;
            StopAllCoroutines();
            if (boss != null) Destroy(boss.gameObject);
            if (princess != null) Destroy(princess);
            boss = null;
            princess = null;
            world = mazeWorld;
            if (!level.bossLevel) return;

            boss = DungeonBossController.Create(transform, this, BossConfig.Default);
            boss.gameObject.SetActive(false);
            CreatePrincess();
            world.SetBossGate(false);
            world.SetBossSafeLights(false);
        }

        public void Cancel()
        {
            generation++;
            StopAllCoroutines();
            if (boss != null) boss.gameObject.SetActive(false);
            if (world != null)
            {
                world.SetBossGate(false);
                world.SetBossSafeLights(false);
            }
        }

        public void Begin(bool teleportPlayer = false)
        {
            if (boss == null || IsActive) return;
            generation++;
            pool.DespawnAll();
            if (teleportPlayer) director.Player.Teleport(world.BossPlayerStart + Vector3.up * .08f);
            director.Player.Health.Heal(55f);
            director.Player.Weapon.RefillForBoss();
            world.SetBossGate(true);
            world.SetBossSafeLights(false);
            boss.Spawn(world.BossSpawnPosition);
            director.EnterBossIntro();
            director.Hud.ShowBossIntro("ABYSS WARDEN", "DEFEAT THE WARDEN, OPEN THE CAGE, RESCUE THE PRINCESS");
            StartCoroutine(BeginAfterIntro(generation));
        }

        private IEnumerator BeginAfterIntro(int expectedGeneration)
        {
            yield return new WaitForSeconds(1.8f);
            if (expectedGeneration != generation || boss == null) yield break;
            director.EnterBossFight();
            boss.ActivateFight();
        }

        private void Update()
        {
            if (director == null || world == null || director.Phase != GamePhase.BossFight || boss == null) return;
            if (boss.Phase == BossPhase.PhaseThree
                && !world.IsWithinBossSafeLight(director.Player.transform.position, 7f))
                director.Player.Health.Damage(7f * Time.deltaTime);
            director.Hud.RefreshBoss(boss.Health.Current, boss.Health.Maximum, boss.Phase);
        }

        public void NotifyPhaseChanged(BossPhase phase)
        {
            world.SetBossSafeLights(phase == BossPhase.PhaseThree);
            director.Hud.ShowBossPhase(phase);
        }

        public void SummonAdds(int count)
        {
            var available = Mathf.Max(0, BossConfig.Default.summonCap - pool.ActiveCount);
            count = Mathf.Min(count, available);
            for (var i = 0; i < count; i++)
            {
                var angle = (i + 1f) / (count + 1f) * Mathf.PI * 2f;
                var position = world.BossArenaCenter + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 8f;
                var elite = i % 2 == 1;
                var expected = generation;
                SpawnTelegraph.Create(position, elite, () =>
                {
                    if (expected == generation && director.Phase == GamePhase.BossFight
                        && pool.ActiveCount < BossConfig.Default.summonCap)
                        pool.Spawn(director.Player, world, position, elite);
                });
            }
        }

        public void BossDefeated()
        {
            if (director.Phase != GamePhase.BossFight) return;
            generation++;
            pool.DespawnAll();
            world.SetBossSafeLights(false);
            world.SetBossGate(false);
            world.OpenPrincessCage();
            director.EnterRescue();
            director.Hud.ShowRescuePrompt("THE CAGE IS OPEN. REACH THE PRINCESS.");
        }

        public void CompleteRescue()
        {
            if (director.Phase == GamePhase.Rescue) director.CompleteLevel();
        }

        private void CreatePrincess()
        {
            princess = new GameObject("Princess Rescue");
            princess.transform.SetParent(transform, false);
            princess.transform.position = world.PrincessPosition;
            var model = Instantiate(Resources.Load<GameObject>("Characters/Survivor"), princess.transform);
            model.name = "Princess Visual";
            model.transform.localScale = Vector3.one * .88f;
            foreach (var renderer in model.GetComponentsInChildren<Renderer>())
            {
                var properties = new MaterialPropertyBlock();
                properties.SetColor("_BaseColor", new Color(1f, .72f, .86f));
                properties.SetColor("_Color", new Color(1f, .72f, .86f));
                renderer.SetPropertyBlock(properties);
            }
            foreach (var collider in model.GetComponentsInChildren<Collider>()) collider.enabled = false;
            var trigger = princess.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 1.5f;
            var body = princess.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            princess.AddComponent<PrincessRescueController>().Initialize(this);
        }
    }

    public sealed class PrincessRescueController : MonoBehaviour
    {
        private BossEncounterController encounter;
        public void Initialize(BossEncounterController value) => encounter = value;
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>() != null) encounter?.CompleteRescue();
        }
    }
}
