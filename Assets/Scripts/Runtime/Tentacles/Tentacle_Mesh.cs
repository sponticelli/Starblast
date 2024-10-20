using Starblast.Extensions;
using UnityEngine;

namespace Starblast.Tentacles
{
    public partial class Tentacle
    {
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Vector2[] vertices;
        private Vector2[] points;
        private Vector2[] uvs;
        private Mesh mesh;
        private MaterialPropertyBlock materialBlock;

        private bool renewUVs = true;

        private int VerticesCount => smoothness * 2 + tipCapSmoothness + pivotCapSmoothness;

        private static readonly int ShaderColorProperty = Shader.PropertyToID("_Color");

#if UNITY_EDITOR
        private UVsLayout storedTextureType;
#endif

        private void InitializeMesh()
        {
            meshFilter = Pivot.GetComponent<MeshFilter>();
            meshRenderer = Pivot.GetComponent<MeshRenderer>();
            meshRenderer.material = material;
            materialBlock = new MaterialPropertyBlock();
            materialBlock.SetColor(ShaderColorProperty, color);
            meshRenderer.SetPropertyBlock(materialBlock);

            mesh = new Mesh { name = "TentacleMesh" };
            mesh.MarkDynamic();
            meshFilter.sharedMesh = mesh;

            vertices = new Vector2[VerticesCount];
        }

        private void InitializeUVs()
        {
            uvs = new Vector2[VerticesCount];
            renewUVs = true;
        }

        private void DefineVertices()
        {
            EnsureVertexArraySizes();

            UpdateUVs(out float segmentStep);

            points[0] = Pivot.position.GetCubicBezierPoint(Segments[0].position, Segments[1].position, Tip.position, 0);

            CalculateVerticesPositions(segmentStep);
            CalculateTipCapVertices(segmentStep);
            CalculatePivotCapVertices();
        }

        private void EnsureVertexArraySizes()
        {
            if (vertices.Length != VerticesCount)
                vertices = new Vector2[VerticesCount];

            if (points == null || points.Length != smoothness)
                points = new Vector2[smoothness];
        }

        private void CalculateVerticesPositions(float segmentStep)
        {
            int firstIndex = (pivotCapSmoothness + 1) / 2;
            int lastIndex = vertices.Length - (pivotCapSmoothness + 1) + firstIndex;
            Vector2 perpendicular = default;

            for (int i = 1; i < smoothness; i++)
            {
                points[i] = Pivot.position.GetCubicBezierPoint(Segments[0].position, Segments[1].position, Tip.position,
                    i / (smoothness - 1f));
                var curve = shape.Evaluate(((float)i).Normalize(0, smoothness));

                points[i - 1].GetPerpendicularNoAlloc(points[i], ref perpendicular);
                vertices[i + firstIndex] = points[i] + perpendicular.normalized * (curve * width);

                points[i - 1].GetPerpendicularNoAlloc(points[i], ref perpendicular, -1f);
                vertices[lastIndex - i] = points[i] + perpendicular.normalized * (curve * width);

                if (renewUVs)
                {
                    var y = segmentStep * i / width * .5f;
                    uvs[i + firstIndex] = new Vector2(0, y);
                    uvs[lastIndex - i] = new Vector2(1f, y);

#if UNITY_EDITOR
                    if (debugUVs) Debug.DrawLine(uvs[i + firstIndex], uvs[lastIndex - i], Color.cyan);
#endif
                }
            }

            SetFirstPointVertices(firstIndex, lastIndex);
        }

        private void SetFirstPointVertices(int firstIndex, int lastIndex)
        {
            var curve = shape.Evaluate(0) * width;
            Vector2 perpendicular = default;

            points[0].GetPerpendicularNoAlloc(points[1], ref perpendicular);
            vertices[firstIndex] = points[0] + perpendicular.normalized * curve;

            points[0].GetPerpendicularNoAlloc(points[1], ref perpendicular, -1f);
            vertices[lastIndex] = points[0] + perpendicular.normalized * curve;

            if (renewUVs)
            {
                uvs[firstIndex] = new Vector2(0, 0);
                uvs[lastIndex] = new Vector2(1f, 0);
            }
        }

