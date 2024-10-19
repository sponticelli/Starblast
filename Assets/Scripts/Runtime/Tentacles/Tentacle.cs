using Starblast.Extensions;
using UnityEngine;

namespace Starblast.Tentacles
{
    [ExecuteInEditMode]
    public partial class Tentacle : MonoBehaviour
    {
        private int VerticesCount => smoothness * 2 + tipCapSmoothness + pivotCapSmoothness;
        

        [SerializeField] private Color color = Color.white;
        [SerializeField] private Material material;
        [SerializeField] private UVsLayout textureType;
        [SerializeField] private int smoothness = 12;
        [SerializeField] private int pivotCapSmoothness = 0, tipCapSmoothness = 4;
        [SerializeField] private float width = 1.2f;
        [SerializeField] private AnimationCurve shape = AnimationCurve.Linear(0, 1f, 1f, .3f);

        /// <summary>
        /// The width multiplier of the tentacle's mesh
        /// </summary>
        public float Width { get => width; set { width = value; } }
        /// <summary>
        /// The shape of the tentacle's mesh
        /// </summary>
        public AnimationCurve Shape { get => shape; set { shape = value; } }
        /// <summary> 
        /// Tint color for the material 
        /// </summary> 
        public Color Color { get { return color; } set { color = value; materialBlock.SetColor("_Color", color); meshRenderer.SetPropertyBlock(materialBlock); } }


        //#region Collider
        [SerializeField] private int reduction = 2;


        //#region Behaviour
        public enum Animations { none, wave, swing }

        //[SerializeField] private Rigidbody2D parentRigidbody;
        //[SerializeField] private Vector2 parentBodyOffset;
        [SerializeField] private new Animations animation = Animations.wave;

        /// <summary>
        /// The length of the whole tentacle
        /// </summary>
        public float Length
        {
            get
            {
                var length = 0f;
                for (int i = 0; i < Joints.Length; i++)
                    length += Joints[i].distance;
                return length;
            }
            set
            {
                float segmentLength = value / Joints.Length;
                for (int i = 0; i < Joints.Length; i++)
                    Joints[i].distance = segmentLength;
            }
        }

        /// <summary>
        /// Tentacle will try to reach this Transform but won't be able to catch it
        /// </summary>
        public Transform TargetTransform { get => target; set { target = value; } }
        /// <summary>
        /// Tentacle will try reach this Rgidbody and will be able to catch it
        /// </summary>
        public Rigidbody2D TargetRigidbody { get => targetRigidbody; set { targetRigidbody = value; target = value?.transform; } }
        /// <summary>
        /// The type of an additional animation applied to this tentacle
        /// </summary>
        public Animations Animation { get { return animation; } set { animation = value; } }

        /// <summary>
        /// Parent rigidbody2d the tentacle is attached to
        /// </summary>
        public Rigidbody2D ParentRigidbody { get => pivotHingeJoint.connectedBody; set => pivotHingeJoint.connectedBody = value; }
        /// <summary>
        /// Local-space anchor from the parent rigidbody2d 
        /// </summary>
        public Vector2 ParentBodyOffset { get => pivotHingeJoint.connectedAnchor; set => pivotHingeJoint.connectedAnchor = value; }

        // Debug 
#if UNITY_EDITOR
        [SerializeField] private bool debugOutline, debugTriangles, debugUVs;
#endif

        private void Awake()
        {
            Initialize();
            InitializeMesh();
            InitializeUVs();
        }

        private void Update()
        {
            if (meshRenderer.isVisible)
            {
                DefineVertices();
                RenderMesh();

#if UNITY_EDITOR
                DrawDebug();
                if (!Application.isPlaying) BuildCollider();
#endif
            }
            else
            {
                UpdateBounds(); // TODO: efficiency is really questionable
            }
        }

        private void FixedUpdate()
        {
            ReachTarget(); // TODO: fix this when invisible

            if (meshRenderer.isVisible)
            {
                ApplyBehaviour();
                BuildCollider();
            }
        }

