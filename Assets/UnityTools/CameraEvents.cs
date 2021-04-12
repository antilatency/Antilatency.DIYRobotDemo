using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CameraEvents : MonoBehaviour {
    public UnityEvent PreRender;
    public UnityEvent PostRender;

    private void Start() {
        if (PreRender == null) {
            PreRender = new UnityEvent();
        }
        if (PostRender == null) {
            PostRender = new UnityEvent();
        }
    }

    private void OnPreRender() {
        if (PreRender != null) {
            PreRender.Invoke();
        }
    }

    private void OnPostRender() {
        if (PostRender != null) {
            PostRender.Invoke();
        }
    }
}