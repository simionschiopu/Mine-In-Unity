using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World.Externals;

namespace World.Chunks
{
    public sealed class NoiseChunk : Chunk
    {
        private byte GetTheoreticalByte(Vector3 pos)
        {
            Random.InitState(World.Seed);

            var grain0Offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
            var grain1Offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
            var grain2Offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);

            return GetTheoreticalByte(pos, grain0Offset, grain1Offset, grain2Offset);
        }

        private byte GetTheoreticalByte(Vector3 pos, Vector3 offset0, Vector3 offset1, Vector3 offset2)
        {
            float heightBase = 10;
            float maxHeight = Height - 10;
            float heightSwing = maxHeight - heightBase;
            byte brick = 1;
            float clusterValue = CalculateNoiseValue(pos, offset1, 0.02f);
            float blobValue = CalculateNoiseValue(pos, offset1, 0.05f);
            float mountainValue = CalculateNoiseValue(pos, offset0, 0.009f);

            if (mountainValue == 0 && blobValue < 0.2f)
                brick = 2;
            else if (clusterValue > 0.9f)
                brick = 1;
            else if (clusterValue > 0.8f)
                brick = 3;

            mountainValue = Mathf.Sqrt(mountainValue);
            mountainValue *= heightSwing;
            mountainValue += heightBase;
            mountainValue += blobValue * 10 - 5f;

            return mountainValue >= pos.y ? brick : (byte) 0;
        }

        protected override void CalculateMapFromScratch()
        {
            Random.InitState(World.Seed);

            var grain0Offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
            var grain1Offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
            var grain2Offset = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);

