using UnityEngine;
using UnityEngine.AI;

namespace MobSpawner.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyBrain : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float detectRange = 15f;
        [SerializeField] private float attackRange = 3f;
        [SerializeField] private float attackExitBuffer = 0.5f; // stops flickering at the boundary

        public NavMeshAgent Agent { get; private set; }
        public Transform Target => target;
        public float AttackRange => attackRange;
        public float AttackExitRange => attackRange + attackExitBuffer;
        public float DetectRange => detectRange;

        public float DistanceToTarget =>
            Vector3.Distance(transform.position, target.position);

        // Placeholder: later this asks for eyes instead.
        public bool CanSeeTarget => DistanceToTarget <= detectRange;

        public IdleState Idle { get; private set; }
        public ChaseState Chase { get; private set; }
        public AttackState Attack { get; private set; }

        private StateMachine machine;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            machine = new StateMachine();
            Idle = new IdleState(this, machine);
            Chase = new ChaseState(this, machine);
            Attack = new AttackState(this, machine);
        }

        private void Start() => machine.ChangeState(Idle);
        private void Update() => machine.Tick();

        public void FaceTarget(float turnSpeed = 8f)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.001f) return;
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }
    }
}