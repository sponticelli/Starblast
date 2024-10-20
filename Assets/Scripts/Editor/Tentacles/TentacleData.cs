using System;
using UnityEditor;
using UnityEngine;

namespace Starblast.Tentacles
{
    public struct TentacleData
    {
        public Entities.Tentacles.Tentacle Tentacle { get; private set; }

        public MeshRenderer meshRenderer;
        private PolygonCollider2D polygonCollider;
        private CircleCollider2D[] circleColliders;
        private Rigidbody2D[] segments;
        private SpringJoint2D[] joints;
        private HingeJoint2D pivotHingeJoint;
        private MaterialPropertyBlock materialBlock;

        public enum ColliderType
        {
            /*polygonal, */
            Circles,
            CircleOnTip,
            None
        };

        public ColliderType CurrentColliderType { get; private set; }
        public Tuple<bool, bool> IsTrigger { get; private set; }

        public bool PolygonalEnabled
        {
            get => polygonCollider.enabled;
            set
            {
                Undo.RecordObject(polygonCollider, "Set Polygon Collider");
                polygonCollider.enabled = value;
            }
        }

        public Tuple<float, bool> Mass { get; private set; }
        public Tuple<float, bool> Gravity { get; private set; }
        public Tuple<float, bool> Drag { get; private set; }
        public Tuple<float, bool> Length { get; private set; }
        public Tuple<float, bool> Stiffness { get; private set; }
        public Tuple<bool, bool> HierarchyType { get; private set; }

        public enum BodyType
        {
            world,
            rigidbody,
            detached
        };

        public BodyType TentacleBodyType { get; private set; }
        public Rigidbody2D ParentBody { get; private set; }
        public Vector2 ParentBodyOffset { get; private set; }

        public TentacleData(Entities.Tentacles.Tentacle tentacle)
        {
            Tentacle = tentacle;
            var transform = tentacle.transform;
            var pivot = transform.GetChild(0).GetComponent<Rigidbody2D>();
            var tip = transform.GetChild(3).GetComponent<Rigidbody2D>();

            meshRenderer = pivot.GetComponent<MeshRenderer>();
            materialBlock = new MaterialPropertyBlock();
            polygonCollider = tip.GetComponent<PolygonCollider2D>();

            circleColliders = new CircleCollider2D[4];
            circleColliders[0] = pivot.GetComponent<CircleCollider2D>();
            for (int i = 1; i < circleColliders.Length - 1; i++)
                circleColliders[i] = transform.GetChild(i).GetComponent<CircleCollider2D>();
            circleColliders[circleColliders.Length - 1] = tip.GetComponent<CircleCollider2D>();

            bool isPivotEnabled = circleColliders[0].enabled,
                isSegment1Enabled = circleColliders[1].enabled,
                isSegment2Enabled = circleColliders[2].enabled,
                isTipEnabled = circleColliders[3].enabled;

            CurrentColliderType = isPivotEnabled switch
            {
                true when isSegment1Enabled && isSegment2Enabled && isTipEnabled => ColliderType.Circles,
                false when !isSegment1Enabled && !isSegment2Enabled && isTipEnabled => ColliderType.CircleOnTip,
                false when !isSegment1Enabled && !isSegment2Enabled && !isTipEnabled => ColliderType.None,
                _ => (ColliderType)(-1)
            };

            var isTrigger = circleColliders[0].isTrigger;
            var isTriggerHasMixedValues = false;

            for (int i = 1; i < circleColliders.Length; i++)
            {
                if (isTrigger != circleColliders[i].isTrigger)
                {
                    isTriggerHasMixedValues = true;
                    break;
                }
            }

            IsTrigger = new Tuple<bool, bool>(isTrigger, isTriggerHasMixedValues);
            segments = new Rigidbody2D[4];
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i] = circleColliders[i].GetComponent<Rigidbody2D>();
            }

            var mass = segments[0].mass;
            var massHasMixedValues = false;
            for (int i = 1; i < segments.Length; i++)
            {
                if (mass != segments[i].mass)
                {
                    massHasMixedValues = true;
                    break;
                }
            }

            Mass = new Tuple<float, bool>(mass * segments.Length, massHasMixedValues);

