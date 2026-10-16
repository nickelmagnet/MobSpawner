using UnityEngine;
using UnityEngine.InputSystem;

namespace MobSpawner.Player
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            Vector3 dir = Vector3.zero;
            if (kb.wKey.isPressed) { dir.z += 1f;}
            if (kb.sKey.isPressed) { dir.z -= 1f;}
            if (kb.dKey.isPressed) { dir.x += 1f;}
            if (kb.aKey.isPressed) { dir.x -= 1f;}

            if (dir.sqrMagnitude > 1f) dir.Normalize();
            transform.position += dir * (moveSpeed * Time.deltaTime);
        }
    }
}
