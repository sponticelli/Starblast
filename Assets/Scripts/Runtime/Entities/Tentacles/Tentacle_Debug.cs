using UnityEngine;

namespace Starblast.Entities.Tentacles
{
    public partial class Tentacle
    {
#if UNITY_EDITOR
        private void DrawDebug()
        {
            if (debugTriangles)
            {
                for (int i = 0; i < vertices.Length - 2; i++)
                {
                    Debug.DrawLine(vertices[i], vertices[vertices.Length - 2 - i], Color.magenta);
                    Debug.DrawLine(vertices[i], vertices[vertices.Length - 1 - i], Color.magenta);
                }
            }

            if (debugOutline || debugTriangles)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    Debug.DrawLine(vertices[i], i >= vertices.Length - 1 ? vertices[0] : vertices[i + 1],
                        Color.magenta);
                }
            }

            if (debugUVs)
            {
                for (int i = 0; i < uvs.Length; i++)
                {
                    Debug.DrawLine(uvs[i], i >= uvs.Length - 1 ? uvs[0] : uvs[i + 1], Color.cyan);
                }
            }
        }

        

        private void OnDrawGizmos()
        {
            if (meshRenderer != null)
            {
                var bounds2 = meshRenderer.bounds;
                Gizmos.color = Color.gray;
                Gizmos.DrawWireCube(bounds2.center, bounds2.size);
            }
        }
#endif
    }
}