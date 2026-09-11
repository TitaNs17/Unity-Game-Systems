using UnityEngine;

namespace UnityGameSystems.Weapons
{
    [CreateAssetMenu(menuName = "Game/Weapon", fileName = "Weapon")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        [SerializeField] private string weaponId = "weapon";
        [SerializeField, Min(0.01f)] private float damage = 20f;
        [SerializeField, Min(0.01f)] private float fireRate = 5f;
        [SerializeField, Min(1)] private int magazineSize = 12;
        [SerializeField, Min(0f)] private float reloadDuration = 1.5f;
        [SerializeField, Min(0.1f)] private float range = 100f;
        [SerializeField, Min(0f)] private float spreadDegrees;
        [SerializeField] private AudioClip fireClip;
        [SerializeField] private AudioClip reloadClip;

        public string Id => weaponId;
        public float Damage => damage;
        public float FireRate => fireRate;
        public int MagazineSize => magazineSize;
        public float ReloadDuration => reloadDuration;
        public float Range => range;
        public float SpreadDegrees => spreadDegrees;
        public AudioClip FireClip => fireClip;
        public AudioClip ReloadClip => reloadClip;
    }
}
