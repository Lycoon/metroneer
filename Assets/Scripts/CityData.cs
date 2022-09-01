using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Hierarchy
{
    MINOR, MAJOR
}

public enum Growth
{
    UNFINISHED, FINISHED
}

public class VertexInfo
{
    public Hierarchy hierarchy { get; set; }
    public Growth growth { get; set; }
}

public class StreetInfo
{
    public enum Status
    {
        PLANNED, BUILT
    }

    public Hierarchy hierarchy { get; set; }
    public Growth growth { get; set; }
    public int traffic { get; set; }
    public int residents { get; set; }
}

public class Trip
{
    public DCEL.HalfEdge start { get; set; }
    public DCEL.HalfEdge end { get; set; }
    public int volume { get; set; }
    public float attraction { get; set; }
}

public class LandUseType
{
    public enum Type
    {
        LOW_RESIDENTIAL, HIGH_RESIDENTIAL,
        LOW_INDUSTRIAL, HIGH_INDUSTRIAL,
        COMMERCIAL, PARK, PUBLIC
    }

    public class ValuationFunction
    {
        public enum Function
        {
            STEP, LINEAR_UP_RAMP, LINEAR_DOWN_RAMP,
            GAIN_UP_RAMP, GAIN_DOWN_RAMP
        }

        public enum Attribute
        {
            CLUSTER, NEIGHBORHOOD,
            FOREST_DIST, WATER_DIST, CENTER_DIST,
            TRAFFIC, SLOPE, ELEVATION
        }

        static Func<float, float, float> stepFunction = (value, pMin) =>
        {
            if (value > pMin)
                return 1;
            return 0;
        };

        static Func<float, float, float, float> linearUpFunction = (value, pMin, pMax) =>
        {
            if (value < pMin)
                return 0;
            else if (value > pMax)
                return 1;

            var a = 1 / (pMax - pMin);
            var b = -a * pMin;

            return value * a + b;
        };

        static Func<float, float, float, float> linearDownFunction = (value, pMin, pMax) =>
        {
            return 1 - linearUpFunction(value, pMin, pMax);
        };

        public float weight { get; set; }
        public float pMin { get; set; }
        public float pMax { get; set; }
    }

    public Type type { get; set; }
    public float percentage { get; set; }
    public List<ValuationFunction> functions { get; set; }
}

public class LotInfo
{
    public LandUseType landUseType { get; set; }
    public float landUseValue { get; set; }
}

public class SimulationInput
{
    public List<Vector2> cityCenters { get; set; }
    public List<Vector2> growthCenters { get; set; }
    public List<float> streetGrowth { get; set; }
    public List<float> avgPrice { get; set; }
}