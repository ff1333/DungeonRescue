using System.Linq;
using UnityEngine;

namespace FogboundMaze
{
    public sealed class PlayerVisual : MonoBehaviour
    {
        public const float HeldRifleScale = 0.82f;
        public const float HeldBladeLengthScale = 1.35f;
        public const float BackWeaponScale = 0.72f;
        private Animation animationPlayer;
        private PlayerController player;
        private GameObject pistol;
        private GameObject knife;
        private GameObject backRifle;
        private GameObject backBlade;
        private Transform upperArmLeft;
        private Transform lowerArmLeft;
        private Transform palmLeft;
        private Transform upperArmRight;
        private Transform lowerArmRight;
        private Transform palmRight;
        private Vector3 previousPosition;
        private float attackUntil;
        private Vector3 gunMuzzleLocal;
        private Vector3 pistolRestPosition;
        private Quaternion pistolRestRotation;
        private Vector3 bladeRestPosition;
        private Quaternion bladeRestRotation;
        private bool deathEquipmentHidden;
        public Transform HeldWeapon => (pistol != null && pistol.activeSelf ? pistol : knife)?.transform;
        public Vector3 GunMuzzle
        {
            get
            {
                return pistol != null ? pistol.transform.TransformPoint(gunMuzzleLocal)
                    : transform.position + transform.up * 1.2f + transform.forward;
            }
        }

        private void Awake()
        {
            player = GetComponent<PlayerController>();
            animationPlayer = GetComponentInChildren<Animation>(true);
            var visual = transform.Find("Visual");
            var rightSocket = FindBone(visual, "handslot.r") ?? FindBone(visual, "Middle1.R") ?? visual;
            pistol = CreateHeldWeapon("Weapons/SniperRifle", "Rifle", rightSocket,
                new Vector3(.01f, -.015f, -.015f), Quaternion.Euler(2f, 88f, 2f), HeldRifleScale);
            knife = CreateHeldWeapon("Weapons/LongBlade", "Knife", rightSocket,
                new Vector3(.015f, -.01f, .01f), Quaternion.Euler(0f, 0f, -88f), .86f);
            if (pistol != null)
            {
                pistolRestPosition = pistol.transform.localPosition;
                pistolRestRotation = pistol.transform.localRotation;
            }
            if (knife != null)
            {
                bladeRestPosition = knife.transform.localPosition;
                bladeRestRotation = knife.transform.localRotation;
            }
            var torso = visual.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(child => child.name is "chest" or "Torso") ?? visual;
            upperArmLeft = FindBone(visual, "upperarm.l") ?? FindBone(visual, "UpperArm.L");
            lowerArmLeft = FindBone(visual, "lowerarm.l") ?? FindBone(visual, "LowerArm.L");
            palmLeft = FindBone(visual, "hand.l") ?? FindBone(visual, "Middle1.L");
            upperArmRight = FindBone(visual, "upperarm.r") ?? FindBone(visual, "UpperArm.R");
            lowerArmRight = FindBone(visual, "lowerarm.r") ?? FindBone(visual, "LowerArm.R");
            palmRight = FindBone(visual, "hand.r") ?? FindBone(visual, "Middle1.R");
            backRifle = CreateBackWeapon("Weapons/SniperRifle", torso,
                new Vector3(-0.24f, 0.02f, -0.18f), Quaternion.Euler(12f, 8f, 48f));
            backBlade = CreateBackWeapon("Weapons/LongBlade", torso,
                new Vector3(0.24f, 0.01f, -0.16f), Quaternion.Euler(6f, 178f, -42f));
            // Bake the socket in the editor; imported meshes are not CPU-readable in release players.
            var muzzle = pistol != null ? pistol.transform.Find("Muzzle") : null;
            gunMuzzleLocal = muzzle != null ? muzzle.localPosition : Vector3.forward;
            previousPosition = transform.position;
        }

        public void Equip(WeaponType weapon)
        {
            deathEquipmentHidden = false;
            if (pistol != null) pistol.SetActive(weapon == WeaponType.SniperRifle);
            if (knife != null) knife.SetActive(weapon == WeaponType.LongBlade);
            if (backRifle != null) backRifle.SetActive(weapon == WeaponType.LongBlade);
            if (backBlade != null) backBlade.SetActive(weapon == WeaponType.SniperRifle);
            attackUntil = 0f;
        }

        public void Attack()
        {
            if (player.Weapon.Type != WeaponType.LongBlade || animationPlayer == null) return;
            attackUntil = Time.time + 0.55f;
            animationPlayer["Slash"].speed = animationPlayer["Slash"].length / 0.55f;
            animationPlayer.CrossFade("Slash", 0.08f);
        }

        public void BoltAction(float duration)
        {
            if (player.Weapon.Type != WeaponType.SniperRifle || animationPlayer == null
                || animationPlayer["Idle_Attack"] == null) return;
            attackUntil = Time.time + duration;
            animationPlayer["Idle_Attack"].speed = animationPlayer["Idle_Attack"].length / duration;
            animationPlayer.CrossFade("Idle_Attack", 0.05f);
        }

