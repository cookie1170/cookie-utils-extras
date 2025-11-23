using CookieUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = System.Diagnostics.Debug;

namespace Samples.Juice
{
    public class PlacementManager : MonoBehaviour
    {
        [SerializeField] private GameObject squarePrefab;

        private void Awake() {
            InputAction action = new(binding: Mouse.current.leftButton.path);
            Camera cam = Camera.main;
            action.performed += _ => {
                Debug.Assert(cam != null, nameof(cam) + " != null");
                Vector3 position = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue()).With(z: 0);
                Collider2D result = Physics2D.OverlapCircle(position, 0.5f);
                if (!result)
                    Instantiate(
                        squarePrefab,
                        position, Quaternion.identity
                    );
            };
            action.Enable();
        }
    }
}