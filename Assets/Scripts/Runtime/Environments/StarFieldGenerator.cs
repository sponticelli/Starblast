using System.Collections.Generic;
using UnityEngine;

namespace Starblast.Environments
{
    public class StarFieldGenerator : MonoBehaviour
    {
        // Public variables to set in the Unity Inspector
        [Header("Star Field Settings")]
        public float width = 10f;
        public float height = 10f;
        public int numberOfStars = 100;
        public List<GameObject> prefabs;

        private void Start()
        {
            GenerateStarField();
        }

        void GenerateStarField()
        {
            // Create a parent GameObject to hold the stars
            GameObject starField = new GameObject("StarField (0, 0)");
            starField.transform.SetParent(transform);

            // Generate stars in the central area centered at (0,0)
            for (int i = 0; i < numberOfStars; i++)
            {
                // Random position within the area
                Vector2 position = new Vector2(
                    Random.Range(-width / 2f, width / 2f),
                    Random.Range(-height / 2f, height / 2f)
                );

                // Randomly select a prefab from the list
                GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];

                // Instantiate the star as a child of the starField GameObject
                Instantiate(prefab, position, Quaternion.identity, starField.transform);
            }

            // Create copies of the star field at adjacent positions
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // Skip the central area
                    if (x == 0 && y == 0) continue;

                    // Calculate the offset position
                    Vector2 offset = new Vector2(x * width, y * height);

                    // Instantiate a copy of the star field
                    GameObject copy = Instantiate(starField);
                    copy.transform.SetParent(transform);
                    copy.transform.position = offset;
                    copy.name = $"StarField ({x}, {y})";
                }
            }
        }
    }
}