using UnityEngine;

namespace Starblast.Tentacles
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

        private void InitializeJoints()
        {
            Joints = new SpringJoint2D[1 + Segments.Length];
            for (int i = 0; i < Segments.Length; i++)
                Joints[i] = Segments[i].GetComponent<SpringJoint2D>();
            Joints[Joints.Length - 1] = Tip.GetComponent<SpringJoint2D>();

            pivotHingeJoint = Pivot.GetComponent<HingeJoint2D>();
            tipHingeJoint = Tip.GetComponent<HingeJoint2D>();
        }

        private void InitializeSegments()
        {
            Segments = new Rigidbody2D[2];
            for (int i = 0; i < Segments.Length;)
                Segments[i++] = transform.GetChild(i).GetComponent<Rigidbody2D>();
            Tip = transform.GetChild(3).GetComponent<Rigidbody2D>();
        }
    }
}