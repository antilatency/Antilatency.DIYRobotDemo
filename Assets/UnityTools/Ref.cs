using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ref<T> where T : struct {
    T _value;

    public Ref(T value) {
        _value = value;
    }

    public Ref() {
        Debug.Log("Ref crated");
        _value = default;
    }

    public T Value {
        get { return _value; }
        set { _value = value; }
    }

    public override string ToString() {
        return _value.ToString();
    }

    public static implicit operator T(Ref<T> wrapper) {
        return wrapper.Value;
    }

    public static implicit operator Ref<T>(T value) {
        return new Ref<T>(value);
    }
}