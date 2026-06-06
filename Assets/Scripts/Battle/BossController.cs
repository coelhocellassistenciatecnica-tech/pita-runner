using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using PitaRunner.Core;
using PitaRunner.Crowd;

namespace PitaRunner.Battle
{
    public enum BossType { GiantRobot, MechMonster, ArmoredTank }

    public class BossController : MonoBehaviour
    {
        [Header("Boss Settings")]
        [SerializeField] private BossType bossType = BossType.GiantRobot;
        [SerializeField] private float maxHealth = 2000f;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float attackRange = 3f;
        [SerializeField] private float attackDamage = 25f;
        [SerializeField] private float attackCooldown = 2f;

        [Header("Special Attacks")]
        [SerializeField] private float specialAttackCooldown = 8f;
        [SerializeField] private int specialAttackDamageUnits = 10;
        [SerializeField] private float specialAttackRadius = 4f;

        [Header("UI")]
        [SerializeField] private Slider bossHealthBar;
        [SerializeField] private TextMeshProUGUI bossNameText;
        [SerializeField] private Canvas bossHUDCanvas;

        [Header("Animation")]
        [SerializeField] private Animator animator;

        private float currentHealth;
        private float attackTimer = 0f;
        private float specialTimer = 0f;
        private BattleManager battleManager;
        private bool isActive = false;
        private bool dead = false;

        public bool IsDead => dead;

        public void Initialize(BattleManager manager)
        {
            battleManager = manager;
            currentHealth = maxHealth;
            isActive = true;
            UpdateBossUI();

            if (bossNameText != null)
                bossNameText.text = GetBossName();

            if (bossHUDCanvas != null)
                bossHUDCanvas.gameObject.SetActive(true);

            if (animator != null) animator.SetTrigger("Appear");
        }

        private string GetBossName() => bossType switch
        {
            BossType.GiantRobot => "MECA BOSS",
            BossType.MechMonster => "MONSTRO MECÂNICO",
            BossType.ArmoredTank => "TANQUE BLINDADO",
            _ => "BOSS"
        };

        private void Update()
        {
            if (!isActive || dead) return;

            attackTimer -= Time.deltaTime;
            specialTimer -= Time.deltaTime;

            var target = battleManager?.FindNearestPlayerUnit(transform.position);
            if (target != null && target.IsAlive)
            {
                float dist = Vector3.Distance(transform.position, target.transform.position);
                if (dist > attackRange)
                {
                    Vector3 dir = (target.transform.position - transform.position).normalized;
                    transform.position += dir * moveSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.LookRotation(dir);
                }
                else if (attackTimer <= 0f)
                {
                    NormalAttack(target);
                }
            }

            if (specialTimer <= 0f)
            {
                SpecialAttack();
                specialTimer = specialAttackCooldown;
            }
        }

        private void NormalAttack(Unit target)
        {
            attackTimer = attackCooldown;
            target.TakeDamage(attackDamage);
            if (animator != null) animator.SetTrigger("Attack");
            AudioManager.Instance?.PlayBossAttack();
            Effects.ParticleManager.Instance?.SpawnHitEffect(target.transform.position);
        }

        private void SpecialAttack()
        {
            if (animator != null) animator.SetTrigger("SpecialAttack");
            AudioManager.Instance?.PlayExplosion();
            Effects.ParticleManager.Instance?.SpawnExplosion(transform.position);
            CrowdManager.Instance?.RemoveUnits(specialAttackDamageUnits);

            var units = CrowdManager.Instance?.ActiveUnits;
            if (units == null) return;
            foreach (var u in units)
            {
                if (Vector3.Distance(u.transform.position, transform.position) <= specialAttackRadius)
                    u.TakeDamage(attackDamage * 1.5f);
            }
        }

        public void TakeDamage(float amount)
        {
            if (dead) return;
            currentHealth -= amount;
            currentHealth = Mathf.Max(0, currentHealth);
            UpdateBossUI();
            if (animator != null) animator.SetTrigger("Hit");
            Effects.DamageNumberPool.Instance?.Show(transform.position + Vector3.up * 2f, (int)amount);

            if (currentHealth <= 0) Die();
        }

        private void Die()
        {
            dead = true;
            isActive = false;
            if (animator != null) animator.SetTrigger("Die");
            AudioManager.Instance?.PlayExplosion();
            Effects.ParticleManager.Instance?.SpawnExplosion(transform.position);

            if (bossHUDCanvas != null) bossHUDCanvas.gameObject.SetActive(false);

            StartCoroutine(DieAndCleanup());
        }

        private IEnumerator DieAndCleanup()
        {
            yield return new WaitForSeconds(2f);
            battleManager?.CheckBattleEnd();
            Destroy(gameObject);
        }

        private void UpdateBossUI()
        {
            if (bossHealthBar != null)
                bossHealthBar.value = currentHealth / maxHealth;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Unit"))
            {
                var unit = other.GetComponent<Unit>();
                if (unit != null) unit.TakeDamage(attackDamage);
            }
        }
    }
}