            for (int x = 0; x < World.ChunkWidth; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z < Width; z++)
                    {
                        Map[x, y, z] = GetTheoreticalByte(
                            new Vector3(x, y, z) + transform.position,
                            grain0Offset,
                            grain1Offset,
                            grain2Offset);
                    }
                }
            }
        }

        private static float CalculateNoiseValue(Vector3 pos, Vector3 offset, float scale)
        {
            var noiseX = Mathf.Abs((pos.x + offset.x) * scale);
            var noiseY = Mathf.Abs((pos.y + offset.y) * scale);
            var noiseZ = Mathf.Abs((pos.z + offset.z) * scale);

            return Mathf.Max(0, Noise.Generate(noiseX, noiseY, noiseZ));
        }

        public override IEnumerator CreateVisualMesh(bool isChunkload = true)
        {
            VisualMesh = new Mesh();
            var verts = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    for (var z = 0; z < Width; z++)
                    {
                        if (Map[x, y, z] == 0)
                            continue;

                        var brick = Map[x, y, z];
                        // Left wall
                        if (IsTransparent(x - 1, y, z))
                            BuildFace(brick == 0x1 ? (byte) 5 : brick, new Vector3(x, y, z), Vector3.up,
                                Vector3.forward,
                                false, verts, uvs, tris);
                        // Right wall
                        if (IsTransparent(x + 1, y, z))
                            BuildFace(brick == 0x1 ? (byte) 5 : brick, new Vector3(x + 1, y, z), Vector3.up,
                                Vector3.forward, true, verts, uvs, tris);
                        // Bottom wall
                        if (IsTransparent(x, y - 1, z))
                            BuildFace(brick, new Vector3(x, y, z), Vector3.forward, Vector3.right, false, verts, uvs,
                                tris);
                        // Top wall
                        if (IsTransparent(x, y + 1, z))
                            BuildFace(brick == 0x1 ? (byte) 6 : brick, new Vector3(x, y + 1, z), Vector3.forward,
                                Vector3.right, true, verts, uvs, tris);
                        // Back
                        if (IsTransparent(x, y, z - 1))
                            BuildFace(brick == 0x1 ? (byte) 5 : brick, new Vector3(x, y, z), Vector3.up, Vector3.right,
                                true, verts, uvs, tris);
                        // Front
                        if (IsTransparent(x, y, z + 1))
                            BuildFace(brick == 0x1 ? (byte) 5 : brick, new Vector3(x, y, z + 1), Vector3.up,
                                Vector3.right,
                                false, verts, uvs, tris);
                    }
                }

                if (isChunkload && Time.time > Time.deltaTime)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            VisualMesh.Clear();

            VisualMesh.vertices = verts.ToArray();
            VisualMesh.uv = uvs.ToArray();
            VisualMesh.triangles = tris.ToArray();
            VisualMesh.RecalculateBounds();
            VisualMesh.RecalculateNormals();
            MeshFilter.mesh = VisualMesh;
            MeshCollider.sharedMesh = null;
            MeshCollider.sharedMesh = VisualMesh;

            yield return 0;
        }

        private void BuildFace(byte brick, Vector3 corner, Vector3 up, Vector3 right, bool reversed,
            List<Vector3> verts, List<Vector2> uvs, List<int> tris)
        {
            int index = verts.Count;
            verts.Add(corner);
            verts.Add(corner + up);
            verts.Add(corner + up + right);
            verts.Add(corner + right);
            var uvWidth = new Vector2(0.25f, 0.25f);
            var uvCorner = new Vector2((float) (brick % 4 - 1) / 4, 1f - ((Mathf.Floor(brick / 4f) + 1) * 0.25f));
            uvs.Add(uvCorner);
            uvs.Add(new Vector2(uvCorner.x, uvCorner.y + uvWidth.y));
            uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y + uvWidth.y));
            uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y));

            if (reversed)
            {
                tris.Add(index + 0);
                tris.Add(index + 1);
                tris.Add(index + 2);
                tris.Add(index + 2);
                tris.Add(index + 3);
                tris.Add(index + 0);
            }
            else
            {
                tris.Add(index + 1);
                tris.Add(index + 0);
                tris.Add(index + 2);
                tris.Add(index + 3);
                tris.Add(index + 2);
                tris.Add(index + 0);
            }
        }

        private bool IsTransparent(int x, int y, int z)
        {
            if (y < 0)
                return false;

            byte brick = GetByte(x, y, z);
            switch (brick)
            {
                case 0:
                    return true;
                default:
                    return false;
            }
        }

        public override byte GetByte(int x, int y, int z)
        {
            if (y < 0 || y >= Height)
                return 0;

            if (x >= 0 && z >= 0 && x < Width && z < Width) return Map[x, y, z];

            var worldPos = new Vector3(x, y, z) + transform.position;
            var chunk = World.FindChunk(worldPos);
            if (chunk == this)
                return 0;

            return chunk == null
                ? GetTheoreticalByte(worldPos)
                : chunk.GetByte(worldPos);
        }

        public override byte GetByte(Vector3 worldPos)
        {
            worldPos -= transform.position;
            var x = Mathf.FloorToInt(worldPos.x);
            var y = Mathf.FloorToInt(worldPos.y);
            var z = Mathf.FloorToInt(worldPos.z);

            return GetByte(x, y, z);
        }

        public override bool SetBrick(byte brick, Vector3 worldPos)
        {
            worldPos -= transform.position;

            return SetBrick(brick, Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y),
                Mathf.FloorToInt(worldPos.z));
        }

        public override bool SetBrick(byte brick, int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Height || z >= Width)
                return false;

            if (Map[x, y, z] == brick)
                return false;

            Map[x, y, z] = brick;
            StartCoroutine(CreateVisualMesh(false));

            if (x == 0)
            {
                var chunk = World.FindChunk(new Vector3(x - 2, y, z) + transform.position);
                if (chunk != null)
                {
                    StartCoroutine(chunk.CreateVisualMesh(false));
                }
            }

            if (x == Width - 1)
            {
                var chunk = World.FindChunk(new Vector3(x + 2, y, z) + transform.position);
                if (chunk != null)
                {
                    StartCoroutine(chunk.CreateVisualMesh(false));
                }
            }

            if (z == 0)
            {
                var chunk = World.FindChunk(new Vector3(x, y, z - 2) + transform.position);
                if (chunk != null)
                {
                    StartCoroutine(chunk.CreateVisualMesh(false));
                }
            }

            if (z == Width - 1)
            {
                var chunk = World.FindChunk(new Vector3(x, y, z + 2) + transform.position);
                if (chunk != null)
                {
                    StartCoroutine(chunk.CreateVisualMesh(false));
                }
            }

            return true;
        }

        private void OnDestroy()
        {
            Destroy(MeshFilter.sharedMesh);
            Destroy(MeshRenderer.material);
        }
    }
}
