using Starblast.Extensions;
using UnityEngine;

namespace Starblast.Entities.Tentacles
{
    public partial class Tentacle
    {
        [SerializeField] private Transform target;
        [SerializeField] private Rigidbody2D targetRigidbody;
        [SerializeField] private float speed;
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float amplitude = 1f;
        [SerializeField] private float animationDelay = 0f;

        // Properties
        public bool IsAttached => pivotHingeJoint.connectedBody != null || Pivot.bodyType == RigidbodyType2D.Static;
        public bool IsTargetSet => TargetRigidbody != null || TargetTransform != null;
        public bool IsHoldingTarget => tipHingeJoint.enabled;

        // Public Methods
        public void Catch() => SetTipConnection(targetRigidbody);
        public void Release() => SetTipConnection(null);
        public void Attach(Rigidbody2D rigidbody) => SetPivotConnection(rigidbody, RigidbodyType2D.Dynamic);
        public void Attach() => SetPivotConnection(null, RigidbodyType2D.Static);
        public void Detach() => SetPivotConnection(null, RigidbodyType2D.Dynamic);

        // Private Methods
        private void ReachTarget()
        {
            if (target != null && tipHingeJoint.connectedBody == null)
            {
                var direction = ((Vector2)target.position - Tip.position) * (Time.deltaTime * speed);
                Tip.AddForce(direction);
                Debug.DrawLine(target.position, Tip.position, Color.gray);
            }
        }

        private void SetTipConnection(Rigidbody2D connection)
        {
            tipHingeJoint.connectedBody = connection;
            tipHingeJoint.enabled = connection != null;
        }

        private void SetPivotConnection(Rigidbody2D connection, RigidbodyType2D bodyType)
        {
            Pivot.bodyType = bodyType;
            pivotHingeJoint.connectedBody = connection;
            pivotHingeJoint.enabled = connection != null;
            if (connection == null) pivotHingeJoint.connectedAnchor = Vector2.zero;
        }

        private void ApplyBehaviour()
        {
            switch (animation)
            {
                case Animations.None:
                    break;
                case Animations.Wave:
                    ApplyWaveAnimation();
                    break;
                case Animations.Swing:
                    ApplySwingAnimation();
                    break;
            }
        }

        private void ApplyWaveAnimation()
        {
            ApplyTentacleAnimation(1.5f, .9f);
        }

        private void ApplySwingAnimation()
        {
            ApplyTentacleAnimation(0, 1f);
        }

        private void ApplyTentacleAnimation(float phaseShift, float forceFactor)
        {
            var pivotPosition = Pivot.position;
            var middle1 = GetMiddlePoint(pivotPosition, Segments[1].position);
            var sinWave = Mathf.Sin(Time.time * frequency + animationDelay) * amplitude;
            
            Vector2 perpendicular = default;
            middle1.GetPerpendicularNoAlloc(pivotPosition, ref perpendicular);
            Segments[0].AddForce(perpendicular.normalized * sinWave);

            var middle2 = GetMiddlePoint(Segments[0].position, Tip.position);
            middle2.GetPerpendicularNoAlloc(Tip.position, ref perpendicular);
            Segments[1].AddForce(perpendicular.normalized * (forceFactor * sinWave * phaseShift));
        }

        private Vector2 GetMiddlePoint(Vector2 from, Vector2 to)
        {
            return from + (to - from).normalized * ((to - from).magnitude * 0.5f);
        }
    }
}