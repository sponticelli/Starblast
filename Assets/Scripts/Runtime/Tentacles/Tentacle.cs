using Starblast.Extensions;
using UnityEngine;

namespace Starblast.Tentacles
{
    [ExecuteInEditMode]
    public partial class Tentacle : MonoBehaviour
    {
        public enum Animations
        {
            None,
            Wave,
            Swing
        }

        [SerializeField] private Color color = Color.white;
        [SerializeField] private Material material;
        [SerializeField] private UVsLayout textureType;
        [SerializeField] private int smoothness = 12;
        [SerializeField] private int pivotCapSmoothness = 0;
        [SerializeField] private int tipCapSmoothness = 4;
        [SerializeField] private float width = 1.2f;
        [SerializeField] private AnimationCurve shape = AnimationCurve.Linear(0, 1f, 1f, .3f);


        [SerializeField] private int reduction = 2;
        [SerializeField] private new Animations animation = Animations.Wave;

        // Debug 
#if UNITY_EDITOR
        [SerializeField] private bool debugOutline, debugTriangles, debugUVs;
#endif


        /// <summary>
        /// The width multiplier of the tentacle's mesh
        /// </summary>
        public float Width
        {
            get => width;
            set => width = value;
        }

        /// <summary>
        /// The shape of the tentacle's mesh
        /// </summary>
        public AnimationCurve Shape
        {
            get => shape;
            set => shape = value;
        }

        /// <summary> 
        /// Tint color for the material 
        /// </summary> 
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                materialBlock.SetColor("_Color", color);
                meshRenderer.SetPropertyBlock(materialBlock);
            }
        }

        /// <summary>
        /// The length of the whole tentacle
        /// </summary>
        public float Length
        {
            get
            {
                var length = 0f;
                for (int i = 0; i < Joints.Length; i++)
                {
                    length += Joints[i].distance;
                }

                return length;
            }
            set
            {
                float segmentLength = value / Joints.Length;
                for (int i = 0; i < Joints.Length; i++)
                {
                    Joints[i].distance = segmentLength;
                }
            }
        }

        /// <summary>
        /// Tentacle will try to reach this Transform but won't be able to catch it
        /// </summary>
        public Transform TargetTransform
        {
            get => target;
            set => target = value;
        }

        /// <summary>
        /// Tentacle will try reach this Rgidbody and will be able to catch it
        /// </summary>
        public Rigidbody2D TargetRigidbody
        {
            get => targetRigidbody;
            set
            {
                targetRigidbody = value;
                target = value?.transform;
            }
        }

        /// <summary>
        /// The type of an additional animation applied to this tentacle
        /// </summary>
        public Animations Animation
        {
            get => animation;
            set => animation = value;
        }

        /// <summary>
        /// Parent rigidbody2d the tentacle is attached to
        /// </summary>
        public Rigidbody2D ParentRigidbody
        {
            get => pivotHingeJoint.connectedBody;
            set => pivotHingeJoint.connectedBody = value;
        }

        /// <summary>
        /// Local-space anchor from the parent rigidbody2d 
        /// </summary>
        public Vector2 ParentBodyOffset
        {
            get => pivotHingeJoint.connectedAnchor;
            set => pivotHingeJoint.connectedAnchor = value;
        }


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

        private void UpdateBounds()
        {
            if (meshFilter.sharedMesh == null) return;

            Mesh sharedMesh;
            (sharedMesh = meshFilter.sharedMesh).RecalculateBounds();

            var bounds = sharedMesh.bounds;
            bounds.Encapsulate(transform.InverseTransformPoint(Tip.position));
            bounds.Expand(width * shape.Evaluate(1f) * 2.5f);
            meshFilter.sharedMesh.bounds = bounds;
        }
    }
}