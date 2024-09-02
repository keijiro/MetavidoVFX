using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UIElements;
using Klak.Motion;
using Unity.Mathematics;
using Unity.Properties;
using System;

public sealed class Switcher : MonoBehaviour
{
    #region Scene object references

    [Space]
    [SerializeField] SmoothFollow _follower = null;
    [SerializeField] BrownianMotion _swing = null;
    [SerializeField] VisualEffect[] _vfxList = null;
    [SerializeField] VisualEffect _proxyVfx = null;
    [SerializeField] VisualEffect _afterimageVfx = null;

    #endregion

    #region Per-state configuration

    [Serializable]
    public struct Config
    {
        public float followerSpeed;
        public float3 positionSwing;
        public float3 rotationSwing;
        public float cameraDistance;
        public float fieldOfView;
        public Color vfxColor;
    }

    [Space, SerializeField] Config _config1st;
    [Space, SerializeField] Config _config3rd;
    [Space, SerializeField] float _switchSpeed = 1;

    #endregion

    #region Dynamic property

    [field:Space][field:SerializeField]
    [CreateProperty] public bool ThirdPerson { get; set; }

    [field:SerializeField]
    [CreateProperty] public int VfxSelect { get; set; }

    #endregion

    #region Private members

    float _blend = 1;

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => GetComponent<UIDocument>().rootVisualElement.
           Q("info-box").dataSource = this;

    void Update()
    {
        var delta = Time.deltaTime * _switchSpeed * (ThirdPerson ? 1 : -1);
        _blend = math.saturate(_blend + delta);

        var p = math.smoothstep(0, 1, _blend);
        ref var c1 = ref _config1st;
        ref var c3 = ref _config3rd;

        _follower.positionSpeed = math.lerp(c1.followerSpeed, c3.followerSpeed, p);
        _follower.rotationSpeed = _follower.positionSpeed;

        _swing.positionAmount = math.lerp(c1.positionSwing, c3.positionSwing, p);
        _swing.rotationAmount = math.lerp(c1.rotationSwing, c3.rotationSwing, p);

        var dist = math.lerp(c1.cameraDistance, c3.cameraDistance, p);
        Camera.main.transform.localPosition = -Vector3.forward * dist;
        Camera.main.fieldOfView = math.lerp(c1.fieldOfView, c3.fieldOfView, p);

        var vfxColor = Color.Lerp(c1.vfxColor, c3.vfxColor, p);
        _proxyVfx.SetVector4("Line Color", vfxColor);
        _afterimageVfx.SetBool("Spawn", ThirdPerson);

        for (var i = 0; i < _vfxList.Length; i++)
            _vfxList[i].SetBool("Spawn", VfxSelect == i);
    }

    #endregion
}
