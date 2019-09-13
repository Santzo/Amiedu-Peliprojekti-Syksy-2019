
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
        public bool corridorDone;
    }

    private int exitSize = 6;
    private int maxRooms = 20;
    private int numOfRooms = 1;
    private float pixelsPerUnit = 128f;

    private Vector2 cornerPieceSize;
    private Vector2 bigCornerPieceSize;



    [HideInInspector]
    public float locationX, locationY;

    private void Awake()
    {
        brown = Resources.LoadAll<Sprite>("Brown");
        cornerPieceSize = SpriteSizeInPixels("corner");
        cornerPieceSize = cornerPieceSize / 2f;
        bigCornerPieceSize = SpriteSizeInPixels("bigCorner");
        bigCornerPieceSize = bigCornerPieceSize / 2f;
        GenerateStartRoom(2);

    }


    void CreateCorridors(Room ori)                      // Luodaan käytävät huoneiden väliin
    {
        int corridorLength = Random.Range(2, 16);

        foreach (var door in ori.doors)
        {

            Debug.Log(ori.trans.name + " " + door.exit + " " + door.corridorDone);
            if (door.corridorDone)
                continue;


            if (door.exit == Exits.East)
            {
                CreateFloor(ori, door.location.x + corridorLength * 0.5f + 0.5f, door.location.y, Piece(brown, "middle"), door.size, corridorLength + 2f, 90f);
                CreateWall(ori, Piece(brown, "bottom"), door.location.x + corridorLength * 0.5f + 0.5f, door.location.y - door.size * 0.5f + 0.5f, corridorLength, 1, 0f);
                CreateWall(ori, Piece(brown, "top"), door.location.x + corridorLength * 0.5f + 0.5f, door.location.y + door.size * 0.5f - 1f, corridorLength, 2f, 0f);

                CreateCornerPiece(ori, door.location.x + 0.5f - (cornerPieceSize.x / pixelsPerUnit), door.location.y - door.size * 0.5f + (cornerPieceSize.y / pixelsPerUnit), Piece(brown, "corner"), 5, 0);
                CreateCornerPiece(ori, door.location.x + corridorLength + 0.5f + (cornerPieceSize.x / pixelsPerUnit), door.location.y - door.size * 0.5f + (cornerPieceSize.y / pixelsPerUnit), Piece(brown, "corner"), 5, 270);

                CreateCornerPiece(ori, door.location.x + 0.5f - (bigCornerPieceSize.x / pixelsPerUnit), door.location.y + door.size * 0.5f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(brown, "bigCorner"), 5, 0);
                CreateCornerPiece(ori, door.location.x + corridorLength + 0.5f + (bigCornerPieceSize.x / pixelsPerUnit), door.location.y + door.size * 0.5f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(brown, "bigCorner"), 5, 0, -1);


            }
            if (door.exit == Exits.West)
            {
                CreateFloor(ori, door.location.x - corridorLength * 0.5f - 0.5f, door.location.y, Piece(brown, "middle"), door.size, corridorLength + 2f, 90f);
                CreateWall(ori, Piece(brown, "bottom"), door.location.x - corridorLength * 0.5f - 0.5f, door.location.y - door.size * 0.5f + 0.5f, corridorLength, 1, 0f);
                CreateWall(ori, Piece(brown, "top"), door.location.x - corridorLength * 0.5f - 0.5f, door.location.y + door.size * 0.5f - 1f, corridorLength, 2f, 0f);


                CreateCornerPiece(ori, door.location.x - 0.5f + (cornerPieceSize.x / pixelsPerUnit), door.location.y - door.size * 0.5f + (cornerPieceSize.y / pixelsPerUnit), Piece(brown, "corner"), 5, 270);
                CreateCornerPiece(ori, door.location.x - corridorLength - 0.5f - (cornerPieceSize.x / pixelsPerUnit), door.location.y - door.size * 0.5f + (cornerPieceSize.y / pixelsPerUnit), Piece(brown, "corner"), 5, 0);

                CreateCornerPiece(ori, door.location.x - 0.5f + (bigCornerPieceSize.x / pixelsPerUnit), door.location.y + door.size * 0.5f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(brown, "bigCorner"), 5, 0, -1);
                CreateCornerPiece(ori, door.location.x - corridorLength - 0.5f - (bigCornerPieceSize.x / pixelsPerUnit), door.location.y + door.size * 0.5f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(brown, "bigCorner"), 5, 0);



            }
            if (door.exit == Exits.North)
            {
                CreateFloor(ori, door.location.x, door.location.y + corridorLength * 0.5f + 0.5f, Piece(brown, "middle"), door.size + 1f, corridorLength + 2f);
                CreateWall(ori, Piece(brown, "left"), door.location.x - door.size * 0.5f, door.location.y + corridorLength * 0.5f + 1f, 1, corridorLength - 1f, 0f);
                CreateWall(ori, Piece(brown, "right"), door.location.x + door.size * 0.5f, door.location.y + corridorLength * 0.5f + 1f, 1, corridorLength - 1f, 0f);

                CreateCornerPiece(ori, door.location.x - 0.5f - (door.size * 0.5f - cornerPieceSize.x / pixelsPerUnit), door.location.y + corridorLength + 0.5f + (cornerPieceSize.y / pixelsPerUnit), Piece(brown, "corner"), 5, 270);
                CreateCornerPiece(ori, door.location.x + 0.5f + (door.size * 0.5f - cornerPieceSize.x / pixelsPerUnit), door.location.y + corridorLength + 0.5f + (cornerPieceSize.y / pixelsPerUnit), Piece(brown, "corner"), 5, 0);

                CreateCornerPiece(ori, door.location.x - 0.5f - (door.size * 0.5f - bigCornerPieceSize.x / pixelsPerUnit), door.location.y + 1.5f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(brown, "bigCorner"), 5, 0);
                CreateCornerPiece(ori, door.location.x + 0.5f + (door.size * 0.5f - bigCornerPieceSize.x / pixelsPerUnit), door.location.y + 1.5f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(brown, "bigCorner"), 5, 0, -1);
            }

            if (door.exit == Exits.South)
            {
                CreateFloor(ori, door.location.x, door.location.y - corridorLength * 0.5f - 1.75f, Piece(brown, "middle"), door.size, corridorLength + 2.5f);
                CreateWall(ori, Piece(brown, "left"), door.location.x - door.size * 0.5f + 0.5f, door.location.y - corridorLength * 0.5f - 1f, 1, corridorLength, 0f);
                CreateWall(ori, Piece(brown, "right"), door.location.x + door.size * 0.5f - 0.5f, door.location.y - corridorLength * 0.5f - 1f, 1, corridorLength, 0f);


                CreateCornerPiece(ori, door.location.x - (door.size * 0.5f - cornerPieceSize.x / pixelsPerUnit), door.location.y - 1f + (cornerPieceSize.y / pixelsPerUnit), Piece(brown, "corner"), 5, 270);
                CreateCornerPiece(ori, door.location.x + (door.size * 0.5f - cornerPieceSize.x / pixelsPerUnit), door.location.y - 1f + (cornerPieceSize.y / pixelsPerUnit), Piece(brown, "corner"), 5, 0);

                CreateCornerPiece(ori, door.location.x - (door.size * 0.5f - bigCornerPieceSize.x / pixelsPerUnit), door.location.y - corridorLength - 1f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(brown, "bigCorner"), 5, 0);
                CreateCornerPiece(ori, door.location.x + (door.size * 0.5f - bigCornerPieceSize.x / pixelsPerUnit), door.location.y - corridorLength - 1f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(brown, "bigCorner"), 5, 0, -1);


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
        room.name = "Size X " + roomStats.maxX + " Size Y " + roomStats.maxY;
        room.transform.position = roomStats.location;
        roomStats.trans = room.transform;

        foreach (var door in roomStats.doors)
        {
            door.location = roomStats.trans.InverseTransformPoint(door.worldLocation);
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
        if (numOfRooms < maxRooms)
        {
            CreateDoors(roomStats, roomStats.maxX, roomStats.maxY, 0);                  // Arvotaan ja luodaan huoneelle ovet
            CreateCorridors(roomStats);
            CreateAlignedRooms(roomStats);
        }
    }

    void CreateAlignedRooms(Room ori)
    {
        if (numOfRooms > maxRooms)
            return;

        numOfRooms++;

        foreach (var door in ori.doors)
        {
            if (numOfRooms > maxRooms)
                return;

            if (door.corridorDone)
                continue; 

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
                float multi = Mathf.Abs((ori.trans.position.x % 1)) != Mathf.Abs((newRoom.location.x % 1)) ? 0f : 0.5f;
                newRoom.location = new Vector2(newRoom.location.x + multi, newRoom.location.y);
            }
            else if (door.exit == Exits.South)
            {
                tempdoor.exit = Exits.North;
                tempdoor.worldLocation = new Vector2(tempdoor.worldLocation.x, tempdoor.worldLocation.y - door.length);
                int xPos = Random.Range(3, roomX - 1);
                newRoom.location = new Vector2(ori.trans.position.x + door.location.x - xPos, ori.trans.position.y - door.location.y - roomY - door.length - 1f);
                float multi = Mathf.Abs((ori.trans.position.x % 1)) != Mathf.Abs((newRoom.location.x % 1)) ? 0f : 0.5f;
                newRoom.location = new Vector2(newRoom.location.x + multi, newRoom.location.y);
            }
            else if (door.exit == Exits.East)
            {
                tempdoor.exit = Exits.West;
                tempdoor.worldLocation = new Vector2(tempdoor.worldLocation.x + door.length, tempdoor.worldLocation.y);
                int yPos = Random.Range(3, roomY - 1);
                newRoom.location = new Vector2(ori.trans.position.x + door.location.x + door.length + 1f, ori.trans.position.y + door.location.y - yPos);
                float multi = Mathf.Abs((ori.trans.position.y % 1)) != Mathf.Abs((newRoom.location.y % 1)) ? 0.5f : 0f;
                newRoom.location = new Vector2(newRoom.location.x, newRoom.location.y + multi);
            }
            else
            {
                tempdoor.exit = Exits.East;
                tempdoor.worldLocation = new Vector2(tempdoor.worldLocation.x - door.length, tempdoor.worldLocation.y);
                int yPos = Random.Range(3, roomY - 1);
                newRoom.location = new Vector2(ori.trans.position.x - door.location.x - door.length - 1f - roomX, ori.trans.position.y + door.location.y - yPos);
                float multi = Mathf.Abs((ori.trans.position.y % 1)) != Mathf.Abs((newRoom.location.y % 1)) ? 0.5f : 0f;
                newRoom.location = new Vector2(newRoom.location.x, newRoom.location.y + multi);

            }

            tempdoor.size = door.size;
            tempdoor.corridorDone = true;
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

        if (ori.doors == null)
        {
            ori.doors = new List<Door>();
        }

        numOfExits = 1;


        for (int i = 0; i < numOfExits; i++)
        {
            exitSize = Random.Range(2, 6);
            var tempDoor = new Door();
            int a = Random.Range(0, 2);
            bool reserved = true;

            if (ori.doors.Count > 0)
            {
                while (reserved)
                {
                    a = Random.Range(0, 2);
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



            float lX = Mathf.Abs(locationX);
            float lY = Mathf.Abs(locationY);
            switch ((Exits)a)
            {
                case Exits.North:
                    {
                        int exitX = Random.Range(2, roomX - exitSize - 1);
                        tempDoor.location = new Vector2(exitX + exitSize * 0.5f - 0.5f, roomY - 0.5f);
                        break;
                    }
                case Exits.South:
                    {
                        int exitX = Random.Range(2, roomX - exitSize - 1);
                        tempDoor.location = new Vector2(exitX + exitSize * 0.5f - 0.5f, 0.5f);
                        break;
                    }
                case Exits.East:
                    {
                        exitSize = exitSize < 4 ? 4 : exitSize;
                        int exitY = Random.Range(2, roomY - exitSize - 1);
                        tempDoor.location = new Vector2(roomX, exitY + exitSize * 0.5f - 0.5f);
                        break;
                    }
                case Exits.West:
                    {
                        exitSize = exitSize < 4 ? 4 : exitSize;
                        int exitY = Random.Range(2, roomY - exitSize - 1);
                        tempDoor.location = new Vector2(0f, exitY + exitSize * 0.5f - 0.5f);
                        break;
                    }
            }
            tempDoor.exit = (Exits)a;
            tempDoor.size = exitSize;
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
        {
            wall.transform.localPosition = new Vector2(ori.maxX * 0.5f, ori.maxY);
            sr.size = new Vector2(sr.size.x, 2f);
        }
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

    void CreateCornerPiece(Room ori, float x, float y, Sprite sprit, int sort = 3, float rot = 0, float xSwap = 1f)
    {
        GameObject corner = new GameObject();
        corner.transform.parent = ori.trans;

        var sr = corner.AddRenderer(material);
        sr.sortingOrder = sort;
        sr.sprite = sprit;
        corner.transform.localPosition = new Vector2(x, y);
        corner.transform.localScale = new Vector3(xSwap, 1f, 1f);
        corner.transform.eulerAngles = new Vector3(0f, 0f, rot);
    }

    void CreateRoomFloor(Room ori, Sprite sprit, int sort = 0)
    {
        var floor = new GameObject();
        floor.transform.parent = ori.trans;
        var fsr = floor.AddRenderer(material);
        fsr.sprite = sprit;
        fsr.drawMode = SpriteDrawMode.Tiled;
        fsr.size = new Vector2(ori.maxX - 1f, ori.maxY - 1f);
        fsr.sortingOrder = sort;
        floor.transform.localPosition = new Vector2(ori.maxX * 0.5f, ori.maxY * 0.5f);
    }

    Vector2 CreateFloor(Room ori, float x, float y, Sprite sprit, float distance, float width, float rot = 0f, int sort = 4)
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

    public Vector2 SpriteSizeInPixels(string name)
    {
        var sr = Piece(brown, name);
        return new Vector2(sr.bounds.size.x * pixelsPerUnit, sr.bounds.size.y * pixelsPerUnit);
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

