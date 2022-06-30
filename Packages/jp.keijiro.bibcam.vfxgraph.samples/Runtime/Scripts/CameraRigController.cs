using UnityEngine;
using BrownianMotion = Klak.Motion.BrownianMotion;
using SmoothFollow = Klak.Motion.SmoothFollow;
using VisualEffect = UnityEngine.VFX.VisualEffect;

namespace Bibcam {

public sealed class CameraRigController : MonoBehaviour
{
    #region Scene object references

    [SerializeField] Transform _mainCamera = null;
    [SerializeField] Transform _viewTarget = null;
    [SerializeField] BrownianMotion _floatingMotion = null;
    [SerializeField] SmoothFollow _targetFollow = null;
    [SerializeField] VisualEffect _cameraVfx = null;

    #endregion

    #region Original values

    float _cameraDistance;
    float _targetOffset;
    float _positionNoise;
    float _rotationNoise;
    float _followSpeed;
    Color _vfxColor;

    #endregion

    #region Interpolation parameter

    ((float x, float v) state, float target) _param = ((1, 0), 1);

    #endregion

    #region Public methods (switcher methods)

    public void SwitchToLongRangeView()   => _param.target = 1;
    public void SwitchToCloseRangeView()  => _param.target = 0.4f;
    public void SwitchToFirstPersonView() => _param.target = 0;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _cameraDistance = _mainCamera.localPosition.z;
        _targetOffset = _viewTarget.localPosition.z;
        _positionNoise = _floatingMotion.positionAmount.x;
        _rotationNoise = _floatingMotion.rotationAmount.x;
        _followSpeed = _targetFollow.positionSpeed;
        _vfxColor = _cameraVfx.GetVector4("Line Color");
    }

    void Update()
    {
        _param.state = CdsTween.Step(_param.state, _param.target, 4);

        var p = _param.state.x;
        _mainCamera.localPosition = Vector3.forward * _cameraDistance * p;
        _viewTarget.localPosition = Vector3.forward * _targetOffset * p;
        _floatingMotion.positionAmount = Vector3.one * _positionNoise * p;
        _floatingMotion.rotationAmount = Vector3.one * _rotationNoise * p;
        _targetFollow.positionSpeed = _followSpeed * (8 - p * 7);
        _targetFollow.rotationSpeed = _followSpeed * (8 - p * 7);
        _cameraVfx.SetVector4("Line Color", _vfxColor * Mathf.Clamp01(p * 4));
    }

    #endregion
}

} // namespace Bibcam
