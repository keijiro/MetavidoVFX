using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;

public sealed class CameraController : MonoBehaviour
{
    #region Scene object references

    [SerializeField] UIDocument _ui = null;
    [SerializeField] Camera _camera = null;
    [SerializeField] Transform _pivotNode = null;
    [SerializeField] Transform _distanceNode = null;

    #endregion

    #region Camera controlling parameters

    [field:SerializeField] public float AngleSpeed = 90;
    [field:SerializeField] public float PitchLimit = 60;
    [field:SerializeField] public float DistanceSpeed = 4;
    [field:SerializeField] public float2 DistanceLimit = math.float2(3, 6);
    [field:SerializeField] public float2 FovRange = math.float2(20, 45);

    #endregion

    #region TouchDragManipulator callbacks

    void OnDragging(float2 delta)
    {
        var r = (float3)_pivotNode.localEulerAngles;
        r.x = (r.x + 180) % 360 - 180; // (0, 360) => (-180, 180)
        r.xy += delta.yx * AngleSpeed;
        r.x = math.clamp(r.x, -PitchLimit, PitchLimit);
        _pivotNode.localEulerAngles = r;
    }

    void OnScrolling(float delta)
    {
        var dist = _distanceNode.localPosition.z;
        dist += DistanceSpeed * delta;
        dist = math.clamp(dist, -DistanceLimit.y, -DistanceLimit.x);
        _distanceNode.localPosition = new float3(0, 0, dist);
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        var drag = new TouchDragManipulator();
        drag.OnDragging += OnDragging;
        drag.OnScrolling += OnScrolling;

        var area = _ui.rootVisualElement.Q("drag-area");
        area.AddManipulator(drag);
    }

    void Update()
    {
        var dist = -_distanceNode.localPosition.z;
        var ndist = (dist - DistanceLimit.x) / (DistanceLimit.y - DistanceLimit.x);
        _camera.fieldOfView = math.lerp(FovRange.x, FovRange.y, ndist);
    }

    #endregion
}
