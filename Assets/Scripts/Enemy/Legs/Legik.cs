using UnityEngine;

namespace MobSpawner.Enemy
{
    [ExecuteAlways] public class LegIK : MonoBehaviour
    {
        [Header("IK inputs")]
        [SerializeField] private Transform target;
        [SerializeField] private Transform pole;
        [SerializeField] private float upperLength = 1.5f;
        [SerializeField] private float middleLength = 1.5f;
        [SerializeField] private float lowerLength = 1.5f;

        [Header("Visuals (default Unity cylinders, colliders removed)")]
        [SerializeField] private Transform upperVisual;
        [SerializeField] private Transform middleVisual;
        [SerializeField] private Transform lowerVisual;
        [SerializeField] private float thickness = 0.15f;

        private Vector3 knee;
        private Vector3 ankle;
        private Vector3 foot;

        private void LateUpdate()
        {
            if (target == null || pole == null) return;

            TwoBoneIK.Solve(
                transform.position, target.position, pole.position, Vector2.up,
                upperLength, middleLength, lowerLength,
                out knee,out ankle, out foot);

            PlaceSegment(upperVisual, transform.position, knee);
            PlaceSegment(middleVisual, knee, ankle);
            PlaceSegment(lowerVisual, ankle, foot);
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
    }
}