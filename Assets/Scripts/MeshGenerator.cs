using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public Vector2 start, end;
    public float width = 0.3f;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    public static Vector3 GetPerpendicular(Vector2 v)
    {
        return new Vector3(v.y, 0, -v.x).normalized;
    }

    void CreateShape()
    {
        Vector3 ortho = GetPerpendicular(start);
        vertices = new Vector3[] {
            new Vector3(start.x, 0, start.y) - (ortho * width),
            new Vector3(start.x, 0, start.y) + (ortho * width),
            new Vector3(end.x, 0, end.y) - (ortho * width),
            new Vector3(end.x, 0, end.y) + (ortho * width),
        };

        triangles = new int[] {
            0, 1, 2,
            1, 3, 2
        };
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
