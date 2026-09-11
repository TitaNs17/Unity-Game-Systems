using UnityEngine;
using UnityEngine.AI;
using UnityGameSystems.Combat;

namespace UnityGameSystems.EnemyAI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [DisallowMultipleComponent]
    public sealed class EnemyBrain : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField, Min(0.1f)] private float detectionRange = 12f;
        [SerializeField, Min(0.1f)] private float attackRange = 1.8f;
        [SerializeField, Min(0.01f)] private float attackDamage = 10f;
        [SerializeField, Min(0.05f)] private float attackCooldown = 1f;
        [SerializeField] private bool requireLineOfSight = true;
        [SerializeField] private LayerMask lineOfSightMask = ~0;
        [SerializeField] private Animator animator;

        private NavMeshAgent agent;
        private float nextAttackTime;
        private Health ownHealth;

        public Transform Target => target;
        public bool HasTarget => target != null;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            ownHealth = GetComponent<Health>();

            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            if (ownHealth != null)
                ownHealth.Died += HandleDeath;
        }

        private void OnDisable()
        {
            if (ownHealth != null)
                ownHealth.Died -= HandleDeath;
        }

        private void Update()
        {
            if (ownHealth != null && ownHealth.IsDead)
                return;

            if (target == null)
            {
                agent.isStopped = true;
                UpdateAnimator(false);
                return;
            }

            var sqrDistance = (target.position - transform.position).sqrMagnitude;
            var detectionSqr = detectionRange * detectionRange;

            if (sqrDistance > detectionSqr)
            {
                agent.isStopped = true;
                UpdateAnimator(false);
                return;
            }

            var attackSqr = attackRange * attackRange;
            if (sqrDistance <= attackSqr && HasLineOfSight())
            {
                agent.isStopped = true;
                FaceTarget();
                UpdateAnimator(false);
                TryAttack();
                return;
            }

            agent.isStopped = false;
            agent.stoppingDistance = Mathf.Max(0.05f, attackRange * 0.85f);
            agent.SetDestination(target.position);
            UpdateAnimator(agent.velocity.sqrMagnitude > 0.01f);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void TryAttack()
        {
            if (Time.time < nextAttackTime)
                return;

            var targetHealth = target.GetComponentInParent<Health>();
            if (targetHealth == null)
                return;

            nextAttackTime = Time.time + attackCooldown;
            targetHealth.TakeDamage(attackDamage);

            if (animator != null)
                animator.SetTrigger("Attack");
        }

        private bool HasLineOfSight()
        {
            if (!requireLineOfSight)
                return true;

            var origin = transform.position + Vector3.up * 1.2f;
            var destination = target.position + Vector3.up;
            var direction = destination - origin;

            if (!Physics.Raycast(origin, direction.normalized, out var hit, direction.magnitude, lineOfSightMask, QueryTriggerInteraction.Ignore))
                return true;

            return hit.transform == target || hit.transform.IsChildOf(target);
        }

        private void FaceTarget()
        {
            var direction = target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f)
                return;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
        }

        private void UpdateAnimator(bool moving)
        {
            if (animator != null)
                animator.SetBool("Moving", moving);
        }

        private void HandleDeath()
        {
            agent.isStopped = true;
            agent.enabled = false;

            if (animator != null)
                animator.SetTrigger("Die");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
