using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Security.Cryptography;

//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;




public class MapGenerator : MonoBehaviour
{

    public Map[] maps;
    Map currentMap;
    [SerializeField] int mapIndex = 0;
    [SerializeField] Transform tilePrefap;
    [SerializeField] Transform obstaclePrefap;

    [SerializeField][Range(0.01f, 1f)] float tilePercent = 0.9f;
    [SerializeField][Range(0.01f, 3f)] float tileSize = 1f;


    [SerializeField] Vector2Int maxMapSize = new Vector2Int(10, 10);
    [SerializeField] Transform navMeshPlane;
    [SerializeField] Transform worldBorder;

    [SerializeField] DeadZone deadZonePrefap;

    //[SerializeField] BoxCollider floor;
    Transform[,] tileMap;
    List<Coord> allTileCoords;
    Queue<Coord> randomCoords;
    Queue<Coord> randomOpenCoords;


    void OnNewWave(int waveIndex){
        mapIndex = waveIndex % maps.Length; //  길이 3일 경우 0,1,2,0,1,2... 
        GenerateMap();

    }
    void Awake()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        GenerateMap();
    }


    public void GenerateMap()
    {
        currentMap = maps[mapIndex];

        // Set navMesh walkable Floor
        navMeshPlane.localScale = new Vector3(maxMapSize.x, 0, maxMapSize.y) * tileSize;

        // 바닥 피하라고~
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x, -0.1f, currentMap.mapSize.y);

        
        

        //Destroy and Update a new map
        string mapHolderName = "이부키";
        Transform mapHolder = transform.Find(mapHolderName);
        if (mapHolder)
        {
            DestroyImmediate(mapHolder.gameObject);
        }
        mapHolder = new GameObject(mapHolderName).transform;
        mapHolder.SetParent(transform);

        // 데드존(마크의 공허 느낌) 구현. 월드 밖으로 떨어질 경우 사망
        if(deadZonePrefap != null){            
            Transform deadZone = Instantiate(deadZonePrefap, Vector3.down * 5f, Quaternion.identity, mapHolder).transform;
            deadZone.GetComponent<BoxCollider>().size = new Vector3(1000,0,1000);
        }

        //Generate Tile Coords
        allTileCoords = new List<Coord>();
        
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        randomCoords = new Queue<Coord>(Utilities.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));


        //Generate Tiles
        tileMap = new Transform[currentMap.mapSize.x,currentMap.mapSize.y];
        

        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                if (tilePrefap == null)
                    return;
                Vector3 tilePosition = CoordToVector3(x, y);
                Transform newTile = Instantiate(tilePrefap, tilePosition, Quaternion.identity, mapHolder);
                newTile.localScale *= tilePercent * tileSize;
                tileMap[x,y] = newTile;
                
            }
        }

        List<Coord> allOpenTileCoords = new List<Coord>(allTileCoords);
        //Generate Obstacles
        
        //맵플래그: 장애물, 또는 이미 체크한 타일.
        bool[,] obstacleFlags = new bool[currentMap.mapSize.x, currentMap.mapSize.y];

        int targetObstacleCount = (int)(currentMap.TileCount * currentMap.obstaclePercent);
        int obstacleCount = 0;
        for (int i = 0; i < targetObstacleCount; i++)
        {
            if (obstaclePrefap == null)
                break;

            Coord coord = GetRandomCoord();
            if (coord == currentMap.MapCenter)
                continue;

            obstacleCount++;
            obstacleFlags[coord.x, coord.y] = true;

            if (IsMapFullyAccessible(obstacleFlags, obstacleCount))
            {
                Transform newObstacle = Instantiate(obstaclePrefap, CoordToVector3(coord), Quaternion.identity, mapHolder);
                newObstacle.localScale *= tilePercent * tileSize;
                newObstacle.Translate(Vector3.up * transform.localScale.y * 0.5f);

                allOpenTileCoords.Remove(coord);
            }
            else
            {
                obstacleCount--;
                obstacleFlags[coord.x, coord.y] = false;
            }
        }

        // Generate Shuffled Open Coords
        randomOpenCoords = new Queue<Coord>(Utilities.ShuffleArray(allOpenTileCoords.ToArray(), currentMap.seed));




        // Generate World Borders(아우 드러워!)

        Vector3 worldBorderPosition = new Vector3(0, 0, (Mathf.Ceil(currentMap.mapSize.y * 0.5f) + 0.5f) * tileSize);
        Transform worldBorderN = Instantiate(worldBorder, worldBorderPosition, Quaternion.identity, mapHolder);

        worldBorderN.localScale = new Vector3(currentMap.mapSize.x * 2, 10, 1) * tileSize;

        worldBorderPosition = new Vector3(0, 0, (Mathf.Ceil(-currentMap.mapSize.y * 0.5f - 1) + 0.5f) * tileSize);
        Transform worldBorderS = Instantiate(worldBorder, worldBorderPosition, Quaternion.identity, mapHolder);
        worldBorderS.localScale = new Vector3(currentMap.mapSize.x * 2, 10, 1) * tileSize;

        worldBorderPosition = new Vector3((Mathf.Ceil(currentMap.mapSize.x * 0.5f) + 0.5f) * tileSize, 0, 0);
        Transform worldBorderE = Instantiate(worldBorder, worldBorderPosition, Quaternion.identity, mapHolder);
        worldBorderE.localScale = new Vector3(1, 10, currentMap.mapSize.y * 2) * tileSize;

        worldBorderPosition = new Vector3((Mathf.Ceil(-currentMap.mapSize.x * 0.5f) + 0.5f - 1) * tileSize, 0, 0);
        Transform worldBorderW = Instantiate(worldBorder, worldBorderPosition, Quaternion.identity, mapHolder);
        worldBorderW.localScale = new Vector3(1, 10, currentMap.mapSize.y * 2) * tileSize;



    }

    /*
    
    맵 중앙 타일부터 시작해서 인접한 타일(위, 아래, 왼, 오)을 큐에 넣고 반복함.
    */
    bool IsMapFullyAccessible(bool[,] obstacleFlags, int obstacleCount)
    {

        bool[,] mapFlags = new bool[currentMap.mapSize.x, currentMap.mapSize.y];
        Coord coordToCheck;
        Queue<Coord> nextCoordsToCheck = new Queue<Coord>();

        int currentAccessibleTileCount = 1;
        nextCoordsToCheck.Enqueue(currentMap.MapCenter);

        mapFlags[currentMap.MapCenter.x, currentMap.MapCenter.y] = true;
        while (nextCoordsToCheck.Count > 0)
        {
            coordToCheck = nextCoordsToCheck.Dequeue();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int tilePosition = new Vector2Int(x + coordToCheck.x, y + coordToCheck.y);
                    /*
                     xox
                     oxo   |x| == |y|인 경우 검사하지 않음.
                     xox     
                    */
                    if (Mathf.Abs(x) == Mathf.Abs(y))
                        continue;
                    //인덱스 밖을 만지는 경우 검사하지 않음.
                    if (tilePosition.x < 0 || tilePosition.x >= currentMap.mapSize.x)
                        continue;
                    if (tilePosition.y < 0 || tilePosition.y >= currentMap.mapSize.y)
                        continue;
                    //이미 검사했거나 장애물인 경우 검사하지 않음.
                    if (obstacleFlags[tilePosition.x, tilePosition.y] || mapFlags[tilePosition.x, tilePosition.y])
                        continue;

                    nextCoordsToCheck.Enqueue(new Coord(tilePosition.x, tilePosition.y));
                    mapFlags[tilePosition.x, tilePosition.y] = true;
                    currentAccessibleTileCount++;

                }
            }
        }

        if (obstacleCount + currentAccessibleTileCount == currentMap.TileCount)
            return true;
        else
            return false;

    }


    Coord GetRandomCoord()
    {
        Coord coord = randomCoords.Dequeue();
        randomCoords.Enqueue(coord);

        return coord;
    }

    public Transform GetRandomOpenTile(){
        Coord coord = randomOpenCoords.Dequeue();
        randomOpenCoords.Enqueue(coord);
        return CoordToTile(coord).GetChild(0);

    }

    public Transform PositionToTile(Vector3 position){
        position /= tileSize;
        Coord coord = new Coord(Mathf.RoundToInt(position.x - 0.5f + currentMap.mapSize.x / 2), Mathf.RoundToInt(position.z - 0.5f + currentMap.mapSize.y / 2));
        return tileMap[ Mathf.Clamp(coord.x, 0, currentMap.mapSize.x-1), Mathf.Clamp(coord.y, 0, currentMap.mapSize.y -1 )].GetChild(0);
    }

    //Tileposition.x = ( -mapsize.x / 2 + 0.5f + Coord.x ) * tileSize
    public Vector3 CoordToVector3(Coord coord)
    {
        return CoordToVector3(coord.x, coord.y);
    }
    public Vector3 CoordToVector3(int x, int y)
    {
        Vector3 tilePosition = new Vector3(-currentMap.mapSize.x / 2 + 0.5f + x, 0, -currentMap.mapSize.y / 2 + 0.5f + y);

        return tilePosition * tileSize;
    }
    Transform CoordToTile(Coord coord){
        return tileMap[coord.x, coord.y];
    }
    Transform CoordToTile(int x, int y){
        return CoordToTile(new Coord(x,y));
    }
}

public struct Coord
{

    public int x;
    public int y;
    public Coord(int x, int y)
    {
        this.x = x;
        this.y = y;

    }

    public static bool operator ==(Coord a, Coord b)
    {
        if (a.x == b.x && a.y == b.y)
            return true;
        else
            return false;
    }

    public static bool operator !=(Coord a, Coord b)
    {
        return !(a == b);
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    // public Coord(Vector2Int vector){
    //     Coord(vector.x, vector.y);
    // }

}


[Serializable]
public class Map
{

    
    public Vector2Int mapSize;
    
    [Range(0,1)]
    public float obstaclePercent;
    public float minObstalceHeight;
    public float maxObstacleHeight;
    public Color foreColor;
    public Color backColor;

    public int seed;

    public int TileCount
    {
        get
        {
            return mapSize.x * mapSize.y;
        }
    }

    public Coord MapCenter { get { return new Coord(mapSize.x / 2, mapSize.y / 2); } }
}