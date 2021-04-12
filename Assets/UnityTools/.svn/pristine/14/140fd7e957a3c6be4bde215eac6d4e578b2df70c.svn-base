using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlotChannelBlockView : PlotChannelView {
    public PlotColorChannel ColorChannel;

    public Material LineMaterial;
    public int LinesPerBlock = 100;
    public bool ShowPartialBlock = true;
    private int _globalBlocksCounter = 0;
    private int _blocksCount = 0;

    private List<GameObject> _fullBlocks = new List<GameObject>();

    private List<Vector3> _currentVertices = new List<Vector3>();
    private List<Color> _currentColors = new List<Color>();

    private int _fullWriteId = 0;


   
    private GameObject _partialBlock;
    private GameObject _blockScaleContainer;
    private GameObject _blockOffsetContainer;


    protected void ResetGeometry() {
        foreach(var b in _fullBlocks) {
            var m = b.GetComponent<MeshFilter>().sharedMesh;
            m.Clear();
        }

        _partialBlock.GetComponent<MeshFilter>().sharedMesh.Clear();

        _currentVertices.Clear();
        _currentColors.Clear();
        _fullWriteId = 0;
    }

    protected override void Reset() {
        base.Reset();
        PlotUtils.AutoAssignColorChannel(this, ref ColorChannel);
        LineMaterial = PlotUtils.FindPlotChannelLineMaterial();

    }

    protected virtual void Start() {
        _blockScaleContainer = PlotUtils.CreateGameObject(gameObject, "BlockScaleContainer");
        _blockOffsetContainer = PlotUtils.CreateGameObject(_blockScaleContainer, "BlockOffsetContainer");

        _partialBlock = CreateMeshBlock("PartialBlock");

        if (Channel != null) {
            Channel.Reset += ResetGeometry;
        }
    }

    protected virtual void Update() {

        int blocksCount = (Channel.Samples.Count + (LinesPerBlock - 1)) / LinesPerBlock;
        if (_blocksCount < blocksCount) {
            while (_blocksCount < blocksCount) {
                AddFullMeshBlock();
            }
        }

    }

    protected void AddLine(Vector3 p0, Color c0, Vector3 p1, Color c1) {
        _currentVertices.Add(p0);
        _currentVertices.Add(p1);
        _currentColors.Add(c0);
        _currentColors.Add(c1);
        if (_currentVertices.Count >= LinesPerBlock * 2) {
            WriteCurrentFullBlock();
        }

        if (ShowPartialBlock) {
            ApplyVerticesToBlock(_partialBlock);
        }

        
        //TODO: move code from this place
        Vector3 dataBoundsSize = DataBounds.Max - DataBounds.Min;
        Vector3 dataBoundsCenter = (DataBounds.Max + DataBounds.Min) / 2.0f;


        _blockScaleContainer.transform.localScale = dataBoundsSize.RcpSafe().Multiply(Plot.PlotSize);
        _blockOffsetContainer.transform.localPosition = -dataBoundsCenter;

        if (Offset == OffsetMode.None) {
            _blockScaleContainer.transform.localPosition = Vector3.zero;
        } else if (Offset == OffsetMode.ToRight) {
            _blockScaleContainer.transform.localPosition = new Vector3(Plot.PlotSize.x / 2, 0, 0);

            var v = _blockOffsetContainer.transform.localPosition;
            v.x = -p1.x;
            _blockOffsetContainer.transform.localPosition = v;
        }
    }

    GameObject CreateMeshBlock(string name) {
        var meshBlock = PlotUtils.CreateGameObject(_blockOffsetContainer, name);
        var mr = meshBlock.AddComponent<MeshRenderer>();

        MaterialPropertyBlock pb = new MaterialPropertyBlock();
        pb.SetColor("LineColor", Color.black);

        mr.sharedMaterials = new Material[] { LineMaterial };
        var mf = meshBlock.AddComponent<MeshFilter>();
        var mesh = new Mesh();
        mesh.vertices = new Vector3[LinesPerBlock * 2];
        mesh.SetIndices(Enumerable.Range(0, LinesPerBlock * 2).ToArray(), MeshTopology.Lines, 0, true);
        mf.sharedMesh = mesh;
        return meshBlock;
    }

    private void AddFullMeshBlock() {
        var meshBlock = CreateMeshBlock("GeometryBlock_" + _globalBlocksCounter.ToString());
        _fullBlocks.Add(meshBlock);
        _blocksCount++;
        _globalBlocksCounter++;
    }

    private void WriteCurrentFullBlock() {
        ApplyVerticesToBlock(_fullBlocks[_fullWriteId]);
        _currentVertices.Clear();
        _currentColors.Clear();

        _fullWriteId = (_fullWriteId + 1) % _fullBlocks.Count;
    }

    private void ApplyVerticesToBlock(GameObject block) {
        var mf = block.GetComponent<MeshFilter>();
        var m = mf.sharedMesh;

        block.SetActive(_currentVertices.Count != 0);

        if (_currentVertices.Count != 0) {
            bool rebuild = m.vertexCount != _currentVertices.Count;
            if (rebuild) {
                m.Clear();
            }

            m.vertices = _currentVertices.ToArray();

            if (rebuild) {

                m.SetIndices(Enumerable.Range(0, _currentVertices.Count).ToArray(), MeshTopology.Lines, 0);
            }
            m.colors = _currentColors.ToArray();
            m.RecalculateBounds();
        }
    }

}