        private void Initialize()
        {
            Pivot = transform.GetChild(0).GetComponent<Rigidbody2D>();
            InitializeSegments();
            InitializeJoints();
            polygonCollider = Tip.GetComponent<PolygonCollider2D>();
        }

#if UNITY_EDITOR
        private UVsLayout storedTextureType;

#else
        //private Mesh mesh;

        private void RenderMesh()
        {
            var meshVertices = new Vector3[vertices.Length];

            var z = transform.position.z;
            for (int i = 0; i < vertices.Length; i++)
                meshVertices[i] = Pivot.transform.InverseTransformPoint(new Vector3(vertices[i].x, vertices[i].y, z));
            meshFilter.sharedMesh.vertices = meshVertices;

            int index = 0, length = (vertices.Length - (vertices.Length % 2 == 0 ? 2 : 1)) * 3;
            var triangles = new int[length];
            for (int i = 0; i < length; i += 6)
            {
                triangles[i] = index;
                triangles[i + 1] = vertices.Length - 2 - index;
                triangles[i + 2] = vertices.Length - 1 - index;

                triangles[i + 3] = index;
                triangles[i + 4] = index + 1;
                triangles[i + 5] = vertices.Length - 2 - index;
                index++;
            }
            meshFilter.sharedMesh.triangles = triangles;

            if (renewUVs)
            {
                meshFilter.sharedMesh.uv = uvs;
                if (textureType == UVsLayout.stretchy) renewUVs = false;
            }
        }
#endif

        private void UpdateBounds()
        {
            if (meshFilter.sharedMesh == null) return;

            meshFilter.sharedMesh.RecalculateBounds();

            var bounds = meshFilter.sharedMesh.bounds;
            bounds.Encapsulate(transform.InverseTransformPoint(Tip.position));
            bounds.Expand(width * shape.Evaluate(1f) * 2.5f);
            meshFilter.sharedMesh.bounds = bounds;
        }

#if UNITY_EDITOR
        private void DrawDebug()
        {
            if (debugTriangles)
            {
                for (int i = 0; i < vertices.Length - 2; i++)
                {
                    Debug.DrawLine(vertices[i], vertices[vertices.Length - 2 - i], Color.magenta);
                    Debug.DrawLine(vertices[i], vertices[vertices.Length - 1 - i], Color.magenta);
                }
            }

            if (debugOutline || debugTriangles)
                for (int i = 0; i < vertices.Length; i++) Debug.DrawLine(vertices[i], i >= vertices.Length - 1 ? vertices[0] : vertices[i + 1], Color.magenta);

            if (debugUVs)
                for (int i = 0; i < uvs.Length; i++) Debug.DrawLine(uvs[i], i >= uvs.Length - 1 ? uvs[0] : uvs[i + 1], Color.cyan);
        }

        private void OnDestroy()
        {
            if (meshFilter.sharedMesh != null)
                DestroyImmediate(meshFilter.sharedMesh);
        }

        private void OnDrawGizmos()
        {
            if (meshRenderer != null)
            {
                var bounds2 = meshRenderer.bounds;
                Gizmos.color = Color.gray;
                Gizmos.DrawWireCube(bounds2.center, bounds2.size);
            }
        }
#endif

        private struct Utilities
        {
            public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle) => Quaternion.Euler(new Vector3(0, 0, angle)) * (point - pivot) + pivot;

            public static float Normalize(float value, float max, float min = 0) => (value - min) / (max - min);

            public static Vector2 GetPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
            {
                float u = 1 - t;
                return u * u * u * p0 + 3 * u * u * t * p1 + 3 * u * t * t * p2 + t * t * t * p3;
            }

            public static Vector2 GetPerpendicular(Vector2 from, Vector2 to, float side = 1f)
            {
                var direction = from - to;
                return new Vector2(direction.y * side, -direction.x * side);
            }
        }
    }
}