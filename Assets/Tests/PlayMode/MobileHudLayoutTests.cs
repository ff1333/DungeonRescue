using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace FogboundMaze.Tests
{
    public sealed class MobileHudLayoutTests
    {
        [UnityTest]
        public IEnumerator MoveAreaUsesOnlyLeftHalfAndStartsHidden()
        {
            var canvasObject = new GameObject(
                "Test Canvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler));
            var inputObject = new GameObject("Input", typeof(GameInput));

            try
            {
                var canvas = canvasObject.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                MobileHudBuilder.Build(
                    canvasObject.transform,
                    inputObject.GetComponent<GameInput>(),
                    Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
                yield return null;

                var area = canvasObject.transform.Find("Move Touch Area")
                    .GetComponent<RectTransform>();
                var visuals = area.Find("Move Stick Visuals");

                Assert.That(area.anchorMin, Is.EqualTo(Vector2.zero));
                Assert.That(area.anchorMax, Is.EqualTo(new Vector2(0.5f, 1f)));
                Assert.That(visuals.gameObject.activeSelf, Is.False);

                var pointer = new PointerEventData(null)
                {
                    position = RectTransformUtility.WorldToScreenPoint(
                        null,
                        area.TransformPoint(Vector3.zero))
                };
                var stick = area.GetComponent<MobileStick>();
                stick.OnPointerDown(pointer);
                Assert.That(visuals.gameObject.activeSelf, Is.True);

                pointer.position += new Vector2(60f, 0f);
                stick.OnDrag(pointer);
                yield return null;
                Assert.That(inputObject.GetComponent<GameInput>().Move.x,
                    Is.GreaterThan(0f));

                stick.OnPointerUp(pointer);
                yield return null;
                Assert.That(visuals.gameObject.activeSelf, Is.False);
                Assert.That(inputObject.GetComponent<GameInput>().Move,
                    Is.EqualTo(Vector2.zero));
            }
            finally
            {
                Object.Destroy(canvasObject);
                Object.Destroy(inputObject);
            }
        }

        [UnityTest]
        public IEnumerator CombatButtonsOccupySeparatePositions()
        {
            var canvasObject = new GameObject(
                "Test Canvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler));
            var inputObject = new GameObject("Input", typeof(GameInput));

            try
            {
                canvasObject.GetComponent<Canvas>().renderMode =
                    RenderMode.ScreenSpaceOverlay;
                MobileHudBuilder.Build(
                    canvasObject.transform,
                    inputObject.GetComponent<GameInput>(),
                    Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
                yield return null;

                string[] names =
                {
                    "ATTACK", "SCOPE", "RELOAD", "SNIPER",
                    "BLADE", "SPRINT", "VIEW"
                };

                for (int i = 0; i < names.Length; i++)
                {
                    RectTransform first = canvasObject.transform.Find(names[i])
                        .GetComponent<RectTransform>();
                    for (int j = i + 1; j < names.Length; j++)
                    {
                        RectTransform second = canvasObject.transform.Find(names[j])
                            .GetComponent<RectTransform>();
                        Assert.That(
                            RectTransformUtility.RectangleContainsScreenPoint(
                                first,
                                second.position),
                            Is.False,
                            $"{names[j]} center overlaps {names[i]}");
                    }
                }
            }
            finally
            {
                Object.Destroy(canvasObject);
                Object.Destroy(inputObject);
            }
        }

        [UnityTest]
        public IEnumerator ScopeAndSprintButtonsUseTapToToggle()
        {
            var canvasObject = new GameObject(
                "Test Canvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler));
            var inputObject = new GameObject("Input", typeof(GameInput));

            try
            {
                canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var input = inputObject.GetComponent<GameInput>();
                MobileHudBuilder.Build(
                    canvasObject.transform,
                    input,
                    Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
                yield return null;

                var pointer = new PointerEventData(null);
                var scope = canvasObject.transform.Find("SCOPE").GetComponent<MobileActionButton>();
                scope.OnPointerDown(pointer);
                scope.OnPointerUp(pointer);
                yield return null;
                Assert.That(input.AimHeld, Is.True,
                    "Releasing the scope button must not close a toggled scope.");

                var attack = canvasObject.transform.Find("ATTACK").GetComponent<MobileActionButton>();
                attack.OnPointerDown(pointer);
                yield return null;
                Assert.That(input.AttackHeld, Is.True);
                Assert.That(input.AimHeld, Is.True,
                    "Pressing attack must not cancel a toggled scope.");
                attack.OnPointerUp(pointer);
                yield return null;
                Assert.That(input.AttackHeld, Is.False);
                Assert.That(input.AimHeld, Is.True);

                scope.OnPointerDown(pointer);
                yield return null;
                Assert.That(input.AimHeld, Is.False);

                var sprint = canvasObject.transform.Find("SPRINT").GetComponent<MobileActionButton>();
                sprint.OnPointerDown(pointer);
                sprint.OnPointerUp(pointer);
                yield return null;
                Assert.That(input.SprintHeld, Is.True,
                    "Sprint should remain locked after a tap.");

                sprint.OnPointerDown(pointer);
                yield return null;
                Assert.That(input.SprintHeld, Is.False);
            }
            finally
            {
                Object.Destroy(canvasObject);
                Object.Destroy(inputObject);
            }
        }

        [UnityTest]
        public IEnumerator ForwardDragLocksAutoSprintUntilTheStickIsTouchedAgain()
        {
            var canvasObject = new GameObject(
                "Test Canvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler));
            var inputObject = new GameObject("Input", typeof(GameInput));

            try
            {
                canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var input = inputObject.GetComponent<GameInput>();
                MobileHudBuilder.Build(
                    canvasObject.transform,
                    input,
                    Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
                yield return null;

                var area = canvasObject.transform.Find("Move Touch Area").GetComponent<RectTransform>();
                var stick = area.GetComponent<MobileStick>();
                var pointer = new PointerEventData(null)
                {
                    position = RectTransformUtility.WorldToScreenPoint(
                        null,
                        area.TransformPoint(Vector3.zero))
                };

                stick.OnPointerDown(pointer);
                pointer.position += new Vector2(0f, 180f);
                stick.OnDrag(pointer);
                stick.OnPointerUp(pointer);
                yield return null;

                Assert.That(input.MobileAutoSprint, Is.True);
                Assert.That(input.SprintHeld, Is.True);
                Assert.That(input.Move.y, Is.EqualTo(1f).Within(0.01f));

                pointer.position = RectTransformUtility.WorldToScreenPoint(
                    null,
                    area.TransformPoint(Vector3.zero));
                stick.OnPointerDown(pointer);
                yield return null;
                Assert.That(input.MobileAutoSprint, Is.False);
                Assert.That(input.SprintHeld, Is.False);
                Assert.That(input.Move, Is.EqualTo(Vector2.zero));

                stick.OnPointerUp(pointer);
            }
            finally
            {
                Object.Destroy(canvasObject);
                Object.Destroy(inputObject);
            }
        }
    }
}
