using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OVRInputListener : MonoBehaviour {

    public float DoubleClickTime = 0.25f;

    public UnityEvent OnBackPressed = new UnityEvent();
    public UnityEvent OnBackDoublePressed = new UnityEvent();

    public UnityEvent OnSwipeRight = new UnityEvent();
    public UnityEvent OnSwipeLeft = new UnityEvent();
    public UnityEvent OnSwipeUp = new UnityEvent();
    public UnityEvent OnSwipeDown = new UnityEvent();
#if ALT_OVR
    private IEnumerator _coroutine;
#endif
	private void Update () {
#if ALT_OVR
        if (OVRInput.GetUp(OVRInput.Button.Two) || Input.GetKeyUp(KeyCode.A)) {
            Debug.Log("Press!");
	        if (_coroutine == null) {
	            _coroutine = DoubleClick();
	            StartCoroutine(_coroutine);
	        } else {
	            StopCoroutine(_coroutine);
	            _coroutine = null;
                OnBackDoublePressed.Invoke();

                Debug.Log("Double click");
	        }
	    }
	    if (OVRInput.GetUp(OVRInput.RawButton.DpadRight)) {
	        OnSwipeRight.Invoke();
        }
	    if (OVRInput.GetUp(OVRInput.RawButton.DpadLeft)) {
	        OnSwipeLeft.Invoke();
	    }
	    if (OVRInput.GetUp(OVRInput.RawButton.DpadUp)) {
	        OnSwipeUp.Invoke();
	    }
	    if (OVRInput.GetUp(OVRInput.RawButton.DpadDown)) {
	        OnSwipeDown.Invoke();
	    }
#endif
    }

    private IEnumerator DoubleClick() {
        yield return new WaitForSeconds(DoubleClickTime);
#if ALT_OVR
        _coroutine = null;
#endif
        OnBackPressed.Invoke();
        Debug.Log("Single click");
    }
}
