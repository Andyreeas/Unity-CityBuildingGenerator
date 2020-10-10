using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour {

    //Prefabs
    public Transform wallPrefab;
    public Transform floorPrefab;
    public Transform roofPrefab;

    //Buildings settings
    [Header("Amount of Rooms / Floors")]
    public Building building;
    public int amountFloors;

    [Header("Room size")]
    //x
    public float width;
    //y
    public float height;
    //z
    public float length;

    Floor[] floors;
    Room[,] rooms;

    public void GenerateBuilding() {
        Generator();
        Renderer();
    }

    //Create Logic for a Building
    void Generator() {
        rooms = new Room[building.x, building.z];
        floors = new Floor[amountFloors];

        int floorCount = 0;

        foreach (Floor floor in floors) {
            for (int i = 0; i < building.x; i++) {
                for (int j = 0; j < building.z; j++) {
                    rooms[i, j] = new Room(new Vector2(i * width, j * length));
                }
            }
            floors[floorCount] = new Floor(floorCount++, rooms);
        }
    }

    //Create the Building with the logic
    void Renderer() {
        //Parent object if already exists it gets destroyed
        string buildingHolderName = "Building";
        if (GameObject.Find(buildingHolderName)) {
            DestroyImmediate(GameObject.Find(buildingHolderName));
        }
        Transform buildingHolder = new GameObject(buildingHolderName).transform;

        foreach (Floor floor in floors) {
            string floorName = "Floor" + floor.amountFloors;
            Transform floorHolder = new GameObject(floorName).transform;
            floorHolder.parent = buildingHolder;
            for (int i = 0; i < building.x; i++) {
                for (int j = 0; j < building.z; j++) {
                    //Create parent object for the room and a room Name
                    string roomName = "Room-" + i + j;
                    Transform roomHolder = new GameObject(roomName).transform;
                    roomHolder.parent = floorHolder;

                    Room room = rooms[i, j];
                    Transform ground = Instantiate(floorPrefab, new Vector3(room.roomPosition.x, floor.amountFloors * height + floorPrefab.localScale.y / 2f, room.roomPosition.y), Quaternion.Euler(0, 0, 0));
                    ground.localScale = new Vector3(width, floorPrefab.localScale.y, length);
                    ground.parent = roomHolder;

                    Transform wall01 = Instantiate(wallPrefab, new Vector3(room.roomPosition.x + width / 2 + wallPrefab.localScale.z / 2, floor.amountFloors * height + wallPrefab.localScale.y * height / 2, room.roomPosition.y), Quaternion.Euler(0, 90, 0), roomHolder);
                    wall01.localScale = new Vector3(wall01.localScale.x * length + wallPrefab.localScale.z * 2, wall01.localScale.y * height, wallPrefab.localScale.z);

                    Transform wall02 = Instantiate(wallPrefab, new Vector3(room.roomPosition.x - width / 2 - wallPrefab.localScale.z / 2, floor.amountFloors * height + wallPrefab.localScale.y * height / 2, room.roomPosition.y), Quaternion.Euler(0, 90, 0), roomHolder);
                    wall02.localScale = new Vector3(wall02.localScale.x * length + wallPrefab.localScale.z * 2, wall02.localScale.y * height, wallPrefab.localScale.z);

                    Transform wall03 = Instantiate(wallPrefab, new Vector3(room.roomPosition.x, floor.amountFloors * height + wallPrefab.localScale.y * height / 2, room.roomPosition.y + length / 2 + wallPrefab.localScale.z / 2), Quaternion.Euler(0, 0, 0), roomHolder);
                    wall03.localScale = new Vector3(wall03.localScale.x * width, wall03.localScale.y * height, wallPrefab.localScale.z);

                    Transform wall04 = Instantiate(wallPrefab, new Vector3(room.roomPosition.x, floor.amountFloors * height + wallPrefab.localScale.y * height / 2, room.roomPosition.y - length / 2 - wallPrefab.localScale.z / 2), Quaternion.Euler(0, 0, 0), roomHolder);
                    wall04.localScale = new Vector3(wall04.localScale.x * width, wall04.localScale.y * height, wallPrefab.localScale.z);

                    if (building.hasRoof && floor.amountFloors == floors.Length - 1){
                        Transform roof = Instantiate(roofPrefab, new Vector3(room.roomPosition.x, floor.amountFloors * height + wall01.localScale.y, room.roomPosition.y), Quaternion.Euler(0, 0, 0));
                        roof.localScale = new Vector3(roof.localScale.x * width, roof.localScale.y, roof.localScale.z * length);
                        roof.parent = roomHolder;
                    }
                }
            }
        }
    }
    
    [System.Serializable]
    public class Building {
        public int x;
        public int z;

        public bool hasRoof;

    }

}
