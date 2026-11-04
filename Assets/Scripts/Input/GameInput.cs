using UnityEngine;
using UnityEngine.InputSystem;

namespace FogboundMaze
{
    public sealed class GameInput : MonoBehaviour
    {
        private Vector2 mobileMove;
        private Vector2 mobileLook;
        private bool mobileAttack;
        private bool mobileAim;
        private bool mobileSprintButton;
        private bool mobileAutoSprint;
        private bool mobileReload;
        private bool mobileGun;
        private bool mobileBlade;
        private bool mobileToggle;
        private bool mobilePause;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool AttackHeld { get; private set; }
        public bool AttackPressed { get; private set; }
        public bool AimHeld { get; private set; }
        public bool ReloadPressed { get; private set; }
        public bool SelectGunPressed { get; private set; }
        public bool SelectBladePressed { get; private set; }
        public bool ToggleViewPressed { get; private set; }
        public bool PausePressed { get; private set; }
        public bool SprintHeld { get; private set; }
        public bool MobileAimToggled => mobileAim;
        public bool MobileSprintLocked => mobileSprintButton || mobileAutoSprint;
        public bool MobileAutoSprint => mobileAutoSprint;

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            var keyboardMove = Vector2.zero;
            if (keyboard != null)
            {
                keyboardMove.x = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
                keyboardMove.y = (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f);
            }

            Move = Vector2.ClampMagnitude(keyboardMove + mobileMove, 1f);
            Look = (mouse != null ? mouse.delta.ReadValue() * 0.075f : Vector2.zero) + ConsumeMobileLook();
            AttackHeld = (mouse != null && mouse.leftButton.isPressed) || mobileAttack;
            AttackPressed = (mouse != null && mouse.leftButton.wasPressedThisFrame) || mobileAttack;
            AimHeld = (mouse != null && mouse.rightButton.isPressed) || mobileAim;
            ReloadPressed = (keyboard != null && keyboard.rKey.wasPressedThisFrame) || Consume(ref mobileReload);
            var scroll = mouse != null ? mouse.scroll.ReadValue().y : 0f;
            SelectGunPressed = (keyboard != null && keyboard.digit1Key.wasPressedThisFrame)
                || scroll > 0.01f || Consume(ref mobileGun);
            SelectBladePressed = (keyboard != null && keyboard.digit2Key.wasPressedThisFrame)
                || scroll < -0.01f || Consume(ref mobileBlade);
            ToggleViewPressed = (keyboard != null && keyboard.vKey.wasPressedThisFrame) || Consume(ref mobileToggle);
            PausePressed = (keyboard != null && keyboard.escapeKey.wasPressedThisFrame) || Consume(ref mobilePause);
            SprintHeld = (keyboard != null && keyboard.leftShiftKey.isPressed)
                || mobileSprintButton || mobileAutoSprint;
        }

        public void SetMobileMove(Vector2 value) => mobileMove = Vector2.ClampMagnitude(value, 1f);
        public void ResetMobile()
        {
            mobileMove = mobileLook = Vector2.zero;
            mobileAttack = mobileAim = mobileSprintButton = mobileAutoSprint = mobileReload =
                mobileGun = mobileBlade = mobileToggle = mobilePause = false;
        }
        public void AddMobileLook(Vector2 value) => mobileLook += value;
        public void SetMobileAttack(bool value) => mobileAttack = value;
        public void SetMobileAim(bool value) => mobileAim = value;
        public void ToggleMobileAim() => mobileAim = !mobileAim;
        public void SetMobileSprint(bool value) => mobileSprintButton = value;
        public void ToggleMobileSprint()
        {
            if (mobileAutoSprint)
            {
                mobileAutoSprint = false;
                mobileMove = Vector2.zero;
                return;
            }
            mobileSprintButton = !mobileSprintButton;
        }
        public void BeginMobileMove()
        {
            if (!mobileAutoSprint) return;
            mobileAutoSprint = false;
            mobileMove = Vector2.zero;
        }
        public void SetMobileAutoSprint(bool value)
        {
            mobileAutoSprint = value;
            if (value) mobileSprintButton = false;
        }
        public void PressMobileReload()
        {
            mobileAim = false;
            mobileReload = true;
        }
        public void PressMobileGun() => mobileGun = true;
        public void PressMobileBlade()
        {
            mobileAim = false;
            mobileBlade = true;
        }
        public void PressMobileToggle() => mobileToggle = true;
        public void PressMobilePause() => mobilePause = true;

        private Vector2 ConsumeMobileLook()
        {
            var value = mobileLook;
            mobileLook = Vector2.zero;
            return value;
        }

        private static bool Consume(ref bool value)
        {
            var current = value;
            value = false;
            return current;
        }
    }
}
