using UnityEngine;

namespace MobSpawner.Enemy
{
    public static class TwoBoneIK
    {
        private const float Epsilon = 0.0001f;
        public static void Solve(Vector3 hip, Vector3 target, Vector3 pole, float upperLength, float lowerLength,
            out Vector3 knee, out Vector3 foot)
        {
            Vector3 toTarget = target - hip;
            float l = toTarget.magnitude;
            Vector3 dir = l > Epsilon ? toTarget / l : Vector3.forward;

            float minReach = Mathf.Abs(upperLength - lowerLength) + Epsilon;
            float maxReach = upperLength + lowerLength - Epsilon;
            l = Mathf.Clamp(l, minReach, maxReach);
            //ABC is triangle with A=hip, B=knee, C=foot, a=lowerlength, b=l, c=upperlength
            float along = (upperLength * upperLength - lowerLength * lowerLength + l * l) / (2f * l); // c.cosA
            float height = Mathf.Sqrt(Mathf.Max(0f, upperLength * upperLength - along * along)); //c.sinA

            Vector3 bend = Vector3.ProjectOnPlane(pole - hip, dir);
            if (bend.sqrMagnitude < Epsilon) bend = Vector3.ProjectOnPlane(Vector3.up, dir);
            if (bend.sqrMagnitude < Epsilon) bend = Vector3.ProjectOnPlane(Vector3.right, dir);
            bend.Normalize();

            knee = hip + dir * along + bend * height;
            foot = hip + dir * l;
        }
    }
}