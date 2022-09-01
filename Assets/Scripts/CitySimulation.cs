using UnityEngine;
using System.Collections.Generic;

public class CitySimulation : MonoBehaviour
{
    public GameObject meshGenerator;
    public GameObject parentGenerated;

    void render(DCEL dcel)
    {
        List<DCEL.HalfEdge> rendered = new List<DCEL.HalfEdge>();
        foreach (DCEL.HalfEdge h in dcel.halfEdges)
        {
            if (rendered.Contains(h.twin)) // prevent rendering streets twice
                continue;
            rendered.Add(h);

            GameObject mesh = GameObject.Instantiate(meshGenerator);
            MeshGenerator props = mesh.GetComponent<MeshGenerator>();
            mesh.name = "Street";

            props.start = h.twin.target.position;
            props.end = h.target.position;
            props.width = 0.2f;
        }
    }

    void Start()
    {
        DCEL dcel = new DCEL();
        for (int i = 0; i < 30; i++)
        {
            int nbHalfEdges = dcel.halfEdges.Count;
            DCEL.HalfEdge h = dcel.halfEdges[Random.Range(0, nbHalfEdges - 1)];
            DCEL.Vertex v = new DCEL.Vertex(new Vector2(Random.Range(-100, 100), Random.Range(-100, 100)), h);
            dcel.AddVertex(v, h);
        }

        render(dcel);
    }
}
