using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CreateFlame : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.name = "Pyramid";

        // Vertices
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 1, 0),
            new Vector3(-1, 0, -1),
            new Vector3(1, 0, -1),
            new Vector3(1, 0, 1),
            new Vector3(-1, 0, 1)
        };

        // Triangles
        int[] triangles = new int[]
        {
            // Base
            1, 2, 3,
            1, 3, 4,

            0, 2, 1, // Side 1
            0, 3, 2, // Side 2
            0, 4, 3, // Side 3
            0, 1, 4  // Side 4
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        meshFilter.mesh = mesh;
    }
}
