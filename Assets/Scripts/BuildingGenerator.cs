using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour {

    //Prefabs
    public Transform wallPrefab;
    public Transform doorPrefab;
    public Transform windowPrefab;
    public Transform floorPrefab;
    public Transform roofPrefab;

    [Header("Amount of Rooms / Floors")]
    public Building building;

    //Buildings settings
    [System.Serializable]
    public class Building {
        [Range(1, 20)]
        public int rows;
        [Range(1, 20)]
        public int columns;

        public int amountFloors;

        [Range(0,1)]
        public float windowsPercentChance;
        [Range(0, 1)]
        public float doorPercentChance;

        public bool hasWall;
        public bool hasFloor;
        public bool hasRoof;
    }

    [Header("Room size")]
    //x
    public float width;
    //y
    public float height;
    //z
    public float length;

    bool getsRoof = false;

    Floor[] floors;
    Room[,] rooms;

    public void GenerateBuilding() {
        Generator();
        Renderer();
    }

    //Create Logic for a Building
    void Generator() {
        floors = new Floor[building.amountFloors];
        int floorCount = 0;

        foreach (Floor floor in floors) {
            rooms = new Room[building.rows, building.columns];
            for (int i = 0; i < building.rows; i++) {
                for (int j = 0; j < building.columns; j++) {
                    if (floorCount == building.amountFloors - 1) {
                        getsRoof = true;
                    } else {
                        getsRoof = false;
                    }
                    rooms[i, j] = new Room(new Vector3(i * width, floorCount, j * length), floorCount, getsRoof);
                    Room room = rooms[i, j];
                    room.walls = new Wall[4];

                    room.walls[0] = new Wall(new Vector3(room.RoomPosition.x + width / 2 + wallPrefab.localScale.z / 2, floorCount * height + wallPrefab.localScale.y * height / 2, room.RoomPosition.z), Quaternion.Euler(0, 90, 0));
                    room.walls[1] = new Wall(new Vector3(room.RoomPosition.x - width / 2 - wallPrefab.localScale.z / 2, floorCount * height + wallPrefab.localScale.y * height / 2, room.RoomPosition.z), Quaternion.Euler(0, -90, 0));
                    room.walls[2] = new Wall(new Vector3(room.RoomPosition.x, floorCount * height + wallPrefab.localScale.y * height / 2, room.RoomPosition.z + length / 2 + wallPrefab.localScale.z / 2), Quaternion.Euler(0, 0, 0));
                    room.walls[3] = new Wall(new Vector3(room.RoomPosition.x, floorCount * height + wallPrefab.localScale.y * height / 2, room.RoomPosition.z - length / 2 - wallPrefab.localScale.z / 2), Quaternion.Euler(0, 180, 0));
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

        foreach(Floor floor in floors) {
            string floorName = "Floor" + floor.FloorLevel;
            Transform floorHolder = new GameObject(floorName).transform;
            floorHolder.parent = buildingHolder;
            floorHolder.position = new Vector3(building.rows * width / 2f - (width / 2), floor.FloorLevel * height, building.columns * length / 2f - (length / 2));

            for (int i = 0; i < building.rows; i++) {
                for (int j = 0; j < building.columns; j++) {
                    //Create parent object for the room and a room Name
                    string roomName = "Room-" + i + j;
                    Transform roomHolder = new GameObject(roomName).transform;
                    roomHolder.parent = floorHolder;

                    Room room = floor.rooms[i, j];

                    roomHolder.position = new Vector3(room.RoomPosition.x, floor.FloorLevel * height + floorPrefab.localScale.y / 2f, room.RoomPosition.z);

                    PlaceFloor(room, floor, roomHolder);
                    PlaceWalls(building, room, floor, roomHolder);
                    PlaceRoof(room, floor, roomHolder);
                }
            }
        }
    }

    void PlaceFloor(Room room, Floor floor, Transform roomHolder) {
        if (building.hasFloor) {
            Vector3 groundPosition = new Vector3(room.RoomPosition.x, floor.FloorLevel * height + floorPrefab.localScale.y / 2f, room.RoomPosition.z);
            Transform ground = Instantiate(floorPrefab, groundPosition, Quaternion.Euler(0, 0, 0));
            ground.localScale = new Vector3(width, floorPrefab.localScale.y, length);
            ground.parent = roomHolder;
        }
    }

    void PlaceWalls(Building building, Room room, Floor floor, Transform roomHolder) {
        if (building.hasWall) {
            for (int i = 0; i < room.walls.Length; i++) {
                if (room.floorLevel == 0) {
                    InstantiateWall(room, Random.Range(0f, 1f) <= building.doorPercentChance ? doorPrefab : wallPrefab, roomHolder, i);
                } else if (room.floorLevel >= 1) {
                    InstantiateWall(room, Random.Range(0f, 1f) <= building.windowsPercentChance ? windowPrefab : wallPrefab, roomHolder, i);
                }
            }
        }
    }

    void InstantiateWall(Room room, Transform prefab, Transform roomHolder, int index) {
        Transform wall = Instantiate(prefab, room.walls[index].WallPosition, room.walls[index].WallRotation, roomHolder);
        SetlocalSacleWall(wall, index);
    }

    void SetlocalSacleWall(Transform wall, int index) {
        switch (index) {
            case 0:
                wall.localScale = new Vector3(wall.localScale.x * length + wallPrefab.localScale.z * 2, wall.localScale.y * height, wallPrefab.localScale.z);
                break;
            case 1:
                wall.localScale = new Vector3(wall.localScale.x * length + wallPrefab.localScale.z * 2, wall.localScale.y * height, wallPrefab.localScale.z);
                break;
            case 2:
                wall.localScale = new Vector3(wall.localScale.x * width, wall.localScale.y * height, wallPrefab.localScale.z);
                break;
            case 3:
                wall.localScale = new Vector3(wall.localScale.x * width, wall.localScale.y * height, wallPrefab.localScale.z);
                break;
        }
    }

    void PlaceRoof(Room room, Floor floor, Transform roomHolder) {
        if (building.hasRoof && room.HasRoof) {
            Vector3 roofPosition = new Vector3(room.RoomPosition.x, wallPrefab.localScale.y * height * (floor.FloorLevel + 1) + roofPrefab.localScale.y / 2, room.RoomPosition.z);
            Transform roof = Instantiate(roofPrefab, roofPosition, Quaternion.Euler(0, 0, 0));
            roof.localScale = new Vector3(roof.localScale.x * width + wallPrefab.localScale.z * 2, roof.localScale.y, roof.localScale.z * length + wallPrefab.localScale.z * 2);
            roof.parent = roomHolder;
        }
    }
}