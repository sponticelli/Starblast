using UnityEngine;

namespace Starblast.Tentacles
{
    
    
    public partial class Tentacle
    {
        private PolygonCollider2D polygonCollider;

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

                int sideLength = smoothness / (reduction + 1),
                    length = (sideLength * 2) + (pivotCapSmoothness >= 3 ? 4 : (pivotCapSmoothness + 1)) +
                             (tipCapSmoothness >= 3 ? 4 : (tipCapSmoothness + 1));
                var colliderPoints = new Vector2[length];

                int i = 0, j = firstIndex;

                switch (pivotCapSmoothness)
                {
                    case 1:
                        colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[0]);
                        break;
                    case 2:
                    {
                        var pivotCapHalf = firstIndex / 2;
                        colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[pivotCapHalf]);
                        colliderPoints[length - 1] =
                            tipTransform.InverseTransformPoint(vertices[vertices.Length - pivotCapHalf - 1]);
                        break;
                    }
                    case >= 3:
                    {
                        var pivotCapHalf = firstIndex / 2;
                        colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[0]);
                        colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[pivotCapHalf]);
                        colliderPoints[colliderPoints.Length - 1] =
                            tipTransform.InverseTransformPoint(vertices[vertices.Length - pivotCapHalf]);
                        break;
                    }
                }

                for (var k = 0; k < sideLength; k++)
                {
                    colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[j]);
                    j += (reduction + 1);
                }

                colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip]);

                switch (tipCapSmoothness)
                {
                    case 1:
                        colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip + 1]);
                        break;
                    case 2:
                        colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip + 1]);
                        colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexAfterTip - 1]);
                        break;
                    case >= 3:
                    {
                        var tipCapHalf = (tipCapSmoothness + 1) / 4;
                        colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexBeforeTip + tipCapHalf]);
                        colliderPoints[i++] =
                            tipTransform.InverseTransformPoint(vertices[indexBeforeTip + (tipCapSmoothness + 1) / 2]);
                        colliderPoints[i++] = tipTransform.InverseTransformPoint(vertices[indexAfterTip - tipCapHalf]);
                        break;
                    }
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
    }
}