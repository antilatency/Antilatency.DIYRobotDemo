// Copyright (c) 2020 ALT LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of source code located below and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//  
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Antilatency.DeviceNetwork;
using Antilatency.HardwareExtensionInterface;
using Antilatency.HardwareExtensionInterface.Interop;
using Antilatency.Integration;
using UnityEngine.UI;

// AHEI Library implemented to control tank.
public sealed class TankExtensionBoard : MonoBehaviour
{
    
    public DeviceNetwork Network;

    public string Tag;

    private Antilatency.HardwareExtensionInterface.ILibrary _aheiLibrary;

    private NodeHandle _aheiNode;

    private Antilatency.HardwareExtensionInterface.ICotask _aheiCotask;
    
    private Queue<float> _voltageBuffer;

    private void Awake()
    {
        Init();
        
        
        //Creating float buffer for calculating average voltage on tank battery
        _voltageBuffer = new Queue<float>(64);
        for (int i = 0; i < 64; i++)
            _voltageBuffer.Enqueue(11.8f);
    }

    private void Init()
    {
        if (Network == null)
        {
            Debug.LogError("Network is null");
            return;
        }

        _aheiLibrary = Antilatency.HardwareExtensionInterface.Library.load();

        if (_aheiLibrary == null)
        {
            Debug.LogError("Failed to create hardware extension interface library");
            return;
        }
    }

    private void OnEnable()
    {
        if (Network == null)
        {
            return;
        }

        Network.DeviceNetworkChanged.AddListener(OnDeviceNetworkChanged);

        OnDeviceNetworkChanged();
    }

    private void OnDisable()
    {
        if (Network == null)
        {
            return;
        }

        Network.DeviceNetworkChanged.RemoveListener(OnDeviceNetworkChanged);

        StopAhei();
    }

    private void OnDeviceNetworkChanged()
    {
        if (_aheiCotask != null)
        {
            if (_aheiCotask.isTaskFinished())
            {
                StopAhei();
            }
            else
            {
                return;
            }
        }

        if (_aheiCotask != null) return;
        var node = GetAvailableAheiNode();
        if (node != NodeHandle.Null)
        {
            StartAhei(node);
        }
    }

    private void Update()
    {
        if (_aheiCotask != null && _aheiCotask.isTaskFinished())
        {
            StopAhei();
        }
    }

    // Running ahei examples
    private void RunAhei()
    {
        
        //Initializing extension board connected devices
        
        _mLeft = new Motor(_aheiCotask, _enaPin, _in1Pin, _in2Pin, TankActionController.frequency);
        _mRight = new Motor(_aheiCotask, _enbPin, _in3Pin, _in4Pin, TankActionController.frequency);
        _fanControl = _aheiCotask.createOutputPin(_fanPin, PinState.Low);
        _mBatterySense = new BatterySense(_aheiCotask, _batterySensePin);
        _aheiCotask.run();

    }

    public void RecreateMotors()
    {
        
        //"Rebooting" extension board
        
        var tmp = _aheiNode;
        StopAhei();
        StartAhei(tmp);
    }

    private Motor _mLeft;
    private IOutputPin _fanControl;
    private Motor _mRight;
    private BatterySense _mBatterySense;

    private float getBatterySense() =>
        _mBatterySense.getVoltage();
    


    //Disposing cotasks
    private void OnDestroy()
    {
        StopAllCoroutines();
        
        if (_aheiCotask != null)
        {
            _aheiCotask.Dispose();
            _aheiCotask = null;
        }

        if (_aheiLibrary != null)
        {
            _aheiLibrary.Dispose();
            _aheiLibrary = null;
   
        }

    }

    private INetwork GetNativeNetwork()
    {
        if (Network == null)
        {
            Debug.LogError("Network is null");
            return null;
        }

        if (Network.NativeNetwork == null)
        {
            Debug.LogError("Native network is null");
            return null;
        }

        return Network.NativeNetwork;
    }

    private void StartAhei(NodeHandle node)
    {
        var network = GetNativeNetwork();
        if (network == null)
        {
            return;
        }

        if (network.nodeGetStatus(node) != NodeStatus.Idle)
        {
            Debug.LogError("Wrong node status");
            return;
        }

        using (var cotaskConstructor = _aheiLibrary.getCotaskConstructor())
        {
            _aheiCotask = cotaskConstructor.startTask(network, node);

            if (_aheiCotask == null)
            {
                StopAhei();
                Debug.LogWarning("Failed to start hardware extension interface task on node " + node.value);
                return;
            }

            _aheiNode = node;

            RunAhei();
        }
    }


    //Extension board connected devices pins

   //Motor 1
   private readonly Pins _enaPin = Pins.IO8;
   private readonly Pins _in1Pin = Pins.IO1;
   private readonly Pins _in2Pin = Pins.IO2;
   
   //Motor 2
   private readonly Pins _enbPin = Pins.IO5;
   private readonly Pins _in3Pin = Pins.IO7;
   private readonly Pins _in4Pin = Pins.IO6;
   