            var gravity = segments[0].gravityScale;
            var gravityHaveMixedValues = false;
            for (int i = 1; i < segments.Length; i++)
            {
                if (gravity != segments[i].gravityScale)
                {
                    gravityHaveMixedValues = true;
                    break;
                }
            }

            Gravity = new Tuple<float, bool>(gravity, gravityHaveMixedValues);

            var drag = segments[0].drag;
            var dragHaveMixedValues = false;
            for (int i = 1; i < segments.Length; i++)
            {
                if (drag != segments[i].drag)
                {
                    dragHaveMixedValues = true;
                    break;
                }
            }

            Drag = new Tuple<float, bool>(drag, dragHaveMixedValues);

            joints = new SpringJoint2D[3];
            for (int i = 0; i < joints.Length; i++)
                joints[i] = circleColliders[i + 1].GetComponent<SpringJoint2D>();

            var length = joints[0].distance;
            var lengthHasMixedValues = false;
            for (int i = 1; i < joints.Length; i++)
            {
                if (length != joints[i].distance)
                {
                    lengthHasMixedValues = true;
                    break;
                }
            }

            Length = new Tuple<float, bool>(length * joints.Length, lengthHasMixedValues);

            var stiffness = joints[0].frequency;
            var stiffnessHasMixedValues = false;
            for (int i = 1; i < joints.Length; i++)
            {
                if (stiffness != joints[i].frequency)
                {
                    stiffnessHasMixedValues = true;
                    break;
                }
            }

            Stiffness = new Tuple<float, bool>(stiffness, stiffnessHasMixedValues);

            pivotHingeJoint = segments[0].GetComponent<HingeJoint2D>();
            var bodyType = segments[0].bodyType;
            var pivotHingeJointEnabled = pivotHingeJoint.enabled;
            TentacleBodyType = bodyType switch
            {
                RigidbodyType2D.Dynamic when !pivotHingeJointEnabled => BodyType.detached,
                RigidbodyType2D.Static when !pivotHingeJointEnabled => BodyType.world,
                RigidbodyType2D.Dynamic when pivotHingeJointEnabled => BodyType.rigidbody,
                _ => (BodyType)(-1)
            };
            ParentBody = pivotHingeJoint.connectedBody;
            ParentBodyOffset = pivotHingeJoint.connectedAnchor;

            var hideFlags = segments[0].gameObject.hideFlags;
            var hideFlagsHasMixedValues = false;
            for (int i = 1; i < segments.Length; i++)
            {
                if (hideFlags != segments[i].gameObject.hideFlags)
                {
                    hideFlagsHasMixedValues = true;
                    break;
                }
            }

