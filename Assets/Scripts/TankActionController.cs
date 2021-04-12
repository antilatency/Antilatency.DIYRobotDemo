using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class TankActionController : MonoBehaviour
{

    public ControlType controlType = ControlType.Tracking;
    public DrawingType drawingType = DrawingType.Manual;

    public GameObject GeneratedCups;
    public GameObject ManualCups;
    public Image progressBar;
    

    public int firstIndex = 0;
    
    public string backupFilePath = "currentState";
    public bool isMoving;
    public TankTaskState tankTaskState = TankTaskState.Stop;
    public TankSpeedConfiguration tankSpeedConfiguration = TankSpeedConfiguration.Default;
    
    public Transform targetTransform;
    public Transform tracksCenter;

    public uint frequency = 10000;


    [Range(-1f, 1f)]
    public float velocity = 0f;

    private List<Transform> _sortedPositions;

    public UnityEvent onCupReleaseEvent;
    public UnityEvent onCupTakenEvent;

    private CustomLogger _logger;


    public enum DrawingType
    {
        Manual,
        Generated
    }
    
    public void SetTankSpeedConfiguration(TankSpeedConfiguration tankSpeedConfiguration)
    {
        this.tankSpeedConfiguration = tankSpeedConfiguration;
        frequency = (uint)(tankSpeedConfiguration == TankSpeedConfiguration.Default || tankSpeedConfiguration == TankSpeedConfiguration.Medium ? 10000 : 20);
        velocity = (tankSpeedConfiguration == TankSpeedConfiguration.Default ? .89f : tankSpeedConfiguration == TankSpeedConfiguration.Medium ? .815f :  .128f);
        //todo check for uncleared cotask
        GetComponent<TankExtensionBoard>().RecreateMotors();
    }

    public Transform manipulatorTransform;
    public bool rotateFan;


    private float _time = 0f;



    private void Awake()
    {
        _logger = GetComponent<CustomLogger>();
    }

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.W))
        {
            GetComponent<TankTracking>().enabled = true;
        }
        
        _time += Time.deltaTime;
        if (controlType == ControlType.Keyboard)
        {


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                pin1.gameObject.SetActive(true);
            }

            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                pin1.gameObject.SetActive(false);
            }



            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                pin2.gameObject.SetActive(true);
            }

            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                pin2.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {

                SetTankSpeedConfiguration(TankSpeedConfiguration.Slow);
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                SetTankSpeedConfiguration(TankSpeedConfiguration.Default);
            }
        }
        else GeneratedCups.GetComponent<MeshRenderer>().enabled = true;
    }

    private Transform _targetTestPoint;

    public enum ControlType
    {
        Keyboard,
        Tracking
    }

    private void WriteCurrentState(string stateInfo)
    {
        var path = $"{Application.persistentDataPath}\\{backupFilePath}.txt";
        Debug.Log(path);
        var fileStream = new FileStream(path, 
            FileMode.Create, 
            FileAccess.Write, 
            FileShare.None);
        var sw = new StreamWriter(fileStream);
        sw.Write(stateInfo);
        sw.Close();
    }

    private List<Vector3> GetSortedPositionsFromVectors(List<Vector3> reference) =>
        reference.OrderByDescending(x => Vector3.Distance(x, cupBaseTransform.position)).ToList();




    [FormerlySerializedAs("minDistanceBetweenCups")] [Header("Visualising Settings")]
    
    public float maxDistanceBetweenCups = 0.105f;

    public float cupMeshSizeMultiplier = 50f;

    public bool drawRoute = true;

    public bool displayBadRoutes = true;

    public Mesh cupMesh;

    public Material cupMaterial;

    private string _cupsObjectName ;

    public float multiplier = 1.1f;
    
    public void DrawCups()
    {
        _cupsObjectName = "GeneratedCups";
        var positions = GetSortedPositionsFromVectors(Deserializer.GetPositions($"{Application.dataPath}\\cups.txt", multiplier));
        var cupsGameObject = GameObject.Find(_cupsObjectName);
        if (cupsGameObject == null)
            cupsGameObject = new GameObject(_cupsObjectName, new[] {typeof(MeshFilter), typeof(MeshRenderer)});
        cupsGameObject.GetComponent<MeshFilter>().mesh = GetComposedCupsMesh(positions, cupMesh);
        cupsGameObject.GetComponent<MeshRenderer>().material = cupMaterial;
    }
    
    private Mesh GetComposedCupsMesh(List<Vector3> positions, Mesh cupMesh)
    {
        var composedMesh = new Mesh {indexFormat = IndexFormat.UInt32};

        var vertices = cupMesh.vertices;
        var triangles = cupMesh.triangles;

        Debug.Log(vertices.Length);
        Debug.Log(triangles.Length);
        Debug.Log("--------------------------------");
        
        var normals = cupMesh.normals;
        
        var newTriangles = new List<int>();
        var newVertices = new List<Vector3>();
        var newNormals = new List<Vector3>();
        var newColors = new List<Color>();
        var offset = cupMesh.vertices.Length;
        for (int i = 0; i < positions.Count; ++i)
        {
            
            var intersects = false;
            for (int k = i+1; k < positions.Count-1; ++k)
            {
                if (Vector3.Distance(positions[i], positions[k]) < maxDistanceBetweenCups)
                    intersects = true;
            }
            
            for (int j = 0; j < cupMesh.vertices.Length; ++j)
            {
                newVertices.Add(vertices[j] * cupMeshSizeMultiplier + positions[i]);
                newNormals.Add(normals[j] * cupMeshSizeMultiplier + positions[i]);
                newColors.Add(intersects ? Color.red : Color.white);
            }

            foreach (var t in triangles)
            {
                newTriangles.Add(t + i * offset);
            }
        }
        composedMesh.vertices = newVertices.ToArray();
        composedMesh.triangles = newTriangles.ToArray();
        Debug.Log(newVertices.Count);
        Debug.Log(newTriangles.Count);
        
        
        composedMesh.normals = newNormals.ToArray();
        composedMesh.colors = newColors.ToArray();
        return composedMesh;
    }

    private void OnDrawGizmos()
    {
        var fakeEpsilonPosition = fakeEpsilonTransform.position;
        Gizmos.DrawSphere(fakeEpsilonPosition, .1f);
        Gizmos.DrawLine(fakeEpsilonPosition, cupBaseEntryTransform.position);

        if (drawingType == DrawingType.Manual)
        {
            ManualCups.SetActive(true);
            GeneratedCups.SetActive(false);
            var position = transform.position;
            Vector3 toPosition = (cupBaseTransform.position - position).normalized;
            Handles.Label(position + new Vector3(0, .1f,0), CustomMaths.GetAngle(transform, toPosition).ToString(CultureInfo.InvariantCulture));
            var visPositions = targetPositions.OrderByDescending(
                x=> Vector3.Distance(x.position, cupBaseTransform.position)).ToList();
            for (int i = 0; i < visPositions.Count; i++)
            {
                var curColor = new Color(0, (float) i / visPositions.Count, 1f);
                var defaultStyle = new GUIStyle {normal = {textColor = curColor}};
                Handles.DrawDottedLine(cupBaseTransform.position, visPositions[i].position, 2f);
                Handles.color = curColor; 
                Handles.Label(visPositions[i].position + new Vector3(0, .3f, 0), i.ToString(), defaultStyle);
            }
            
        }

        else
        {
            ManualCups.SetActive(false);
            GeneratedCups.SetActive(true);
            var positions = GetSortedPositionsFromVectors(Deserializer.GetPositions($"{Application.dataPath}\\cups.txt", multiplier));
        var minX  = positions[0].x;
        var maxX  = positions[0].x;
        var minZ  = positions[0].z;
        var maxZ  = positions[0].z;
        foreach (var target in positions)
        {
            if (target.x < minX)
                minX = target.x;
            if (target.x > maxX)
                maxX = target.x;
            if (target.z < minZ)
                minZ = target.z;
            if (target.z > maxZ)
                maxZ = target.z;
        }
        Handles.color = Color.magenta;
        var position = cupBaseTransform.position;
        Handles.DrawSolidDisc(position, Vector3.up,  .05f);
        Handles.color = Color.yellow;;
        var position1 = cupBaseEntryTransform.position;
        Handles.DrawSolidDisc(position1, Vector3.up, 0.05f);
            
        Handles.color = Color.green;;
            
        Handles.DrawDottedLine(position1, position, 2f);
        Handles.color = Color.white;
        Handles.DrawLine(position + new Vector3(0,.3f, 0), position1 + new Vector3(0,.3f, 0));
        Handles.Label((position + position1)/2f + new Vector3(0,.35f, 0), Vector3.Distance(
            position, position1).ToString(CultureInfo.InvariantCulture), new GUIStyle{normal = {textColor = Color.white}});
        Handles.color = Color.yellow;;
        Handles.color = Color.yellow;;
        Handles.DrawLine(position1, position1 + Vector3.up * .5f);
        Handles.Label(position1 + Vector3.up * .54f, "Entry", new GUIStyle{normal = {textColor = Color.yellow}});
        Handles.color = Color.magenta;
        Handles.DrawLine(position, position + Vector3.up * .5f);
        Handles.Label(position + Vector3.up * .54f, "Base", new GUIStyle{normal = {textColor = Color.magenta}});
        if (drawRoute)
        {
            for (int i = 1; i < positions.Count; i++)
            {
                Gizmos.color = Color.white;
                var relation = maxDistanceBetweenCups / Vector3.Distance(positions[i], positions[i - 1]);
                if (displayBadRoutes)
                {
                    var style = new GUIStyle {normal = {textColor = Color.red}};
                    var distance = Vector3.Distance(positions[i], positions[i-1]);
                    if (distance <= maxDistanceBetweenCups)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(positions[i], positions[i] + Vector3.up * .3f);
                        Handles.Label(positions[i] + Vector3.up * .3f, $"Route distance: {distance}", style);
                        Handles.color = Color.red;
                    }
                        
                    Gizmos.color = new Color(relation, 1f - relation, 0, relation * 2f);
                }
                    
                Gizmos.DrawLine(positions[i], positions[i -1 ]);
            }
            Gizmos.color = Color.white;
            Gizmos.DrawLine(cupBaseTransform.position, positions[firstIndex]);
        }
        
        Handles.Label(new Vector3(minX - 2.7f, 0, (Math.Abs(maxZ) - Math.Abs((minZ)))/2f ), $"Height = {Math.Round( maxZ - minZ, 3)} m");
        
        Handles.Label(new Vector3((Math.Abs(maxX) - Math.Abs(minX)) / 2f, 0, minZ - .1f ), $"Width = {Math.Round( maxX - minX, 3)} m");
        
        Gizmos.DrawLine(new Vector3(minX, 0, minZ), new Vector3(maxX, 0, minZ));

        Gizmos.DrawLine(new Vector3(minX, 0, minZ), new Vector3(minX, 0, maxZ));
        }
    }


    public Text pin1;
    public Text pin2;


    private void Start()
    {
        _sortedPositions = new List<Transform>();
        if (_sortedPositions != null)
        {
            _sortedPositions = targetPositions.OrderByDescending(
                x=> Vector3.Distance(x.position, cupBaseTransform.position)).ToList();
        }
        
        _extensionBoard = GetComponent<TankExtensionBoard>();

            
        _finalPositions =  GetSortedPositionsFromVectors(Deserializer.GetPositions($"{Application.dataPath}\\cups.txt", multiplier));
        previousTargetTransform.position = _finalPositions[0];
        StartCoroutine(FinalDrawingSequence());
        
       
    }

    private List<Vector3> _finalPositions;

    private TankExtensionBoard _extensionBoard;



    public Transform fakeEpsilonTransform;


    public Text cupCounter;

    
    
    private IEnumerator FinalDrawingSequence()
    {
         while (true)
         {
             if (firstIndex == _finalPositions.Count)
             {
                 SetTankTaskState(TankTaskState.Stop);
                 
                 yield break;
             }

             progressBar.fillAmount = (float) (firstIndex) / (float)_finalPositions.Count;
             progressBar.color = new Color(1f - (float)firstIndex / _finalPositions.Count, (float)firstIndex / _finalPositions.Count, 0);
             cupCounter.text = $"{firstIndex + 1}/{_finalPositions.Count}";
             CustomMaths.SetEpsilon(.03f);
             SetTargetPoint(cupBaseEntryTransform);
             WriteCurrentState($"{firstIndex} base");
             while (Vector3.Distance(fakeEpsilonTransform.position, cupBaseEntryTransform.position) >= .03f)
             {
                 yield return new WaitForEndOfFrame();
                 Debug.Log($"Distance left to base entry = {Vector3.Distance(manipulatorTransform.position, cupBaseEntryTransform.position)}");
             }

             var transform1 = transform;
             var toPosition = (cupBaseTransform.position - transform1.position).normalized;
             var angle = CustomMaths.GetAngle(transform1, toPosition);
             SetTankSpeedConfiguration(TankSpeedConfiguration.Medium);
             SetTankTaskState(angle > 0 ? TankTaskState.TurningRight : TankTaskState.TurningLeft);
             while (Mathf.Abs(CustomMaths.GetAngle(transform, toPosition)) > 4f)
             {
                 yield return new WaitForEndOfFrame();
             }
             
             SetTankTaskState(TankTaskState.Stop);
             
             yield return new WaitForSeconds(.7f);
             CustomMaths.SetEpsilon(.25f);
             SetTankTaskState(TankTaskState.MovingToCup);
             
             SetTankSpeedConfiguration(TankSpeedConfiguration.Medium);
             
             SetTargetPoint(cupBaseTransform);
             while (Vector3.Distance(manipulatorTransform.position, cupBaseTransform.position) >= .026f)
             {
                 yield return new WaitForEndOfFrame();
                 Debug.Log($"Distance left to base = {Vector3.Distance(manipulatorTransform.position, cupBaseTransform.position)}");
             }
             
             
             SetTankTaskState(TankTaskState.Stop);
             yield return new WaitForSeconds(.5f);
             

             SetTankSpeedConfiguration(TankSpeedConfiguration.Default);
             
             SetTankTaskState(TankTaskState.ForwardMove);
             
             
             yield return new WaitForSeconds(1.3f);
             
             onCupTakenEvent?.Invoke();
             _extensionBoard.SetFanRotation(true);
             
             yield return new WaitForSeconds(.8f);
             
             SetTankTaskState(TankTaskState.ReverseMove);
             yield return new WaitForSeconds(1f);
             
             SetTankTaskState(TankTaskState.TurningRight);
             yield return new WaitForSeconds(1f);
             
             SetTankTaskState(TankTaskState.MovingToPlaceCup);
             
             SetTargetPoint(_finalPositions[firstIndex]);
             
             WriteCurrentState($"{firstIndex} placing");
             
             while (Vector3.Distance(manipulatorTransform.position, _finalPositions[firstIndex]) >= .55f)
             {
                 yield return new WaitForEndOfFrame();

                 Debug.Log($"Distance left to current target = {Vector2.Distance(new Vector2(  manipulatorTransform.position.x, manipulatorTransform.position.z), new Vector2( _finalPositions[firstIndex].x, _finalPositions[firstIndex].z))}, target index = {firstIndex.ToString()}");
             }
             SetTankSpeedConfiguration(TankSpeedConfiguration.Slow);
             _extensionBoard.SetFanRotation(true);
      
             float tmp = 0f;
             do
             {
                 yield return new WaitForEndOfFrame();
                 tmp = Vector2.Distance(
                     new Vector2(manipulatorTransform.position.x, manipulatorTransform.position.z),
                     new Vector2(_finalPositions[firstIndex].x, _finalPositions[firstIndex].z));
                 Debug.Log(
                     $"Slowed  Distance left to current target = {tmp}, target index = {firstIndex.ToString()}");
             } while (tmp > 0.0015f);

             var obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
             obj.transform.localScale = Vector3.one *.07f;
             obj.transform.position = manipulatorTransform.position;
             SetTankTaskState(TankTaskState.Stop);
             _extensionBoard.SetFanRotation(false);
             yield return new WaitForSeconds(0.8f);
             SetTankSpeedConfiguration(TankSpeedConfiguration.Slow);
             SetTankTaskState(TankTaskState.ReverseMove);
             yield return new WaitForSeconds(2f);
             SetTankSpeedConfiguration(TankSpeedConfiguration.Default);
             SetTankTaskState(TankTaskState.TurningRight);
             yield return new WaitForSeconds(1f);
             SetTankTaskState(TankTaskState.MovingToCup);
             SetTankSpeedConfiguration(TankSpeedConfiguration.Default);
             firstIndex++;
         }
    }
    


    public List<Transform> targetPositions;

    public Transform cupBaseEntryTransform;
    public Transform cupBaseTransform;

    public void SetTargetPoint(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    public Transform previousTargetTransform;

    private void SetTargetPoint(Vector3 targetPosition)
    {
        previousTargetTransform.position = targetPosition;
        this.targetTransform = previousTargetTransform;
    }

    
    private void SetTankTaskState(TankTaskState tankTaskState) =>
        this.tankTaskState = tankTaskState;


    public enum TankTaskState
    {
        MovingToCup,
        MovingToPlaceCup,
        TurningRight,
        TurningLeft,
        Stop,
        ReverseMove,
        ForwardMove
    }

    public enum TankSpeedConfiguration
    {
        Default,
        Medium,
        Slow
    }
}