using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FogboundMaze.Tests
{
    public sealed class GameFlowTests
    {
        private readonly System.Collections.Generic.Dictionary<string, int?> savedProgress = new();
        [UnityTest]
        public IEnumerator HealthBar_TracksDamageHealingAndRetryInRenderedWidth()
        {
            var director = GameDirector.Instance;
            director.SelectWeapon(WeaponType.SniperRifle);
            director.Player.Health.Damage(65f);
            yield return null;
            AssertHealthDisplay(director, 35, 0.35f);
            director.Player.Health.Heal(15f);
            yield return null;
            AssertHealthDisplay(director, 50, 0.5f);
            director.Retry();
            yield return null;
            AssertHealthDisplay(director, 100, 1f);
        }

        [Test]
        public void FatalDamage_ImmediatelyDisplaysZeroWithResultPanel()
        {
            var director = GameDirector.Instance;
            director.SelectWeapon(WeaponType.LongBlade);
            director.BeginRun();
            director.Player.Health.Damage(91f);
            director.Hud.Refresh(director, director.Player);
            director.Player.Health.Damage(20f);
            Assert.That(director.Phase, Is.EqualTo(GamePhase.Lost));
            Assert.That(director.Hud.transform.Find("Result").gameObject.activeSelf, Is.True);
            AssertHealthDisplay(director, 0, 0f);
        }

        private static void AssertHealthDisplay(GameDirector director, int current, float fraction)
        {
            Canvas.ForceUpdateCanvases();
            var top = director.Hud.transform.Find("Top Bar");
            Assert.That(top.Find("Health").GetComponent<UnityEngine.UI.Text>().text,
                Is.EqualTo($"HP  {current} / 100"));
            var track = top.Find("Health Track").GetComponent<RectTransform>();
            var fill = track.Find("Health Fill").GetComponent<RectTransform>();
            Assert.That(fill.rect.width / track.rect.width, Is.EqualTo(fraction).Within(0.005f));
        }

        [Test]
        public void MiniMap_ShowsUnvisitedExitMouthWithoutRevealingNeighbor()
        {
            var director = GameDirector.Instance;
            var texture = (Texture2D)director.Hud.transform.Find("Exploration Map/Visited Cells")
                .GetComponent<UnityEngine.UI.RawImage>().texture;
            var start = director.World.Layout.Start;
            var checkedExits = 0;
            foreach (var neighbor in director.World.Layout.GetOpenNeighbors(start))
            {
                var offset = neighbor - start;
                var mouth = new Vector2Int(start.x * 12 + 6, start.y * 12 + 6) + offset * 6;
                Assert.That((Color32)texture.GetPixel(mouth.x, mouth.y), Is.EqualTo(new Color32(211, 164, 80, 255)));
                Assert.That((Color32)texture.GetPixel(neighbor.x * 12 + 6, neighbor.y * 12 + 6),
                    Is.EqualTo(new Color32(13, 22, 25, 255)));
                checkedExits++;
            }
            Assert.That(checkedExits, Is.GreaterThan(0));
            Assert.That(director.Hud.ExploredCells, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator PhysicalMazeRoutes_AreWalkableInAllTenLevels()
        {
            var director = GameDirector.Instance;
            director.enabled = false;
            director.Player.enabled = false;
            var controller = director.Player.GetComponent<CharacterController>();
            foreach (var level in LevelCatalog.CreateDefault())
            {
                director.World.Build(level);
                director.World.OpenStartGate();
                // Primitive decorations remove their colliders with deferred Destroy.
                yield return null;
                Physics.SyncTransforms();
                AssertFloorPropsAreSolid(director.World, level.levelNumber);
                AssertTorchesAreWallMounted(director.World, level);
                PlaceController(controller, director.World.StartPosition);
                var path = MazePathfinder.FindPath(director.World.Layout,
                    director.World.Layout.Start, director.World.Layout.Goal);
                foreach (var cell in path)
                    WalkTo(controller, director.World.CellToWorld(cell), $"Level {level.levelNumber}, cell {cell}");
                WalkTo(controller, director.World.ExitPosition, $"Level {level.levelNumber}, exit");

                // Check every open corridor, including branches outside the solution route.
                for (var x = 0; x < director.World.Layout.Width; x++)
                for (var y = 0; y < director.World.Layout.Height; y++)
                {
                    var cell = new Vector2Int(x, y);
                    foreach (var neighbor in director.World.Layout.GetOpenNeighbors(cell))
                    {
                        PlaceController(controller, director.World.CellToWorld(cell));
                        WalkTo(controller, director.World.CellToWorld(neighbor),
                            $"Level {level.levelNumber}, corridor {cell} -> {neighbor}");
                    }
                }
            }
        }

        private static void AssertFloorPropsAreSolid(MazeWorld world, int levelNumber)
        {
            var generated = world.transform.Find("Generated Maze");
            var checkedCount = 0;
            foreach (Transform child in generated)
            {
                if (!child.name.StartsWith("Dungeon Prop ")) continue;
                var resource = child.name.Substring("Dungeon Prop ".Length);
                var solid = resource is "rubble_large" or "barrel_small_stack" or "crates_stacked"
                    or "table_small_decorated_A" or "chest_gold";
                if (!solid) continue;

                var box = child.GetComponent<BoxCollider>();
                Assert.That(box, Is.Not.Null, $"Level {levelNumber}: {resource} has no prop collider");
                Assert.That(box.enabled && !box.isTrigger, Is.True,
                    $"Level {levelNumber}: {resource} has no solid collider");
                var origin = box.bounds.center + Vector3.up * (box.bounds.extents.y + .2f);
                Assert.That(Physics.Raycast(origin, Vector3.down, out var hit, .4f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) && hit.collider == box,
                    Is.True, $"Level {levelNumber}: {resource} is missing from physics queries");
                var bounds = box.bounds;
                var maximumSize = world.IsInside(child.position) ? 1.41f : 1.81f;
                Assert.That(Mathf.Max(bounds.size.x, bounds.size.z), Is.LessThanOrEqualTo(maximumSize),
                    $"Level {levelNumber}: {resource} blocks too much of a cell; " +
                    $"rootScale={child.localScale}, boxSize={box.size}, boxScale={box.transform.lossyScale}");
                checkedCount++;
            }
            Assert.That(checkedCount, Is.GreaterThan(0), $"Level {levelNumber}: no floor props tested");
        }

        private static void AssertTorchesAreWallMounted(MazeWorld world, LevelDefinition level)
        {
            var generated = world.transform.Find("Generated Maze");
            var torches = 0;
            var lights = 0;
            foreach (Transform child in generated)
            {
                if (child.name == "Dungeon Wall Light") lights++;
                if (child.name != "Dungeon Prop torch_mounted") continue;
                torches++;
                var cell = world.WorldToCell(child.position);
                var center = world.CellToWorld(cell);
                var outward = -child.forward;
                var direction = Mathf.Abs(outward.x) > .5f
                    ? (outward.x > 0f ? MazeDirection.East : MazeDirection.West)
                    : (outward.z > 0f ? MazeDirection.North : MazeDirection.South);
                Assert.That(world.Layout[cell].IsOpen(direction), Is.False,
                    $"Level {level.levelNumber}: torch faces an open passage at {cell}");
                Assert.That(Vector3.Dot(child.position - center, outward),
                    Is.EqualTo(MazeWorld.CellSize * .5f - .16f).Within(.03f),
                    $"Level {level.levelNumber}: torch floats away from wall at {cell}");
                Assert.That(Physics.Raycast(child.position + Vector3.up * .2f, outward,
                    out var hit, .5f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
                    && hit.collider.name == "Wall", Is.True,
                    $"Level {level.levelNumber}: torch has no physical wall behind it at {cell}");
                foreach (var collider in child.GetComponentsInChildren<Collider>(true))
                    Assert.That(collider.enabled, Is.False, "Wall torch must not block the corridor");
            }
            Assert.That(lights, Is.EqualTo(torches), $"Level {level.levelNumber}: wall-light count mismatch");
            if (level.environment != DungeonCondition.Blackout && level.environment != DungeonCondition.DeepCore)
                Assert.That(torches, Is.GreaterThan(0), $"Level {level.levelNumber}: no wall torches generated");
        }

        [Test]
        public void StagingAndExitRooms_HaveSolidPerimeters()
        {
            var director = GameDirector.Instance;
            director.Player.enabled = false;
            director.World.OpenStartGate();
            var controller = director.Player.GetComponent<CharacterController>();
            Physics.SyncTransforms();
            foreach (var side in new[] { Vector3.left, Vector3.right, Vector3.back })
            {
                PlaceController(controller, Vector3.back * MazeWorld.CellSize);
                for (var i = 0; i < 80; i++) controller.Move(side * 0.2f + Vector3.down * 0.03f);
                Assert.That(Mathf.Abs(controller.transform.position.x), Is.LessThan(2.5f), "Staging side escape");
                Assert.That(controller.transform.position.z, Is.GreaterThan(-7.5f), "Staging rear escape");
                Assert.That(controller.transform.position.y, Is.GreaterThan(-0.2f), "Staging fall");
                // Reproduce walking beside the gate toward the former dark, unguarded edge.
                for (var i = 0; i < 80; i++) controller.Move(Vector3.forward * 0.2f + Vector3.down * 0.03f);
                Assert.That(controller.transform.position.y, Is.GreaterThan(-0.2f), "Fall beside entry");
            }
            foreach (var side in new[] { Vector3.forward, Vector3.back, Vector3.right })
            {
                PlaceController(controller, director.World.ExitPosition);
                for (var i = 0; i < 80; i++) controller.Move(side * 0.2f + Vector3.down * 0.03f);
                var delta = controller.transform.position - director.World.ExitPosition;
                Assert.That(Mathf.Abs(delta.z), Is.LessThan(2.5f), "Exit side escape");
                Assert.That(delta.x, Is.LessThan(2.5f), "Exit rear escape");
                Assert.That(controller.transform.position.y, Is.GreaterThan(-0.2f), "Exit fall");
            }
        }

        private static void PlaceController(CharacterController controller, Vector3 ground)
        {
            controller.enabled = false;
            controller.transform.position = ground + Vector3.up * 0.1f;
            controller.enabled = true;
            Physics.SyncTransforms();
        }

        [UnityTest]
        public IEnumerator UnexpectedFall_RecoversAndRetryResetsGravity()
        {
            var director = GameDirector.Instance;
            director.SelectWeapon(WeaponType.SniperRifle);
            yield return new WaitForSeconds(0.4f);
            var controller = director.Player.GetComponent<CharacterController>();
            var grounded = director.Player.transform.position;
            PlaceController(controller, new Vector3(-20f, -10f, -20f));
            yield return null;
            yield return null;
            Assert.That(Vector3.Distance(director.Player.transform.position, grounded), Is.LessThan(0.3f));
            director.Retry();
            yield return new WaitForSeconds(0.4f);
            Assert.That(director.Player.transform.position.y, Is.GreaterThan(-0.2f));
            Assert.That(director.Phase, Is.EqualTo(GamePhase.Staging));
        }

        private static void WalkTo(CharacterController controller, Vector3 destination, string context)
        {
            for (var i = 0; i < 60; i++)
            {
                var delta = destination - controller.transform.position;
                delta.y = 0f;
                if (delta.magnitude < 0.08f) break;
                controller.Move(Vector3.ClampMagnitude(delta, 0.2f) + Vector3.down * 0.03f);
                Assert.That(controller.transform.position.y, Is.GreaterThan(-0.2f), context + " fell below floor");
            }
            var remaining = destination - controller.transform.position;
            remaining.y = 0f;
            Assert.That(remaining.magnitude, Is.LessThan(0.08f),
                context + $" blocked at {controller.transform.position}");
        }

        [UnitySetUp]
        public IEnumerator LoadMainScene()
        {
            foreach (var key in new[] { CampaignProgress.CompletionKey, "Fogbound.SelectedLevel", "Fogbound.UnlockedLevel" })
            {
                savedProgress[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : null;
                PlayerPrefs.DeleteKey(key);
            }
            PlayerPrefs.DeleteKey("Fogbound.SelectedLevel");
            PlayerPrefs.DeleteKey("Fogbound.UnlockedLevel");
            SceneManager.LoadScene("Main");
            yield return null;
            yield return null;
            GameDirector.Instance.ShowLevelSelection();
            Assert.That(GameDirector.Instance.SelectLevel(1), Is.True);
            yield return null;
        }

        [TearDown]
        public void RestoreProgress()
        {
            foreach (var pair in savedProgress)
            {
                if (pair.Value.HasValue) PlayerPrefs.SetInt(pair.Key, pair.Value.Value);
                else PlayerPrefs.DeleteKey(pair.Key);
            }
            PlayerPrefs.Save();
            Time.timeScale = 1f;
        }

        [UnityTest]
        public IEnumerator PlayerCanWalkThroughOpenGateIntoMaze()
        {
            var director = GameDirector.Instance;
            director.SelectWeapon(WeaponType.SniperRifle);
            Assert.That(director.Phase, Is.EqualTo(GamePhase.Staging));
            var controller = director.Player.GetComponent<CharacterController>();
            for (var step = 0; step < 35 && director.Phase == GamePhase.Staging; step++)
            {
                controller.Move(Vector3.forward * 0.18f);
                yield return null;
            }
            Assert.That(director.Phase, Is.EqualTo(GamePhase.Playing),
                $"Player stopped at {director.Player.transform.position} before reaching the entry trigger.");
            Assert.That(director.World.IsInside(director.Player.transform.position), Is.True);
        }

        [UnityTest]
        public IEnumerator ExplorationMap_TracksVisitedCellsAndResetsForNewRun()
        {
            var director = GameDirector.Instance;
            Assert.That(director.Hud.ExploredCells, Is.EqualTo(1));
            director.SelectWeapon(WeaponType.SniperRifle);
            director.BeginRun();
            foreach (var next in director.World.Layout.GetOpenNeighbors(director.World.Layout.Start))
            {
                director.Player.Teleport(director.World.CellToWorld(next) + Vector3.up * 0.2f);
                Physics.SyncTransforms();
                Assert.That(director.World.IsInside(director.Player.transform.position), Is.True);
                Assert.That(Object.FindFirstObjectByType<MiniMapHud>().isActiveAndEnabled, Is.True);
                yield return null;
                yield return null;
                Assert.That(director.Hud.ExploredCells, Is.EqualTo(2),
                    $"Player at {director.Player.transform.position}, next cell {next}.");
                director.ReturnToLoadout();
                Assert.That(director.Hud.ExploredCells, Is.EqualTo(1));
                yield break;
            }
            Assert.Fail("Generated maze start has no open neighbor.");
        }

        [UnityTest]
        public IEnumerator EscapePauseFromStaging_CanReturnToLoadout()
        {
            var director = GameDirector.Instance;
            director.SelectWeapon(WeaponType.SniperRifle);
            director.SetPaused(true);
            Assert.That(director.Phase, Is.EqualTo(GamePhase.Paused));
            Assert.That(Time.timeScale, Is.EqualTo(0f));
            director.SetPaused(false);
            Assert.That(director.Phase, Is.EqualTo(GamePhase.Staging));
            director.ReturnToLoadout();
            yield return null;
            Assert.That(director.Player.Weapon.Type, Is.EqualTo(WeaponType.LongBlade));
            Assert.That(director.LoadoutConfirmed, Is.True);
            Assert.That(Time.timeScale, Is.EqualTo(1f));
        }

        [UnityTest]
        public IEnumerator MacheteSwing_DamagesEnemyInFront()
        {
            var director = GameDirector.Instance;
            director.SelectWeapon(WeaponType.LongBlade);
            director.BeginRun();
            var enemyObject = new GameObject("Melee Test Enemy");
            enemyObject.AddComponent<CharacterController>();
            var health = enemyObject.AddComponent<Health>();
            var enemy = enemyObject.AddComponent<EnemyAgent>();
            enemy.Spawn(director.Player, director.World,
                director.Player.transform.position + Vector3.forward * 1.5f, false);
            Physics.SyncTransforms();

            director.Player.Weapon.Tick(true, true, false);
            Assert.That(director.Player.Weapon.IsSwinging, Is.True);
            yield return new WaitForSeconds(.2f);
            Assert.That(health.Current, Is.LessThan(health.Maximum));
            Object.Destroy(enemyObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PistolCrosshair_DamagesEnemyInSight()
        {
            var director = GameDirector.Instance;
            director.SelectWeapon(WeaponType.SniperRifle);
            director.BeginRun();
            var enemyObject = new GameObject("Aim Test Enemy");
            var controller = enemyObject.AddComponent<CharacterController>();
            controller.height = 1.9f;
            controller.center = Vector3.up * 0.95f;
            var health = enemyObject.AddComponent<Health>();
            var enemy = enemyObject.AddComponent<EnemyAgent>();
            var position = director.CameraRig.Camera.transform.position
                + director.CameraRig.Camera.transform.forward * 8f;
            position.y = 0f;
            enemy.Spawn(director.Player, director.World, position, false);
            Physics.SyncTransforms();

            director.Player.Weapon.Tick(true, true, false);
            Assert.That(health.Current, Is.LessThan(health.Maximum));
            Assert.That(director.Player.Weapon.Ammunition, Is.EqualTo(4));
            Object.Destroy(enemyObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayingRun_SpawnsEnemyAfterVisibleTelegraphDelay()
        {
            var director = GameDirector.Instance;
            director.SelectWeapon(WeaponType.LongBlade);
            director.BeginRun();
            yield return new WaitForSeconds(3.1f);
            Assert.That(director.ActiveEnemyCount, Is.GreaterThanOrEqualTo(1));
        }

        [UnityTest]
        public IEnumerator ExitAndSpawnSignalLightsStayLocalAndCastWallShadows()
        {
            var exitLight = GameObject.Find("Exit Beacon").GetComponent<Light>();
            Assert.That(exitLight.range, Is.EqualTo(MazeWorld.ExitLightRange));
            Assert.That(exitLight.intensity, Is.EqualTo(MazeWorld.ExitLightIntensity));
            Assert.That(exitLight.shadows, Is.EqualTo(LightShadows.Hard));

            SpawnTelegraph.Create(new Vector3(500f, 0f, 500f), false, null);
            SpawnTelegraph.Create(new Vector3(510f, 0f, 500f), true, null);
            yield return null;

            var normal = GameObject.Find("Goblin Spawn Warning");
            var elite = GameObject.Find("Abyss Brute Spawn Warning");
            Assert.That(normal.GetComponent<Light>().range, Is.EqualTo(SpawnTelegraph.NormalLightRange));
            Assert.That(elite.GetComponent<Light>().range, Is.EqualTo(SpawnTelegraph.EliteLightRange));
            foreach (var signal in new[] { normal, elite })
            {
                var light = signal.GetComponent<Light>();
                Assert.That(light.intensity, Is.LessThanOrEqualTo(
                    SpawnTelegraph.LightBaseIntensity + SpawnTelegraph.LightPulseIntensity));
                Assert.That(light.shadows, Is.EqualTo(LightShadows.Hard));
                Object.Destroy(signal);
            }
        }

        [UnityTest]
        public IEnumerator UnifiedInput_MovesPlayerAndSwitchesCameraMode()
        {
            var director = GameDirector.Instance;
            director.SelectWeapon(WeaponType.SniperRifle);
            director.BeginRun();
            var start = director.Player.transform.position;
            director.Input.SetMobileMove(Vector2.right);
            yield return new WaitForSeconds(0.45f);
            director.Input.SetMobileMove(Vector2.zero);
            var horizontalDelta = director.Player.transform.position - start;
            horizontalDelta.y = 0f;
            Assert.That(horizontalDelta.magnitude, Is.GreaterThan(0.05f));

            Assert.That(director.CameraRig.IsFirstPerson, Is.False);
            director.Input.PressMobileToggle();
            yield return null;
            yield return null;
            Assert.That(director.CameraRig.IsFirstPerson, Is.True);
        }

        [UnityTest]
        public IEnumerator MiasmaOutsideSafeLight_DamagesPlayer()
        {
            var director = GameDirector.Instance;
            for (var level = 2; level <= 7; level++)
            {
                director.SelectWeapon(WeaponType.SniperRifle);
                director.BeginRun();
                director.CompleteLevel();
                director.NextLevel();
                yield return null;
            }
            director.SelectWeapon(WeaponType.LongBlade);
            director.BeginRun();

            var unsafePosition = FindUnsafeCell(director);
            Assert.That(unsafePosition.HasValue, Is.True);
            director.Player.Teleport(unsafePosition.Value + Vector3.up * 0.2f);
            Physics.SyncTransforms();
            var before = director.Player.Health.Current;
            yield return new WaitForSeconds(0.45f);
            Assert.That(director.Player.Health.Current, Is.LessThan(before));
        }

        private static Vector3? FindUnsafeCell(GameDirector director)
        {
            for (var y = 0; y < director.World.Layout.Height; y++)
            {
                for (var x = 0; x < director.World.Layout.Width; x++)
                {
                    var position = director.World.CellToWorld(new Vector2Int(x, y));
                    if (!director.World.IsWithinSafeLight(position, director.CurrentLevel.safeLightRadius))
                        return position;
                }
            }
            return null;
        }

        [UnityTest]
        public IEnumerator StartChooseEnterAndWin_CompletesCoreLoop()
        {
            var director = GameDirector.Instance;
            Assert.That(director, Is.Not.Null);
            Assert.That(director.Phase, Is.EqualTo(GamePhase.Staging));
            Assert.That(director.World.Layout.Width, Is.EqualTo(7));
            Assert.That(director.Player.Weapon.Type, Is.EqualTo(WeaponType.LongBlade));
            Assert.That(director.LoadoutConfirmed, Is.True);

            director.Player.Weapon.Equip(WeaponType.SniperRifle);
            yield return null;
            Assert.That(director.Player.Weapon.Type, Is.EqualTo(WeaponType.SniperRifle));
            Assert.That(director.LoadoutConfirmed, Is.True);
            Assert.That(director.World.StartGate.activeSelf, Is.False);

            director.BeginRun();
            Assert.That(director.Phase, Is.EqualTo(GamePhase.Playing));
            director.CompleteLevel();
            Assert.That(director.Phase, Is.EqualTo(GamePhase.Won));
        }

        [UnityTest]
        public IEnumerator CampaignCanAdvanceToLevelTen_WithDifferentMazeSize()
        {
            var director = GameDirector.Instance;
            for (var level = 2; level <= 10; level++)
            {
                director.SelectWeapon(WeaponType.SniperRifle);
                director.BeginRun();
                director.CompleteLevel();
                director.NextLevel();
                yield return null;
            }

            Assert.That(director.CurrentLevel.levelNumber, Is.EqualTo(10));
            Assert.That(director.World.Layout.Width, Is.EqualTo(15));
            Assert.That(director.World.Layout.Height, Is.EqualTo(14));
            Assert.That(director.CurrentLevel.miasma, Is.True);
            Assert.That(director.World.SafeLights, Is.Not.Empty);
            Assert.That(director.CurrentLevel.environment, Is.EqualTo(DungeonCondition.DeepCore));
            Assert.That(director.World.FogZones.Count, Is.EqualTo(8));
            Assert.That(director.World.LocalVisibilityMultiplier(director.World.FogZones[0]), Is.EqualTo(.55f));
            var fogZone = GameObject.Find("Local Fog Zone");
            Assert.That(fogZone, Is.Not.Null);
            var fogLayers = fogZone.GetComponentsInChildren<ParticleSystem>();
            Assert.That(fogLayers, Has.Length.EqualTo(3));
            var fogParticleBudget = 0;
            foreach (var layer in fogLayers) fogParticleBudget += layer.main.maxParticles;
            Assert.That(fogParticleBudget, Is.LessThanOrEqualTo(140));
            Assert.That(director.EnemyPoolCapacity, Is.GreaterThanOrEqualTo(14));
            director.SelectWeapon(WeaponType.SniperRifle);
            director.BeginRun();
            director.ReachExit();
            yield return new WaitForSeconds(1.9f);
            director.BossEncounter.Boss.TakeDamage(10000f, false);
            director.Player.Teleport(director.World.PrincessPosition + Vector3.up * .08f);
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            Assert.That(director.Progress.CompletedCount, Is.EqualTo(10));
            director.NextLevel();
            Assert.That(director.Phase, Is.EqualTo(GamePhase.Won));
        }
    }
}
