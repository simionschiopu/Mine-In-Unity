using System.Collections;
using UnityEngine;

namespace World.Chunks
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshFilter))]
    public abstract class Chunk : MonoBehaviour
    {
        [SerializeField]
        protected MeshCollider MeshCollider;

        [SerializeField]
        protected MeshFilter MeshFilter;

        [SerializeField]
        protected MeshRenderer MeshRenderer;

        protected Mesh VisualMesh;

        protected WorldController World;

        protected int Width;
        protected int Height;

        protected byte[,,] Map;

        public void Init(WorldController worldController)
        {
            World = worldController;
            name = "Chunk " + transform.position.x + " | " + transform.position.z;

            Width = worldController.ChunkWidth;
            Height = World.ChunkHeight;

            Map = new byte[Width, Height, Width];

            CalculateMapFromScratch();
            StartCoroutine(CreateVisualMesh());
        }

        protected abstract void CalculateMapFromScratch();

        public abstract IEnumerator CreateVisualMesh(bool isChunkload = true);

        public abstract byte GetByte(int x, int y, int z);
        public abstract byte GetByte(Vector3 worldPos);

        public abstract bool SetBrick(byte brick, Vector3 worldPos);
        public abstract bool SetBrick(byte brick, int x, int y, int z);
    }
}
