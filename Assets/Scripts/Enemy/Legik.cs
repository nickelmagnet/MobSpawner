using UnityEngine;

namespace MobSpawner.Enemy
{
    /// <summary>
    /// Thin glue: reads the hip/target/pole positions, asks TwoBoneIK for the knee and foot,
    /// then places two cylinder visuals along hip->knee and knee->foot.
    /// </summary>
    public class LegIK : MonoBehaviour
    {
        [Header("IK inputs")]
        [SerializeField] private Transform target;
        [SerializeField] private Transform pole;
        [SerializeField] private float upperLength = 1.5f;
        [SerializeField] private float lowerLength = 1.5f;

        [Header("Visuals (default Unity cylinders, colliders removed)")]
        [SerializeField] private Transform upperVisual;
        [SerializeField] private Transform lowerVisual;
        [SerializeField] private float thickness = 0.15f;

        private Vector3 knee;
        private Vector3 foot;

        // LateUpdate: runs after all Updates, so the target has already moved this frame.
        private void LateUpdate()
        {
            if (target == null || pole == null) return;

            TwoBoneIK.Solve(
                transform.position, target.position, pole.position,
                upperLength, lowerLength,
                out knee, out foot);

            PlaceSegment(upperVisual, transform.position, knee);
            PlaceSegment(lowerVisual, knee, foot);
        }

        //scale.y = length / 2, so y=1 
        private void PlaceSegment(Transform segment, Vector3 from, Vector3 to)
        {
            if (segment == null) return;

            Vector3 delta = to - from;
            float length = delta.magnitude;
            if (length < 0.0001f) return;

            segment.position = (from + to) * 0.5f;
            segment.rotation = Quaternion.FromToRotation(Vector3.up, delta / length);
            segment.localScale = new Vector3(thickness, length * 0.5f, thickness);
        }

        // Debug drawing
        private void OnDrawGizmos()
        {
            if (target == null || pole == null) return;

            TwoBoneIK.Solve(
                transform.position, target.position, pole.position,
                upperLength, lowerLength,
                out Vector3 k, out Vector3 f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, k);
            Gizmos.DrawLine(k, f);
            Gizmos.DrawWireSphere(k, 0.08f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(f, 0.1f);
        }
    }
}