using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class PlotChannelLineView : PlotChannelBlockView {
    
    public enum ViewMode {
        LineStrip,
        LineList
    }

  
    private int _lineId = 0;
    private int _lastUpdateId = 0;
    private bool _firstVertex = true;
    private Vector3 _lastVertex;

    public ViewMode Mode;

    protected override void Start() {
        base.Start();
        if (Channel != null) {
            Channel.Reset += Channel_Reset;
        }
    }

    private void Channel_Reset() {
        _firstVertex = true;
        _lineId = 0;
        _lastUpdateId = 0;

    }

    protected override void Update() {
        base.Update();
      

        if(Mode == ViewMode.LineList) {
            if (_lastUpdateId != Channel.UpdateId) {
                _lastUpdateId = Channel.UpdateId;

                ResetGeometry();
                for (int i = 0; i < Channel.Samples.Count - 1; i += 2) {
                    var color = ColorChannel.GetSample(_lineId++);
                    AddLine(Channel.Samples.Read(i), color, Channel.Samples.Read(i+1), color);
                }
            }
           
        } else {
            int newSamples = Channel.UpdateId - _lastUpdateId;
            newSamples = newSamples > Channel.Samples.Count ? Channel.Samples.Count : newSamples;
            _lastUpdateId = Channel.UpdateId;

            int start = Channel.Samples.Count - newSamples;


            //emit lines from new samples
            for (int i = 0; i < newSamples; ++i) {
                var sample = Channel.Samples.Read(i + start);
                var color = ColorChannel.GetSample(_lineId++);
                if (!_firstVertex) {
                    AddLine(_lastVertex, color, sample, color);
                } else {
                    _firstVertex = false;
                }

                _lastVertex = sample;
            }
        }
    }
}
