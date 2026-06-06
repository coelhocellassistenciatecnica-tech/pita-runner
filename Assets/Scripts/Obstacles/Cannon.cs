using UnityEngine;

namespace PitaRunner.Obstacles
{
    public class Cannon : MonoBehaviour
    {
        [Header("Cannon Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireRate = 2f;
        [SerializeField] private float bulletSpeed = 15f;
        [SerializeField] private int unitsKilledPerBullet = 3;

        private float fireTimer = 0f;

        private void Update()
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= 1f / fireRate)
            {
                fireTimer = 0f;
                Fire();
            }
        }

        private void Fire()
        {
            if (bulletPrefab == null || firePoint == null) return;
            var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            var rb = bullet.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = -firePoint.forward * bulletSpeed;
            var bulletScript = bullet.GetComponent<CannonBullet>();
            if (bulletScript != null) bulletScript.SetDamage(unitsKilledPerBullet);
            Destroy(bullet, 5f);
        }
    }

    public class CannonBullet : MonoBehaviour
    {
        private int damageUnits = 3;

        public void SetDamage(int d) => damageUnits = d;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Unit") || other.CompareTag("Player"))
            {
                Crowd.CrowdManager.Instance?.RemoveUnits(damageUnits);
                Effects.ParticleManager.Instance?.SpawnExplosion(transform.position);
                Core.AudioManager.Instance?.PlayExplosion();
                Destroy(gameObject);
            }
        }
    }
}
