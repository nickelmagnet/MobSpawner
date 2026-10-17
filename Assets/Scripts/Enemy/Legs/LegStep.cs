using UnityEngine;
using UnityEngine.AI;

namespace MobSpawner.Enemy
{
    /// <summary>
    /// Moves one leg's IK target: it stays planted in world space while the body moves,
    /// and when the body gets too far away it takes a step to a new spot.
    /// LegIK then bends the leg to reach wherever this target is.
    /// </summary>
    public class LegStepper : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform target;           // the IK foot target (same one LegIK uses)

        [Header("Stepping")]
        [SerializeField] private float stepDistance = 1.2f;  // how far the foot may trail behind home before stepping
        [SerializeField] private float stepDuration = 0.25f; // seconds per step
        [SerializeField] private float stepHeight = 0.4f;    // how high the foot lifts
        [SerializeField] private float overshootTime = 0.15f;// aim this many seconds ahead of the body's velocity

        [Header("Gait")]
        [Tooltip("This leg won't start a step while any of these legs is mid-step.")]
        [SerializeField] private LegStepper[] mustBePlanted;

        [SerializeField] private LayerMask groundMask = ~0;

        public bool IsStepping { get; private set; }

        private NavMeshAgent agent;
        private Transform home;   // where the foot "wants" to be; rides along with the body
        private Vector3 stepFrom;
        private Vector3 stepTo;
        private float stepTime;

        private void Awake()
        {
            agent = GetComponentInParent<NavMeshAgent>();

            // 'home' is a marker that stays attached to the body at the foot's starting spot.
            home = new GameObject(name + "_Home").transform;
            home.SetParent(target.parent);
            home.position = target.position;

            // Detach the target so it stays in world space instead of riding along with the body.
            target.SetParent(null);
            target.position = GroundPoint(target.position);
        }

        private void Update()
        {
            if (IsStepping)
            {
                AdvanceStep();
                return;
            }

            if (FlatDistance(target.position, home.position) < stepDistance) return;

            foreach (LegStepper other in mustBePlanted)
                if (other != null && other.IsStepping) return;

            BeginStep();
        }

        private void BeginStep()
        {
            Vector3 velocity = agent != null ? agent.velocity : Vector3.zero;
            velocity.y = 0f;

            stepFrom = target.position;
            stepTo = GroundPoint(home.position + velocity * overshootTime);
            stepTime = 0f;
            IsStepping = true;
        }

        private void AdvanceStep()
        {
            stepTime += Time.deltaTime / stepDuration;
            float k = Mathf.Clamp01(stepTime);

            Vector3 p = Vector3.Lerp(stepFrom, stepTo, k);
            p.y += Mathf.Sin(k * Mathf.PI) * stepHeight; // arc: 0 at both ends, peak in the middle
            target.position = p;

            if (k >= 1f)
            {
                target.position = stepTo;
                IsStepping = false;
            }
        }

        private Vector3 GroundPoint(Vector3 p)
        {
            if (Physics.Raycast(p + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 6f, groundMask))
                return hit.point;
            return p;
        }

        private static float FlatDistance(Vector3 a, Vector3 b)
        {
            a.y = 0f;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }
    }
}