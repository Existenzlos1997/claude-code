using UnityEngine;

namespace EarthUnderFreelancer.Customization
{
    // Iteration 31-32: Aircraft appearance customization
    public class AircraftCustomization : MonoBehaviour
    {
        private Color primaryColor = Color.gray;
        private Color secondaryColor = Color.white;
        private string decalName = "None";
        
        public void SetPrimaryColor(Color color)
        {
            primaryColor = color;
            ApplyColors();
        }
        
        public void SetDecal(string decal)
        {
            decalName = decal;
            Debug.Log($"[Custom] Applied decal: {decal}");
        }
        
        private void ApplyColors()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                r.material.color = primaryColor;
            }
        }
    }
}
