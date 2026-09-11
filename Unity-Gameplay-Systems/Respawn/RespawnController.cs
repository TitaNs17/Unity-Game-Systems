using UnityEngine;
using UnityGameSystems.Combat;

namespace UnityGameSystems.Respawn
{
    [DisallowMultipleComponent]
    public sealed class RespawnController : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Transform fallbackSpawnPoint;
        [SerializeField] private bool resetVelocity = true;

        private Checkpoint currentCheckpoint;
        private Rigidbody cachedRigidbody;
        private CharacterController cachedCharacterController;

        private void Awake()
        {
            if (health == null)
                health = GetComponent<Health>();

            cachedRigidbody = GetComponent<Rigidbody>();
            cachedCharacterController = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            if (health != null)
                health.Died += Respawn;
        }

        private void OnDisable()
        {
            if (health != null)
                health.Died -= Respawn;
        }

        public void SetCheckpoint(Checkpoint checkpoint)
        {
            currentCheckpoint = checkpoint;
        }

        public void Respawn()
        {
            var position = currentCheckpoint != null
                ? currentCheckpoint.Position
                : fallbackSpawnPoint != null ? fallbackSpawnPoint.position : transform.position;

            var rotation = currentCheckpoint != null
                ? currentCheckpoint.Rotation
                : fallbackSpawnPoint != null ? fallbackSpawnPoint.rotation : transform.rotation;

            if (cachedCharacterController != null)
                cachedCharacterController.enabled = false;

            transform.SetPositionAndRotation(position, rotation);

            if (cachedCharacterController != null)
                cachedCharacterController.enabled = true;

            if (resetVelocity && cachedRigidbody != null)
            {
                cachedRigidbody.linearVelocity = Vector3.zero;
                cachedRigidbody.angularVelocity = Vector3.zero;
            }

            if (health != null)
                health.RestoreFull();
        }
    }
}
