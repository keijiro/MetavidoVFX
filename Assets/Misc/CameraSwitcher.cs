using UnityEngine;
using UnityEngine.UIElements;
using Klak.Motion;
using Unity.Mathematics;
using Unity.Properties;

public sealed class CameraSwitcher : MonoBehaviour
{
    [field:SerializeField] public Camera Proxy { get; set; }
    [field:SerializeField] public SmoothFollow Follower { get; set; }
    [field:SerializeField] public BrownianMotion Motor { get; set; }
    [field:SerializeField] public float SwitchSpeed { get; set; } = 1;

    [CreateProperty]
    [field:SerializeField] public bool ThirdPerson { get; set; }

    (float low, float high) _followerSpeeds;
    (float3 pos, float3 rot, float fov) _original;
    float _blend = 1;

    void Start()
    {
        _followerSpeeds = (20, Follower.positionSpeed);
        _original.pos = Motor.positionAmount;
        _original.rot = Motor.rotationAmount;
        _original.fov = Camera.main.fieldOfView;

        GetComponent<UIDocument>().rootVisualElement.
          Q<Toggle>("camera-toggle").dataSource = this;
    }

    void Update()
    {
        var delta = Time.deltaTime * SwitchSpeed;
        _blend = math.saturate(_blend + delta * (ThirdPerson ? 1 : -1));

        var p = math.smoothstep(0, 1, _blend);

        Follower.positionSpeed = Follower.rotationSpeed =
          math.lerp(_followerSpeeds.low, _followerSpeeds.high, p);

        Motor.positionAmount = _original.pos * p;
        Motor.rotationAmount = _original.rot * p;

        Camera.main.transform.localPosition =
          Vector3.forward * -math.lerp(2, 2.4f, p);
        Camera.main.fieldOfView =
          math.lerp(Proxy.fieldOfView, _original.fov, p);
    }
}