            HierarchyType =
                new Tuple<bool, bool>(hideFlags == HideFlags.None ? true : false, hideFlagsHasMixedValues);
        }

        public void SetSortingLayerName(string name)
        {
            Undo.RecordObject(meshRenderer, "Set MeshRenderer SortingLayerName");
            meshRenderer.sortingLayerName = name;
        }

        public void SetSortingOrder(int order)
        {
            Undo.RecordObject(meshRenderer, "Set MeshRenderer SortingOrder");
            meshRenderer.sortingOrder = order;
        }

        public void SetCollider(ColliderType type)
        {
            Undo.RecordObjects(circleColliders, "Set Circle Colliders");

            switch (type)
            {
                case ColliderType.Circles:
                    for (int i = 0; i < circleColliders.Length; i++)
                    {
                        circleColliders[i].enabled = true;
                    }

                    break;
                case ColliderType.CircleOnTip:
                    for (int i = 0; i < circleColliders.Length - 1; i++)
                        circleColliders[i].enabled = false;
                    circleColliders[^1].enabled = true;
                    break;
                case ColliderType.None:
                    for (int i = 0; i < circleColliders.Length; i++)
                    {
                        circleColliders[i].enabled = false;
                    }

                    break;
                default:
                    Debug.LogWarning("Unexpected colliders setup detected.");
                    break;
            }

            CurrentColliderType = type;
        }

        public void SetTrigger(bool isTrigger)
        {
            Undo.RecordObjects(circleColliders, "Set CircleColliders2D isTrigger");
            for (int i = 0; i < circleColliders.Length; i++)
            {
                circleColliders[i].isTrigger = isTrigger;
            }

            IsTrigger = new Tuple<bool, bool>(isTrigger, false);
        }

        public void UpdateCircleColliders(float width, AnimationCurve shape)
        {
            Undo.RecordObjects(circleColliders, "Circle Colliders Radius");
            for (int i = 0; i < circleColliders.Length; i++)
            {
                circleColliders[i].radius = width * shape.Evaluate(.3f * i);
            }
        }

        public void SetMass(float mass)
        {
            Undo.RecordObjects(segments, "Set Segments Rigidbody2D Mass");
            var segmentMass = mass / segments.Length;
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i].mass = segmentMass;
            }

            Mass = new Tuple<float, bool>(mass, false);
        }

        public void SetGravity(float gravity)
        {
            Undo.RecordObjects(segments, "Set Segments Rigidbody2D Gravity");
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i].gravityScale = gravity;
            }

            Gravity = new Tuple<float, bool>(gravity, false);
        }

        public void SetDrag(float drag)
        {
            Undo.RecordObjects(segments, "Set Segments Rigidbody2D Drag");
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i].drag = drag;
            }

            Drag = new Tuple<float, bool>(drag, false);
        }

        public void SetLength(float length)
        {
            Undo.RecordObjects(joints, "Set SpringJoint2D Length");
            var segmentLength = length / (segments.Length - 1);
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i].distance = segmentLength;
            }

            Length = new Tuple<float, bool>(length, false);

            if (!Application.isPlaying)
            {
                for (int i = 0; i < joints.Length; i++)
                {
                    var transform = joints[i].transform;
                    Undo.RecordObject(transform, $"Set Joint{i} Transform Length");
                    var position = transform.localPosition;
                    position.y = segmentLength * (i + 1);
                    transform.localPosition = position;
                }
            }
        }

        public void SetStiffness(float stiffness)
        {
            Undo.RecordObjects(joints, "Set SpringJoints2D Frequency");
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i].frequency = stiffness;
            }

            Stiffness = new Tuple<float, bool>(stiffness, false);
        }

        public void SetBodyType(BodyType type)
        {
            Undo.RecordObjects(segments, "Set Segments BodyType");
            Undo.RecordObject(pivotHingeJoint, "Set Pivot HingeJoint BodyType");
            switch (type)
            {
                case BodyType.world:
                    segments[0].bodyType = RigidbodyType2D.Static;
                    pivotHingeJoint.enabled = false;
                    break;
                case BodyType.rigidbody:
                    segments[0].bodyType = RigidbodyType2D.Dynamic;
                    pivotHingeJoint.enabled = true;
                    break;
                case BodyType.detached:
                    segments[0].bodyType = RigidbodyType2D.Dynamic;
                    pivotHingeJoint.enabled = false;
                    break;
                default:
                    break;
            }

            TentacleBodyType = type;
        }

        public void SetParentBody(Rigidbody2D rigidbody)
        {
            Undo.RecordObject(pivotHingeJoint, "Pivot HingeJoint2D ConnectedBody");
            pivotHingeJoint.connectedBody = rigidbody;
            ParentBody = rigidbody;
        }

        public void SetParentBodyOffset(Vector2 offset)
        {
            Undo.RecordObject(pivotHingeJoint, "Pivot HingeJoint2D ConnectedAnchor");
            pivotHingeJoint.connectedAnchor = offset;
            ParentBodyOffset = offset;

            if (!Application.isPlaying)
            {
                Undo.RecordObject(segments[0], $"Set ParentBodyOffset on Pivot rigidbody");
                segments[0].position = ParentBody.position + ParentBodyOffset;
            }
        }

        public void SetColor(Color color)
        {
            Undo.RecordObject(meshRenderer, "MeshRenderer Color");
            materialBlock.SetColor("_Color", color);
            meshRenderer.SetPropertyBlock(materialBlock);
        }

        public void ChangeHiererachy(bool show)
        {
            Undo.RecordObjects(segments, "Segments HideFlags");
            if (show)
            {
                for (int i = 0; i < segments.Length; i++)
                {
                    segments[i].gameObject.hideFlags = HideFlags.None;
                }
            }
            else
            {
                for (int i = 0; i < segments.Length; i++)
                {
                    segments[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
                }
            }

            HierarchyType = new Tuple<bool, bool>(segments[0].gameObject.hideFlags == HideFlags.None ? true : false,
                false);
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}