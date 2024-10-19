using UnityEngine;

namespace Starblast.Tentacles
{
    public partial class Tentacle
    {
        [SerializeField] private Transform target;
        [SerializeField] private Rigidbody2D targetRigidbody;
        [SerializeField] private float speed;
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float amplitude = 1f;
        [SerializeField] private float animationDelay = 0f;

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
                    WaveAnimation();
                    break;
                case Animations.swing:
                    SwingAnimation();
                    break;
            }
        }

        private void WaveAnimation()
        {
            var fromPivotToSegment2 = Segments[1].position - Pivot.position;
            //var middle1 = Pivot.position + fromPivotToSegment2.normalized * fromPivotToSegment2.magnitude * .5f;
            //var pivotSin = Mathf.Sin(Time.time * frequency + animationDelay) * amplitude;
            Segments[0].AddForce(
                Utilities.GetPerpendicular(
                    Pivot.position + fromPivotToSegment2.normalized * fromPivotToSegment2.magnitude * .5f,
                    Pivot.position).normalized * (Mathf.Sin(Time.time * frequency + animationDelay) * amplitude));

            var fromSegment1ToTip = Tip.position - Segments[0].position;
            //var middle2 = Segments[0].position + fromSegment1ToTip.normalized * fromSegment1ToTip.magnitude * .5f;
            //var tipSin = Mathf.Sin(Time.time * frequency + 1.5f + animationDelay) * amplitude;
            Segments[1].AddForce(
                Utilities.GetPerpendicular(
                    Segments[0].position + fromSegment1ToTip.normalized * fromSegment1ToTip.magnitude * .5f,
                    Tip.position).normalized * .9f *
                (Mathf.Sin(Time.time * frequency + 1.5f + animationDelay) * amplitude));
        }

        private void SwingAnimation()
        {
            var fromPivotToSegment2 = Segments[1].position - Pivot.position;
            //var middle1 = Pivot.position + fromPivotToSegment2.normalized * fromPivotToSegment2.magnitude * .5f;
            var sin = Mathf.Sin(Time.time * frequency + animationDelay) * amplitude;
            Segments[0].AddForce(Utilities
                .GetPerpendicular(
                    (Pivot.position + fromPivotToSegment2.normalized * fromPivotToSegment2.magnitude * .5f),
                    Pivot.position).normalized * sin);

            var fromSegment1ToTip = Tip.position - Segments[0].position;
            //var middle2 = Segments[0].position + fromSegment1ToTip.normalized * fromSegment1ToTip.magnitude * .5f;
            Segments[1].AddForce(Utilities.GetPerpendicular(Tip.position,
                                     Segments[0].position + fromSegment1ToTip.normalized * fromSegment1ToTip.magnitude *
                                     .5f).normalized *
                                 sin);
        }
    }
}