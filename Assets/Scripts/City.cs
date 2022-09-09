using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public City()
    {
        graph = new DCEL();
    }

    public int GetNumberOfStreets()
    {
        return graph.halfEdges.Count / 2;
    }

    public List<DCEL.Vertex> GetUnfinishedMajorVertices()
    {
        List<DCEL.Vertex> unfinished = new List<DCEL.Vertex>();
        foreach (DCEL.Vertex v in graph.vertices)
        {
            if (v.vertexInfo.hierarchy == Hierarchy.MAJOR
                    && v.vertexInfo.growth != Growth.UNFINISHED)
                unfinished.Add(v);
        }
        return unfinished;
    }

    public DCEL graph { get; set; }
}
