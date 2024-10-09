using UnityEngine;

namespace Starblast.Meshes
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class CircleOutline : MonoBehaviour
    {
        [Header("Circle Parameters")]
        [SerializeField] private float radius = 100f;     // Radius of the outer circle
        [SerializeField] private float width = 1f;        // Width of the ring (thickness of the circle outline)
        [SerializeField] private int segments = 128;       // Number of segments to approximate the circle
        [SerializeField] private string sortingLayerName = "Background";
        [SerializeField] private int sortingOrder = 0;
        
        private MeshRenderer _meshRenderer;
        
        public void SetRadius(float radius)
        {
            Debug.Log("Setting radius to " + radius);
            this.radius = radius;
        }

        private void Start()
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            mf.mesh = CreateRingMesh(radius, width, segments);
            
            if (_meshRenderer==null) _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.sortingLayerName = sortingLayerName;
            _meshRenderer.sortingOrder = sortingOrder;
        }

        private Mesh CreateRingMesh(float radius, float width, int segments)
        {
            Mesh mesh = new Mesh();

            int vertexCount = segments * 2;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[segments * 6];
            Vector2[] uv = new Vector2[vertexCount];

            float angleIncrement = (2f * Mathf.PI) / segments;
            float innerRadius = radius - width;

            for (int i = 0; i < segments; i++)
            {
                float angle = i * angleIncrement;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                // Outer vertex
                vertices[i * 2] = new Vector3(cos * radius, sin * radius, 0f);
                // Inner vertex
                vertices[i * 2 + 1] = new Vector3(cos * innerRadius, sin * innerRadius, 0f);

                // UV mapping (optional)
                uv[i * 2] = new Vector2((float)i / segments, 1f);
                uv[i * 2 + 1] = new Vector2((float)i / segments, 0f);

                // Triangles
                int nextI = (i + 1) % segments;
                // First triangle of the quad
                triangles[i * 6] = i * 2;
                triangles[i * 6 + 1] = nextI * 2;
                triangles[i * 6 + 2] = i * 2 + 1;
                // Second triangle of the quad
                triangles[i * 6 + 3] = nextI * 2;
                triangles[i * 6 + 4] = nextI * 2 + 1;
                triangles[i * 6 + 5] = i * 2 + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;

            // Recalculate normals for lighting
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public void SetColor(Color color)
        {
            if (_meshRenderer==null) _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.material.color = color;
        }
    }
}