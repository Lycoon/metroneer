using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DCEL
{
    public class HalfEdge
    {
        public HalfEdge(
            Vertex target,
            Face face,
            HalfEdge twin, HalfEdge next, HalfEdge prev)
        {
            this.target = target;
            this.face = face;
            this.twin = twin;
            this.next = next;
            this.prev = prev;
        }

        public HalfEdge() { }

        public Vertex target { get; set; }
        public Face face { get; set; }
        public HalfEdge twin { get; set; }
        public HalfEdge next { get; set; }
        public HalfEdge prev { get; set; }
    }

    public class Vertex
    {
        public Vertex(Vector2 position, HalfEdge halfEdge)
        {
            this.position = position;
            this.halfEdge = halfEdge;
        }

        public Vertex() { }

        public Vector2 position { get; set; }
        public HalfEdge halfEdge { get; set; }
    }

    public class Face
    {
        public Face(HalfEdge halfEdge)
        {
            this.halfEdge = halfEdge;
        }

        public Face() { }

        public HalfEdge halfEdge { get; set; }
    }

    public DCEL()
    {
        halfEdges = new List<HalfEdge>();
        vertices = new List<Vertex>();
        faces = new List<Face>();
    }

    public void AddVertex(Vertex vertex, HalfEdge halfEdge)
    {
        Vertex u = halfEdge.target;

        HalfEdge h1 = new HalfEdge();
        HalfEdge h2 = new HalfEdge();
        halfEdges.Add(h1);
        halfEdges.Add(h2);

        h1.twin = h2;
        h2.twin = h1;
        vertex.halfEdge = h2;

        h1.target = vertex;
        h2.target = u;

        h1.face = halfEdge.face;
        h2.face = halfEdge.face;

        h1.next = h2;
        h2.next = halfEdge.next;

        h1.prev = halfEdge;
        h2.prev = h1;

        halfEdge.next = h1;
        h2.next.prev = h2;
    }

    public void SplitFace(HalfEdge halfEdge, Vertex vertex)
    {
        Vertex u = halfEdge.target;
        Face f = halfEdge.face;

        Face f1 = new Face();
        Face f2 = new Face();
        faces.Add(f1);
        faces.Add(f2);

        HalfEdge h1 = new HalfEdge();
        HalfEdge h2 = new HalfEdge();
        halfEdges.Add(h1);
        halfEdges.Add(h2);

        f1.halfEdge = h1;
        f2.halfEdge = h2;

        h1.twin = h2;
        h2.twin = h1;

        h1.target = vertex;
        h2.target = u;

        h2.next = halfEdge.next;
        h2.next.prev = h2;
        h1.prev = halfEdge;
        halfEdge.next = h1;

        HalfEdge curr = h2;
        while (true)
        {
            curr.face = f2;
            if (curr.target.Equals(vertex))
                break;

            curr = curr.next;
        }

        h1.next = curr.next;
        h1.next.prev = h1;
        curr.next = h2;
        h2.prev = curr;

        curr = h1;
        while (!curr.target.Equals(u))
        {
            curr.face = f1;
            curr = curr.next;
        }

        faces.Remove(f);
    }

    public List<HalfEdge> GetFaceEdges(Face f)
    {
        List<HalfEdge> edges = new List<HalfEdge>();
        HalfEdge start = f.halfEdge;
        HalfEdge curr = start;

        do
        {
            edges.Add(curr);
            curr = curr.next;
        }
        while (!curr.Equals(start));

        return edges;
    }

    /*public Dictionary<Vertex, double> GetSurroundingVertices(Vector2 position, double radius)
    {
        double x1 = position.x;
        double y1 = position.y;

        var found = new Dictionary<Vertex, double>();
        foreach (Vertex v in vertices)
        {
            if (v.position.Equals(position))
                continue;

            double x2 = v.position.x;
            double y2 = v.position.y;
            double dist = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));

            if (dist < radius)
                found[v] = dist;
        }

        return found;
    }*/

    public Tuple<Vertex, double> GetClosestVertex(Vector2 position)
    {
        double x1 = position.x;
        double y1 = position.y;

        Vertex found = null;
        double minDist = double.PositiveInfinity;
        foreach (Vertex v in vertices)
        {
            if (v.position.Equals(position)) // WARNING for identity
                continue;

            double x2 = v.position.x;
            double y2 = v.position.y;
            double dist = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));

            if (dist < minDist)
            {
                found = v;
                minDist = dist;
            }
        }

        return Tuple.Create(found, minDist);
    }

    private List<HalfEdge> halfEdges;
    private List<Vertex> vertices;
    private List<Face> faces;
}
