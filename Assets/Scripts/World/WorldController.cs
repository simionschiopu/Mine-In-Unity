using System.Collections.Generic;
using UnityEngine;

namespace World
{
    public class WorldController : MonoBehaviour
    {
        private readonly List<FlatChunk> chunks = new List<FlatChunk>();

        public int ChunkWidth = 16;

        public int ChunkHeight = 50;

        public int Seed;

        [Range(40, 200)]
        [SerializeField]
        private float viewRange = 65;

        [SerializeField]
        private FlatChunk noiseChunkFab;

        [SerializeField]
        private Transform playerTransfrom;

        private void Awake()
        {
            if (Seed == 0)
                Seed = Random.Range(0, int.MaxValue);
        }

        private void Update()
        {
            for (var x = playerTransfrom.position.x - viewRange;
                x < playerTransfrom.position.x + viewRange;
                x += ChunkWidth)
            {
                for (var z = playerTransfrom.position.z - viewRange;
                    z < playerTransfrom.position.z + viewRange;
                    z += ChunkWidth)
                {
                    var pos = new Vector3(x, 0, z);
                    pos.x = Mathf.Floor(pos.x / ChunkWidth) * ChunkWidth;
                    pos.z = Mathf.Floor(pos.z / ChunkWidth) * ChunkWidth;
                    var chunk = FindChunk(pos);

                    if (chunk != null) continue;

                    if (Vector3.Distance(pos, playerTransfrom.position) < viewRange)
                    {
                        var newChunk = Instantiate(noiseChunkFab, pos, Quaternion.identity, transform);
                        newChunk.Init(this);
                        chunks.Add(newChunk);
                    }
                }
            }

            for (var chunkIndex = 0; chunkIndex < chunks.Count; chunkIndex++)
            {
                var chunk = chunks[chunkIndex];

                if (Vector3.Distance(playerTransfrom.position - Vector3.up * playerTransfrom.position.y,
                        chunk.transform.position) > viewRange)
                {
                    Destroy(chunk);
                    chunks.RemoveAt(chunkIndex);
                }
            }
        }

        public FlatChunk FindChunk(Vector3 pos)
        {
            foreach (var chunk in chunks)
            {
                var cpos = chunk.transform.position;
                if (pos.x < cpos.x
                    || pos.z < cpos.z
                    || pos.x >= cpos.x + ChunkWidth
                    || pos.z >= cpos.z + ChunkWidth) continue;

                return chunk;
            }

            return null;
        }
    }
}
