using UnityEngine;
using PitaRunner.Core;
using PitaRunner.Crowd;

namespace PitaRunner.Battle
{
    public class EnemyUnit : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private float damage = 8f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float moveSpeed = 4f;

        private float currentHealth;
        private float attackTimer = 0f;
        private Unit attackTarget;
        private BattleManager battleManager;
        private bool isInBattle = false;
        private Animator animator;

        public bool IsAlive => currentHealth > 0;

        private void Awake() => animator = GetComponentInChildren<Animator>();

        public void Initialize(float hp, float dmg, BattleManager manager)
        {
            maxHealth = hp;
            currentHealth = hp;
            damage = dmg;
            battleManager = manager;
        }

        public void EnterBattle()
        {
            isInBattle = true;
            if (animator != null) animator.SetBool("InBattle", true);
        }

        private void Update()
        {
            if (!IsAlive || !isInBattle) return;
            attackTimer -= Time.deltaTime;

            if (attackTarget == null || !attackTarget.IsAlive)
                attackTarget = battleManager?.FindNearestPlayerUnit(transform.position);

            if (attackTarget != null && attackTarget.IsAlive)
            {
                float dist = Vector3.Distance(transform.position, attackTarget.transform.position);
                if (dist > attackRange)
                {
                    Vector3 dir = (attackTarget.transform.position - transform.position).normalized;
                    transform.position += dir * moveSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.LookRotation(dir);
                }
                else if (attackTimer <= 0f)
                {
                    Attack();
                }
            }
            else
            {
                MoveTowardsPlayerBase();
            }
        }

        private void Attack()
        {
            if (attackTarget == null) return;
            attackTimer = attackCooldown;
            attackTarget.TakeDamage(damage);
            AudioManager.Instance?.PlayAttack();
            if (animator != null) animator.SetTrigger("Attack");
            Effects.ParticleManager.Instance?.SpawnHitEffect(attackTarget.transform.position);
            Effects.DamageNumberPool.Instance?.Show(attackTarget.transform.position + Vector3.up, (int)damage, false);
        }

        private void MoveTowardsPlayerBase()
        {
            var playerBase = BattleManager.Instance?.PlayerBase;
            if (playerBase == null) return;
            Vector3 dir = (playerBase.transform.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;
            currentHealth -= amount;
            Effects.ParticleManager.Instance?.SpawnHitEffect(transform.position);
            if (currentHealth <= 0) Die();
        }

        private void Die()
        {
            currentHealth = 0;
            isInBattle = false;
            AudioManager.Instance?.PlayUnitDie();
            Effects.ParticleManager.Instance?.SpawnDeathEffect(transform.position);
            if (animator != null) animator.SetTrigger("Die");
            battleManager?.OnEnemyDied(this);
            Destroy(gameObject, 1f);
        }
    }
}
