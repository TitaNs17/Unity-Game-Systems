using System;
using UnityEngine;
using UnityEngine.AI;
using UnityGameSystems.Combat;

namespace UnityGameSystems.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [DisallowMultipleComponent]
    public sealed class AdvancedNPCBrain : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PerceptionController perception;
        [SerializeField] private PatrolAgent patrol;
        [SerializeField] private Animator animator;

        [Header("Movement")]
        [SerializeField, Min(0.1f)] private float investigateStoppingDistance = 0.75f;
        [SerializeField, Min(0.1f)] private float chaseStoppingDistance = 1.6f;
        [SerializeField, Min(0.1f)] private float searchRadius = 4f;
        [SerializeField, Min(0.1f)] private float searchDuration = 5f;
        [SerializeField, Min(0.05f)] private float repathInterval = 0.2f;

        [Header("Combat")]
        [SerializeField, Min(0f)] private float attackRange = 1.8f;
        [SerializeField, Min(0f)] private float attackDamage = 15f;
        [SerializeField, Min(0.05f)] private float attackCooldown = 1f;
        [SerializeField] private bool requireLineOfSightToAttack = true;

        [Header("Animator")]
        [SerializeField] private string movingBool = "Moving";
        [SerializeField] private string alertBool = "Alert";
        [SerializeField] private string attackTrigger = "Attack";

        private NavMeshAgent agent;
        private AIStateMachine stateMachine;
        private Transform target;
        private Vector3 lastKnownPosition;
        private float nextAttackTime;
        private float nextRepathTime;

        public string CurrentStateName => stateMachine != null && stateMachine.Current != null
            ? stateMachine.Current.GetType().Name
            : string.Empty;

        public Transform Target => target;
        public Vector3 LastKnownPosition => lastKnownPosition;

        public event Action<string> StateChanged;
        public event Action<Transform> TargetChanged;

        private void Reset()
        {
            perception = GetComponent<PerceptionController>();
            patrol = GetComponent<PatrolAgent>();
            animator = GetComponentInChildren<Animator>();
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (perception == null) perception = GetComponent<PerceptionController>();
            if (patrol == null) patrol = GetComponent<PatrolAgent>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
            BuildStateMachine();
        }

        private void OnEnable()
        {
            if (perception != null)
            {
                perception.InterestChanged += HandleInterestChanged;
                perception.Alerted += HandleAlerted;
            }

            stateMachine?.Change<PatrolState>();
        }

        private void OnDisable()
        {
            if (perception != null)
            {
                perception.InterestChanged -= HandleInterestChanged;
                perception.Alerted -= HandleAlerted;
            }

            stateMachine?.Stop();
        }

        private void Update()
        {
            stateMachine?.Tick(Time.deltaTime);
            UpdateAnimator();
        }

        private void BuildStateMachine()
        {
            stateMachine = new AIStateMachine();
            stateMachine.Register(new PatrolState(this));
            stateMachine.Register(new InvestigateState(this));
            stateMachine.Register(new ChaseState(this));
            stateMachine.Register(new SearchState(this));
            stateMachine.Register(new AttackState(this));
            stateMachine.Changed += (_, next) => StateChanged?.Invoke(next != null ? next.GetType().Name : string.Empty);
        }

        private void HandleInterestChanged(Vector3 position)
        {
            lastKnownPosition = position;

            var visualTarget = perception != null ? perception.VisualTarget : null;
            if (visualTarget != null)
            {
                SetTarget(visualTarget);
                stateMachine.Change<ChaseState>();
                return;
            }

            if (target == null)
                stateMachine.Change<InvestigateState>();
        }

        private void HandleAlerted()
        {
            if (perception != null && perception.VisualTarget != null)
            {
                SetTarget(perception.VisualTarget);
                stateMachine.Change<ChaseState>();
            }
            else
            {
                stateMachine.Change<InvestigateState>();
            }
        }

        private void SetTarget(Transform value)
        {
            if (target == value) return;
            target = value;
            TargetChanged?.Invoke(target);
        }

        private bool CanNavigate()
        {
            return agent != null && agent.enabled && agent.isOnNavMesh;
        }

        private bool MoveTo(Vector3 destination, float stoppingDistance)
        {
            if (!CanNavigate()) return false;

            agent.stoppingDistance = Mathf.Max(0f, stoppingDistance);
            agent.isStopped = false;

            if (!NavMesh.SamplePosition(destination, out var hit, 2f, agent.areaMask))
                return false;

            return agent.SetDestination(hit.position);
        }

        private bool HasArrived(float tolerance = 0.1f)
        {
            if (!CanNavigate() || agent.pathPending) return false;
            if (agent.pathStatus == NavMeshPathStatus.PathInvalid) return true;
            return !agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + tolerance;
        }

        private void StopMoving()
        {
            if (!CanNavigate()) return;
            agent.isStopped = true;
            agent.ResetPath();
        }

        private bool HasVisualTarget()
        {
            return perception != null && perception.VisualTarget != null;
        }

        private bool IsTargetValid()
        {
            return target != null && target.gameObject.activeInHierarchy;
        }

        private bool InAttackRange()
        {
            if (!IsTargetValid()) return false;
            return (target.position - transform.position).sqrMagnitude <= attackRange * attackRange;
        }

        private bool CanAttackTarget()
        {
            if (!InAttackRange()) return false;
            if (!requireLineOfSightToAttack) return true;

            var vision = perception != null ? perception.GetComponent<VisionSensor>() : null;
            return vision == null || vision.CanSee(target);
        }

        private void DealDamage()
        {
            if (!IsTargetValid()) return;

            var health = target.GetComponentInParent<Health>();
            if (health == null)
                health = target.GetComponentInChildren<Health>();

            if (health != null)
                health.TakeDamage(attackDamage);
        }

        private Vector3 RandomSearchPoint()
        {
            if (!CanNavigate()) return lastKnownPosition;

            var offset = UnityEngine.Random.insideUnitSphere * searchRadius;
            offset.y = 0f;
            var candidate = lastKnownPosition + offset;

            return NavMesh.SamplePosition(candidate, out var hit, searchRadius, agent.areaMask)
                ? hit.position
                : lastKnownPosition;
        }

        private void UpdateAnimator()
        {
            if (animator == null) return;

            var moving = CanNavigate() && !agent.isStopped && agent.velocity.sqrMagnitude > 0.01f;
            if (!string.IsNullOrWhiteSpace(movingBool)) animator.SetBool(movingBool, moving);
            if (!string.IsNullOrWhiteSpace(alertBool)) animator.SetBool(alertBool, perception != null && perception.IsAlerted);
        }

        private sealed class PatrolState : IAIState
        {
            private readonly AdvancedNPCBrain owner;
            public PatrolState(AdvancedNPCBrain owner) => this.owner = owner;

            public void Enter()
            {
                owner.SetTarget(null);
                if (owner.patrol != null)
                    owner.patrol.StartPatrol();
            }

            public void Tick(float deltaTime)
            {
                if (owner.HasVisualTarget())
                {
                    owner.SetTarget(owner.perception.VisualTarget);
                    owner.lastKnownPosition = owner.target.position;
                    owner.stateMachine.Change<ChaseState>();
                    return;
                }

                if (owner.perception != null && owner.perception.IsAlerted)
                    owner.stateMachine.Change<InvestigateState>();
            }

            public void Exit()
            {
                if (owner.patrol != null)
                    owner.patrol.StopPatrol();
            }
        }

        private sealed class InvestigateState : IAIState
        {
            private readonly AdvancedNPCBrain owner;
            public InvestigateState(AdvancedNPCBrain owner) => this.owner = owner;

            public void Enter()
            {
                owner.MoveTo(owner.lastKnownPosition, owner.investigateStoppingDistance);
            }

            public void Tick(float deltaTime)
            {
                if (owner.HasVisualTarget())
                {
                    owner.SetTarget(owner.perception.VisualTarget);
                    owner.lastKnownPosition = owner.target.position;
                    owner.stateMachine.Change<ChaseState>();
                    return;
                }

                if (owner.HasArrived())
                    owner.stateMachine.Change<SearchState>();
            }

            public void Exit() { }
        }

        private sealed class ChaseState : IAIState
        {
            private readonly AdvancedNPCBrain owner;
            public ChaseState(AdvancedNPCBrain owner) => this.owner = owner;

            public void Enter()
            {
                owner.nextRepathTime = 0f;
            }

            public void Tick(float deltaTime)
            {
                if (!owner.IsTargetValid())
                {
                    owner.SetTarget(null);
                    owner.stateMachine.Change<SearchState>();
                    return;
                }

                if (owner.HasVisualTarget())
                    owner.lastKnownPosition = owner.target.position;
                else
                {
                    owner.SetTarget(null);
                    owner.stateMachine.Change<InvestigateState>();
                    return;
                }

                if (owner.CanAttackTarget())
                {
                    owner.stateMachine.Change<AttackState>();
                    return;
                }

                if (Time.time >= owner.nextRepathTime)
                {
                    owner.nextRepathTime = Time.time + owner.repathInterval;
                    owner.MoveTo(owner.target.position, owner.chaseStoppingDistance);
                }
            }

            public void Exit() { }
        }

        private sealed class AttackState : IAIState
        {
            private readonly AdvancedNPCBrain owner;
            public AttackState(AdvancedNPCBrain owner) => this.owner = owner;

            public void Enter()
            {
                owner.StopMoving();
            }

            public void Tick(float deltaTime)
            {
                if (!owner.IsTargetValid())
                {
                    owner.SetTarget(null);
                    owner.stateMachine.Change<SearchState>();
                    return;
                }

                if (!owner.HasVisualTarget())
                {
                    owner.lastKnownPosition = owner.target.position;
                    owner.SetTarget(null);
                    owner.stateMachine.Change<InvestigateState>();
                    return;
                }

                if (!owner.CanAttackTarget())
                {
                    owner.stateMachine.Change<ChaseState>();
                    return;
                }

                var flatDirection = owner.target.position - owner.transform.position;
                flatDirection.y = 0f;
                if (flatDirection.sqrMagnitude > 0.001f)
                {
                    var desired = Quaternion.LookRotation(flatDirection.normalized);
                    owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, desired, deltaTime * 10f);
                }

                if (Time.time < owner.nextAttackTime)
                    return;

                owner.nextAttackTime = Time.time + owner.attackCooldown;
                if (owner.animator != null && !string.IsNullOrWhiteSpace(owner.attackTrigger))
                    owner.animator.SetTrigger(owner.attackTrigger);

                owner.DealDamage();
            }

            public void Exit() { }
        }

        private sealed class SearchState : IAIState
        {
            private readonly AdvancedNPCBrain owner;
            private float endTime;
            private float nextMoveTime;

            public SearchState(AdvancedNPCBrain owner) => this.owner = owner;

            public void Enter()
            {
                endTime = Time.time + owner.searchDuration;
                nextMoveTime = 0f;
                owner.SetTarget(null);
            }

            public void Tick(float deltaTime)
            {
                if (owner.HasVisualTarget())
                {
                    owner.SetTarget(owner.perception.VisualTarget);
                    owner.lastKnownPosition = owner.target.position;
                    owner.stateMachine.Change<ChaseState>();
                    return;
                }

                if (Time.time >= endTime)
                {
                    owner.stateMachine.Change<PatrolState>();
                    return;
                }

                if (Time.time >= nextMoveTime && owner.HasArrived(0.25f))
                {
                    nextMoveTime = Time.time + 0.5f;
                    owner.MoveTo(owner.RandomSearchPoint(), owner.investigateStoppingDistance);
                }
            }

            public void Exit() { }
        }
    }
}
