using UnityEngine;

namespace EarthUnderFreelancer.Debug
{
    // Iteration 76-77: Runtime entity inspector
    public class EntityInspector : MonoBehaviour
    {
        private GameObject selectedObject;
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(2)) // Middle mouse
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    selectedObject = hit.collider.gameObject;
                    Debug.Log($"[Inspector] Selected: {selectedObject.name}");
                }
            }
        }
    }
}
