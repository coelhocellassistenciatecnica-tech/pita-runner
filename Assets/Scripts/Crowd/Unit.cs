using UnityEngine;
using PitaRunner.Core;

namespace PitaRunner.Crowd
{
    public class Unit : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 0.8f;

        [Header("Battle Movement")]
        [SerializeField] private float battleMoveSpeed = 5f;

        private float currentHealth;
        private float attackTimer = 0f;
        private bool isInBattle = false;
        private Unit attackTarget;
        private Animator animator;
        private Renderer unitRenderer;
        private MaterialPropertyBlock propBlock;

        public bool IsAlive => currentHealth > 0;
        public float Damage => damage;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            unitRenderer = GetComponentInChildren<Renderer>();
            propBlock = new MaterialPropertyBlock();
        }

        public void Initialize(float health, float dmg)
        {
            maxHealth = health;
            currentHealth = health;
            damage = dmg;
            isInBattle = false;
            attackTarget = null;
            attackTimer = 0f;
            gameObject.SetActive(true);

            if (animator != null)
                animator.SetBool("IsRunning", true);
        }

        public void SetColor(Color color)
        {
            if (unitRenderer != null)
            {
                unitRenderer.GetPropertyBlock(propBlock);
                propBlock.SetColor("_Color", color);
                unitRenderer.SetPropertyBlock(propBlock);
            }
        }

        private void Update()
        {
            if (!IsAlive || !isInBattle) return;

            attackTimer -= Time.deltaTime;

            if (attackTarget == null || !attackTarget.IsAlive)
            {
                attackTarget = CrowdManager.Instance?.FindNearestEnemy(transform.position);
            }

            if (attackTarget != null && attackTarget.IsAlive)
            {
                float dist = Vector3.Distance(transform.position, attackTarget.transform.position);
                if (dist > attackRange)
                {
                    Vector3 dir = (attackTarget.transform.position - transform.position).normalized;
                    transform.position += dir * battleMoveSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.LookRotation(dir);
                }
                else if (attackTimer <= 0f)
                {
                    Attack();
                }
            }
            else
            {
                MoveTowardsEnemyBase();
            }
        }

        private void Attack()
        {
            if (attackTarget == null) return;
            attackTimer = attackCooldown;
            attackTarget.TakeDamage(damage);
            AudioManager.Instance?.PlayAttack();

            if (animator != null)
                animator.SetTrigger("Attack");

            Effects.ParticleManager.Instance?.SpawnHitEffect(attackTarget.transform.position);
            Effects.DamageNumberPool.Instance?.Show(attackTarget.transform.position + Vector3.up, (int)damage);
        }

        private void MoveTowardsEnemyBase()
        {
            var enemyBase = BattleManager.Instance?.EnemyBase;
            if (enemyBase == null) return;
            Vector3 dir = (enemyBase.transform.position - transform.position).normalized;
            transform.position += dir * battleMoveSpeed * Time.deltaTime;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                Effects.ParticleManager.Instance?.SpawnHitEffect(transform.position);
            }
        }

        private void Die()
        {
            currentHealth = 0;
            isInBattle = false;
            AudioManager.Instance?.PlayUnitDie();
            Effects.ParticleManager.Instance?.SpawnDeathEffect(transform.position);

            if (animator != null)
                animator.SetTrigger("Die");

            CrowdManager.Instance?.OnUnitDied(this);
            gameObject.SetActive(false);
        }

        public void EnterBattle()
        {
            isInBattle = true;
            if (animator != null)
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("InBattle", true);
            }
        }

        public void SetDamage(float dmg) => damage = dmg;
        public void SetHealth(float hp) { maxHealth = hp; currentHealth = hp; }
    }
}
