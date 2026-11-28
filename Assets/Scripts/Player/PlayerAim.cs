using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAim : MonoBehaviour
{

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }


    private void Update()
    {
        if (Mouse.current == null || mainCamera == null)
            return;

        // creates a ray from camera through mouse cursor
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // creates horizontal plane at player's height
        Plane aimPlane = new Plane(Vector3.up, transform.position);

        if (aimPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);

            Vector3 direction = targetPoint - transform.position;

            // only rotate horizontally
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
