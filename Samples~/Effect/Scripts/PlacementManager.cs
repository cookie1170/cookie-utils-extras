using System;
using CookieUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = System.Diagnostics.Debug;

namespace Samples.Juice
{
    public class PlacementManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject squarePrefab;

        private Camera cam;
        private InputAction action;

        private void Awake()
        {
            cam = Camera.main;
            action = new(binding: Mouse.current.leftButton.path);
            action.performed += OnActionPerformed;
            action.Enable();
        }

        private void OnActionPerformed(InputAction.CallbackContext context)
        {
            Debug.Assert(cam != null, nameof(cam) + " != null");
            Vector3 position = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue())
                .With(z: 0);
            Collider2D result = Physics2D.OverlapCircle(position, 0.5f);
            if (!result)
                Instantiate(squarePrefab, position, Quaternion.identity);
        }

        private void OnDestroy()
        {
            action.performed -= OnActionPerformed;
            action.Dispose();
        }
    }
}
