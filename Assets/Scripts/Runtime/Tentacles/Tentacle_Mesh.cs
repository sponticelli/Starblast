using Starblast.Extensions;
using UnityEngine;

namespace Starblast.Tentacles
{
    public partial class Tentacle
    {
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Vector2[] vertices;
        private Vector2[] uvs;
        private Mesh mesh;
        private MaterialPropertyBlock materialBlock;

        private bool renewUVs = true;

        private int VerticesCount => smoothness * 2 + tipCapSmoothness + pivotCapSmoothness;

        private static readonly int ShaderColorProperty = Shader.PropertyToID("_Color");

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
            points[0] = Pivot.position.GetCubicBezierPoint(Segments[0].position, Segments[1].position, Tip.position, 0);

            var firstIndex = (pivotCapSmoothness + 1) / 2;
            var lastIndex = vertices.Length - (pivotCapSmoothness + 1) + firstIndex;
            Vector2 perpendicular = default;
            for (int i = 1; i < smoothness; i++)
            {
                points[i] = Pivot.position.GetCubicBezierPoint(Segments[0].position, Segments[1].position, Tip.position,
                    i / (smoothness - 1f));
                var curve = shape.Evaluate(((float)i).Normalize(0, smoothness));
                points[i - 1].GetPerpendicularNoAlloc(points[i], ref perpendicular);
                vertices[i + firstIndex] = points[i] +
                                           perpendicular.normalized * (curve * width);
                points[i - 1].GetPerpendicularNoAlloc(points[i], ref perpendicular, -1f);
                vertices[lastIndex - i] = points[i] +
                                          perpendicular.normalized * (curve * width);

                if (renewUVs)
                {
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
            points[0].GetPerpendicularNoAlloc(points[1], ref perpendicular);
            vertices[firstIndex] = points[0] + perpendicular.normalized * curve2;
            points[0].GetPerpendicularNoAlloc(points[1], ref perpendicular, -1f);
            vertices[lastIndex] = points[0] + perpendicular.normalized * curve2;

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

                var step = 180 / (tipCapSmoothness + 1);
                var indexAfterTip = smoothness + tipCapSmoothness + firstIndex;
                var indexBeforeTip = smoothness - 1 + firstIndex;
                var length = tipCapSmoothness + 1;
                for (int i = 1; i < length; i++)
                {
                    vertices[indexAfterTip - i] =
                        RotatePointAroundPivot(vertices[indexBeforeTip], Tip.position, step * i + 180);
                    if (renewUVs)
                    {
                        uvs[indexAfterTip - i] =
                            RotatePointAroundPivot(uvs[indexBeforeTip], pivot, step * i + 180);
                    }
                }
            }

            // Define pivot cap
            if (pivotCapSmoothness != 0)
            {
                Vector2 pivot = default;
                if (renewUVs) pivot = new Vector2(.5f, 0);

                var step = 180 / (pivotCapSmoothness + 1);
                var j = 1;
                var length = pivotCapSmoothness + 1;
                for (int i = 1; i <= length; i++)
                {
                    var angle = step * i + 180;
                    if (i <= firstIndex)
                    {
                        vertices[firstIndex - i] =
                            RotatePointAroundPivot(vertices[lastIndex], Pivot.position, angle);
                        if (renewUVs)
                            uvs[firstIndex - i] = RotatePointAroundPivot(uvs[lastIndex], pivot, angle);
                    }
                    else
                    {
                        var index = vertices.Length - j++;
                        vertices[index] = RotatePointAroundPivot(vertices[lastIndex], Pivot.position, angle);
                        if (renewUVs) uvs[index] = RotatePointAroundPivot(uvs[lastIndex], pivot, angle);
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
                if (textureType == UVsLayout.SolidColor)
                {
                    var position = new Vector2(.5f, .5f);
                    for (int i = 0; i < uvs.Length; i++)
                    {
                        uvs[i] = position;
                    }

                    renewUVs = false;
                }
                else
                {
                    var distance = 0f;
                    var minDistance = 0f;
                    for (int i = 0; i < Joints.Length; i++)
                    {
                        minDistance += Joints[i].distance;
                    }

                    distance = (Tip.position - Pivot.position).magnitude;
                    segmentStep = Mathf.Clamp(distance, minDistance * .5f, distance) / (smoothness - 1);
                }
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
            {
                meshVertices[i] = Pivot.transform.InverseTransformPoint(new Vector3(vertices[i].x, vertices[i].y, z));
            }

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


            if (renewUVs && textureType == UVsLayout.Stretchy)
            {
                renewUVs = false;
            }
        }


#else
        //private Mesh mesh;

        private void RenderMesh()
        {
            var meshVertices = new Vector3[vertices.Length];

            var z = transform.position.z;
            for (int i = 0; i < vertices.Length; i++)
            {
                meshVertices[i] = Pivot.transform.InverseTransformPoint(new Vector3(vertices[i].x, vertices[i].y, z));
            }
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
                if (textureType == UVsLayout.stretchy) 
                {
                    renewUVs = false;
                }
            }
        }
#endif

        private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle) =>
            Quaternion.Euler(new Vector3(0, 0, angle)) * (point - pivot) + pivot;
    }
}