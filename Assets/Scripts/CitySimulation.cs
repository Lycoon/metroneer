using UnityEngine;
using System.Collections.Generic;
using static LandUseType.Type;
using static LandUseType.ValuationFunction;

public class SimulationInput
{
    public SimulationInput(
        List<LandUseType> landUseTypes,
        List<Vector2> cityCenters,
        List<Vector2> growthCenters,
        List<float> streetGrowth,
        List<float> avgPrice
        )
    {
        this.landUseTypes = landUseTypes;
        this.cityCenters = cityCenters;
        this.growthCenters = growthCenters;
        this.streetGrowth = streetGrowth;
        this.avgPrice = avgPrice;
    }

    public List<LandUseType> landUseTypes { get; set; }
    public List<Vector2> cityCenters { get; set; }
    public List<Vector2> growthCenters { get; set; }
    public List<float> streetGrowth { get; set; }
    public List<float> avgPrice { get; set; }
}

public class CitySimulation : MonoBehaviour
{
    public GameObject meshGenerator;

    void Start()
    {
        City city = new City();
        SimulationInput input = GetSimulationInput();

        /*for (int i = 0; i < 30; i++)
        {
            int nbHalfEdges = dcel.halfEdges.Count;
            DCEL.HalfEdge h = dcel.halfEdges[Random.Range(0, nbHalfEdges - 1)];
            DCEL.Vertex v = new DCEL.Vertex(new Vector2(Random.Range(-100, 100), Random.Range(-100, 100)), h);
            dcel.AddVertex(v, h);
        }*/

        render(city.graph);
    }

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

    SimulationInput GetSimulationInput()
    {
        var landUseTypes = new List<LandUseType>();

        // LOW RESIDENTIAL - one or two family houses
        var lowResidentialFunctions = new[]{new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearDownFunc,
            Attribute.TRAFFIC, // little traffic
            0.45f, 0, 1
        ), new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearDownFunc,
            Attribute.INFLUENCE, // far from high density industrial
            0.4f, 0, 0.1f
        ), new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearUpFunc,
            Attribute.INFLUENCE, // proximity to commercial
            0.15f, 0, 0.2f
        )};
        LandUseType lowResidential = new LandUseType(LOW_RESIDENTIAL, 0.4f, lowResidentialFunctions);
        landUseTypes.Add(lowResidential);

        // HIGH RESIDENTIAL - blocks, apartments, condos
        var highResidentialFunctions = new[]{new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearDownFunc,
            Attribute.INFLUENCE, // far from high density industrial
            0.3f, 0, 1
        ), new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearDownFunc,
            Attribute.CENTER_DIST, // proximity to city center
            0.25f, 0, 1
        ), new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearDownFunc,
            Attribute.TRAFFIC, // little traffic
            0.25f, 0, 1
        ), new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearUpFunc,
            Attribute.INFLUENCE, // proximity to commercial
            0.2f, 0, 1
        )};
        LandUseType highResidential = new LandUseType(HIGH_RESIDENTIAL, 0.2f, highResidentialFunctions);
        landUseTypes.Add(highResidential);

        // LOW INDUSTRIAL - service industriy, offices
        var lowIndustrialFunctions = new[]{new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearDownFunc,
            Attribute.CENTER_DIST, // proximity to city center
            0.3f, 0, 1
        ), new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.stepFunc,
            Attribute.NOOP, // constant value 1
            0.7f, 0, 0
        )};
        LandUseType lowIndustrial = new LandUseType(LOW_INDUSTRIAL, 0.08f, lowIndustrialFunctions);
        landUseTypes.Add(lowIndustrial);

        // HIGH INDUSTRIAL - heavy industry
        var highIndustrialFunctions = new[]{new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearDownFunc,
            Attribute.INFLUENCE, // far from residential
            0.45f, 0, 1
        ), new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearDownFunc,
            Attribute.CENTER_DIST, // far from city center
            0.35f, 0, 1
        ), new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearDownFunc,
            Attribute.INFLUENCE, // far from public buildings
            0.2f, 0, 1
        )};
        LandUseType highIndustrial = new LandUseType(HIGH_INDUSTRIAL, 0.1f, highIndustrialFunctions);
        landUseTypes.Add(highIndustrial);

        // COMMERCIAL - retail sales, offices, inns
        var commercialFunctions = new[]{new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearUpFunc,
            Attribute.TRAFFIC, // much traffic
            0.75f, 0, 1
        ), new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearUpFunc,
            Attribute.INFLUENCE, // proximity to residential
            0.25f, 0, 1
        )};
        LandUseType commercial = new LandUseType(COMMERCIAL, 0.15f, commercialFunctions);
        landUseTypes.Add(commercial);

        // PARKS - recreation, memorials
        var parkFunctions = new[]{new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearUpFunc,
            Attribute.TRAFFIC, // much traffic
            0.75f, 0, 1
        ), new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearUpFunc,
            Attribute.INFLUENCE, // proximity to residential
            0.25f, 0, 1
        )};
        LandUseType parks = new LandUseType(PARK, 0.04f, parkFunctions);
        landUseTypes.Add(parks);

        // PUBLIC - schools, communal, transportation
        var publicFunctions = new[]{new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearUpFunc,
            Attribute.INFLUENCE, // proximity to parks
            0.3f, 0, 1
        ), new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearDownFunc,
            Attribute.CENTER_DIST, // proximity to city center
            0.4f, 0, 1
        ), new LandUseType.ValuationFunction(
            LandUseType.ValuationFunction.linearDownFunc,
            Attribute.INFLUENCE, // far from high industrial
            0.3f, 0, 1
        )};
        LandUseType publicArea = new LandUseType(PUBLIC, 0.03f, publicFunctions);
        landUseTypes.Add(publicArea);

        var cityCenters = new List<Vector2>(new[]{
            new Vector2(170, 20),
            new Vector2(-40, -90),
        });

        var growthCenters = new List<Vector2>(new[]{
            new Vector2(170, 20),
            new Vector2(-40, -90),
        });

        var streetGrowth = new List<float>();
        var avgPrice = new List<float>();

        return new SimulationInput(landUseTypes, cityCenters, growthCenters, streetGrowth, avgPrice);
    }
}
