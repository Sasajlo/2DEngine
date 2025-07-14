using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Engine.Components
{
    /// <summary>
    /// Represents a chunk mesh with vertices and indices for optimized rendering
    /// </summary>
    public class ChunkMesh
    {
        public List<Vector3> vertices;
        public List<int> indices;
        public List<uint> textureIndices;  // Texture index for each quad
        public List<Vector4> colors;       // Color for each quad
        public bool isGenerated;
        // Cached arrays for efficient rendering
        public float[] vertexArray;
        public uint[] indexArray;
        public float[] colorArray;
        public uint[] textureIndexArray;
        
        public ChunkMesh()
        {
            vertices = new List<Vector3>();
            indices = new List<int>();
            textureIndices = new List<uint>();
            colors = new List<Vector4>();
            isGenerated = false;
            vertexArray = null;
            indexArray = null;
            colorArray = null;
            textureIndexArray = null;
        }
        
        public void Clear()
        {
            vertices.Clear();
            indices.Clear();
            textureIndices.Clear();
            colors.Clear();
            isGenerated = false;
            vertexArray = null;
            indexArray = null;
            colorArray = null;
            textureIndexArray = null;
        }
    }

    public class Chunk
    {
        public const int CHUNK_SIZE = 32;
        
        public int chunkX { get; private set; }
        public int chunkY { get; private set; }
        public Vector2 worldPosition { get; private set; }
        public Vector2 worldBounds { get; private set; } // Size of chunk in world units
        
        private Tile[,] _tiles;
        private bool _isVisible;
        private ChunkMesh _chunkMesh;
        private float _tileSize;
        
        public Chunk(int chunkX, int chunkY, float tileSize)
        {
            this.chunkX = chunkX;
            this.chunkY = chunkY;
            this._tileSize = tileSize;
            
            // Calculate world position (bottom-left corner of chunk)
            worldPosition = new Vector2(chunkX * CHUNK_SIZE * tileSize, chunkY * CHUNK_SIZE * tileSize);
            worldBounds = new Vector2(CHUNK_SIZE * tileSize, CHUNK_SIZE * tileSize);
            
            _tiles = new Tile[CHUNK_SIZE, CHUNK_SIZE];
            _isVisible = false;
            _chunkMesh = new ChunkMesh();
        }
        
        public void SetTile(int localX, int localY, Tile tile)
        {
            if (IsValidLocalPosition(localX, localY))
            {
                _tiles[localX, localY] = tile;
                // Invalidate mesh when tiles change
                _chunkMesh.isGenerated = false;
            }
        }
        
        public Tile GetTile(int localX, int localY)
        {
            if (IsValidLocalPosition(localX, localY))
            {
                return _tiles[localX, localY];
            }
            return null;
        }
        
        public bool IsValidLocalPosition(int localX, int localY)
        {
            return localX >= 0 && localX < CHUNK_SIZE && localY >= 0 && localY < CHUNK_SIZE;
        }
        
        public void SetVisible(bool visible)
        {
            _isVisible = visible;
        }
        
        public bool IsVisible()
        {
            return _isVisible;
        }
        
        /// <summary>
        /// Generate chunk mesh by combining all tile vertices
        /// </summary>
        public void GenerateChunkMesh()
        {
            if (_chunkMesh.isGenerated)
                return;
                
            _chunkMesh.Clear();
            
            // Create vertex grid: (CHUNK_SIZE + 1) x (CHUNK_SIZE + 1) vertices
            int vertexGridSize = CHUNK_SIZE + 1;
            
            // Generate all vertices first (Y outer, X inner)
            for (int y = 0; y < vertexGridSize; y++)
            {
                for (int x = 0; x < vertexGridSize; x++)
                {
                    // Calculate world position for this vertex (centered like tiles)
                    Vector2 worldPos = LocalToWorldPosition(new Vector2((x - 0.5f) * _tileSize, (y - 0.5f) * _tileSize));
                    Vector3 vertex = new Vector3(worldPos.X, worldPos.Y, 0);
                    _chunkMesh.vertices.Add(vertex);
                }
            }
            
            // Generate indices and texture data for visible tiles
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    var tile = _tiles[x, y];
                    if (tile != null && tile.isVisible && tile.tileData != null)
                    {
                        AddTileIndices(x, y, vertexGridSize);
                        
                        // Add texture index for this quad
                        _chunkMesh.textureIndices.Add(tile.tileData.textureIndex);
                        
                        // Add color for this quad
                        _chunkMesh.colors.Add(tile.tileData.color.ToVector4());
                    }
                }
            }
            
            // Cache vertex and index arrays for efficient rendering
            _chunkMesh.vertexArray = new float[_chunkMesh.vertices.Count * 3];
            for (int i = 0; i < _chunkMesh.vertices.Count; i++)
            {
                _chunkMesh.vertexArray[i * 3] = _chunkMesh.vertices[i].X;
                _chunkMesh.vertexArray[i * 3 + 1] = _chunkMesh.vertices[i].Y;
                _chunkMesh.vertexArray[i * 3 + 2] = _chunkMesh.vertices[i].Z;
            }
            _chunkMesh.indexArray = new uint[_chunkMesh.indices.Count];
            for (int i = 0; i < _chunkMesh.indices.Count; i++)
            {
                _chunkMesh.indexArray[i] = (uint)_chunkMesh.indices[i];
            }
            
            _chunkMesh.colorArray = new float[_chunkMesh.colors.Count * 4];
            for (int i = 0; i < _chunkMesh.colors.Count; i++)
            {
                _chunkMesh.colorArray[i * 4] = _chunkMesh.colors[i].X;
                _chunkMesh.colorArray[i * 4 + 1] = _chunkMesh.colors[i].Y;
                _chunkMesh.colorArray[i * 4 + 2] = _chunkMesh.colors[i].Z;
                _chunkMesh.colorArray[i * 4 + 3] = _chunkMesh.colors[i].W;
            }

            _chunkMesh.textureIndexArray = _chunkMesh.textureIndices.ToArray();
            
            _chunkMesh.isGenerated = true;
            
            // Debug output: print vertex count for this chunk
            if (DebugSystem.IsDebugMode)
            {
                Console.WriteLine($"Chunk ({chunkX}, {chunkY}) mesh: {_chunkMesh.vertices.Count} vertices, {_chunkMesh.indices.Count} indices, {_chunkMesh.textureIndices.Count} quads");
            }
        }
        
        /// <summary>
        /// Add indices for a single tile using shared vertices
        /// </summary>
        private void AddTileIndices(int localX, int localY, int vertexGridSize)
        {
            // Calculate vertex indices for this tile's corners
            int bottomLeft = localY * vertexGridSize + localX;
            int bottomRight = localY * vertexGridSize + (localX + 1);
            int topLeft = (localY + 1) * vertexGridSize + localX;
            int topRight = (localY + 1) * vertexGridSize + (localX + 1);
            
            // Add indices for two triangles forming a quad
            _chunkMesh.indices.Add(bottomLeft);   // bottom-left
            _chunkMesh.indices.Add(bottomRight);  // bottom-right
            _chunkMesh.indices.Add(topLeft);      // top-left
            
            _chunkMesh.indices.Add(bottomRight);  // bottom-right
            _chunkMesh.indices.Add(topRight);     // top-right
            _chunkMesh.indices.Add(topLeft);      // top-left
        }
        
        /// <summary>
        /// Render chunk mesh using efficient batch rendering
        /// </summary>
        public void RenderChunkMesh()
        {
            if (!_isVisible || !_chunkMesh.isGenerated || _chunkMesh.textureIndices.Count == 0)
                return;
            
            // Use cached arrays for vertices and indices
            var vertexArray = _chunkMesh.vertexArray;
            var indexArray = _chunkMesh.indexArray;
            
            // Convert texture indices to uint array
            var textureIndexArray = _chunkMesh.textureIndexArray;
            
            // Convert colors to float array
            var colorArray = _chunkMesh.colorArray;
            
            // Call C++ chunk rendering function
            EngineCore.RenderChunkMesh(
                vertexArray,
                (uint)_chunkMesh.vertices.Count,
                indexArray,
                (uint)_chunkMesh.indices.Count,
                textureIndexArray,
                colorArray,
                (uint)_chunkMesh.textureIndices.Count
            );
        }
        
        /// <summary>
        /// Render chunk mesh as wireframe in debug mode
        /// </summary>
        public void RenderChunkWireframe()
        {
            if (!_isVisible || !_chunkMesh.isGenerated)
                return;
            
            // Get camera zoom for line thickness scaling
            float lineThickness = 0.02f;
            var camera = Camera.mainCamera;
            if (camera != null)
            {
                // Normalize to zoom level (5.0f = default zoom)
                float zoomFactor = camera.orthographicSize / 5.0f;
                lineThickness = Math.Clamp(0.02f * zoomFactor, 0.01f, 0.15f);
            }
            
            // Render wireframe for each quad in the mesh
            for (int i = 0; i < _chunkMesh.indices.Count; i += 6) // 6 indices per quad
            {
                if (i + 5 >= _chunkMesh.indices.Count)
                    break;
                    
                // Get quad vertices
                Vector3 v0 = _chunkMesh.vertices[_chunkMesh.indices[i + 0]]; // bottom-left
                Vector3 v1 = _chunkMesh.vertices[_chunkMesh.indices[i + 1]]; // bottom-right
                Vector3 v2 = _chunkMesh.vertices[_chunkMesh.indices[i + 2]]; // top-left
                Vector3 v3 = _chunkMesh.vertices[_chunkMesh.indices[i + 3]]; // top-right
                
                // Draw wireframe lines
                Vector4 wireframeColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f); // Black
                
                // Draw quad outline
                DebugSystem.DrawLine(new Vector2(v0.X, v0.Y), new Vector2(v1.X, v1.Y), wireframeColor, lineThickness); // bottom
                DebugSystem.DrawLine(new Vector2(v1.X, v1.Y), new Vector2(v3.X, v3.Y), wireframeColor, lineThickness); // right
                DebugSystem.DrawLine(new Vector2(v3.X, v3.Y), new Vector2(v2.X, v2.Y), wireframeColor, lineThickness); // top
                DebugSystem.DrawLine(new Vector2(v2.X, v2.Y), new Vector2(v0.X, v0.Y), wireframeColor, lineThickness); // left
            }
        }
        
        /// <summary>
        /// Check if this chunk intersects with a given rectangular area
        /// </summary>
        public bool IntersectsWithRect(Vector2 rectMin, Vector2 rectMax)
        {
            Vector2 chunkMin = worldPosition;
            Vector2 chunkMax = worldPosition + worldBounds;
            
            return !(rectMax.X < chunkMin.X || rectMin.X > chunkMax.X || 
                     rectMax.Y < chunkMin.Y || rectMin.Y > chunkMax.Y);
        }
        
        /// <summary>
        /// Get all visible tiles in this chunk for rendering
        /// </summary>
        public List<Tile> GetVisibleTiles()
        {
            var visibleTiles = new List<Tile>();
            
            if (!_isVisible)
                return visibleTiles;
            
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    var tile = _tiles[x, y];
                    if (tile != null && tile.isVisible)
                    {
                        visibleTiles.Add(tile);
                    }
                }
            }
            
            return visibleTiles;
        }
        
        /// <summary>
        /// Render all visible tiles in this chunk
        /// </summary>
        public void Render()
        {
            if (!_isVisible)
                return;
            
            // Generate mesh if not already generated
            if (!_chunkMesh.isGenerated)
            {
                GenerateChunkMesh();
            }
            
            // Render chunk mesh efficiently
            RenderChunkMesh();
            
            // In debug mode, overlay chunk mesh wireframe
            if (DebugSystem.IsDebugMode)
            {
                RenderChunkWireframe();
            }
        }
        
        private void RenderTile(Tile tile)
        {
            if (tile.tileData == null)
                return;
                
            // Create transform matrix for the tile
            var transform = Matrix4x4.CreateScale(tile.scale) * 
                           Matrix4x4.CreateTranslation(tile.position);
            
            // Convert Matrix4x4 to float array
            var matrixArray = new float[16]
            {
                transform.M11, transform.M12, transform.M13, transform.M14,
                transform.M21, transform.M22, transform.M23, transform.M24,
                transform.M31, transform.M32, transform.M33, transform.M34,
                transform.M41, transform.M42, transform.M43, transform.M44
            };
            
            // Convert color to float array
            var color = tile.tileData.color.ToVector4();
            var colorArray = new float[4] { color.X, color.Y, color.Z, color.W };
            
            // Set default size (1x1 for tiles)
            var sizeArray = new float[2] { 1.0f, 1.0f };
            
            // Use texture index if available, otherwise fall back to path
            if (tile.tileData.textureIndex != uint.MaxValue)
            {
                // Call C++ rendering function with texture index
                EngineCore.RenderSpriteWithIndex(
                    tile.tileData.textureIndex,
                    matrixArray,
                    colorArray,
                    sizeArray,
                    tile.sortingOrder,
                    false, // flipX
                    false  // flipY
                );
            }
            else
            {
                // Fallback to path-based rendering
                EngineCore.RenderSprite(
                    tile.tileData.texture,
                    matrixArray,
                    colorArray,
                    sizeArray,
                    tile.sortingOrder,
                    false, // flipX
                    false  // flipY
                );
            }
        }
        
        /// <summary>
        /// Convert world coordinates to local chunk coordinates
        /// </summary>
        public Vector2 WorldToLocalPosition(Vector2 worldPos)
        {
            return worldPos - worldPosition;
        }
        
        /// <summary>
        /// Convert local chunk coordinates to world coordinates
        /// </summary>
        public Vector2 LocalToWorldPosition(Vector2 localPos)
        {
            return worldPosition + localPos;
        }
        
        /// <summary>
        /// Get mesh statistics for debugging
        /// </summary>
        public (int vertexCount, int indexCount, int quadCount) GetMeshStats()
        {
            if (!_chunkMesh.isGenerated)
                return (0, 0, 0);
                
            return (_chunkMesh.vertices.Count, _chunkMesh.indices.Count, _chunkMesh.indices.Count / 6);
        }
    }
} 