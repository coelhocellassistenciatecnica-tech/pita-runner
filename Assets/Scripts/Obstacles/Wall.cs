using UnityEngine;

namespace PitaRunner.Obstacles
{
    public class Wall : ObstacleBase
    {
        [Header("Wall Settings")]
        [SerializeField] private int durability = 5;
        [SerializeField] private bool breakable = true;
        [SerializeField] private GameObject breakEffect;

        private int hitsRemaining;

        private void Start() => hitsRemaining = durability;

        protected override void HitUnit(Crowd.Unit unit)
        {
            base.HitUnit(unit);
            if (!breakable) return;
            hitsRemaining--;
            if (hitsRemaining <= 0) Break();
        }

        private void Break()
        {
            if (breakEffect != null) Instantiate(breakEffect, transform.position, Quaternion.identity);
            Effects.ParticleManager.Instance?.SpawnExplosion(transform.position);
            gameObject.SetActive(false);
        }
    }
}
