using UnityEngine;

namespace Starblast.Entities.Tentacles
{
    public partial class Tentacle
    {
        private HingeJoint2D pivotHingeJoint;
        private HingeJoint2D tipHingeJoint;

        /// <summary>
        /// The mass of the whole tentacle
        /// </summary>
        public float Mass
        {
            get => Pivot.mass + Tip.mass + TotalSegmentMass;
            set => SetSegmentMass(value / (Segments.Length + 2));
        }

        /// <summary>
        /// The drag of each segment of this tentacle
        /// </summary>
        public float Drag
        {
            get => Pivot.drag;
            set => SetSegmentProperty(segment => segment.drag = value);
        }

        /// <summary>
        /// How much gravity affects each segment of the tentacle
        /// </summary>
        public float Gravity
        {
            get => Pivot.gravityScale;
            set => SetSegmentProperty(segment => segment.gravityScale = value);
        }

        /// <summary>
        /// The stiffness of each segment of this tentacle
        /// </summary>
        public float Stiffness
        {
            get => Joints[0].frequency;
            set => SetJointProperty(joint => joint.frequency = value);
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
        /// Rigidbodies2D on the segments between tip and pivot
        /// </summary>
        public Rigidbody2D[] Segments { get; private set; }

        /// <summary>
        /// SpringJoints2D on all of the segments
        /// </summary>
        public SpringJoint2D[] Joints { get; private set; }

        private float TotalSegmentMass
        {
            get
            {
                float totalMass = 0;
                foreach (var segment in Segments)
                {
                    totalMass += segment.mass;
                }
                return totalMass;
            }
        }

        private void InitializeJoints()
        {
            Joints = new SpringJoint2D[Segments.Length + 1];
            for (int i = 0; i < Segments.Length; i++)
            {
                Joints[i] = Segments[i].GetComponent<SpringJoint2D>();
            }
            Joints[^1] = Tip.GetComponent<SpringJoint2D>();

            pivotHingeJoint = Pivot.GetComponent<HingeJoint2D>();
            tipHingeJoint = Tip.GetComponent<HingeJoint2D>();
        }

        private void InitializeSegments()
        {
            Segments = new Rigidbody2D[2];
            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i] = transform.GetChild(i).GetComponent<Rigidbody2D>();
            }

            Tip = transform.GetChild(3).GetComponent<Rigidbody2D>();
        }

        private void SetSegmentMass(float mass)
        {
            Pivot.mass = Tip.mass = mass;
            foreach (var segment in Segments)
            {
                segment.mass = mass;
            }
        }

        private void SetSegmentProperty(System.Action<Rigidbody2D> setPropertyAction)
        {
            setPropertyAction(Pivot);
            setPropertyAction(Tip);
            foreach (var segment in Segments)
            {
                setPropertyAction(segment);
            }
        }

        private void SetJointProperty(System.Action<SpringJoint2D> setPropertyAction)
        {
            foreach (var joint in Joints)
            {
                setPropertyAction(joint);
            }
        }
    }
}