   //Battery

   private readonly Pins _batterySensePin = Pins.IOA3;
   
   //Fan

   private readonly Pins _fanPin = Pins.IOA4;
   

    private Antilatency.Alt.Tracking.Stability Stability => GetComponent<TankTracking>().stability;

    private TankActionController TankActionController => GetComponent<TankActionController>();
    

    public Text voltageText;
    private void DefaultControl()
    {
        if (_mBatterySense != null)
        {
            var voltage = _mBatterySense.getVoltage();
            Debug.Log($"Battery {voltage} V");
            voltageText.text = $"Battery voltage : {voltage.ToString(CultureInfo.InvariantCulture)}V";
        }
        if (_mLeft != null && _mRight != null && !_aheiCotask.isTaskFinished())
        {
            var motor1Speed = 0f;
            var motor2Speed = 0f;

            var motor1Mode = Motor.Mode.Forward;
            var motor2Mode = Motor.Mode.Forward;


            if (TankActionController.isMoving &&
                TankActionController.tankTaskState == TankActionController.TankTaskState.TurningRight)
            {
                motor1Speed = .8f;

                motor2Speed = .8f;

                motor2Mode = Motor.Mode.Reverse;
            }
            
            if (TankActionController.isMoving &&
                TankActionController.tankTaskState == TankActionController.TankTaskState.TurningLeft)
            {
                motor1Speed = .8f;

                motor2Speed = .8f;

                motor1Mode = Motor.Mode.Reverse;
            }
            
            
            if (TankActionController.isMoving &&
                TankActionController.tankTaskState == TankActionController.TankTaskState.ForwardMove)
            {
                motor1Speed = .95f * TankActionController.velocity;

                motor2Speed = .95f * TankActionController.velocity;
                
                motor1Mode = Motor.Mode.Forward;
                
                motor2Mode = Motor.Mode.Forward;
            }
            
            
            

            if (TankActionController.isMoving &&
                TankActionController.tankTaskState == TankActionController.TankTaskState.ReverseMove)
            {
                motor1Speed = -.8f * TankActionController.velocity;

                motor2Speed = -.8f * TankActionController.velocity;
                
                motor1Mode = Motor.Mode.Reverse;
                
                motor2Mode = Motor.Mode.Reverse;
            }

            if (TankActionController.isMoving && (TankActionController.tankTaskState ==
                TankActionController.TankTaskState.MovingToCup || TankActionController.tankTaskState ==
                TankActionController.TankTaskState.MovingToPlaceCup))
            {
                var speeds = CustomMaths.GetEfficientSpeeds(transform, TankActionController.targetTransform);
                motor1Speed = speeds.LeftSpeed * TankActionController.velocity;
                motor2Speed = speeds.RightSpeed * TankActionController.velocity;

                motor1Mode = motor1Speed > 0 ? Motor.Mode.Forward : Motor.Mode.Reverse;
                motor2Mode = motor2Speed > 0 ? Motor.Mode.Forward : Motor.Mode.Reverse;
                motor1Speed = Math.Abs(motor1Speed);
                motor2Speed = Math.Abs(motor2Speed);

            }
            if (!TankActionController.isMoving || Stability.stage != Antilatency.Alt.Tracking.Stage.Tracking6Dof)
            {
                motor1Speed = 0;
                motor2Speed = 0;
            }


            _mLeft.setMode(motor1Mode);
            _mRight.setMode(motor2Mode);
            
            //Left border = 10.5, Right border = 12.6f;
            _mLeft.setSpeed(Mathf.Abs(motor1Speed) * (0.9f + 0.1f * Mathf.Clamp((12.6f - _voltageBuffer.Average()) / (12.6f - 10.5f), 0, 1)));
            _mRight.setSpeed(Mathf.Abs(motor2Speed) * (0.9f + 0.1f * Mathf.Clamp((12.6f - _voltageBuffer.Average()) / (12.6f - 10.5f), 0, 1)));
        }
    }
    

    private void UpdateBuffer(float value)
    {
        _voltageBuffer.Dequeue();
        _voltageBuffer.Enqueue(value);
    }
    
    public void SetFanRotation(bool isRotating) =>
        _fanControl.setState(isRotating ? PinState.High : PinState.Low);

