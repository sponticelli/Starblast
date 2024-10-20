using UnityEngine;

namespace Starblast.Entities.Tentacles
{
    public partial class Tentacle
    {
        private PolygonCollider2D polygonCollider;

        private void BuildCollider()
        {
            if (!polygonCollider.enabled || vertices == null) return;

            var tipTransform = Tip.transform;

            if (reduction == 0)
            {
                SetFullCollider(tipTransform);
            }
            else
            {
                SetReducedCollider(tipTransform);
            }
        }

        private void SetFullCollider(Transform tipTransform)
        {
            // Create collider points for all vertices
            var colliderPoints = new Vector2[vertices.Length];
            for (int i = 0; i < colliderPoints.Length; i++)
            {
                colliderPoints[i] = tipTransform.InverseTransformPoint(vertices[i]);
            }
            polygonCollider.points = colliderPoints;
        }

        private void SetReducedCollider(Transform tipTransform)
        {
            // Calculate indices for the reduced collider
            int firstIndex = (pivotCapSmoothness + 1) / 2;
            int lastIndex = vertices.Length - (pivotCapSmoothness + 1) + firstIndex;
            int indexAfterTip = smoothness + tipCapSmoothness + firstIndex;
            int indexBeforeTip = smoothness - 1 + firstIndex;

            int sideLength = smoothness / (reduction + 1);
            int length = (sideLength * 2) + GetCapPointCount(pivotCapSmoothness) + GetCapPointCount(tipCapSmoothness);

            var colliderPoints = new Vector2[length];
            int i = 0;

            // Add points for pivot cap
            i = AddPivotCapPoints(colliderPoints, i, tipTransform, firstIndex);

            // Add side points before the tip
            i = AddSidePoints(colliderPoints, i, tipTransform, firstIndex, sideLength);

            // Add points for tip cap
            i = AddTipCapPoints(colliderPoints, i, tipTransform, indexBeforeTip, indexAfterTip);

            // Add side points after the tip
            i = AddSidePoints(colliderPoints, i, tipTransform, indexAfterTip, sideLength);

            // Add the last point
            colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[lastIndex]);

            polygonCollider.points = colliderPoints;
        }

        private int AddPivotCapPoints(Vector2[] colliderPoints, int currentIndex, Transform tipTransform, int firstIndex)
        {
            switch (pivotCapSmoothness)
            {
                case 1:
                    colliderPoints[currentIndex++] = tipTransform.InverseTransformPoint(vertices[0]);
                    break;
                case 2:
                    int pivotCapHalf = firstIndex / 2;
                    colliderPoints[currentIndex++] = tipTransform.InverseTransformPoint(vertices[pivotCapHalf]);
                    colliderPoints[colliderPoints.Length - 1] = tipTransform.InverseTransformPoint(vertices[vertices.Length - pivotCapHalf - 1]);
                    break;
                case >= 3:
                    pivotCapHalf = firstIndex / 2;
                    colliderPoints[currentIndex++] = tipTransform.InverseTransformPoint(vertices[0]);
                    colliderPoints[currentIndex++] = tipTransform.InverseTransformPoint(vertices[pivotCapHalf]);
                    colliderPoints[colliderPoints.Length - 1] = tipTransform.InverseTransformPoint(vertices[vertices.Length - pivotCapHalf]);
                    break;
            }
            return currentIndex;
        }

        private int AddSidePoints(Vector2[] colliderPoints, int currentIndex, Transform tipTransform, int startIndex, int sideLength)
        {
            int j = startIndex;
            for (int k = 0; k < sideLength; k++)
            {
                colliderPoints[currentIndex++] = tipTransform.InverseTransformPoint(vertices[j]);
                j += (reduction + 1);
            }
            return currentIndex;
        }

        private int AddTipCapPoints(Vector2[] colliderPoints, int currentIndex, Transform tipTransform, int indexBeforeTip, int indexAfterTip)
        {
            switch (tipCapSmoothness)
            {
                case 1:
                    colliderPoints[currentIndex++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip + 1]);
                    break;
                case 2:
                    colliderPoints[currentIndex++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip + 1]);
                    colliderPoints[currentIndex++] = tipTransform.InverseTransformPoint(vertices[indexAfterTip - 1]);
                    break;
                case >= 3:
                    int tipCapHalf = (tipCapSmoothness + 1) / 4;
                    colliderPoints[currentIndex++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip + tipCapHalf]);
                    colliderPoints[currentIndex++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip + (tipCapSmoothness + 1) / 2]);
                    colliderPoints[currentIndex++] = tipTransform.InverseTransformPoint(vertices[indexAfterTip - tipCapHalf]);
                    break;
            }
            return currentIndex;
        }

        private int GetCapPointCount(int capSmoothness)
        {
            return (capSmoothness >= 3) ? 4 : (capSmoothness + 1);
        }
    }
}