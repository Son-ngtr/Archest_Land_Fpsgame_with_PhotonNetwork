using UnityEngine;

public class SafeMouseInput : MonoBehaviour
{
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        // Check if mouse position is within screen bounds
        if (mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height)
        {
            Debug.LogWarning("Mouse position out of screen bounds: " + mousePos);
            return;
        }

        // If the mouse position is valid, proceed with your logic
        // For example, you might want to cast a ray from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Process the hit
            Debug.Log("Hit: " + hit.collider.gameObject.name);
        }
    }
}
