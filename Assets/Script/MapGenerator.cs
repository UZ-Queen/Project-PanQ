using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;




public class MapGenerator : MonoBehaviour
{
    [SerializeField] int seed = 1000;
    [SerializeField] Vector2Int mapSize = new Vector2Int(10,10);

    [SerializeField] int obstacleCount = 10;
    [SerializeField][Range(0f,1f)] float obstaclePercent = 0.2f; 
    [SerializeField] Transform tilePrefap;
    [SerializeField] Transform obstaclePrefap;

    [SerializeField][Range(0.01f,1f)] float tilePercent = 0.9f;


    List<Coord> allTileCoords;
    Queue<Coord> randomCoords;
    void Start()
    {
        GenerateMap();
    }


    public void GenerateMap(){
        
        //Destroy and Update New Map
        string mapHolderName = "이부키";
        Transform mapHolder = transform.Find(mapHolderName);
        if(mapHolder){
            DestroyImmediate(mapHolder.gameObject);
        }
        mapHolder = new GameObject(mapHolderName).transform;
        mapHolder.SetParent(transform);

        //Generate Tile Coords
        allTileCoords = new List<Coord>();
        for(int x=0; x<mapSize.x; x++){
            for(int y=0;y<mapSize.y; y++){
                allTileCoords.Add(new Coord(x,y));
            }
        }
        randomCoords = new Queue<Coord>(Utilities.ShuffleArray(allTileCoords.ToArray(), seed));



        //Generate Tiles
        for(int x=0; x<mapSize.x; x++){
            for(int y=0;y<mapSize.y; y++){
                if(tilePrefap == null)
                    return;
                Vector3 tilePosition = CoordToVector3(x,y);
                Transform newTile = Instantiate(tilePrefap, tilePosition, Quaternion.identity, mapHolder); 
                newTile.localScale *= tilePercent;
            }
        }


        //Generate Obstacles
        bool[,] mapFlags = new bool[mapSize.x, mapSize.y];
        
        int obstacleCount = (int)(TileCount * obstaclePercent);
        

        for(int i=0; i<obstacleCount; i++){

            if(obstaclePrefap == null)
                break;
            Coord coord = GetRandomCoord();
            Transform newObstacle = Instantiate(obstaclePrefap, CoordToVector3(coord), Quaternion.identity, mapHolder );
            newObstacle.Translate(Vector3.up * transform.localScale.y * 0.5f);
            newObstacle.localScale *= tilePercent;

        }
    }

    /*
    맵플래그: 장애물, 또는 이미 체크한 타일.
    맵 중앙 타일부터 시작해서 인접한 타일(위, 아래, 왼, 오)을 큐에 넣고 반복함.

    */
    bool IsMapFullyAccessible(bool[,] mapFlags, int accessibleTileCount, int obstacleCount){

        Coord coordToCheck = MapCenter;
        Queue<Coord> nextCoordsToCheck = new Queue<Coord>();

        nextCoordsToCheck.Enqueue(MapCenter);
        int currentAccessibleTileCount = 0;
        while(nextCoordsToCheck.Count >0){
                currentAccessibleTileCount++;
                for(int x = -1 ; x <= 1; x++){
                    for(int y = -1 ; y <= 1; y++){
                        Vector2Int tilePosition = new Vector2Int(x + coordToCheck.x,y + coordToCheck.y);
                        /*
                         xox
                         oxo   |x| == |y|인 경우 검사하지 않음.
                         xox     
                        */
                        if( Mathf.Abs(x) == Mathf.Abs(y))
                            continue;
                        //인덱스 밖을 만지는 경우 검사하지 않음.
                        if( tilePosition.x < 0 || tilePosition.x >= mapSize.x) 
                            continue;
                        if( tilePosition.y < 0 || tilePosition.y >= mapSize.y)
                            continue;
                        //맵플래그 상 트루인 경우(이미 검사했거나 장애물인 경우) 검사하지 않음.
                        if(mapFlags[tilePosition.x, tilePosition.y]) 
                            continue;
                        
                        

                    }
                }
        }


        return true;
    }

    public int TileCount {get{return mapSize.x * mapSize.y;}}
    
    public Coord MapCenter{get{return new Coord(mapSize.x /2, mapSize.y / 2);}}
    Coord GetRandomCoord(){
        Coord coord = randomCoords.Dequeue();
        randomCoords.Enqueue(coord);

        return coord;
    }

    Vector3 CoordToVector3(Coord coord){
        return CoordToVector3(coord.x, coord.y);
    }

    Vector3 CoordToVector3(int x, int y){
        Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x,0,-mapSize.y / 2 + 0.5f + y);

        return tilePosition;
    }


    


}

public struct Coord{

    public int x;
    public int y;
    public Coord(int x, int y){
        this.x = x;
        this.y = y;

    }

    // public Coord(Vector2Int vector){
    //     Coord(vector.x, vector.y);
    // }

}
