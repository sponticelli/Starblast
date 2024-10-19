using Starblast.Extensions;
using UnityEngine;

namespace Starblast.Tentacles
{
    [ExecuteInEditMode]
    public class Tentacle : MonoBehaviour
    {
        //#region Mesh
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Vector2[] vertices;
        private Vector2[] uvs;
        private Mesh mesh;
        private enum UVsLayout { solidColor, stretchy, tiled }
        private bool renewUVs = true;
        private int VerticesCount => smoothness * 2 + tipCapSmoothness + pivotCapSmoothness;
        private MaterialPropertyBlock materialBlock;

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
        private PolygonCollider2D polygonCollider;
        [SerializeField] private int reduction = 2;


        //#region Behaviour
        private HingeJoint2D pivotHingeJoint;
        private HingeJoint2D tipHingeJoint;
        public enum Animations { none, wave, swing }

        //[SerializeField] private Rigidbody2D parentRigidbody;
        //[SerializeField] private Vector2 parentBodyOffset;
        [SerializeField] private new Animations animation = Animations.wave;
        [SerializeField] private Transform target;
        [SerializeField] private Rigidbody2D targetRigidbody;
        [SerializeField] private float speed;
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float amplitude = 1f;
        [SerializeField] private float animationDelay = 0f;
        /// <summary>
        /// The mass of the whole tentacle
        /// </summary>
        public float Mass
        {
            get
            {
                float mass = Pivot.mass + Tip.mass;
                for (int i = 0; i < Segments.Length; i++)
                    mass += Segments[i].mass;
                return mass;
            }
            set
            {
                float segmentMass = value / (Segments.Length + 2);
                Pivot.mass = Tip.mass = segmentMass;
                for (int i = 0; i < Segments.Length; i++)
                    Segments[i].mass = segmentMass;
            }
        }
        /// <summary>
        /// The drag of an each segment of this tentacle
        /// </summary>
        public float Drag
        {
            get { return Pivot.drag; }
            set
            {
                Pivot.drag = Tip.drag = value;
                for (int i = 0; i < Segments.Length; i++)
                    Segments[i].drag = value;
            }
        }
        /// <summary>
        /// How much gravity affects each segment of the tentacle
        /// </summary>
        public float Gravity
        {
            get { return Pivot.gravityScale; }
            set
            {
                Pivot.gravityScale = Tip.gravityScale = value;
                for (int i = 0; i < Segments.Length; i++)
                    Segments[i].gravityScale = value;
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
        /// The stiffness of an each segment of this tentacle
        /// </summary>
        public float Stiffness
        {
            get { return Joints[0].frequency; }
            set
            {
                for (int i = 0; i < Joints.Length; i++)
                    Joints[i].frequency = value;
            }
        }
        /// <summary>
        /// Rigidbody2D on the tip segment of the tentacle
        /// </summary>
        public Rigidbody2D Tip { get; private set; }
        /// <summary>
        /// Rigidbody2D on the pivot (root) segment of the tentacle
        /// </summary>
        public Rigidbody2D Pivot { get; private set; }
        /// <summary>
        /// Rigidbodies2D on the segments beween tip and pivot
        /// </summary>
        public Rigidbody2D[] Segments { get; private set; }
        /// <summary>
        /// SpringJoints2D on all of the segments
        /// </summary>
        public SpringJoint2D[] Joints { get; private set; }
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
        /// Whether tentacle's pivot attached to another rigidbody
        /// </summary>
        public bool IsAttached => pivotHingeJoint.connectedBody != null || Pivot.bodyType == RigidbodyType2D.Static;
        /// <summary>
        /// Whether tentacle has the target
        /// </summary>
        public bool IsTargetSet => TargetRigidbody != null || TargetTransform != null;
        /// <summary>
        /// Whether tentacle is connected to the target's rigidbody
        /// </summary>
        public bool IsHoldingTarget => tipHingeJoint.enabled;
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
            Segments = new Rigidbody2D[2];
            for (int i = 0; i < Segments.Length;)
                Segments[i++] = transform.GetChild(i).GetComponent<Rigidbody2D>();
            Tip = transform.GetChild(3).GetComponent<Rigidbody2D>();

            Joints = new SpringJoint2D[1 + Segments.Length];
            for (int i = 0; i < Segments.Length; i++)
                Joints[i] = Segments[i].GetComponent<SpringJoint2D>();
            Joints[Joints.Length - 1] = Tip.GetComponent<SpringJoint2D>();

            pivotHingeJoint = Pivot.GetComponent<HingeJoint2D>();
            tipHingeJoint = Tip.GetComponent<HingeJoint2D>();

            polygonCollider = Tip.GetComponent<PolygonCollider2D>();
        }

        private void InitializeMesh()
        {
            meshFilter = Pivot.GetComponent<MeshFilter>();
            meshRenderer = Pivot.GetComponent<MeshRenderer>();
            meshRenderer.material = material;
            materialBlock = new MaterialPropertyBlock();
            materialBlock.SetColor("_Color", color);
            meshRenderer.SetPropertyBlock(materialBlock);

//#if !UNITY_EDITOR
            mesh = new Mesh { name = "Tentacle's Mesh" };
            mesh.MarkDynamic();
            meshFilter.sharedMesh = mesh;
//#endif
        }

        private void InitializeUVs()
        {
            uvs = new Vector2[VerticesCount];
            renewUVs = true;
        }

        private void DefineVertices()
        {
            vertices = new Vector2[VerticesCount];

            UpdateUVs(out float segmentStep);

            var points = new Vector2[smoothness];
            points[0] = Utilities.GetPoint(Pivot.position, Segments[0].position, Segments[1].position, Tip.position, 0);

            int firstIndex = (pivotCapSmoothness + 1) / 2, lastIndex = vertices.Length - (pivotCapSmoothness + 1) + firstIndex;
            //var uvsheight = 0f;
            for (int i = 1; i < smoothness; i++)
            {
                //var t = i / (smoothness - 1f);
                points[i] = Utilities.GetPoint(Pivot.position, Segments[0].position, Segments[1].position, Tip.position, i / (smoothness - 1f));

                //var right = Utilities.GetPerpendicular(points[i - 1], points[i]).normalized;
                //var left = Utilities.GetPerpendicular(points[i - 1], points[i], -1f).normalized;

                var curve = shape.Evaluate(Utilities.Normalize(i, smoothness));
                vertices[i + firstIndex] = points[i] + Utilities.GetPerpendicular(points[i - 1], points[i]).normalized * curve * width;
                vertices[lastIndex - i] = points[i] + Utilities.GetPerpendicular(points[i - 1], points[i], -1f).normalized * curve * width;

                if (renewUVs)
                {
                    //var thickness = Mathf.Clamp01(Mathf.Pow(curve, temp));
                    //uvsheight += segmentStep / width * .5f;
                    ////uvsheight += segmentStep / width * .5f;
                    //uvs[i + firstIndex] = new Vector2(0, uvsheight);
                    //uvs[lastIndex - i] = new Vector2(1f, uvsheight);

                    var y = segmentStep * i / width * .5f /*/ thickness*/; // TODO: consider minimal height
                    //y = Mathf.Clamp(y, segmentStep * 3, y);
                    uvs[i + firstIndex] = new Vector2(0, y);
                    uvs[lastIndex - i] = new Vector2(1f, y);

#if UNITY_EDITOR
                    if (debugUVs) Debug.DrawLine(uvs[i + firstIndex], uvs[lastIndex - i], Color.cyan);
#endif
                }
            }

            var curve2 = shape.Evaluate(0) * width;
            vertices[firstIndex] = points[0] + Utilities.GetPerpendicular(points[0], points[1]).normalized * curve2;
            vertices[lastIndex] = points[0] + Utilities.GetPerpendicular(points[0], points[1], -1f).normalized * curve2;

            if (renewUVs)
            {
                uvs[firstIndex] = new Vector2(0, 0);
                uvs[lastIndex] = new Vector2(1f, 0);
            }

            // Define cap on tip
            if (tipCapSmoothness != 0)
            {
                Vector2 pivot = default;
                if (renewUVs) pivot = new Vector2(.5f, segmentStep * (smoothness - 1) / width * .5f);

                int step = 180 / (tipCapSmoothness + 1), indexAfterTip = smoothness + tipCapSmoothness + firstIndex, indexBeforeTip = smoothness - 1 + firstIndex, length = tipCapSmoothness + 1;
                for (int i = 1; i < length; i++)
                {
                    vertices[indexAfterTip - i] = Utilities.RotatePointAroundPivot(vertices[indexBeforeTip], Tip.position, step * i + 180);
                    if (renewUVs) uvs[indexAfterTip - i] = Utilities.RotatePointAroundPivot(uvs[indexBeforeTip], pivot, step * i + 180);
                }
            }

            // Define pivot cap
            if (pivotCapSmoothness != 0)
            {
                Vector2 pivot = default;
                if (renewUVs) pivot = new Vector2(.5f, 0);

                int step = 180 / (pivotCapSmoothness + 1), j = 1, length = pivotCapSmoothness + 1;
                for (int i = 1; i <= length; i++)
                {
                    var angle = step * i + 180;
                    if (i <= firstIndex)
                    {
                        vertices[firstIndex - i] = Utilities.RotatePointAroundPivot(vertices[lastIndex], Pivot.position, angle);
                        if (renewUVs) uvs[firstIndex - i] = Utilities.RotatePointAroundPivot(uvs[lastIndex], pivot, angle);
                    }
                    else
                    {
                        var index = vertices.Length - j++;
                        vertices[index] = Utilities.RotatePointAroundPivot(vertices[lastIndex], Pivot.position, angle);
                        if (renewUVs) uvs[index] = Utilities.RotatePointAroundPivot(uvs[lastIndex], pivot, angle);
                    }
                }
            }
        }

        private void UpdateUVs(out float segmentStep)
        {
            segmentStep = 0f;

#if UNITY_EDITOR
            if (textureType != storedTextureType || uvs == null || uvs.Length != VerticesCount)
            {
                storedTextureType = textureType;
                uvs = new Vector2[VerticesCount];
                renewUVs = true;
            }
#endif
            if (renewUVs)
            {
                if (textureType == UVsLayout.solidColor)
                {
                    var position = new Vector2(.5f, .5f);
                    for (int i = 0; i < uvs.Length; i++)
                        uvs[i] = position;
                    renewUVs = false;
                }
                else
                {
                    var distance = 0f;
                    var minDistance = 0f;
                    for (int i = 0; i < Joints.Length; i++) minDistance += Joints[i].distance; // TODO: simplify this
                    distance = (Tip.position - Pivot.position).magnitude;
                    segmentStep = Mathf.Clamp(distance, minDistance * .5f, distance) / (smoothness - 1);
                }
            }
        }

        private void BuildCollider()
        {
            if (!polygonCollider.enabled || vertices == null) return; // TODO: fix update vs fixedupdate

            var tipTransform = Tip.transform;
            if (reduction == 0)
            {
                var colliderPoints = new Vector2[vertices.Length];
                for (int i = 0; i < colliderPoints.Length; i++)
                    colliderPoints[i] = tipTransform.InverseTransformPoint(vertices[i]);
                polygonCollider.points = colliderPoints;
            }
            else
            {
                int firstIndex = (pivotCapSmoothness + 1) / 2,
                    lastIndex = vertices.Length - (pivotCapSmoothness + 1) + firstIndex,
                    indexAfterTip = smoothness + tipCapSmoothness + firstIndex,
                    indexBeforeTip = smoothness - 1 + firstIndex;

                int sideLength = smoothness / (reduction + 1), length = (sideLength * 2) + (pivotCapSmoothness >= 3 ? 4 : (pivotCapSmoothness + 1)) + (tipCapSmoothness >= 3 ? 4 : (tipCapSmoothness + 1));
                var colliderPoints = new Vector2[length];

                int i = 0, j = firstIndex;

                if (pivotCapSmoothness == 1)
                {
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[0]);
                }
                else if (pivotCapSmoothness == 2)
                {
                    var pivotCapHalf = firstIndex / 2;
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[pivotCapHalf]);
                    colliderPoints[length - 1] = tipTransform.InverseTransformPoint(vertices[vertices.Length - pivotCapHalf - 1]);
                }
                else if (pivotCapSmoothness >= 3)
                {
                    var pivotCapHalf = firstIndex / 2;
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[0]);
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[pivotCapHalf]);
                    colliderPoints[colliderPoints.Length - 1] = tipTransform.InverseTransformPoint(vertices[vertices.Length - pivotCapHalf]);
                }

