
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public class Room
    {
        public Transform trans;
        public Vector2 location;
        public int maxX;
        public int maxY;
        public List<Door> doors;
    }
    public Material material;

    [HideInInspector]
    public int numOfRooms;
    [HideInInspector]
    public Sprite[] brown;

    [HideInInspector]
    public int tileSize = 128;

    public enum Exits
    {
        North,
        South,
        East,
        West,
        Nothing
    }

    public class Door
    {
        public Exits exit;
        public Vector2 location;
        public Vector2 worldLocation;
        public int size;
        public int length;
        public float rotation;
    }

    private int exitSize = 6;
    [HideInInspector]
    public float locationX, locationY;

    private void Awake()
    {
        brown = Resources.LoadAll<Sprite>("Brown");

        numOfRooms = Random.Range(6, 13);
        GenerateStartRoom(2);

    }


    void CreateCorridors(Room ori)
    {
        int corridorLength = Random.Range(2, 16);

        foreach (var door in ori.doors)
        {
            if (door.exit == Exits.East)
            {
                CreateFloor(ori, door.location.x + corridorLength * 0.5f + 0.5f, door.location.y, Piece(brown, "middle"), door.size, corridorLength, 90f);
                CreateWall(ori, Piece(brown, "bottom"), door.location.x + corridorLength * 0.5f + 0.5f, door.location.y - door.size * 0.5f + 0.5f, corridorLength, 1, 0f);
                CreateWall(ori, Piece(brown, "top"), door.location.x + corridorLength * 0.5f + 0.5f, door.location.y + door.size * 0.5f - 0.5f, corridorLength, 1, 0f);

            }
            if (door.exit == Exits.West)
            {
                CreateFloor(ori, door.location.x - corridorLength * 0.5f - 0.5f, door.location.y, Piece(brown, "middle"), door.size, corridorLength, 90f);
                CreateWall(ori, Piece(brown, "bottom"), door.location.x - corridorLength * 0.5f - 0.5f, door.location.y - door.size * 0.5f + 0.5f, corridorLength, 1, 0f);
                CreateWall(ori, Piece(brown, "top"), door.location.x - corridorLength * 0.5f - 0.5f, door.location.y + door.size * 0.5f - 0.5f, corridorLength, 1, 0f);

            }
            if (door.exit == Exits.North)
            {
                CreateFloor(ori, door.location.x, door.location.y + corridorLength * 0.5f + 0.5f, Piece(brown, "middle"), door.size, corridorLength);
                CreateWall(ori, Piece(brown, "left"), door.location.x - door.size * 0.5f + 0.5f, door.location.y + corridorLength * 0.5f + 0.5f, 1, corridorLength, 0f);
                CreateWall(ori, Piece(brown, "right"), door.location.x + door.size * 0.5f - 0.5f, door.location.y + corridorLength * 0.5f + 0.5f, 1, corridorLength, 0f);

            }

            if (door.exit == Exits.South)
            {
                CreateFloor(ori, door.location.x, door.location.y - corridorLength * 0.5f - 0.5f, Piece(brown, "middle"), door.size, corridorLength);
                CreateWall(ori, Piece(brown, "left"), door.location.x - door.size * 0.5f + 0.5f, door.location.y - corridorLength * 0.5f - 0.5f, 1, corridorLength, 0f);
                CreateWall(ori, Piece(brown, "right"), door.location.x + door.size * 0.5f - 0.5f, door.location.y - corridorLength * 0.5f - 0.5f, 1, corridorLength, 0f);
            }
            door.length = corridorLength;
        }

    }

    void GenerateStartRoom(int minimumExits = 0)
    {
        int roomX = Random.Range(12, 26);
        int roomY = Random.Range(12, 26);


        GameObject room = new GameObject();
        room.transform.position = new Vector2(0f, 0f);

        Room roomStats = new Room();
        roomStats.trans = room.transform;
        roomStats.location = room.transform.position;
        roomStats.maxX = roomX;
        roomStats.maxY = roomY;

        CreateCornerPiece(roomStats, 0, 0, Piece(brown, "bottomLeft"));         // Luodaan huoneen kulmat alavasen
        CreateCornerPiece(roomStats, roomX, 0, Piece(brown, "bottomRight"));    // alaoikea
        CreateCornerPiece(roomStats, 0, roomY, Piece(brown, "topLeft"));        // ylävasen
        CreateCornerPiece(roomStats, roomX, roomY, Piece(brown, "topRight"));   // yläoikea

        CreateRoomWall(roomStats, Piece(brown, "top"));                             // Luodaan huoneen pääseinät yläseinä
        CreateRoomWall(roomStats, Piece(brown, "bottom"));                          // alaseinä
        CreateRoomWall(roomStats, Piece(brown, "left"));                            // vasen seinä
        CreateRoomWall(roomStats, Piece(brown, "right"));                           // oikea seinä

        CreateRoomFloor(roomStats, Piece(brown, "middle"));                         // Luodaan huoneen lattia
        CreateDoors(roomStats, roomX, roomY, minimumExits);                                       // Arvotaan ja luodaan huoneelle ovet
        CreateCorridors(roomStats);
        CreateAlignedRooms(roomStats);
    }

    void CreateRoom(Room roomStats)
    {
        GameObject room = new GameObject();
        room.transform.position = roomStats.location;
        roomStats.trans = room.transform;

        foreach (var door in roomStats.doors)
        {
            door.location = roomStats.trans.InverseTransformPoint(door.worldLocation);
            if (door.exit == Exits.South)
            {
                CreateFloor(roomStats, door.location.x, door.location.y + 1f, Piece(brown, "middle"), door.size, 1, 0, 5);
            }
            else if (door.exit == Exits.North)
            {
                CreateFloor(roomStats, door.location.x, door.location.y - 1f, Piece(brown, "middle"), door.size, 1, 0, 5);
            }
            else if (door.exit == Exits.West)
            {
                CreateFloor(roomStats, door.location.x + 1f, door.location.y, Piece(brown, "middle"), 1, door.size, 0, 5);
            }
            else
            {
                CreateFloor(roomStats, door.location.x - 1f, door.location.y, Piece(brown, "middle"), 1, door.size, 0, 5);
            }
        }

        CreateCornerPiece(roomStats, 0, 0, Piece(brown, "bottomLeft"));         // Luodaan huoneen kulmat alavasen
        CreateCornerPiece(roomStats, roomStats.maxX, 0, Piece(brown, "bottomRight"));    // alaoikea
        CreateCornerPiece(roomStats, 0, roomStats.maxY, Piece(brown, "topLeft"));        // ylävasen
        CreateCornerPiece(roomStats, roomStats.maxX, roomStats.maxY, Piece(brown, "topRight"));   // yläoikea

        CreateRoomWall(roomStats, Piece(brown, "top"));                             // Luodaan huoneen pääseinät yläseinä
        CreateRoomWall(roomStats, Piece(brown, "bottom"));                          // alaseinä
        CreateRoomWall(roomStats, Piece(brown, "left"));                            // vasen seinä
        CreateRoomWall(roomStats, Piece(brown, "right"));                           // oikea seinä

        CreateRoomFloor(roomStats, Piece(brown, "middle"));                         // Luodaan huoneen lattia
        CreateDoors(roomStats, roomStats.maxX, roomStats.maxY, 0);                  // Arvotaan ja luodaan huoneelle ovet
        CreateCorridors(roomStats);
        CreateAlignedRooms(roomStats);
    }

    void CreateAlignedRooms(Room ori)
    {
        foreach(var door in ori.doors)
        {
            Room newRoom = new Room();
            Door tempdoor = new Door();

            int roomX = Random.Range(12, 26);
            int roomY = Random.Range(12, 26);

            tempdoor.worldLocation = door.worldLocation;

            if (door.exit == Exits.North)
            {
                tempdoor.exit = Exits.South;
                tempdoor.worldLocation = new Vector2(tempdoor.worldLocation.x, tempdoor.worldLocation.y + door.length);
                int xPos = Random.Range(3, roomX - 1);
                newRoom.location = new Vector2(ori.trans.position.x + door.location.x - xPos, ori.trans.position.y + door.location.y + door.length + 1f);
            }
            else if (door.exit == Exits.South)
            {
                tempdoor.exit = Exits.North;
                tempdoor.worldLocation = new Vector2(tempdoor.worldLocation.x, tempdoor.worldLocation.y - door.length);
                int xPos = Random.Range(3, roomX - 1);
                newRoom.location = new Vector2(ori.trans.position.x + door.location.x - xPos, ori.trans.position.y - door.location.y - roomY -door.length - 1f);
            }
            else if (door.exit == Exits.East)
            {
                tempdoor.exit = Exits.West;
                tempdoor.worldLocation = new Vector2(tempdoor.worldLocation.x + door.length, tempdoor.worldLocation.y);
                int yPos = Random.Range(3, roomY - 1);
                newRoom.location = new Vector2(ori.trans.position.x + door.location.x + door.length + 1f, ori.trans.position.y + door.location.y - yPos);
            }
            else
            {
                tempdoor.exit = Exits.East;
                tempdoor.worldLocation = new Vector2(tempdoor.worldLocation.x - door.length, tempdoor.worldLocation.y);
                int yPos = Random.Range(3, roomY - 1);
                newRoom.location = new Vector2(ori.trans.position.x - door.location.x - door.length - 1f - roomX, ori.trans.position.y + door.location.y - yPos);

            }
            tempdoor.size = door.size;

            newRoom.doors = new List<Door>();
            
            newRoom.doors.Add(tempdoor);
            newRoom.maxX = roomX;
            newRoom.maxY = roomY;
            CreateRoom(newRoom);
        }
    }

    void CreateDoors(Room ori, int roomX, int roomY, int numOfMinExits = 1)
    {

        //int numOfExits = Random.Range(numOfMinExits, 4);
        int numOfExits = Random.Range(0, 3);
        ori.doors = new List<Door>();


        for (int i = 0; i < numOfExits; i++)
        {
            exitSize = Random.Range(2, 6);
            var tempDoor = new Door();
            int a = Random.Range(0, 4);
            bool reserved = true;

            if (ori.doors.Count > 0)
            {
                while (reserved)
                {
                    a = Random.Range(0, 4);
                    foreach (var door in ori.doors)
                    {
                        reserved = false;
                        if ((Exits)a == door.exit)
                        {
                            reserved = true;
                            break;
                        }
                    }
                }
            }

            tempDoor.exit = (Exits)a;
            tempDoor.size = exitSize;

            float lX = Mathf.Abs(locationX);
            float lY = Mathf.Abs(locationY);
            switch ((Exits)a)
            {
                case Exits.North:
                    {
                        int exitX = Random.Range(2, roomX - exitSize - 1);
                        tempDoor.location = CreateFloor(ori, exitX + exitSize * 0.5f - 0.5f, roomY, Piece(brown, "middle"), exitSize, 1);
                        //CreateCornerPiece(ori, exitX, roomY, Piece(brown, "border"), 5, 0);
                        //CreateCornerPiece(ori.trans, exitX + exitSize, roomY, 90f);
                        break;
                    }
                case Exits.South:
                    {
                        int exitX = Random.Range(2, roomX - exitSize - 1);
                        tempDoor.location = CreateFloor(ori, exitX + exitSize * 0.5f - 0.5f, 0f, Piece(brown, "middle"), exitSize, 1);
                        //CreateCornerPiece(ori.trans, exitX - 1, 0, 270f);
                        //CreateCornerPiece(ori.trans, exitX + exitSize, 0, 0f);
                        break;
                    }
                case Exits.East:
                    {
                        int exitY = Random.Range(2, roomY - exitSize - 1);
                        tempDoor.location = CreateFloor(ori, roomX, exitY + exitSize * 0.5f - 0.5f, Piece(brown, "middle"), exitSize, 1, 90f);
                        //CreateCornerPiece(ori.trans, roomX, exitY - 1, 0f);
                        //CreateCornerPiece(ori.trans, roomX, exitY + exitSize, 90f);
                        break;
                    }
                case Exits.West:
                    {
                        int exitY = Random.Range(2, roomY - exitSize - 1);
                        tempDoor.location = CreateFloor(ori, 0f, exitY + exitSize * 0.5f - 0.5f, Piece(brown, "middle"), exitSize, 1, 90f);
                        //CreateCornerPiece(ori.trans, 0, exitY - 1, 270f);
                        //CreateCornerPiece(ori.trans, 0, exitY + exitSize, 180f);
                        break;
                    }
            }

            tempDoor.worldLocation = ori.trans.TransformPoint(tempDoor.location);
            ori.doors.Add(tempDoor);
        }
    }


    void CreateWall(Room ori, Sprite sprit, float x, float y, float width, float height, float rot, int sort = 5)
    {
        GameObject wall = new GameObject();
        wall.transform.parent = ori.trans;

        var sr = wall.AddRenderer(material);
        sr.sortingOrder = sort;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.sprite = sprit;
        sr.size = new Vector2(width, height);
        wall.transform.eulerAngles = new Vector3(0f, 0f, rot);
        wall.transform.localPosition = new Vector2(x, y);
    }

    void CreateRoomWall(Room ori, Sprite sprit, int sort = 1)
    {
        GameObject wall = new GameObject();
        wall.transform.parent = ori.trans;

        var sr = wall.AddRenderer(material);
        sr.sortingOrder = sort;

        sr.drawMode = SpriteDrawMode.Tiled;
        sr.sprite = sprit;
        sr.size = new Vector2(ori.maxX - 1, 1f);

        if (sprit.name == "top")
            wall.transform.localPosition = new Vector2(ori.maxX * 0.5f, ori.maxY);
        if (sprit.name == "bottom")
            wall.transform.localPosition = new Vector2(ori.maxX * 0.5f, 0f);
        if (sprit.name == "left")
        {
            sr.size = new Vector2(1f, ori.maxY - 1);
            wall.transform.localPosition = new Vector2(0f, ori.maxY * 0.5f);
        }
        if (sprit.name == "right")
        {
            sr.size = new Vector2(1f, ori.maxY - 1);
            wall.transform.localPosition = new Vector2(ori.maxX, ori.maxY * 0.5f);
        }
    }

    void CreateCornerPiece(Room ori, int x, int y, Sprite sprit, int sort = 1, float rot = 0)
    {
        GameObject corner = new GameObject();
        corner.transform.parent = ori.trans;

        var sr = corner.AddRenderer(material);
        sr.sortingOrder = sort;
        sr.sprite = sprit;
        corner.transform.localPosition = new Vector2(x, y);
        corner.transform.eulerAngles = new Vector3(0f, 0f, rot);
    }

    void CreateRoomFloor(Room ori, Sprite sprit, int sort = 0)
    {
        var floor = new GameObject();
        floor.transform.parent = ori.trans;
        var fsr = floor.AddRenderer(material);
        fsr.sprite = sprit;
        fsr.drawMode = SpriteDrawMode.Tiled;
        fsr.size = new Vector2(ori.maxX - 1, ori.maxY - 1);
        fsr.sortingOrder = sort;
        floor.transform.localPosition = new Vector2(ori.maxX * 0.5f, ori.maxY * 0.5f);
    }

    Vector2 CreateFloor(Room ori, float x, float y, Sprite sprit, float distance, float width, float rot = 0f, int sort = 2)
    {
        var floor = new GameObject();
        floor.name = sprit.name;
        floor.transform.parent = ori.trans;
        var fsr = floor.AddRenderer(material);
        fsr.sprite = sprit;
        fsr.drawMode = SpriteDrawMode.Tiled;
        fsr.size = new Vector2(distance, width);
        fsr.sortingOrder = sort;
        floor.transform.localPosition = new Vector2(x, y);
        floor.transform.eulerAngles = new Vector3(0f, 0f, rot);
        return floor.transform.localPosition;
    }

    public Sprite Piece(Sprite[] pieces, string name)
    {
        foreach (var sr in pieces)
        {
            if (sr.name == name)
            {
                return sr;
            }
        }

        return null;
    }

    Exits Opposite(Exits exit)
    {
        if (exit == Exits.East)
            return Exits.West;
        if (exit == Exits.West)
            return Exits.East;
        if (exit == Exits.North)
            return Exits.South;
        if (exit == Exits.South)
            return Exits.North;

        return Exits.North;
    }
}
