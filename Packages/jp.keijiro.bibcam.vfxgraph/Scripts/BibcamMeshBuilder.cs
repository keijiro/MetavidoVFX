using UnityEngine;
using UnityEngine.Rendering;
using Bibcam.Common;
using Bibcam.Decoder;

namespace Bibcam {

[ExecuteInEditMode]
sealed class BibcamMeshBuilder : MonoBehaviour
{
    #region Scene object references

    [SerializeField] BibcamMetadataDecoder _decoder = null;
    [SerializeField] BibcamTextureDemuxer _demuxer = null;

    #endregion

    #region Project asset references

    [SerializeField, HideInInspector] ComputeShader _compute = null;

    #endregion

    #region Private objects

    Mesh _mesh;
    GraphicsBuffer _vertexBuffer;
    GraphicsBuffer _indexBuffer;
    MaterialPropertyBlock _overrides;

    const int Decimation = 8;
    const int ColumnCount = 1920 / Decimation;
    const int RowCount = 1080 / Decimation;
    const int VertexCount = ColumnCount * RowCount;
    const int TriangleCount = (ColumnCount - 1) * (RowCount - 1) * 2;
    const int IndexCount = TriangleCount * 3;

    void PrepareMesh()
    {
        _mesh = new Mesh();
        _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

        // GraphicsBuffer access as Raw (ByteAddress) buffers
        _mesh.indexBufferTarget |= GraphicsBuffer.Target.Raw;
        _mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;

        // Vertex position: float32 x 3
        var vp = new VertexAttributeDescriptor
          (VertexAttribute.Position, VertexAttributeFormat.Float32, 3);

        // Vertex normal: float32 x 3
        var vn = new VertexAttributeDescriptor
          (VertexAttribute.Normal, VertexAttributeFormat.Float32, 3);

        // Texture coordinates: float32 x 2
        var vt = new VertexAttributeDescriptor
          (VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2);

        // Vertex/index buffer formats
        _mesh.SetVertexBufferParams(VertexCount, vp, vn, vt);
        _mesh.SetIndexBufferParams(IndexCount, IndexFormat.UInt32);

        // Submesh initialization
        _mesh.SetSubMesh(0, new SubMeshDescriptor(0, IndexCount),
                         MeshUpdateFlags.DontRecalculateBounds);

        // GraphicsBuffer references
        _vertexBuffer = _mesh.GetVertexBuffer(0);
        _indexBuffer = _mesh.GetIndexBuffer();

        // Mesh filter setup
        var mf = GetComponent<MeshFilter>();
        if (mf == null)
        {
            mf = gameObject.AddComponent<MeshFilter>();
            mf.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
        }
        mf.sharedMesh = _mesh;
    }

    #endregion

    #region MonoBehaviour implementation

    void OnDisable()
      => OnDestroy();

    void OnDestroy()
    {
        _vertexBuffer?.Dispose();
        _vertexBuffer = null;

        _indexBuffer?.Dispose();
        _indexBuffer = null;

        if (_mesh != null)
        {
            _mesh.DestroyDynamic();
            _mesh = null;
        }
    }

    void LateUpdate()
    {
        if (_decoder == null || _demuxer == null) return;

        // Run it only when the textures are ready.
        if (_demuxer.ColorTexture == null) return;

        // Mesh object lazy setup
        if (_mesh == null) PrepareMesh();

        // Data from the decoder/demuxer
        var meta = _decoder.Metadata;
        var ray = BibcamRenderUtils.RayParams(meta);
        var iview = BibcamRenderUtils.InverseView(meta);
        _compute.SetVector(ShaderID.RayParams, ray);
        _compute.SetMatrix(ShaderID.InverseView, iview);
        _compute.SetTexture(0, ShaderID.DepthTexture, _demuxer.DepthTexture);

        // Vertex reconstruction
        _compute.SetInts("Dims", ColumnCount, RowCount);
        _compute.SetBuffer(0, "VertexBuffer", _vertexBuffer);
        _compute.DispatchThreads(0, ColumnCount, RowCount);

        // Index array generation
        _compute.SetBuffer(1, "IndexBuffer", _indexBuffer);
        _compute.DispatchThreads(1, ColumnCount - 1, RowCount - 1);

        // Material override for the color texture
        if (_overrides == null) _overrides = new MaterialPropertyBlock();
        var renderer = GetComponent<MeshRenderer>();
        renderer.GetPropertyBlock(_overrides);
        _overrides.SetTexture(ShaderID.ColorTexture, _demuxer.ColorTexture);
        renderer.SetPropertyBlock(_overrides);
    }

    #endregion
}

} // namespace Bibcam
