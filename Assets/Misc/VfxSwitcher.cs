using UnityEngine;
using UnityEngine.VFX;

public sealed class VfxSwitcher : MonoBehaviour
{
    #region Scene object references

    [SerializeField] CameraController _controller = null;
    [SerializeField] VisualEffect[] _vfxList = null;
    [SerializeField] VisualEffect _proxyVfx = null;
    [SerializeField] VisualEffect _afterimageVfx = null;

    #endregion

    #region Private members

    Color _proxyColor;

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => _proxyColor = _proxyVfx.GetVector4("Line Color");

    void Update()
    {
        var zoom = _controller.ZoomParam;
        _proxyVfx.SetVector4("Line Color", _proxyColor * Mathf.Clamp01(zoom * 3));
        _afterimageVfx.SetBool("Spawn", zoom > 0.1f);

        _vfxList[0].SetBool("Spawn", true);
        _vfxList[1].SetBool("Spawn", false);
    }

    #endregion
}
