using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ValuationLambda = System.Func<float, float, float, float>;

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

    public LandUseType(Type type, float percentage, ValuationFunction[] functions)
    {
        this.type = type;
        this.percentage = percentage;
        this.functions = functions;
    }

    public class ValuationFunction
    {
        public ValuationFunction(ValuationLambda function, Attribute attribute, float weight, float pMin, float pMax)
        {
            this.function = function;
            this.attribute = attribute;
            this.weight = weight;
            this.pMin = pMin;
            this.pMax = pMax;
        }

        public enum Attribute
        {
            CLUSTER, INFLUENCE,
            FOREST_DIST, WATER_DIST, CENTER_DIST,
            TRAFFIC, SLOPE, ELEVATION, NOOP
        }

        public static ValuationLambda stepFunc = (value, pMin, pMax) =>
        {
            if (value > pMin && value < pMax)
                return 1;
            return 0;
        };

        public static ValuationLambda linearUpFunc = (value, pMin, pMax) =>
        {
            if (value < pMin)
                return 0;
            else if (value > pMax)
                return 1;

            var a = 1 / (pMax - pMin);
            var b = -a * pMin;

            return value * a + b;
        };

        public static ValuationLambda linearDownFunc = (value, pMin, pMax) =>
        {
            return 1 - linearUpFunc(value, pMin, pMax);
        };

        public float Execute(float attribute)
        {
            return weight * function(attribute, pMin, pMax);
        }

        public Func<float, float, float, float> function { get; set; }
        public Attribute attribute { get; set; }
        public float weight { get; set; }
        public float pMin { get; set; }
        public float pMax { get; set; }
    }

    public float GetLandUseValue(float attribute)
    {
        float landUseValue = 0;
        foreach (var function in functions)
        {
            landUseValue += function.Execute(attribute);
        }
        return landUseValue;
    }

    public Type type { get; set; }
    public float percentage { get; set; }
    public ValuationFunction[] functions { get; set; }
}

public class LotInfo
{
    public LandUseType.Type landUseType { get; set; }
    public float landUseValue { get; set; }
    public Vector2 position { get; set; }

    // Local land use goals
    public float cluster { get; set; }
    public List<float> influence { get; set; }
    public float traffic { get; set; }
    public float cityCenterDistance { get; set; }
    public float forestDistance { get; set; }
    public float waterDistance { get; set; }
    public float slope { get; set; }
    public float elevation { get; set; }
}
