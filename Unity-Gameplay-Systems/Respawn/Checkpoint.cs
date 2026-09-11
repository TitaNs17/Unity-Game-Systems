using UnityEngine;

namespace UnityGameSystems.Respawn
{
    public sealed class Checkpoint : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private bool activateOnTrigger = true;
        [SerializeField] private string requiredTag = "Player";

        public Vector3 Position => spawnPoint != null ? spawnPoint.position : transform.position;
        public Quaternion Rotation => spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        private void OnTriggerEnter(Collider other)
        {
            if (!activateOnTrigger) return;
            if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;

            var respawn = other.GetComponentInParent<RespawnController>();
            if (respawn != null)
                respawn.SetCheckpoint(this);
        }
    }
}
