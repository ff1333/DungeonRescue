using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FogboundMaze
{
    public enum MobileAction
    {
        Attack,
        Aim,
        Sprint,
        Reload,
        Gun,
        Blade,
        Toggle,
        Pause
    }

    public static class MobileHudBuilder
    {
        public static void Build(Transform canvas, GameInput input, Font font)
        {
            CreateLookArea(canvas, input);
            CreateStick(canvas, input, font);
            CreateButton(canvas, input, font, "ATTACK", MobileAction.Attack, new Vector2(0.91f, 0.17f), 154f);
            CreateButton(canvas, input, font, "SCOPE", MobileAction.Aim, new Vector2(0.76f, 0.19f), 106f);
            CreateButton(canvas, input, font, "RELOAD", MobileAction.Reload, new Vector2(0.91f, 0.36f), 100f);
            CreateButton(canvas, input, font, "SNIPER", MobileAction.Gun, new Vector2(0.78f, 0.38f), 100f);
            CreateButton(canvas, input, font, "BLADE", MobileAction.Blade, new Vector2(0.66f, 0.38f), 100f);
            CreateButton(canvas, input, font, "SPRINT", MobileAction.Sprint, new Vector2(0.56f, 0.16f), 108f);
            CreateButton(canvas, input, font, "VIEW", MobileAction.Toggle, new Vector2(0.79f, 0.55f), 90f);
            CreateButton(canvas, input, font, "II", MobileAction.Pause, new Vector2(0.96f, 0.93f), 74f);
        }

        private static void CreateLookArea(Transform parent, GameInput input)
        {
            var root = new GameObject("Look Area", typeof(RectTransform), typeof(Image), typeof(MobileLookArea));
            root.transform.SetParent(parent, false);
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            root.GetComponent<Image>().color = Color.clear;
            root.GetComponent<MobileLookArea>().Initialize(input);
            root.transform.SetAsFirstSibling();
        }

        private static void CreateStick(Transform parent, GameInput input, Font font)
        {
            var root = new GameObject("Move Touch Area", typeof(RectTransform), typeof(Image), typeof(MobileStick));
            root.transform.SetParent(parent, false);
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            root.GetComponent<Image>().color = Color.clear;

            var background = new GameObject("Move Stick Visuals", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(root.transform, false);
            var backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.sizeDelta = new Vector2(240f, 240f);
            background.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.13f);
            background.GetComponent<Image>().raycastTarget = false;

            var knob = new GameObject("Knob", typeof(RectTransform), typeof(Image));
            knob.transform.SetParent(background.transform, false);
            knob.GetComponent<RectTransform>().sizeDelta = new Vector2(92f, 92f);
            knob.GetComponent<Image>().color = new Color(0.2f, 0.78f, 0.62f, 0.55f);
            knob.GetComponent<Image>().raycastTarget = false;

            var sprintGate = new GameObject("Sprint Gate", typeof(RectTransform), typeof(Image));
            sprintGate.transform.SetParent(background.transform, false);
            var sprintGateRect = sprintGate.GetComponent<RectTransform>();
            sprintGateRect.sizeDelta = new Vector2(132f, 44f);
            sprintGateRect.anchoredPosition = new Vector2(0f, 150f);
            var sprintGateImage = sprintGate.GetComponent<Image>();
            sprintGateImage.color = new Color(0.08f, 0.12f, 0.13f, 0.62f);
            sprintGateImage.raycastTarget = false;

            var sprintLabel = new GameObject("Label", typeof(RectTransform), typeof(Text));
            sprintLabel.transform.SetParent(sprintGate.transform, false);
            var sprintLabelRect = sprintLabel.GetComponent<RectTransform>();
            sprintLabelRect.anchorMin = Vector2.zero;
            sprintLabelRect.anchorMax = Vector2.one;
            sprintLabelRect.offsetMin = sprintLabelRect.offsetMax = Vector2.zero;
            var sprintText = sprintLabel.GetComponent<Text>();
            sprintText.font = font;
            sprintText.text = "SPRINT";
            sprintText.fontSize = 20;
            sprintText.alignment = TextAnchor.MiddleCenter;
            sprintText.color = new Color(0.78f, 0.88f, 0.86f, 0.9f);
            sprintText.raycastTarget = false;
            LocalizedLabel.Attach(sprintLabel);
            root.GetComponent<MobileStick>().Initialize(
                input,
                backgroundRect,
                knob.GetComponent<RectTransform>(),
                sprintGateImage,
                sprintText);
        }

        private static void CreateButton(Transform parent, GameInput input, Font font, string label, MobileAction action, Vector2 anchor, float size)
        {
            var root = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(MobileActionButton));
            root.transform.SetParent(parent, false);
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.sizeDelta = Vector2.one * size;
            root.GetComponent<Image>().color = new Color(0.04f, 0.07f, 0.08f, 0.58f);
            var textRoot = new GameObject("Label", typeof(RectTransform), typeof(Text));
            textRoot.transform.SetParent(root.transform, false);
            var textRect = textRoot.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
            var text = textRoot.GetComponent<Text>();
            text.font = font;
            text.text = label;
            text.fontSize = 25;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 15;
            text.resizeTextMaxSize = 25;
            LocalizedLabel.Attach(textRoot);
            root.GetComponent<MobileActionButton>().Initialize(input, action);
        }
    }

    public sealed class MobileStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private GameInput input;
        private RectTransform root;
        private RectTransform background;
        private RectTransform knob;
        private Image sprintGate;
        private Text sprintLabel;
        private Vector2 pointerOrigin;
        private bool sprintArmed;

        public void Initialize(
            GameInput target,
            RectTransform backgroundTransform,
            RectTransform knobTransform,
            Image sprintGateImage,
            Text sprintGateText)
        {
            input = target;
            root = (RectTransform)transform;
            background = backgroundTransform;
            knob = knobTransform;
            sprintGate = sprintGateImage;
            sprintLabel = sprintGateText;
            background.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!TryGetLocalPoint(eventData, out pointerOrigin)) return;
            input.BeginMobileMove();
            sprintArmed = false;
            var radius = background.rect.width * 0.5f;
            var bounds = root.rect;
            pointerOrigin.x = Mathf.Clamp(pointerOrigin.x, bounds.xMin + radius, bounds.xMax - radius);
            pointerOrigin.y = Mathf.Clamp(pointerOrigin.y, bounds.yMin + radius, bounds.yMax - radius);
            background.anchoredPosition = pointerOrigin;
            knob.anchoredPosition = Vector2.zero;
            background.gameObject.SetActive(true);
            input.SetMobileMove(Vector2.zero);
            RefreshSprintGate();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!background.gameObject.activeSelf ||
                !TryGetLocalPoint(eventData, out var point)) return;
            var radius = background.rect.width * 0.36f;
            var raw = (point - pointerOrigin) / radius;
            sprintArmed = raw.y >= 1.35f && Mathf.Abs(raw.x) <= 0.72f;
            var value = sprintArmed ? Vector2.up : Vector2.ClampMagnitude(raw, 1f);
            knob.anchoredPosition = value * radius;
            input.SetMobileMove(value);
            input.SetMobileAutoSprint(sprintArmed);
            RefreshSprintGate();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            knob.anchoredPosition = Vector2.zero;
            input.SetMobileMove(sprintArmed ? Vector2.up : Vector2.zero);
            input.SetMobileAutoSprint(sprintArmed);
            background.gameObject.SetActive(false);
            sprintArmed = false;
        }

        private void RefreshSprintGate()
        {
            if (sprintGate == null || sprintLabel == null) return;
            sprintGate.color = sprintArmed
                ? new Color(0.14f, 0.72f, 0.52f, 0.9f)
                : new Color(0.08f, 0.12f, 0.13f, 0.62f);
            sprintLabel.color = sprintArmed ? Color.white : new Color(0.78f, 0.88f, 0.86f, 0.9f);
        }

        private bool TryGetLocalPoint(
            PointerEventData eventData,
            out Vector2 point) =>
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                root,
                eventData.position,
                eventData.pressEventCamera,
                out point);

        private void OnDisable()
        {
            if (input != null) input.SetMobileMove(Vector2.zero);
            if (input != null) input.SetMobileAutoSprint(false);
            if (background != null) background.gameObject.SetActive(false);
        }
    }

    public sealed class MobileLookArea : MonoBehaviour, IDragHandler
    {
        private GameInput input;
        public void Initialize(GameInput target) => input = target;
        public void OnDrag(PointerEventData eventData) => input.AddMobileLook(eventData.delta * 0.09f);
    }

    public sealed class MobileActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private GameInput input;
        private MobileAction action;
        private Image background;
        private static readonly Color Idle = new(0.04f, 0.07f, 0.08f, 0.58f);
        private static readonly Color Active = new(0.12f, 0.72f, 0.54f, 0.9f);

        public void Initialize(GameInput target, MobileAction mobileAction)
        {
            input = target;
            action = mobileAction;
            background = GetComponent<Image>();
        }

        private void Update()
        {
            if (background == null || input == null) return;
            var active = action switch
            {
                MobileAction.Attack => input.AttackHeld,
                MobileAction.Aim => input.MobileAimToggled,
                MobileAction.Sprint => input.MobileSprintLocked,
                _ => false
            };
            background.color = active ? Active : Idle;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            switch (action)
            {
                case MobileAction.Attack: input.SetMobileAttack(true); break;
                case MobileAction.Aim: input.ToggleMobileAim(); break;
                case MobileAction.Sprint: input.ToggleMobileSprint(); break;
                case MobileAction.Reload: input.PressMobileReload(); break;
                case MobileAction.Gun: input.PressMobileGun(); break;
                case MobileAction.Blade: input.PressMobileBlade(); break;
                case MobileAction.Toggle: input.PressMobileToggle(); break;
                case MobileAction.Pause: input.PressMobilePause(); break;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (action == MobileAction.Attack) input.SetMobileAttack(false);
        }

        private void OnDisable()
        {
            if (action == MobileAction.Attack && input != null) input.SetMobileAttack(false);
        }
    }
}
