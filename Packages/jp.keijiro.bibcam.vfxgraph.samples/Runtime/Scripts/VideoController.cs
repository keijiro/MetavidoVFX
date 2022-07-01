using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace Bibcam {

public sealed class VideoController : MonoBehaviour
{
    [SerializeField] Slider _slider = null;

    VideoPlayer _player;

    float PlayerNormalizedTime
      => _player.length > 0 ? (float)(_player.time / _player.length) : 0;

    public void OnSliderTouch(float x)
      => _player.time = _player.length * x;

    void Start()
      => _player = GetComponent<VideoPlayer>();

    void Update()
      => _slider.SetValueWithoutNotify(PlayerNormalizedTime);
}

} // namespace Bibcam
