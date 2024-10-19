using UnityEngine;

namespace Starblast.Tentacles
{
    public class TentacleMesh
    {
        // Fields
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Vector2[] vertices;
        private Vector2[] uvs;
        private Mesh mesh;
        private MaterialPropertyBlock materialBlock;

        private Rigidbody2D _pivot;

        public Vector2[] Vertices
        {
            get => vertices;
            set => vertices = value;
        }
        public MaterialPropertyBlock MaterialBlock => materialBlock;
        public MeshRenderer MeshRenderer => meshRenderer;
        public MeshFilter MeshFilter => meshFilter;

        public Vector2[] UVs
        {
            get => uvs;
            set => uvs = value;
        }
        public void InitializeMesh(Rigidbody2D pivot, Material material, Color color) 
        {
            _pivot = pivot;
            meshFilter = _pivot.GetComponent<MeshFilter>();
            meshRenderer = _pivot.GetComponent<MeshRenderer>();
            meshRenderer.material = material;
            materialBlock = new MaterialPropertyBlock();
            materialBlock.SetColor("_Color", color);
            meshRenderer.SetPropertyBlock(materialBlock);

            mesh = new Mesh { name = "TentacleMesh" };
            mesh.MarkDynamic();
            meshFilter.sharedMesh = mesh;
        }

        public void DefineVertices()
        {
            throw new System.NotImplementedException();
        }

        public void RenderMesh()
        {
            throw new System.NotImplementedException();
        }
    }
}