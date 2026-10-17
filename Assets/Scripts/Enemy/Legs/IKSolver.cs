// <summary>
// Solves the two-bone inverse kinematics problem.
// </summary>
using UnityEngine;

namespace MobSpawner.Enemy
{
    public static class TwoBoneIK
    {
        private const float Epsilon = 0.0001f;
        public static void Solve(Vector3 hip, Vector3 target, Vector3 pole,Vector3 upvec,
            float u,float m, float l,
            out Vector3 knee, out Vector3 ankle, out Vector3 foot)
        {
            Vector3 up = upvec.sqrMagnitude > Epsilon ? upvec.normalized : Vector3.up;
            Vector3 toTarget = target - hip + l * up;
            float b = toTarget.magnitude;
            Vector3 dir = b > Epsilon ? toTarget / b : Vector3.forward;

            //knee can bend from 5 to 165 degrees
            float minReach = Mathf.Sqrt(u * u + m * m - 2f * u * m * Mathf.Cos(5f * Mathf.Deg2Rad));
            float maxReach = Mathf.Sqrt(u * u + m * m - 2f * u * m * Mathf.Cos(165f * Mathf.Deg2Rad));
            b = Mathf.Clamp(b, minReach, maxReach);

            //ABC is triangle with A=hip, B=knee, C=ankle, a=m, b=b, c=u
            float along = (u * u - m * m + b * b) / (2f * b); // c.cosA
            float height = Mathf.Sqrt(Mathf.Max(0f, u * u - along * along)); //c.sinA

            Vector3 bend = Vector3.ProjectOnPlane(pole - hip, dir);
            if (bend.sqrMagnitude < Epsilon) bend = Vector3.ProjectOnPlane(up, dir);
            if (bend.sqrMagnitude < Epsilon) bend = Vector3.ProjectOnPlane(Vector3.Cross(up, Vector3.forward), dir);
            bend.Normalize();

            knee = hip + dir * along + bend * height;
            ankle = hip + dir * b;
            foot = hip + dir * b - l * up;
        }
    }
}