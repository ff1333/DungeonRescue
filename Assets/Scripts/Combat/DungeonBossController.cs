using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogboundMaze
{
    [RequireComponent(typeof(CharacterController), typeof(Health))]
    public sealed class DungeonBossController : MonoBehaviour
    {
        private readonly Dictionary<BossActionType, float> cooldowns = new();
        private BossEncounterController encounter;
        private BossConfig config;
        private CharacterController controller;
        private PlayerController player;
        private BossActionType previousAction;
        private float nextDecision;
        private float weakPointUntil;
        private bool acting;
        private Material telegraphMaterial;
        private Renderer coreRenderer;
        private Animation animationPlayer;
        private Renderer[] visualRenderers;
        private MaterialPropertyBlock phaseProperties;

        public Health Health { get; private set; }
        public BossPhase Phase { get; private set; } = BossPhase.Dormant;
        public bool CanAttack => GameDirector.Instance != null && GameDirector.Instance.Phase == GamePhase.BossFight
            && !acting && Phase is BossPhase.PhaseOne or BossPhase.PhaseTwo or BossPhase.PhaseThree;
        public bool WeakPointExposed => Time.time < weakPointUntil;

        public static DungeonBossController Create(Transform parent, BossEncounterController encounter, BossConfig config)
        {
            var root = new GameObject("Abyss Warden");
            root.transform.SetParent(parent, false);
            var controller = root.AddComponent<CharacterController>();
            controller.height = 3.2f;
            controller.radius = .72f;
            controller.center = Vector3.up * 1.6f;
            root.AddComponent<Health>();
            var boss = root.AddComponent<DungeonBossController>();
            boss.Initialize(encounter, config);
            return boss;
        }

        private void Initialize(BossEncounterController value, BossConfig bossConfig)
        {
            encounter = value;
            config = bossConfig;
            phaseProperties = new MaterialPropertyBlock();
            controller = GetComponent<CharacterController>();
            Health = GetComponent<Health>();
            Health.Changed += OnHealthChanged;
            Health.Died += _ => Defeat();
            var visual = Instantiate(Resources.Load<GameObject>("Characters/AbyssWarden"), transform);
            visual.name = "Abyss Warden Visual";
            visual.transform.localScale = Vector3.one * 1.65f;
            foreach (var childCollider in visual.GetComponentsInChildren<Collider>()) childCollider.enabled = false;
            animationPlayer = visual.GetComponentInChildren<Animation>(true);
            visualRenderers = visual.GetComponentsInChildren<Renderer>(true);
            var weakPoint = new GameObject("Exposed Core");
            weakPoint.transform.SetParent(transform, false);
            weakPoint.transform.localPosition = new Vector3(0f, 1.8f, .68f);
            var weakCollider = weakPoint.AddComponent<SphereCollider>();
            weakCollider.radius = .34f;
            weakCollider.isTrigger = false;
            weakPoint.AddComponent<BossWeakPoint>();
            var core = RuntimeArt.Primitive(PrimitiveType.Sphere, "Core Visual", weakPoint.transform,
                Vector3.zero, Vector3.one * .5f,
                RuntimeArt.Material("Boss Core", new Color(1f, .18f, .52f), .2f, .8f), false);
            core.transform.localScale = Vector3.one * .5f;
            coreRenderer = core.GetComponent<Renderer>();
            telegraphMaterial = RuntimeArt.Material("Boss Telegraph", new Color(1f, .12f, .08f, .65f), .05f, .1f);
        }

        public void Spawn(Vector3 position)
        {
            StopAllCoroutines();
            gameObject.SetActive(true);
            controller.enabled = false;
            transform.SetPositionAndRotation(position + Vector3.up * .05f, Quaternion.LookRotation(Vector3.left));
            controller.enabled = true;
            Health.ResetHealth(config.maximumHealth);
            player = GameDirector.Instance.Player;
            Phase = BossPhase.Intro;
            previousAction = BossActionType.None;
            cooldowns.Clear();
            acting = false;
            weakPointUntil = 0f;
            coreRenderer.enabled = false;
            ApplyPhaseColor(new Color(1f, .82f, .72f));
            Play("Idle", true);
        }

        public void ActivateFight()
        {
            if (Phase != BossPhase.Intro) return;
            SetPhase(BossPhase.PhaseOne);
            nextDecision = Time.time + .5f;
        }

        private void Update()
        {
            if (coreRenderer != null) coreRenderer.enabled = WeakPointExposed;
            if (!CanAttack || player == null) return;
            FacePlayer();
            var distance = HorizontalDistance(transform.position, player.transform.position);
            if (distance > 6.5f && !acting)
            {
                controller.SimpleMove((player.transform.position - transform.position).normalized * config.moveSpeed);
                Play("Run");
            }
            else if (!acting) Play("Idle");
            if (Time.time < nextDecision) return;
            var action = BossDecisionModel.Choose(new BossDecisionContext(
                Phase, distance, GameDirector.Instance.ActiveEnemyCount, config.summonCap,
                previousAction, IsReady));
            if (action != BossActionType.None) StartCoroutine(Execute(action));
            else nextDecision = Time.time + .2f;
        }

        public void TakeDamage(float amount, bool weakPoint)
        {
            if (Phase is BossPhase.Dormant or BossPhase.Intro or BossPhase.Defeated) return;
            Health.Damage(amount * (weakPoint && WeakPointExposed ? 1.8f : 1f));
        }

        private void OnHealthChanged(Health value)
        {
            if (value.IsDead || Phase is BossPhase.Intro or BossPhase.Dormant or BossPhase.Stunned) return;
            var target = config.PhaseFor(value.Current / value.Maximum);
            if (target != Phase && target is BossPhase.PhaseTwo or BossPhase.PhaseThree)
                StartCoroutine(Transition(target));
        }

        private IEnumerator Transition(BossPhase target)
        {
            acting = true;
            Phase = BossPhase.Stunned;
            encounter.NotifyPhaseChanged(Phase);
            weakPointUntil = Time.time + 1.2f;
            Play("Stun", true);
            yield return new WaitForSeconds(1.05f);
            target = config.PhaseFor(Health.Current / Health.Maximum);
            SetPhase(target);
            acting = false;
            nextDecision = Time.time + .35f;
        }

        private IEnumerator Execute(BossActionType action)
        {
            acting = true;
            previousAction = action;
            cooldowns[action] = Time.time + Cooldown(action);
            Play(action switch
            {
                BossActionType.Cleave => "Cleave",
                BossActionType.Charge => "Charge",
                BossActionType.CorruptionShot => "Cast",
                BossActionType.Summon => "Summon",
                BossActionType.Shockwave => "Cast",
                _ => "Idle"
            }, true);
            yield return action switch
            {
                BossActionType.Cleave => Cleave(),
                BossActionType.Charge => Charge(),
                BossActionType.CorruptionShot => CorruptionShot(),
                BossActionType.Summon => Summon(),
                BossActionType.Shockwave => Shockwave(),
                _ => null
            };
            acting = false;
            nextDecision = Time.time + config.decisionInterval * (Phase == BossPhase.PhaseThree ? .72f : 1f);
        }

        private IEnumerator Cleave()
        {
            var warning = CreateWarning("Cleave Warning", transform.position + transform.forward * 2f,
                new Vector3(4.5f, .04f, 4.5f));
            yield return new WaitForSeconds(.58f);
            if (CanResolveAttack() && HorizontalDistance(transform.position, player.transform.position) <= 4.5f
                && Vector3.Dot(transform.forward, (player.transform.position - transform.position).normalized) > .15f)
                player.Health.Damage(config.cleaveDamage);
            Destroy(warning);
        }

        private IEnumerator Charge()
        {
            FacePlayer();
            var direction = transform.forward;
            var warning = CreateWarning("Charge Line", transform.position + direction * 6f,
                new Vector3(2f, .04f, 12f));
            warning.transform.rotation = Quaternion.LookRotation(direction);
            yield return new WaitForSeconds(.75f);
            Destroy(warning);
            var hit = false;
            for (var time = 0f; time < .55f && CanResolveAttack(); time += Time.deltaTime)
            {
                controller.Move(direction * 15f * Time.deltaTime);
                if (!hit && HorizontalDistance(transform.position, player.transform.position) < 1.8f)
                {
                    player.Health.Damage(config.chargeDamage);
                    hit = true;
                }
                yield return null;
            }
            weakPointUntil = Time.time + 1.4f;
        }

        private IEnumerator CorruptionShot()
        {
            yield return new WaitForSeconds(.42f);
            if (CanResolveAttack())
                CorruptionProjectile.Create(transform.position + Vector3.up * 1.7f,
                    player.transform.position + Vector3.up - (transform.position + Vector3.up * 1.7f),
                    config.projectileDamage);
        }

        private IEnumerator Summon()
        {
            yield return new WaitForSeconds(.6f);
            if (CanResolveAttack()) encounter.SummonAdds(Phase == BossPhase.PhaseThree ? 2 : 1);
            weakPointUntil = Time.time + 1.1f;
        }

        private IEnumerator Shockwave()
        {
            var warning = CreateWarning("Shockwave Warning", transform.position,
                new Vector3(9f, .03f, 9f), PrimitiveType.Cylinder);
            yield return new WaitForSeconds(.82f);
            if (CanResolveAttack() && HorizontalDistance(transform.position, player.transform.position) <= 8f)
                player.Health.Damage(config.shockwaveDamage);
            Destroy(warning);
            weakPointUntil = Time.time + 1.25f;
        }

        private GameObject CreateWarning(string name, Vector3 position, Vector3 scale,
            PrimitiveType primitive = PrimitiveType.Cube)
            => RuntimeArt.Primitive(primitive, name, transform.parent, position + Vector3.up * .03f,
                scale, telegraphMaterial, false);

        private bool IsReady(BossActionType action)
            => !cooldowns.TryGetValue(action, out var readyAt) || Time.time >= readyAt;

        private static float Cooldown(BossActionType action) => action switch
        {
            BossActionType.Cleave => 2.1f,
            BossActionType.Charge => 5.2f,
            BossActionType.CorruptionShot => 2.8f,
            BossActionType.Summon => 7f,
            BossActionType.Shockwave => 5.8f,
            _ => 1f
        };

        private bool CanResolveAttack() => GameDirector.Instance != null
            && GameDirector.Instance.Phase == GamePhase.BossFight && !Health.IsDead
            && Phase is BossPhase.PhaseOne or BossPhase.PhaseTwo or BossPhase.PhaseThree;

        private void FacePlayer()
        {
            var delta = player.transform.position - transform.position;
            delta.y = 0f;
            if (delta.sqrMagnitude > .01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(delta), 8f * Time.deltaTime);
        }

        private void Defeat()
        {
            StopAllCoroutines();
            acting = false;
            SetPhase(BossPhase.Defeated);
            controller.enabled = false;
            Play("Death", true);
            encounter.BossDefeated();
        }

        private void SetPhase(BossPhase value)
        {
            Phase = value;
            ApplyPhaseColor(value switch
            {
                BossPhase.PhaseTwo => new Color(.9f, .62f, 1f),
                BossPhase.PhaseThree => new Color(1f, .42f, .5f),
                _ => new Color(1f, .82f, .72f)
            });
            encounter.NotifyPhaseChanged(value);
        }

        private void Play(string clip, bool restart = false)
        {
            if (animationPlayer == null || animationPlayer[clip] == null) return;
            if (restart) animationPlayer.Stop();
            if (!animationPlayer.IsPlaying(clip)) animationPlayer.CrossFade(clip, .1f);
        }

        private void ApplyPhaseColor(Color color)
        {
            if (visualRenderers == null) return;
            phaseProperties.Clear();
            phaseProperties.SetColor("_BaseColor", color);
            phaseProperties.SetColor("_Color", color);
            foreach (var renderer in visualRenderers) renderer.SetPropertyBlock(phaseProperties);
        }

        private static float HorizontalDistance(Vector3 a, Vector3 b)
        {
            a.y = b.y = 0f;
            return Vector3.Distance(a, b);
        }
    }

    public sealed class BossWeakPoint : MonoBehaviour { }

    public sealed class CorruptionProjectile : MonoBehaviour
    {
        private Vector3 direction;
        private float damage;
        private float expires;

        public static void Create(Vector3 position, Vector3 direction, float damage)
        {
            var material = RuntimeArt.Material("Corruption Projectile", new Color(.82f, .12f, 1f), .1f, .8f);
            var projectile = RuntimeArt.Primitive(PrimitiveType.Sphere, "Corruption Projectile", null,
                position, Vector3.one * .42f, material, false).AddComponent<CorruptionProjectile>();
            projectile.direction = direction.normalized;
            projectile.damage = damage;
            projectile.expires = Time.time + 4f;
        }

        private void Update()
        {
            if (GameDirector.Instance != null && GameDirector.Instance.Phase == GamePhase.Paused) return;
            if (GameDirector.Instance == null || GameDirector.Instance.Phase != GamePhase.BossFight)
            {
                Destroy(gameObject);
                return;
            }
            var distance = 11f * Time.deltaTime;
            if (Physics.SphereCast(transform.position, .2f, direction, out var hit, distance,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                var player = hit.collider.GetComponentInParent<PlayerController>();
                if (player != null) player.Health.Damage(damage);
                Destroy(gameObject);
                return;
            }
            transform.position += direction * distance;
            if (Time.time >= expires) Destroy(gameObject);
        }
    }
}
