using UnityEngine;
using PitaRunner.Crowd;

namespace PitaRunner.Obstacles
{
    public abstract class ObstacleBase : MonoBehaviour
    {
        [Header("Obstacle Settings")]
        [SerializeField] protected int unitsKilledOnHit = 1;
        [SerializeField] protected bool isActive = true;

        [Header("Effects")]
        [SerializeField] protected ParticleSystem hitEffect;

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!isActive) return;
            if (other.CompareTag("Unit"))
            {
                var unit = other.GetComponent<Unit>();
                if (unit != null && unit.IsAlive)
                {
                    HitUnit(unit);
                }
            }
        }

        protected virtual void HitUnit(Unit unit)
        {
            CrowdManager.Instance?.RemoveUnits(unitsKilledOnHit);
            if (hitEffect != null)
                hitEffect.Play();
            Core.AudioManager.Instance?.PlayExplosion();
        }

        public void SetActive(bool active) => isActive = active;
        public void SetUnitsKilled(int count) => unitsKilledOnHit = count;
    }
}
