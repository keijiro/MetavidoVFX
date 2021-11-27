using UnityEngine;
using Bibcam.Decoder;

namespace Bibcam {

// Supports frame rate conversion

[RequireComponent(typeof(Camera))]
sealed class CameraController : MonoBehaviour
{
    #region Scene object references

    [SerializeField] float _updateRate = 30;
    [SerializeField] BibcamMetadataDecoder _decoder = null;

    #endregion

    #region Private members

    float _timer;

    float Interval => 1 / _updateRate;

    #endregion

    #region MonoBehaviour implementation

    void LateUpdate()
    {
        var meta = _decoder.Metadata;
        if (!meta.IsValid) return;

        _timer += Time.deltaTime;

        if (_timer > Interval)
        {
            transform.position = meta.CameraPosition;
            transform.rotation = meta.CameraRotation;
            _timer %= Interval;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, meta.CameraPosition, 0.5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, meta.CameraRotation, 0.5f);
        }

        var camera = GetComponent<Camera>();
        camera.projectionMatrix =
          meta.ReconstructProjectionMatrix(camera.projectionMatrix);
    }

    #endregion
}

} // namespace Bibcam
