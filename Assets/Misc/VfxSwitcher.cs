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

    #region Public properties

    [field:SerializeField] public float Interval { get; set; } = 3;

    #endregion

    #region Private members

    Color _proxyColor;

    #endregion

    #region MonoBehaviour implementation

    async Awaitable Start()
    {
        _proxyColor = _proxyVfx.GetVector4("Line Color");

        for (var sel = 0;; sel = (sel + 1) % _vfxList.Length)
        {
            for (var i = 0; i < _vfxList.Length; i++)
                _vfxList[i].SetBool("Spawn", i == sel);

            await Awaitable.WaitForSecondsAsync(Interval);
        }
    }

    void Update()
    {
        var zoom = _controller.ZoomParam;
        _proxyVfx.SetVector4("Line Color", _proxyColor * Mathf.Clamp01(zoom * 3));
        _afterimageVfx.SetBool("Spawn", zoom > 0.1f);
    }

    #endregion
}
