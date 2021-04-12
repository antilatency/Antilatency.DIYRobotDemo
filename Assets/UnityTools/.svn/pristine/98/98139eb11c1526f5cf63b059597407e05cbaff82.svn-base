using System;
using System.Collections;
using System.Collections.Generic;

public class CircularBuffer<T> : IEnumerable<T> {
    public int WritePosition;

    private T[] _buffer = null;
    private int _size;

    public int Count {
        get { return _size; }
    }

    public CircularBuffer() {

    }

    public int Capacity {
        get { return _buffer == null ? 0 : _buffer.Length; }
    }

    public CircularBuffer(int size) {
        Resize(size);
    }

    public bool IsEmpty() {
        return _size == 0;
    }

    public bool IsFull() {
        return _size == _buffer.Length;
    }

    public void Clear() {
        if (_size != 0 && typeof(T).IsByRef) {
            for(int i = 0; i < _size; ++i) {
                _buffer[i] = default;
            }
        }
        _size = 0;
    }
    public void Resize(int newCapacity) {

        if (_buffer == null || _buffer.Length != newCapacity) {
            var oldSamples = this.ToArray();

            WritePosition = 0;
            _size = 0;

            Array.Resize(ref _buffer, newCapacity);
            if (newCapacity != 0) {
                for (int i = 0; i < oldSamples.Length; ++i) {
                    Write(oldSamples[i]);
                }
            }
        }
    }

    public T Read(int position) {
        var readDistance = WritePosition + (_buffer.Length - _size) + position;
        return _buffer[readDistance % _buffer.Length];
    }
    public T[] ToArray() {
        T[] result = new T[Count];
        for (int i = 0; i < Count; ++i) {
            result[i] = Read(i);
        }
        return result;
    }

    public void Write(T value) {
        if (_buffer == null) {
            Resize(1);
        }
        _buffer[WritePosition] = value;
        WritePosition = (WritePosition + 1) % _buffer.Length;
        _size++;
        if (_size > _buffer.Length) {
            _size = _buffer.Length;
        }
    }

    public IEnumerator<T> GetEnumerator() {
        for (int i = 0; i < Count; ++i) {
            yield return Read(i);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}