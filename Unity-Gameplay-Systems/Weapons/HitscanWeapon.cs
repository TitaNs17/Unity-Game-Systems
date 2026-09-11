using System;
using System.Collections;
using UnityEngine;
using UnityGameSystems.Combat;

namespace UnityGameSystems.Weapons
{
    [DisallowMultipleComponent]
    public sealed class HitscanWeapon : MonoBehaviour
    {
        [SerializeField] private WeaponDefinition definition;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private Transform muzzle;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private bool autoReload = true;

        private int ammoInMagazine;
        private float nextFireTime;
        private bool reloading;
        private Coroutine reloadRoutine;

        public int AmmoInMagazine => ammoInMagazine;
        public int MagazineSize => definition != null ? definition.MagazineSize : 0;
        public bool IsReloading => reloading;
        public bool CanFire => definition != null && !reloading && ammoInMagazine > 0 && Time.time >= nextFireTime;

        public event Action Fired;
        public event Action<int, int> AmmoChanged;
        public event Action ReloadStarted;
        public event Action ReloadCompleted;
        public event Action<RaycastHit> Hit;

        private void Awake()
        {
            if (aimCamera == null)
                aimCamera = Camera.main;

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            ammoInMagazine = MagazineSize;
        }

        public bool TryFire()
        {
            if (definition == null || reloading || Time.time < nextFireTime)
                return false;

            if (ammoInMagazine <= 0)
            {
                if (autoReload)
                    TryReload();
                return false;
            }

            ammoInMagazine--;
            nextFireTime = Time.time + 1f / Mathf.Max(0.01f, definition.FireRate);
            AmmoChanged?.Invoke(ammoInMagazine, definition.MagazineSize);

            if (muzzleFlash != null)
                muzzleFlash.Play();

            if (audioSource != null && definition.FireClip != null)
                audioSource.PlayOneShot(definition.FireClip);

            var ray = BuildShotRay();
            if (Physics.Raycast(ray, out var hit, definition.Range, hitMask, QueryTriggerInteraction.Ignore))
            {
                var health = hit.collider.GetComponentInParent<Health>();
                if (health != null)
                    health.TakeDamage(definition.Damage);

                Hit?.Invoke(hit);
            }

            Fired?.Invoke();

            if (ammoInMagazine == 0 && autoReload)
                TryReload();

            return true;
        }

        public bool TryReload()
        {
            if (definition == null || reloading || ammoInMagazine >= definition.MagazineSize)
                return false;

            if (reloadRoutine != null)
                StopCoroutine(reloadRoutine);

            reloadRoutine = StartCoroutine(ReloadRoutine());
            return true;
        }

        public void CancelReload()
        {
            if (reloadRoutine != null)
                StopCoroutine(reloadRoutine);

            reloadRoutine = null;
            reloading = false;
        }

        private IEnumerator ReloadRoutine()
        {
            reloading = true;
            ReloadStarted?.Invoke();

            if (audioSource != null && definition.ReloadClip != null)
                audioSource.PlayOneShot(definition.ReloadClip);

            yield return new WaitForSeconds(definition.ReloadDuration);

            ammoInMagazine = definition.MagazineSize;
            reloading = false;
            reloadRoutine = null;
            AmmoChanged?.Invoke(ammoInMagazine, definition.MagazineSize);
            ReloadCompleted?.Invoke();
        }

        private Ray BuildShotRay()
        {
            var originTransform = aimCamera != null ? aimCamera.transform : (muzzle != null ? muzzle : transform);
            var direction = originTransform.forward;

            if (definition.SpreadDegrees > 0f)
            {
                var yaw = UnityEngine.Random.Range(-definition.SpreadDegrees, definition.SpreadDegrees);
                var pitch = UnityEngine.Random.Range(-definition.SpreadDegrees, definition.SpreadDegrees);
                direction = Quaternion.Euler(pitch, yaw, 0f) * direction;
            }

            return new Ray(originTransform.position, direction.normalized);
        }
    }
}