                for (var k = 0; k < sideLength; k++)
                {
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[j]);
                    j += (reduction + 1);
                }
                colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip]);

                if (tipCapSmoothness == 1)
                {
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip + 1]);
                }
                else if (tipCapSmoothness == 2)
                {
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip + 1]);
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexAfterTip - 1]);
                }
                else if (tipCapSmoothness >= 3)
                {
                    var tipCapHalf = (tipCapSmoothness + 1) / 4;
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip + tipCapHalf]);
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip + (tipCapSmoothness + 1) / 2]);
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexAfterTip - tipCapHalf]);
                }

                j = indexAfterTip;
                for (var k = 0; k < sideLength; k++)
                {
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[j]);
                    j += (reduction + 1);
                }
                colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[lastIndex]);

                polygonCollider.points = colliderPoints;
            }
        }

#if UNITY_EDITOR
        private UVsLayout storedTextureType;

        private void RenderMesh()
        {
            meshFilter.sharedMesh.Clear();

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

            meshFilter.sharedMesh.uv = uvs;

            //mesh = new Mesh
            //{
            //    name = "Tentacle's Mesh",
            //    vertices = meshVertices,
            //    triangles = triangles,
            //    uv = uvs
            //};
            //meshFilter.sharedMesh = mesh;

            if (renewUVs && textureType == UVsLayout.stretchy)
                renewUVs = false;
        }
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

        private void ReachTarget()
        {
            if (target != null && tipHingeJoint.connectedBody == null)
            {
                //var direction = (Vector2)target.position - Tip.position;
                Tip.AddForce(((Vector2)target.position - Tip.position) * Time.deltaTime * speed);
                Debug.DrawLine(target.position, Tip.position, Color.gray);
            }
        }

        /// <summary>
        /// Catch (connect tentacle’s tip to the) target if TargetRigidbody is set
        /// </summary>
        public void Catch()
        {
            if (targetRigidbody != null)
            {
                if (!tipHingeJoint.enabled) tipHingeJoint.enabled = true;
                tipHingeJoint.connectedBody = targetRigidbody;
            }
            else
            {
                Debug.LogWarning($"Target {target.name} isn't a Rigidbody2D");
            }
        }

        /// <summary>
        /// Release target transform or rigidbody
        /// </summary>
        public void Release()
        {
            if (tipHingeJoint.enabled)
            {
                tipHingeJoint.enabled = false;
            }
            tipHingeJoint.connectedBody = null;
        }

        /// <summary>
        /// Attach tentacle to the rigidbody
        /// </summary>
        /// <param name="rigidbody">Target rigidbody</param>
        public void Attach(Rigidbody2D rigidbody)
        {
            Pivot.bodyType = RigidbodyType2D.Dynamic;
            if (!pivotHingeJoint.enabled)
            {
                pivotHingeJoint.enabled = true;
                pivotHingeJoint.connectedAnchor = Vector2.zero;
            }
            pivotHingeJoint.connectedBody = rigidbody;
        }

        /// <summary>
        /// Attach tentacle to the current position of the tentacle's pivot
        /// </summary>
        public void Attach()
        {
            Pivot.bodyType = RigidbodyType2D.Static;
            if (pivotHingeJoint.enabled) pivotHingeJoint.enabled = false;
            if (pivotHingeJoint.connectedBody != null) pivotHingeJoint.connectedBody = null;
        }

        /// <summary>
        /// Detach tentacle from the parent rigidbody or the world point
        /// </summary>
        public void Detach()
        {
            Pivot.bodyType = RigidbodyType2D.Dynamic;
            if (pivotHingeJoint.enabled)
            {
                pivotHingeJoint.enabled = false;
                pivotHingeJoint.connectedAnchor = Vector2.zero;
            }
            if (pivotHingeJoint.connectedBody != null) pivotHingeJoint.connectedBody = null;
        }

        private void ApplyBehaviour()
        {
            switch (animation)
            {
                case Animations.none:
                    break;
                case Animations.wave:
                    WaveAnimation(); break;
                case Animations.swing:
                    SwingAnimation(); break;
            }
        }

        private void WaveAnimation()
        {
            var fromPivotToSegment2 = Segments[1].position - Pivot.position;
            //var middle1 = Pivot.position + fromPivotToSegment2.normalized * fromPivotToSegment2.magnitude * .5f;
            //var pivotSin = Mathf.Sin(Time.time * frequency + animationDelay) * amplitude;
            Segments[0].AddForce(Utilities.GetPerpendicular(Pivot.position + fromPivotToSegment2.normalized * fromPivotToSegment2.magnitude * .5f, Pivot.position).normalized * (Mathf.Sin(Time.time * frequency + animationDelay) * amplitude));

            var fromSegment1ToTip = Tip.position - Segments[0].position;
            //var middle2 = Segments[0].position + fromSegment1ToTip.normalized * fromSegment1ToTip.magnitude * .5f;
            //var tipSin = Mathf.Sin(Time.time * frequency + 1.5f + animationDelay) * amplitude;
            Segments[1].AddForce(Utilities.GetPerpendicular(Segments[0].position + fromSegment1ToTip.normalized * fromSegment1ToTip.magnitude * .5f, Tip.position).normalized * .9f * (Mathf.Sin(Time.time * frequency + 1.5f + animationDelay) * amplitude));
        }

        private void SwingAnimation()
        {
            var fromPivotToSegment2 = Segments[1].position - Pivot.position;
            //var middle1 = Pivot.position + fromPivotToSegment2.normalized * fromPivotToSegment2.magnitude * .5f;
            var sin = Mathf.Sin(Time.time * frequency + animationDelay) * amplitude;
            Segments[0].AddForce(Utilities.GetPerpendicular((Pivot.position + fromPivotToSegment2.normalized * fromPivotToSegment2.magnitude * .5f), Pivot.position).normalized * sin);

            var fromSegment1ToTip = Tip.position - Segments[0].position;
            //var middle2 = Segments[0].position + fromSegment1ToTip.normalized * fromSegment1ToTip.magnitude * .5f;
            Segments[1].AddForce(Utilities.GetPerpendicular(Tip.position, Segments[0].position + fromSegment1ToTip.normalized * fromSegment1ToTip.magnitude * .5f).normalized * sin);
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