using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

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

    public class AllRooms
    {
        public Room room;
        public Vector2 start;
        public Vector2 end;
    }

    public Sprite wallShadowSprite;
    public Sprite losBlockerSprite;
    public Material material;
    [HideInInspector]
    public Sprite[] brown;
    [HideInInspector]
    public Sprite[] cellar;
    [HideInInspector]
    public Sprite[] current;


    [HideInInspector]
    public int tileSize = 128;

    public enum Exits
    {
        top,
        bottom,
        right,
        left,
    }

    public class Door
    {
        public Exits exit;
        public Vector2 location;
        public Vector2 worldLocation;
        public float size;
        public float length;
        public float rotation;
        public bool corridorDone;
    }

    private int exitSize = 6;
    private int maxRooms = 50;
    private int numOfRooms = 1;
    private float pixelsPerUnit = 128f;
    private float unitsPerPixel = 100f / 128f / 100f;

    private GameObject wallShadow;
    private GameObject losBlocker;
    private Vector2 cornerPieceSize;
    private Vector2 bigCornerPieceSize;

    private List<AllRooms> allRooms = new List<AllRooms>();
    public AllRooms gameField = new AllRooms();



    [HideInInspector]
    public float locationX, locationY;

    private void Awake()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        brown = Resources.LoadAll<Sprite>("Brown");
        cellar = Resources.LoadAll<Sprite>("Cellar");
        current = cellar;
        cornerPieceSize = SpriteSizeInPixels("corner");
        cornerPieceSize = cornerPieceSize * 0.5f;
        bigCornerPieceSize = SpriteSizeInPixels("bigCorner");
        bigCornerPieceSize = bigCornerPieceSize * 0.5f;
        InitializeAssets();

    }

    private void Start()
    {
        InitializeLevel(2);
    }


    void CreateCorridors(Room ori)                      // Luodaan käytävät huoneiden väliin
    {
        int corridorLength = Random.Range(2, 16);

        foreach (var door in ori.doors)
        {

            if (door.corridorDone)
                continue;


            if (door.exit == Exits.right)
            {
                CreateFloor(ori, door.location.x + corridorLength * 0.5f + 0.5f, door.location.y, Piece(current, "middle"), corridorLength + 2f, door.size, 0f);
                CreateWall(ori, Piece(current, "bottom"), door.location.x + corridorLength * 0.5f + 0.5f, door.location.y - door.size * 0.5f + 0.5f, corridorLength, 1, 0f);

                var loc = new Vector2(door.location.x + corridorLength * 0.5f + 0.5f, door.location.y + door.size * 0.5f - 1f);
                CreateWall(ori, Piece(current, "top"), loc.x, loc.y, corridorLength, 2f, 0f);

                CreateCornerPiece(ori, door.location.x + 0.5f - (cornerPieceSize.x / pixelsPerUnit), door.location.y - door.size * 0.5f + (cornerPieceSize.y / pixelsPerUnit), Piece(current, "corner"), 5, 0);
                CreateCornerPiece(ori, door.location.x + corridorLength + 0.5f + (cornerPieceSize.x / pixelsPerUnit), door.location.y - door.size * 0.5f + (cornerPieceSize.y / pixelsPerUnit), Piece(current, "corner"), 5, 0, -1);

                CreateCornerPiece(ori, door.location.x + 0.5f - (bigCornerPieceSize.x / pixelsPerUnit), door.location.y + door.size * 0.5f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(current, "bigCorner"), 5, 0, -1);
                CreateCornerPiece(ori, door.location.x + corridorLength + 0.5f + (bigCornerPieceSize.x / pixelsPerUnit), door.location.y + door.size * 0.5f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(current, "bigCorner"), 5, 0);
                CreateWallShadow(ori.trans, loc, new Vector2(corridorLength - 0.67f, 0f));

            }
            if (door.exit == Exits.left)
            {
                CreateFloor(ori, door.location.x - corridorLength * 0.5f - 0.5f, door.location.y, Piece(current, "middle"), corridorLength + 2f, door.size, 0f);
                CreateWall(ori, Piece(current, "bottom"), door.location.x - corridorLength * 0.5f - 0.5f, door.location.y - door.size * 0.5f + 0.5f, corridorLength, 1, 0f);

                var loc = new Vector2(door.location.x - corridorLength * 0.5f - 0.5f, door.location.y + door.size * 0.5f - 1f);
                CreateWall(ori, Piece(current, "top"), loc.x, loc.y, corridorLength, 2f, 0f);

                CreateCornerPiece(ori, door.location.x - 0.5f + (cornerPieceSize.x / pixelsPerUnit), door.location.y - door.size * 0.5f + (cornerPieceSize.y / pixelsPerUnit), Piece(current, "corner"), 5, 0, -1);
                CreateCornerPiece(ori, door.location.x - corridorLength - 0.5f - (cornerPieceSize.x / pixelsPerUnit), door.location.y - door.size * 0.5f + (cornerPieceSize.y / pixelsPerUnit), Piece(current, "corner"), 5, 0);

                CreateCornerPiece(ori, door.location.x - 0.5f + (bigCornerPieceSize.x / pixelsPerUnit), door.location.y + door.size * 0.5f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(current, "bigCorner"), 5, 0);
                CreateCornerPiece(ori, door.location.x - corridorLength - 0.5f - (bigCornerPieceSize.x / pixelsPerUnit), door.location.y + door.size * 0.5f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(current, "bigCorner"), 5, 0, -1);
                CreateWallShadow(ori.trans, loc, new Vector2(corridorLength - 0.67f, 0f));


            }
            if (door.exit == Exits.top)
            {
                CreateFloor(ori, door.location.x, door.location.y + corridorLength * 0.5f + 0.5f, Piece(current, "middle"), door.size, corridorLength + 2f);
                CreateWall(ori, Piece(current, "left"), door.location.x - door.size * 0.5f + 0.5f, door.location.y + corridorLength * 0.5f + 1f, 1, corridorLength - 1f, 0f);
                CreateWall(ori, Piece(current, "right"), door.location.x + door.size * 0.5f - 0.5f, door.location.y + corridorLength * 0.5f + 1f, 1, corridorLength - 1f, 0f);

                CreateCornerPiece(ori, door.location.x - (door.size * 0.5f - cornerPieceSize.x / pixelsPerUnit), door.location.y + corridorLength + 0.5f + (cornerPieceSize.y / pixelsPerUnit), Piece(current, "corner"), 5, 0, -1);
                CreateCornerPiece(ori, door.location.x + (door.size * 0.5f - cornerPieceSize.x / pixelsPerUnit), door.location.y + corridorLength + 0.5f + (cornerPieceSize.y / pixelsPerUnit), Piece(current, "corner"), 5, 0);

                CreateCornerPiece(ori, door.location.x - (door.size * 0.5f - bigCornerPieceSize.x / pixelsPerUnit), door.location.y + 1.5f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(current, "bigCorner"), 5, 0);
                CreateCornerPiece(ori, door.location.x + (door.size * 0.5f - bigCornerPieceSize.x / pixelsPerUnit), door.location.y + 1.5f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(current, "bigCorner"), 5, 0, -1);
            }

            if (door.exit == Exits.bottom)
            {
                CreateFloor(ori, door.location.x, door.location.y - corridorLength * 0.5f - 1.75f, Piece(current, "middle"), door.size, corridorLength + 2.5f);
                CreateWall(ori, Piece(current, "left"), door.location.x - door.size * 0.5f + 0.5f, door.location.y - corridorLength * 0.5f - 1f, 1, corridorLength, 0f);
                CreateWall(ori, Piece(current, "right"), door.location.x + door.size * 0.5f - 0.5f, door.location.y - corridorLength * 0.5f - 1f, 1, corridorLength, 0f);


                CreateCornerPiece(ori, door.location.x - (door.size * 0.5f - cornerPieceSize.x / pixelsPerUnit), door.location.y - 1f + (cornerPieceSize.y / pixelsPerUnit), Piece(current, "corner"), 5, 0, -1);
                CreateCornerPiece(ori, door.location.x + (door.size * 0.5f - cornerPieceSize.x / pixelsPerUnit), door.location.y - 1f + (cornerPieceSize.y / pixelsPerUnit), Piece(current, "corner"), 5, 0);

                CreateCornerPiece(ori, door.location.x - (door.size * 0.5f - bigCornerPieceSize.x / pixelsPerUnit), door.location.y - corridorLength - 1f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(current, "bigCorner"), 5, 0);
                CreateCornerPiece(ori, door.location.x + (door.size * 0.5f - bigCornerPieceSize.x / pixelsPerUnit), door.location.y - corridorLength - 1f - (bigCornerPieceSize.y / pixelsPerUnit), Piece(current, "bigCorner"), 5, 0, -1);


            }
            door.length = corridorLength;

        }

    }

    void CreateRoomWallsAndFloors(Room roomStats)
    {
        if (numOfRooms < maxRooms)
        {
            CreateDoors(roomStats, roomStats.maxX, roomStats.maxY, 0);                  // Arvotaan ja luodaan huoneelle ovet
            CreateCorridors(roomStats);
            CreateAlignedRooms(roomStats);
        }
        CreateCornerPiece(roomStats, 0f - SpritePositionAdjusted(SpriteSizeInPixels("bottomLeft").x), 0, Piece(current, "bottomLeft"));         // Luodaan huoneen kulmat alavasen
        CreateCornerPiece(roomStats, roomStats.maxX + SpritePositionAdjusted(SpriteSizeInPixels("bottomRight").x), 0, Piece(current, "bottomRight"));    // alaoikea
        CreateCornerPiece(roomStats, 0, roomStats.maxY, Piece(current, "topLeft"));        // ylävasen
        CreateCornerPiece(roomStats, roomStats.maxX, roomStats.maxY, Piece(current, "topRight"));   // yläoikea

        CreateRoomWall(roomStats, Piece(current, "top"));                             // Luodaan huoneen pääseinät yläseinä
        CreateRoomWall(roomStats, Piece(current, "bottom"));                          // alaseinä
        CreateRoomWall(roomStats, Piece(current, "left"));                            // vasen seinä
        CreateRoomWall(roomStats, Piece(current, "right"));                           // oikea seinä

        // Luodaan huoneen lattia
        CreateRoomFloor(roomStats, Piece(current, "middle"));

        allRooms.Add(new AllRooms { room = roomStats, start = new Vector2(roomStats.trans.position.x, roomStats.trans.position.y), end = new Vector2(roomStats.trans.position.x + roomStats.maxX, roomStats.trans.position.y + roomStats.maxY) });

        CheckGameFieldSize(roomStats);

    }

    private void SortObjects()
    {
        var objects = FindObjectsOfType<SortingGroup>();
        foreach (var obj in objects)
        {
            if (obj.sortingLayerName != "Wall" && obj.sortingLayerName != "FloorObjects")
            {
                float offset = 0f;
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null) offset = sr.bounds.extents.y * 0.5f;
                obj.sortingOrder = Info.SortingOrder(obj.transform.position.y - offset);
            }
        }
    }

    private void CheckGameFieldSize(Room roomStats)
    {
        gameField.start.x = roomStats.location.x < gameField.start.x ? roomStats.location.x : gameField.start.x;
        gameField.start.y = roomStats.location.y < gameField.start.y ? roomStats.location.y : gameField.start.y;

        gameField.end.x = roomStats.location.x + roomStats.maxX > gameField.end.x ? roomStats.location.x + roomStats.maxX : gameField.end.x;
        gameField.end.y = roomStats.location.y + roomStats.maxY > gameField.end.y ? roomStats.location.y + roomStats.maxY : gameField.end.y;

    }

    void InitializeLevel(int minimumExits = 0)
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

        CreateRoomWallsAndFloors(roomStats);
        SortObjects();
        Events.onFieldInitialized(gameField);
    }

    void AdjacentRoom(Room roomStats)
    {
        GameObject room = new GameObject();
        room.name = "Size X " + roomStats.maxX + " Size Y " + roomStats.maxY;
        room.transform.position = roomStats.location;
        roomStats.trans = room.transform;

        foreach (var door in roomStats.doors)
        {
            door.location = roomStats.trans.InverseTransformPoint(door.worldLocation);
        }
        CreateRoomWallsAndFloors(roomStats);

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

            if (door.exit == Exits.top)
            {
                tempdoor.exit = Exits.bottom;
                tempdoor.worldLocation = new Vector2(tempdoor.worldLocation.x, tempdoor.worldLocation.y + door.length);
                int xPos = Random.Range(3, roomX - 1);
                newRoom.location = new Vector2(ori.trans.position.x + door.location.x - xPos, ori.trans.position.y + door.location.y + door.length + 1f);
                float multi = Mathf.Abs((ori.trans.position.x % 1)) != Mathf.Abs((newRoom.location.x % 1)) ? 0f : 0.5f;
                newRoom.location = new Vector2(newRoom.location.x + multi, newRoom.location.y);
            }
            else if (door.exit == Exits.bottom)
            {
                tempdoor.exit = Exits.top;
                tempdoor.worldLocation = new Vector2(tempdoor.worldLocation.x, tempdoor.worldLocation.y - door.length);
                int xPos = Random.Range(3, roomX - 1);
                newRoom.location = new Vector2(ori.trans.position.x + door.location.x - xPos, ori.trans.position.y - door.location.y - roomY - door.length - 1f);
                float multi = Mathf.Abs((ori.trans.position.x % 1)) != Mathf.Abs((newRoom.location.x % 1)) ? 0f : 0.5f;
                newRoom.location = new Vector2(newRoom.location.x + multi, newRoom.location.y);
            }
            else if (door.exit == Exits.right)
            {
                tempdoor.exit = Exits.left;
                tempdoor.worldLocation = new Vector2(tempdoor.worldLocation.x + door.length, tempdoor.worldLocation.y);
                int yPos = Random.Range(3, roomY - 2);
                newRoom.location = new Vector2(ori.trans.position.x + door.location.x + door.length + 1f, ori.trans.position.y + door.location.y - yPos);
                float multi = Mathf.Abs((ori.trans.position.y % 1)) != Mathf.Abs((newRoom.location.y % 1)) ? 0.5f : 0f;
                newRoom.location = new Vector2(newRoom.location.x, newRoom.location.y + multi);
            }
            else
            {
                tempdoor.exit = Exits.right;
                tempdoor.worldLocation = new Vector2(tempdoor.worldLocation.x - door.length, tempdoor.worldLocation.y);
                int yPos = Random.Range(3, roomY - 2);
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

            AdjacentRoom(newRoom);
        }
    }

    void CreateDoors(Room ori, int roomX, int roomY, int numOfMinExits = 1)
    {
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
            a = 3;
            bool reserved = true;

            if (ori.doors.Count > 0)
            {
                while (reserved)
                {
                    a = 0;
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
                case Exits.top:
                    {
                        int exitX = Random.Range(2, roomX - exitSize - 1);
                        tempDoor.location = new Vector2(exitX + exitSize * 0.5f - 0.5f, roomY - 0.5f);
                        break;
                    }
                case Exits.bottom:
                    {
                        int exitX = Random.Range(2, roomX - exitSize - 1);
                        tempDoor.location = new Vector2(exitX + exitSize * 0.5f - 0.5f, 0.5f);
                        break;
                    }
                case Exits.right:
                    {
                        exitSize = exitSize < 4 ? 4 : exitSize;
                        int exitY = Random.Range(3, roomY - exitSize - 1);
                        tempDoor.location = new Vector2(roomX, exitY + exitSize * 0.5f - 0.5f);
                        break;
                    }
                case Exits.left:
                    {
                        exitSize = exitSize < 4 ? 4 : exitSize;
                        int exitY = Random.Range(3, roomY - exitSize - 1);
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

        Vector2 size = SpriteSizeInPixels(sprit.name);
        Vector2 scale = size / pixelsPerUnit;

        wall.name = sprit.name + "Cor";
        switch (sprit.name)
        {
            case "top":
            case "bottom":
                wall.transform.localPosition = sprit.name == "top" ? new Vector2(x, y) : new Vector2(x, y - SpritePositionAdjusted(size.y));
                sr.size = new Vector2(width, scale.y);
                break;
            case "left":
            case "right":
                sr.size = new Vector2(scale.x, height);
                wall.transform.localPosition = sprit.name == "left" ? new Vector2(x - SpritePositionAdjusted(size.x), y) : new Vector2(x + SpritePositionAdjusted(size.x), y);
                break;


        }
        AddWallSortingGroup(wall);
        AddCollisionBox(wall);
        CreateLOSBlocker(wall, (int)x, (int)y, new Vector2(width, height));
    }

    void AddWallSortingGroup(GameObject wall, int order = 0)
    {
        var sortingGroup = wall.AddComponent<SortingGroup>();
        if (wall.name == "top" || wall.name == "topLeft" || wall.name == "topRight" || wall.name == "bigCorner" || wall.name == "topCor" || wall.name == "topDoor" || wall.name == "topOther")
        {
            sortingGroup.sortingLayerName = "FloorObjects";
            sortingGroup.sortingOrder = wall.name != "bigCorner" ? order : 1;
        }
        else
        {
            sortingGroup.sortingLayerName = "Wall";
            sortingGroup.sortingOrder = order;
        }

    }
    void CreateRoomWall(Room ori, Sprite sprit, int sort = 1)
    {
        var wall = NewWall(ori, sprit);
        GameObject otherWall = null;
        var sr = wall.GetComponent<SpriteRenderer>();
        Vector2 size = SpriteSizeInPixels(sprit.name);
        Vector2 scale = size / pixelsPerUnit;
        var doorOnWall = ori.doors.Find(door => door.exit.ToString() == wall.name);

        if (doorOnWall == null)
        {
            switch (wall.name)
            {
                case "top":
                    wall.transform.localPosition = new Vector2(ori.maxX * 0.5f, ori.maxY);
                    sr.size = new Vector2(ori.maxX + size.x / pixelsPerUnit, scale.y);
                    CreateWallShadow(ori.trans, wall.transform.localPosition, sr.size);
                    break;
                case "bottom":
                    wall.transform.localPosition = new Vector2(ori.maxX * 0.5f, 0f - SpritePositionAdjusted(size.y));
                    sr.size = new Vector2(ori.maxX + size.x / pixelsPerUnit, scale.y);
                    break;
                case "left":
                    sr.size = new Vector2(scale.x, ori.maxY + 0.5f);
                    wall.transform.localPosition = new Vector2(0f - SpritePositionAdjusted(size.x), ori.maxY * 0.5f + 0.625f);
                    break;
                case "right":
                    sr.size = new Vector2(scale.x, ori.maxY + 0.5f);
                    wall.transform.localPosition = new Vector2(ori.maxX + SpritePositionAdjusted(size.x), ori.maxY * 0.5f + 0.625f);
                    break;
            }
        }
        else
        {

            switch (wall.name)
            {
                case "right":
                case "left":

                    float wallSize = doorOnWall.location.y - doorOnWall.size * 0.5f - 0.5f;
                    sr.size = new Vector2(scale.x, wallSize);
                    wall.transform.localPosition = wall.name == "right" ? new Vector2(ori.maxX + SpritePositionAdjusted(size.x), wallSize * 0.5f + 0.5f)
                                                                        : new Vector2(0f - SpritePositionAdjusted(size.x), wallSize * 0.5f + 0.5f);
                    wallSize = ori.maxY - (doorOnWall.location.y + doorOnWall.size * 0.5f) + 0.75f;
                    otherWall = NewWall(ori, sprit, "Other");
                    var otherSr = otherWall.GetComponent<SpriteRenderer>();
                    otherSr.size = new Vector2(scale.x, wallSize);
                    otherWall.transform.localPosition = wall.name == "right" ? new Vector2(ori.maxX + SpritePositionAdjusted(size.x), ori.maxY - wallSize * 0.5f + 0.75f)
                                                                                : new Vector2(0f - SpritePositionAdjusted(size.x), ori.maxY - wallSize * 0.5f + 0.75f);
                    break;
                case "bottom":
                case "top":
                    wallSize = doorOnWall.location.x - doorOnWall.size * 0.5f + 0.5f;
                    sr.size = new Vector2(wallSize, scale.y);
                    wall.transform.localPosition = wall.name == "top" ? new Vector2(wallSize * 0.5f - 0.5f, ori.maxY - SpritePositionAdjusted(size.y) - 0.5f)
                                                                      : new Vector2(wallSize * 0.5f - 0.5f, 0f - SpritePositionAdjusted(size.y));
                    if (wall.name == "top")
                        CreateWallShadow(ori.trans, new Vector2(wall.transform.localPosition.x + 0.138f,  wall.transform.localPosition.y), new Vector2(wallSize - 0.95f, sr.size.y));

                    wallSize = ori.maxX - (doorOnWall.location.x + doorOnWall.size * 0.5f) + 0.5f;
                    otherWall = NewWall(ori, sprit, "Other");
                    otherSr = otherWall.GetComponent<SpriteRenderer>();
                    otherSr.size = new Vector2(wallSize, scale.y);
                    otherWall.transform.localPosition = wall.name == "top" ? new Vector2(ori.maxX - wallSize * 0.5f + 0.5f, ori.maxY - SpritePositionAdjusted(size.y) - 0.5f)
                                                                           : new Vector2(ori.maxX - wallSize * 0.5f + 0.5f, 0f - SpritePositionAdjusted(size.y));
                    if (wall.name == "top")
                        CreateWallShadow(ori.trans, new Vector2(otherWall.transform.localPosition.x - 0.138f, otherWall.transform.localPosition.y), new Vector2(wallSize - 0.95f, otherSr.size.y));
                    break;
            }
            wall.name += "Door";
        }
        AddWallSortingGroup(wall);
        AddCollisionBox(wall);
        CreateLOSBlocker(wall, ori.maxX, ori.maxY);
        if (otherWall != null)
        {
            AddWallSortingGroup(otherWall);
            AddCollisionBox(otherWall);
            CreateLOSBlocker(otherWall, ori.maxX, ori.maxY);
        }
    }

    private void CreateLOSBlocker(GameObject wall, int maxX, int maxY, Vector2? doorSize = null)
    {
        Vector2 door = doorSize ?? new Vector2(0f, 0f);
        if (wall.name == "top" || wall.name == "topCor" || wall.name == "topOther" || wall.name == "topDoor") return;
        var losBlock = Instantiate(losBlocker);
        var lsr = losBlock.GetComponent<SpriteRenderer>();
        var wallSr = wall.GetComponent<SpriteRenderer>();
        losBlock.transform.SetParent(wall.transform.parent, false);
        lsr.GetComponent<SortingGroup>().enabled = true;

        switch (wall.name)
        {
            case "bottom":
            case "bottomDoor":
            case "bottomOther":
            case "bottomCor":
                losBlock.transform.localPosition = wall.name == "bottomCor" ? new Vector2(wall.transform.localPosition.x, maxY - door.y)
                                                                            : new Vector2(wall.transform.localPosition.x, -1f);
                lsr.size = new Vector2(wallSr.size.x, 1f);
                break;
            case "right":
            case "left":
                losBlock.transform.localPosition = wall.name == "right" ? new Vector2(maxX + 1f, wall.transform.localPosition.y - 1f)
                                                                        : new Vector2(-1f, wall.transform.localPosition.y - 1f);
                lsr.size = new Vector2(1f, wallSr.size.y + 1f);
                break;

            case "rightOther":
            case "leftOther":
                losBlock.transform.localPosition = wall.name == "rightOther"
                                                    ? new Vector2(maxX + 1f, wall.transform.localPosition.y)
                                                    : new Vector2(-1f, wall.transform.localPosition.y);
                lsr.size = new Vector2(1f, wallSr.size.y);
                break;
            case "leftDoor":
            case "rightDoor":
                losBlock.transform.localPosition = wall.name == "rightDoor"
                                                   ? new Vector2(maxX + 1f, wall.transform.localPosition.y - 1.5f)
                                                   : new Vector2(-1f, wall.transform.localPosition.y - 1.5f);
                lsr.size = new Vector2(1f, wallSr.size.y + 1f);
                break;
            case "rightCor":
            case "leftCor":
                losBlock.transform.localPosition = wall.name == "rightCor"
                                                  ? new Vector2(maxX + 1f, wall.transform.localPosition.y - 0.5f)
                                                  : new Vector2(maxX - door.x, wall.transform.localPosition.y - 0.5f);
                lsr.size = new Vector2(1f, wallSr.size.y - 1f);
                break;
        }
    }
    private void CreateWallShadow(Transform parent, Vector2 ori, Vector2 size)
    {
        var shadow = Instantiate(wallShadow);
        shadow.transform.SetParent(parent, false);
        shadow.transform.localPosition = new Vector2(ori.x, ori.y - 0.93f);
        var sr = shadow.GetComponent<SpriteRenderer>();
        sr.size = new Vector2(size.x + 1f, 1f);
        sr.sortingOrder = 10;

    }

    GameObject NewWall(Room ori, Sprite sprit, string _name = "", int sort = 1)
    {
        GameObject wall = new GameObject();
        wall.transform.parent = ori.trans;

        var sr = wall.AddRenderer(material);
        sr.sortingOrder = sort;

        sr.drawMode = SpriteDrawMode.Tiled;
        sr.sprite = sprit;

        wall.name = sprit.name + _name;
        return wall;
    }
    void CreateCornerPiece(Room ori, float x, float y, Sprite sprit, int sort = 3, float rot = 0, float xSwap = 1f)
    {
        GameObject corner = new GameObject();
        corner.transform.parent = ori.trans;
        corner.name = sprit.name;
        var sr = corner.AddRenderer(material);
        sr.sortingOrder = sort;
        sr.sprite = sprit;
        corner.transform.localPosition = new Vector2(x, y);
        corner.transform.localScale = new Vector3(xSwap, 1f, 1f);
        corner.transform.eulerAngles = new Vector3(0f, 0f, rot);
        AddWallSortingGroup(corner, 10);
        if (corner.name != "bottomLeft" && corner.name != "bottomRight" && corner.name != "corner") AddCollisionBox(corner);
    }

    void CreateRoomFloor(Room ori, Sprite sprit, int sort = 0)
    {
        var floor = new GameObject();
        floor.transform.parent = ori.trans;
        var fsr = floor.AddRenderer(material);
        fsr.sprite = sprit;
        fsr.drawMode = SpriteDrawMode.Tiled;
        fsr.size = new Vector2(ori.maxX + 1f, ori.maxY + 1f);
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
        var sr = Piece(current, name);
        return new Vector2(sr.bounds.size.x * pixelsPerUnit, sr.bounds.size.y * pixelsPerUnit);
    }

    private float SpritePositionAdjusted(float pos)
    {
        return (1f - unitsPerPixel * pos) / 2f;
    }
    Exits Opposite(Exits exit)
    {
        if (exit == Exits.right)
            return Exits.left;
        if (exit == Exits.left)
            return Exits.right;
        if (exit == Exits.top)
            return Exits.bottom;
        if (exit == Exits.bottom)
            return Exits.top;

        return Exits.top;
    }
    private void AddCollisionBox(GameObject wall)
    {
        string name = wall.name;
        var colbox = wall.AddComponent<BoxCollider2D>();
        var sr = wall.GetComponent<SpriteRenderer>();
        switch (name)
        {
            case "left":
            case "right":
                colbox.size = new Vector2(sr.size.x, sr.size.y + 3f);
                colbox.offset = new Vector2(colbox.offset.x, colbox.offset.y - 1f);
                break;
            case "leftCor":
            case "rightCor":
                colbox.size = new Vector2(sr.size.x, sr.size.y);
                colbox.offset = new Vector2(colbox.offset.x, colbox.offset.y - 0.529f);
                break;
            case "rightDoor":
            case "leftDoor":
                colbox.size = new Vector2(sr.size.x, sr.size.y + 1.5f);
                colbox.offset = new Vector2(colbox.offset.x, colbox.offset.y - 1.279f);
                break;
            case "rightOther":
            case "leftOther":
                colbox.size = new Vector2(sr.size.x, sr.size.y);
                break;

            case "bottom":
            case "bottomCor":
            case "bottomDoor":
            case "bottomOther":
                colbox.size = new Vector2(sr.size.x, sr.size.y);
                colbox.offset = new Vector2(colbox.offset.x, colbox.offset.y - 0.7f);
                break;
            case "top":
            case "topCor":
            case "topLeft":
            case "topRight":
            case "bigCorner":
            case "topDoor":
            case "topOther":
                colbox.size = new Vector2(sr.size.x, sr.size.y);
                colbox.offset = new Vector2(colbox.offset.x, colbox.offset.y + 0.05f);
                break;

        }
    }

    void InitializeAssets()
    {
        wallShadow = new GameObject();
        var wsr = wallShadow.AddComponent<SpriteRenderer>();
        wsr.sprite = wallShadowSprite;
        wsr.color = new Color(0f, 0f, 0f, 0.7f);
        wsr.drawMode = SpriteDrawMode.Tiled;
        Destroy(wallShadow);

        losBlocker = new GameObject();
        var lsr = losBlocker.AddComponent<SpriteRenderer>();
        var lsrSort = losBlocker.AddComponent<SortingGroup>();
        lsr.sprite = losBlockerSprite;
        lsr.color = Color.black;
        lsr.drawMode = SpriteDrawMode.Tiled;
        lsrSort.sortingLayerName = "LOSBlocker";
        Destroy(losBlocker);
    }
}
