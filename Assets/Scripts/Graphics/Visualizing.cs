using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Graphics
{
    public class Visualizing : MonoBehaviour
    {

        public bool drawRoute;
        
        public bool displayBadRoutes;

        public string cupsObjectName;

        public float multiplier;

        public float cupMeshSizeMultiplier = 50f;

        public Material cupMaterial;

        public Mesh cupMesh;

        public DrawingController drawingController;


        private List<Vector3> GetSortedPositionsFromVectors(IEnumerable<Vector3> reference) =>
            reference.OrderByDescending(x => Vector3.Distance(x, drawingController.cupBaseTransform.position)).ToList();
    
        
        public void DrawCups()
        {
            cupsObjectName = "GeneratedCups";
            var positions = GetSortedPositionsFromVectors(Deserializer.GetPositions($"{Application.dataPath}\\cups.txt", multiplier));
            var cupsGameObject = GameObject.Find(cupsObjectName);
            if (cupsGameObject == null)
                cupsGameObject = new GameObject(cupsObjectName, new[] {typeof(MeshFilter), typeof(MeshRenderer)});
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
                    if (Vector3.Distance(positions[i], positions[k]) < drawingController.maxDistanceBetweenCups)
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

            if (drawRoute)
            {
                for (int i = 1; i < positions.Count; i++)
                {
                    //Gizmos.DrawSphere(positions[i], .045f);   
                    Gizmos.color = Color.white;
                    var relation = drawingController.maxDistanceBetweenCups / Vector3.Distance(positions[i], positions[i - 1]);
                    if (displayBadRoutes)
                    {
                        GUIStyle style = new GUIStyle {normal = {textColor = Color.red}};
                        var distance = Vector3.Distance(positions[i], positions[i-1]);
                        if (distance <= drawingController.maxDistanceBetweenCups)
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
                Gizmos.DrawLine(drawingController.cupBaseTransform.position, positions[0]);
            }

            Handles.Label(new Vector3(minX - .3f, 0, (Math.Abs(maxZ) - Math.Abs((minZ)))/2f ), $"Height = {Math.Round( maxZ - minZ, 3)} m");
        
            Handles.Label(new Vector3((Math.Abs(maxX) - Math.Abs(minX)) / 2f, 0, minZ - .1f ), $"Width = {Math.Round( maxX - minX, 3)} m");
        
            Gizmos.DrawLine(new Vector3(minX, 0, minZ), new Vector3(maxX, 0, minZ));

            Gizmos.DrawLine(new Vector3(minX, 0, minZ), new Vector3(minX, 0, maxZ));
        }
    }
}