        private void LateUpdate()
        {
            if (animationPlayer == null || player.Health == null) return;
            if (player.Health.IsDead && !deathEquipmentHidden)
            {
                SetEquipmentVisible(false);
                deathEquipmentHidden = true;
            }
            var delta = transform.position - previousPosition;
            delta.y = 0f;
            previousPosition = transform.position;
            var speed = delta.magnitude > 2f ? 0f : delta.magnitude / Mathf.Max(Time.deltaTime, 0.001f);
            if (Time.time >= attackUntil || player.Health.IsDead)
            {
                var suffix = player.Weapon.Type is WeaponType.SniperRifle or WeaponType.LongBlade ? "_Gun" : string.Empty;
                var clip = player.Health.IsDead ? "Death" : speed > 5.8f ? "Run" + suffix
                    : speed > 0.1f ? "Walk" + suffix : "Idle" + suffix;
                if (!animationPlayer.IsPlaying(clip)) animationPlayer.CrossFade(clip, 0.12f);
            }
            if (!player.Health.IsDead)
            {
                ResetHeldWeaponPose();
                if (player.Weapon.ObstructionAmount > .01f)
                    ApplyObstructedWeaponPose(player.Weapon.ObstructionAmount);
                else
                    ApplySupportHandPose();
            }
        }

        private void SetEquipmentVisible(bool visible)
        {
            if (pistol != null) pistol.SetActive(visible && player.Weapon.Type == WeaponType.SniperRifle);
            if (knife != null) knife.SetActive(visible && player.Weapon.Type == WeaponType.LongBlade);
            if (backRifle != null) backRifle.SetActive(visible && player.Weapon.Type == WeaponType.LongBlade);
            if (backBlade != null) backBlade.SetActive(visible && player.Weapon.Type == WeaponType.SniperRifle);
        }

        private void ApplySupportHandPose()
        {
            var held = HeldWeapon;
            if (held == null || player.Weapon.Type == WeaponType.None) return;
            var leftDistance = palmLeft != null ? Vector3.Distance(palmLeft.position, held.position) : float.MaxValue;
            var rightDistance = palmRight != null ? Vector3.Distance(palmRight.position, held.position) : float.MaxValue;
            var upper = leftDistance >= rightDistance ? upperArmLeft : upperArmRight;
            var lower = leftDistance >= rightDistance ? lowerArmLeft : lowerArmRight;
            var palm = leftDistance >= rightDistance ? palmLeft : palmRight;
            if (upper == null || lower == null || palm == null) return;

            Vector3 target;
            if (player.Weapon.Type == WeaponType.SniperRifle)
            {
                var muzzle = GunMuzzle;
                target = Vector3.Lerp(held.position, muzzle, 0.42f) - transform.up * 0.025f;
            }
            else
            {
                target = held.position + transform.up * 0.055f + transform.forward * 0.025f;
            }
            for (var iteration = 0; iteration < 2; iteration++)
            {
                RotateJointToward(lower, palm, target);
                RotateJointToward(upper, palm, target);
            }
        }

        private void ApplyObstructedWeaponPose(float amount)
        {
            if (HeldWeapon == null || player.Weapon.Type == WeaponType.None) return;
            var center = transform.position
                + transform.up * Mathf.Lerp(1.12f, 1.05f, amount)
                + transform.forward * Mathf.Lerp(.42f, .18f, amount);
            var leftTarget = center - transform.right * .13f;
            var rightTarget = center + transform.right * .13f;
            for (var iteration = 0; iteration < 2; iteration++)
            {
                RotateJointToward(lowerArmLeft, palmLeft, leftTarget);
                RotateJointToward(upperArmLeft, palmLeft, leftTarget);
                RotateJointToward(lowerArmRight, palmRight, rightTarget);
                RotateJointToward(upperArmRight, palmRight, rightTarget);
            }
            HeldWeapon.position -= transform.forward * (.28f * amount);
            HeldWeapon.rotation = Quaternion.AngleAxis(-52f * amount, transform.right) * HeldWeapon.rotation;
        }

        private void ResetHeldWeaponPose()
        {
            if (pistol != null)
            {
                pistol.transform.localPosition = pistolRestPosition;
                pistol.transform.localRotation = pistolRestRotation;
            }
            if (knife != null)
            {
                knife.transform.localPosition = bladeRestPosition;
                knife.transform.localRotation = bladeRestRotation;
            }
        }

        private static void RotateJointToward(Transform joint, Transform end, Vector3 target)
        {
            if (joint == null || end == null) return;
            var current = end.position - joint.position;
            var desired = target - joint.position;
            if (current.sqrMagnitude < 0.000001f || desired.sqrMagnitude < 0.000001f) return;
            joint.rotation = Quaternion.FromToRotation(current, desired) * joint.rotation;
        }

        private static Transform FindBone(Transform root, string name)
            => root.GetComponentsInChildren<Transform>(true).FirstOrDefault(child => child.name == name);

        private static GameObject CreateBackWeapon(string resource, Transform parent, Vector3 position, Quaternion rotation)
        {
            var prefab = Resources.Load<GameObject>(resource);
            if (prefab == null) return null;
            var weapon = Instantiate(prefab, parent);
            weapon.name = "Back " + prefab.name;
            weapon.transform.localPosition = position;
            weapon.transform.localRotation = rotation;
            weapon.transform.localScale = Vector3.one * BackWeaponScale;
            foreach (var child in weapon.GetComponentsInChildren<Transform>(true))
                child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            return weapon;
        }

        private static GameObject CreateHeldWeapon(string resource, string name, Transform parent,
            Vector3 position, Quaternion rotation, float scale)
        {
            var prefab = Resources.Load<GameObject>(resource);
            if (prefab == null) return null;
            var weapon = Instantiate(prefab, parent);
            weapon.name = name;
            weapon.transform.localPosition = position;
            weapon.transform.localRotation = rotation;
            weapon.transform.localScale = Vector3.one * scale;
            foreach (var child in weapon.GetComponentsInChildren<Transform>(true))
                child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            return weapon;
        }
    }
}
