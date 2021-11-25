using UnityEngine;
using UnityEngine.Video;

//
// External time controller for VideoPlayer driven by GameTime
// This is needed to control the playback speed while recording.
//
sealed class VideoByGameTime : MonoBehaviour
{
    VideoPlayer _player;
    float _speed;
    double _time;
    int _frame;

    void Start()
    {
        _player = GetComponent<VideoPlayer>();
        _speed = _player.playbackSpeed;
        _player.playbackSpeed = 0;
    }

    void Update()
    {
        _time += Time.deltaTime * _speed;

        var fc = (int)(_time * _player.frameRate);
        if (fc > _frame)
        {
            _player.StepForward();
            _frame = fc;
        }
    }
}
