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
        
        private void InitializeMesh()
        {
            meshFilter = Pivot.GetComponent<MeshFilter>();
            meshRenderer = Pivot.GetComponent<MeshRenderer>();
            meshRenderer.material = material;
            materialBlock = new MaterialPropertyBlock();
            materialBlock.SetColor("_Color", color);
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
            points[0] = Utilities.GetPoint(Pivot.position, Segments[0].position, Segments[1].position, Tip.position, 0);

            int firstIndex = (pivotCapSmoothness + 1) / 2,
                lastIndex = vertices.Length - (pivotCapSmoothness + 1) + firstIndex;
            for (int i = 1; i < smoothness; i++)
            {
                points[i] = Utilities.GetPoint(Pivot.position, Segments[0].position, Segments[1].position, Tip.position,
                    i / (smoothness - 1f));
                var curve = shape.Evaluate(Utilities.Normalize(i, smoothness));
                vertices[i + firstIndex] = points[i] +
                                           Utilities.GetPerpendicular(points[i - 1], points[i]).normalized * (curve * width);
                vertices[lastIndex - i] = points[i] +
                                          Utilities.GetPerpendicular(points[i - 1], points[i], -1f).normalized * (curve * width);

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

                int step = 180 / (tipCapSmoothness + 1),
                    indexAfterTip = smoothness + tipCapSmoothness + firstIndex,
                    indexBeforeTip = smoothness - 1 + firstIndex,
                    length = tipCapSmoothness + 1;
                for (int i = 1; i < length; i++)
                {
                    vertices[indexAfterTip - i] =
                        Utilities.RotatePointAroundPivot(vertices[indexBeforeTip], Tip.position, step * i + 180);
                    if (renewUVs)
                        uvs[indexAfterTip - i] =
                            Utilities.RotatePointAroundPivot(uvs[indexBeforeTip], pivot, step * i + 180);
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
                        vertices[firstIndex - i] =
                            Utilities.RotatePointAroundPivot(vertices[lastIndex], Pivot.position, angle);
                        if (renewUVs)
                            uvs[firstIndex - i] = Utilities.RotatePointAroundPivot(uvs[lastIndex], pivot, angle);
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
                if (textureType == UVsLayout.SolidColor)
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
            

            if (renewUVs && textureType == UVsLayout.Stretchy)
                renewUVs = false;
        }
    }
}