    //Keyboard control type
    private void KeyboardControl()
    {
        if (_mBatterySense != null)
        {
            Debug.Log($"Battery {_mBatterySense.getVoltage()} V");
        }

        _fanControl?.setState(Input.GetKey(KeyCode.Q) ? PinState.High : PinState.Low);

        if (_mLeft == null || _mRight == null || _aheiCotask.isTaskFinished()) return;
        float motor1Speed;
        float motor2Speed;


        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            motor1Speed = 1f;
            motor2Speed = 1f;
        }

        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.D))
        {
            motor1Speed = -1f;
            motor2Speed = -1f;
        }

        else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            motor1Speed = -1f;
            motor2Speed = 1f;
        }

        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            motor1Speed = 1f;
            motor2Speed = -1f;
        }

        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S))
        {
            motor1Speed = .8f;
            motor2Speed = 1f;
        }

        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S))
        {
            motor1Speed = 1f;
            motor2Speed = .8f;
        }

        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W))
        {
            motor1Speed = -.8f;
            motor2Speed = -1f;
        }

        else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A))
        {
            motor1Speed = -1f;
            motor2Speed = -.8f;
        }
        else
        {
            motor1Speed = 0;
            motor2Speed = 0;
        }

        var motor1Mode = motor1Speed > 0 ? Motor.Mode.Forward : Motor.Mode.Reverse;
        var motor2Mode = motor2Speed > 0 ? Motor.Mode.Forward : Motor.Mode.Reverse;
        motor1Speed = Math.Abs(motor1Speed) * TankActionController.velocity;
        motor2Speed = Math.Abs(motor2Speed) * TankActionController.velocity;

        _mLeft.setMode(motor1Mode);
        _mRight.setMode(motor2Mode);
        _mLeft.setSpeed(Mathf.Abs(motor1Speed));
        _mRight.setSpeed(Mathf.Abs(motor2Speed));
    }


    private void FixedUpdate()
    {
        if (TankActionController.controlType == TankActionController.ControlType.Keyboard)
            KeyboardControl();
        if (TankActionController.controlType == TankActionController.ControlType.Tracking)
            DefaultControl();
        
        UpdateBuffer(getBatterySense());
    }


    private void StopAhei() {
        StopAllCoroutines();
        Antilatency.Utils.SafeDispose(ref _mLeft);
        Antilatency.Utils.SafeDispose(ref _mRight);
        Antilatency.Utils.SafeDispose(ref _fanControl);
        Antilatency.Utils.SafeDispose(ref _mBatterySense);
        Antilatency.Utils.SafeDispose(ref _aheiCotask);
        _aheiNode = new NodeHandle();
    }

    private NodeHandle GetAvailableAheiNode() {
        return GetFirstIdleTagAheiNode();
    }

    private NodeHandle[] GetIdleAheiNodes() {
        var nativeNetwork = GetNativeNetwork();

        if (nativeNetwork == null) {
            return new NodeHandle[0];
        }

        using (var cotaskConstructor = _aheiLibrary.getCotaskConstructor()) {
            var nodes = cotaskConstructor.findSupportedNodes(nativeNetwork).Where(v =>
                    nativeNetwork.nodeGetStatus(v) == NodeStatus.Idle
                ).ToArray();

            return nodes;
        }
    }

    private NodeHandle GetFirstIdleTagAheiNode() {
        var nativeNetwork = GetNativeNetwork();

        if (nativeNetwork == null) {
            return new NodeHandle();
        }

        var nodes = GetIdleAheiNodes();
        if (nodes.Length == 0) {
            return new NodeHandle();
        }

        var node = nodes.FirstOrDefault(h => nativeNetwork.nodeGetStringProperty(h, "Tag") == Tag);
        return node;
    }
}


public class BatterySense : IDisposable
{
    public BatterySense(Antilatency.HardwareExtensionInterface.ICotask cotask, Pins sensePin)
    {
        this._sensePin = cotask.createAnalogPin(sensePin, 10);
    }

    public void Dispose()=>
        _sensePin.Dispose();
    

    public float getVoltage()
    {
        const float rtop = 4700f;
        const float rbottom = 330f;
        
        return _sensePin.getValue() * (rtop + rbottom) / rbottom;
    }

    readonly IAnalogPin _sensePin;
}


public class Motor : IDisposable
{

    public enum Mode
    {
        Forward,
        Reverse,
        None
    }

    public Motor(Antilatency.HardwareExtensionInterface.ICotask cotask, Pins en, Pins in1, Pins in2, uint frequency)
    {
        this._en = cotask.createPwmPin(en, frequency, 0f);
        this._in1 = cotask.createOutputPin(in1, PinState.Low);
        this._in2 = cotask.createOutputPin(in2, PinState.Low);
    }

    public void Dispose()
    {
        _in1.Dispose();
        _in2.Dispose();
        _en.Dispose();
    }
    
    public void setSpeed(float value)=>
        _en.setDuty(value);
    

    public void setMode(Motor.Mode mode)
    {
        switch (mode)
        {
            case Motor.Mode.Reverse:
                _in1.setState(PinState.High);
                _in2.setState(PinState.Low);
                break;
            case Motor.Mode.Forward:
                _in1.setState(PinState.Low);
                _in2.setState(PinState.High);
                break;
            case Motor.Mode.None:
                _in1.setState(PinState.Low);
                _in2.setState(PinState.Low);
                break;
        }
    }

    readonly IOutputPin _in1;
    readonly IOutputPin _in2;
    private readonly IPwmPin _en;
}