using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bounds : MonoBehaviour
{
    public Vector3 Min = Vector3.zero;
    public Vector3 Max = Vector3.one;

    public UnityEvent Validate = new UnityEvent();

    public void OnValidate()
    {
        Validate.Invoke();
    }
}
