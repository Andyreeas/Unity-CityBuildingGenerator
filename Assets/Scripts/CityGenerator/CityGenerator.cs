using System;
using System.Collections;
using UnityEngine;

public class CityGenerator : MonoBehaviour {

    [SerializeField]
    public int amountOfBuildings;
    public float buildingFootprint;

    public Transform floor;

    int buildingCounter = 0;

    BuildingGenerator[] buildings;

    float mapWidth;
    float mapLength;

    private void Start() {
        mapWidth = floor.localScale.x / 2;
        mapLength = floor.localScale.z / 2;

        buildings = new BuildingGenerator[amountOfBuildings];

        GenerateCity();
    }

    public void GenerateCity() {
        for (int i = (int)(-mapWidth/2f); i < mapWidth/2f; i++) {
            for (int j = (int)(-mapLength/2f); j < mapLength/2f; j++) {
                GenerateBuilding(i, j);
            }
        }
    }

    void GenerateBuilding(int x, int z) {
        BuildingGenerator building = GetComponent<BuildingGenerator>();
        building.GenerateRandomizedBuilding();

        string buildingName = "Building: " + buildingCounter;
        GameObject.Find("Building").name = buildingName;

        SetPositionOfBuilding(buildingName, x, z);
        try {
            buildings[buildingCounter] = building;
        } catch (IndexOutOfRangeException) {
            Debug.LogError("Not enough buildings");
        }

        buildingCounter++;

    }

    void SetPositionOfBuilding(string buildingName, int x, int z) {
        Vector3 position = new Vector3(x * buildingFootprint, 0, z * buildingFootprint);
        Transform buildingPosition = GameObject.Find(buildingName).transform;
        buildingPosition.position = position;
    }

}


