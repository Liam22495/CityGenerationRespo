using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CityGenerator : MonoBehaviour
    {
    public int numberOfBuildings = 9;
    public Vector3 citySize = new Vector3(60, 0, 50);
    public Material[] buildingMaterials = new Material[6]; 
    public Material roadMaterial;
    public Material buildingRoadMaterial;
    public Material windowMaterial;
    public Material[] doorMaterials = new Material[6]; // Array for multiple door materials
    public GameObject carPrefab; // Assign your car prefab in the inspector
    public GameObject endPointPrefab; // Assign your end point prefab in the inspector
    private List<Vector3> roadPositions = new List<Vector3>();
    public CameraFollow cameraFollowScript;


    void Start()
        {
        int gridRows = 3; // Number of rows in the grid for buildings
        int gridColumns = 3; // Number of columns in the grid for buildings
        float spacing = 10.0f; // The spacing between buildings and roads

        // The offset to align buildings with roads
        float xOffset = (gridColumns - 1) * spacing / 2;
        float zOffset = (gridRows - 1) * spacing / 2;

        // Loop through each grid cell and place a building
        for (int x = 0; x < gridColumns; x++)
            {
            for (int z = 0; z < gridRows; z++)
                {
                Vector3 buildingPosition = new Vector3(x * spacing - xOffset, 0, z * spacing - zOffset);
                Vector3 buildingScale = new Vector3(
                    UnityEngine.Random.Range(1, 5), // width
                    UnityEngine.Random.Range(5, 20), // height
                    UnityEngine.Random.Range(1, 5)  // depth
                );

                // The Y-position based on building height
                buildingPosition.y = buildingScale.y / 2;

                GenerateBuildingAt(buildingPosition, buildingScale);
                GenerateAdjacentRoads(buildingPosition, buildingScale);
                }
            }

        // Generate the road grid
        GenerateRoadGrid(gridRows, gridColumns, spacing);

        // Place the start and end points after generating the city
        PlaceStartAndEndPoint();
        }


    void GenerateRoadGrid(int rows, int cols, float spacing)
        {
        for (int x = 0; x < cols; x++)
            {
            for (int z = 0; z < rows; z++)
                {
                // Generate horizontal roads
                GenerateRoad(new Vector3(x * spacing - spacing / 2, 0, z * spacing), new Vector3(spacing, 0.1f, roadWidth), isBuilding: false);
                // Generate vertical roads
                GenerateRoad(new Vector3(x * spacing, 0, z * spacing - spacing / 2), new Vector3(roadWidth, 0.1f, spacing), isBuilding: false);
                }
            }
        }

    void GenerateBuildingRoad(Vector3 position, Vector3 scale)
        {
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.transform.position = position;
        road.transform.localScale = scale;
        Renderer renderer = road.GetComponent<Renderer>();
        renderer.material = buildingRoadMaterial; // Use building road material
        }

    void GenerateRoad(Vector3 position, Vector3 scale, bool isBuilding)
        {
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.transform.position = position;
        road.transform.localScale = scale;
        Renderer renderer = road.GetComponent<Renderer>();
        renderer.material = isBuilding ? buildingRoadMaterial : roadMaterial;

        // Add position to list of road positions for spawning the car and end point
        roadPositions.Add(position);
        }

    void GenerateBuildingAt(Vector3 position, Vector3 scale)
        {
        GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
        building.transform.position = position;
        building.transform.localScale = scale;

        if (buildingMaterials != null && buildingMaterials.Length > 0)
            {
            Renderer renderer = building.GetComponent<Renderer>();
            renderer.material = buildingMaterials[UnityEngine.Random.Range(0, buildingMaterials.Length)];
            }

        // Calculate the number of windows and doors based on the building's scale
        int numWindows = Mathf.FloorToInt(scale.x / 2);
        int numDoors = 1; // You can change this as needed

        // Define the number of windows in the X and Y directions
        int numWindowsX = 4; // Number of windows along the X-axis (same side as doors)
        int numWindowsY = 4; // Number of windows along the Y-axis (upwards)

        // Calculate the spacing between windows in the X and Y directions
        float windowSpacingX = scale.x / (numWindowsX + 1);
        float windowSpacingY = scale.y / (numWindowsY + 1);

        // Loop through each column of windows (X-axis)
        for (int x = 0; x < numWindowsX; x++)
            {
            // Calculate the X position of the current column of windows
            float windowX = position.x - scale.x / 2 + (x + 1) * windowSpacingX;

            // Loop through each row of windows (Y-axis)
            for (int y = 0; y < numWindowsY; y++)
                {
                // Calculate the Y position of the current row of windows
                float windowY = position.y - scale.y / 2 + (y + 1) * windowSpacingY;

                // Calculate the position for each window
                Vector3 windowPosition = new Vector3(
                    windowX,
                    windowY,
                    position.z + scale.z / 2 // Align with the front of the building
                );

                // Create a window cube
                GameObject window = GameObject.CreatePrimitive(PrimitiveType.Cube);
                window.transform.position = windowPosition;
                window.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); // Adjust the size as needed
                window.transform.parent = building.transform; // Make the window a child of the building
                Renderer windowRenderer = window.GetComponent<Renderer>();
                windowRenderer.material = windowMaterial; // Assign the window material
                }
            }

        // Create doors
        for (int i = 0; i < numDoors; i++)
            {
            // Calculate the position for the door at the base and aligned with the road
            Vector3 doorPosition = new Vector3(
                position.x, // Center the door on the X-axis
                roadWidth / 2, // Place the door at half of the road's height, aligning it with the road
                position.z + scale.z / 2 // Adjust the position to be at the front
            );

            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.transform.position = doorPosition;
            door.transform.localScale = new Vector3(0.5f, 1, 0.1f); // Adjust the size as needed
            door.transform.parent = building.transform; // Make the door a child of the building

            Renderer doorRenderer = door.GetComponent<Renderer>();
            if (doorMaterials != null && doorMaterials.Length > 0)
                {
                // Correctly accessing a single Material from the doorMaterials array
                doorRenderer.material = doorMaterials[Random.Range(0, doorMaterials.Length)];
                }
            }
        }

    void PlaceStartAndEndPoint()
        {
        if (roadPositions.Count < 2) return; // Ensure there are enough road segments

        // Randomly select start and end positions from the list of road positions
        int startIndex = Random.Range(0, roadPositions.Count);
        int endIndex = Random.Range(0, roadPositions.Count);

        // Ensure start and end points are not the same
        while (endIndex == startIndex)
            {
            endIndex = Random.Range(0, roadPositions.Count);
            }

        Vector3 startPoint = roadPositions[startIndex];
        Vector3 endPoint = roadPositions[endIndex];

        // Instantiate car at start point
        GameObject carInstance = Instantiate(carPrefab, startPoint, Quaternion.identity);
        carInstance.name = "Prometheus"; // Assign a specific name to the car

        if (cameraFollowScript != null)
            {
            cameraFollowScript.carTransform = carInstance.transform;
            }

        // Instantiate end point marker
        Instantiate(endPointPrefab, endPoint, Quaternion.identity);
        }



    public float roadWidth = 3f; // Width of the road

    void GenerateAdjacentRoads(Vector3 buildingPosition, Vector3 buildingScale)
        {
        // Calculate the positions and sizes of roads based on building position and scale
        // Road on the left of the building
        GenerateRoad(new Vector3(buildingPosition.x - buildingScale.x / 2 - roadWidth / 2, 0, buildingPosition.z), new Vector3(roadWidth, 0.1f, buildingScale.z + 2 * roadWidth), isBuilding: true);

        // Road on the right of the building
        GenerateRoad(new Vector3(buildingPosition.x + buildingScale.x / 2 + roadWidth / 2, 0, buildingPosition.z), new Vector3(roadWidth, 0.1f, buildingScale.z + 2 * roadWidth), isBuilding: true);

        // Road below the building
        GenerateBuildingRoad(new Vector3(buildingPosition.x, 0, buildingPosition.z - buildingScale.z / 2 - roadWidth / 2), new Vector3(buildingScale.x + 2 * roadWidth, 0.1f, roadWidth));

        // Road above the building
        GenerateBuildingRoad(new Vector3(buildingPosition.x, 0, buildingPosition.z + buildingScale.z / 2 + roadWidth / 2), new Vector3(buildingScale.x + 2 * roadWidth, 0.1f, roadWidth));
        }
    }