        private void CalculateTipCapVertices(float segmentStep)
        {
            if (tipCapSmoothness == 0) return;

            Vector2 pivot = default;
            if (renewUVs) pivot = new Vector2(.5f, segmentStep * (smoothness - 1) / width * .5f);

            var step = 180 / (tipCapSmoothness + 1);
            int indexAfterTip = smoothness + tipCapSmoothness + (pivotCapSmoothness + 1) / 2;
            int indexBeforeTip = smoothness - 1 + (pivotCapSmoothness + 1) / 2;

            for (int i = 1; i <= tipCapSmoothness; i++)
            {
                vertices[indexAfterTip - i] =
                    RotatePointAroundPivot(vertices[indexBeforeTip], Tip.position, step * i + 180);
                if (renewUVs)
                    uvs[indexAfterTip - i] = RotatePointAroundPivot(uvs[indexBeforeTip], pivot, step * i + 180);
            }
        }

        private void CalculatePivotCapVertices()
        {
            if (pivotCapSmoothness == 0) return;

            Vector2 pivot = default;
            if (renewUVs) pivot = new Vector2(.5f, 0);

            var step = 180 / (pivotCapSmoothness + 1);
            int j = 1;
            for (int i = 1; i <= pivotCapSmoothness + 1; i++)
            {
                var angle = step * i + 180;
                if (i <= (pivotCapSmoothness + 1) / 2)
                {
                    vertices[(pivotCapSmoothness + 1) / 2 - i] =
                        RotatePointAroundPivot(vertices[vertices.Length - 1], Pivot.position, angle);
                    if (renewUVs)
                        uvs[(pivotCapSmoothness + 1) / 2 - i] =
                            RotatePointAroundPivot(uvs[vertices.Length - 1], pivot, angle);
                }
                else
                {
                    var index = vertices.Length - j++;
                    vertices[index] = RotatePointAroundPivot(vertices[vertices.Length - 1], Pivot.position, angle);
                    if (renewUVs) uvs[index] = RotatePointAroundPivot(uvs[vertices.Length - 1], pivot, angle);
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
            if (!renewUVs) return;

            if (textureType == UVsLayout.SolidColor)
            {
                Vector2 position = new Vector2(.5f, .5f);
                for (int i = 0; i < uvs.Length; i++)
                    uvs[i] = position;

                renewUVs = false;
            }
            else
            {
                float distance = (Tip.position - Pivot.position).magnitude;
                float minDistance = 0f;
                foreach (var joint in Joints)
                    minDistance += joint.distance;

                segmentStep = Mathf.Clamp(distance, minDistance * .5f, distance) / (smoothness - 1);
            }
        }

        private void RenderMesh()
        {
            var meshVertices = new Vector3[vertices.Length];
            float z = transform.position.z;
            for (int i = 0; i < vertices.Length; i++)
            {
                meshVertices[i] = Pivot.transform.InverseTransformPoint(new Vector3(vertices[i].x, vertices[i].y, z));
            }

            meshFilter.sharedMesh.vertices = meshVertices;

            GenerateTriangles();
            meshFilter.sharedMesh.uv = uvs;

            if (renewUVs && textureType == UVsLayout.Stretchy)
                renewUVs = false;
        }

        private void GenerateTriangles()
        {
            int index = 0;
            int length = (vertices.Length - (vertices.Length % 2 == 0 ? 2 : 1)) * 3;
            int[] triangles = new int[length];

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
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (meshFilter.sharedMesh != null)
            {
                DestroyImmediate(meshFilter.sharedMesh);
            }
#endif
        }

        private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
        {
            return Quaternion.Euler(0, 0, angle) * (point - pivot) + pivot;
        }
    }
}