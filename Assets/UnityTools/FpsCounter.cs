using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCounter {
    int _sampleCount = 0;
    float _lastNotifyTime = Time.realtimeSinceStartup;
    float _notifyInterval;
    System.Action<float> _notifyCallback;

    public FpsCounter(float notifyInterval, System.Action<float> notifyCallback) {
        _notifyInterval = notifyInterval;
        _notifyCallback = notifyCallback;
    }

    public void AddSample() {
        ++_sampleCount;
        float nowTime = Time.realtimeSinceStartup;
        float dt = nowTime - _lastNotifyTime;
        if (dt >= _notifyInterval) {
            _notifyCallback(_sampleCount / dt);
            _lastNotifyTime = nowTime;
            _sampleCount = 0;
        }
    }
}
