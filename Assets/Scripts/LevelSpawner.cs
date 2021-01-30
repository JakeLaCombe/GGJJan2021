using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelSpawner : MonoBehaviour
{

    public Vector2 GameSize;
    private Tilemap tilemap;
    public TileBase Tile;
    public Vector2 platformSize;
    public int PlatformGenerateCount;
    public int PlacementTryMax = 10;

    public GameObject questObject;
    public MonoBehaviour gameManager;


    public int CountToSpawn;
    public GameObject Spawner;

    // Start is called before the first frame update
    void Start()
    {


        tilemap = GetComponent<Tilemap>();
        ChangeMap();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ChangeMap()
    {
        int[,] Tiles = new int[(int)GameSize.x + 1, (int)GameSize.y + 1];

        List<Chunk> platforms = new List<Chunk>();
        for (int i = 0; i < PlatformGenerateCount; i++)
        {
            Chunk platform = new Chunk
            {
                X = Random.Range(1, (int)platformSize.x),
                Y = Random.Range(1, (int)platformSize.y),
            };
            platform.Tiles = new int[platform.X, platform.Y];



            for (int x = 0; x < platform.X; x++)
            {
                for (int y = 0; y < platform.Y; y++)
                {
                    if (Random.value * 100 > 37)
                    {

                        platform.Tiles[x, y] = 1;
                    }
                    else
                    {
                        platform.Tiles[x, y] = 0;
                    }
                }

            }
            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < platform.X; x++)
                {
                    for (int y = 0; y < platform.Y; y++)
                    {
                        int count = checkNeighbors(Tiles, x, y);
                        if (count > 6)
                        {
                            platform.Tiles[x, y] = 0;
                        }
                        else if (count < 5)
                        {
                            platform.Tiles[x, y] = 1;
                        }

                    }

                }

            }
            platforms.Add(platform);
        }
        // Debug.Log("made rooms");

        foreach (Chunk platform in platforms)
        {
            for (int i = 0; i < PlacementTryMax; i++)
            {
                int sizeX = Random.Range(0, (int)GameSize.x - platform.X);
                int sizeY = Random.Range(0, (int)GameSize.y - platform.Y);
                bool oops = false;
                for (int x = 0; x < platform.X; x++)
                {
                    for (int y = 0; y < platform.Y; y++)
                    {
                        //check for other platforms first;
                        if (Tiles[sizeX + x, sizeY + y] == platform.Tiles[x, y])
                        {
                            oops = true;
                            break;
                        }
                    }
                    if (oops)
                    {
                        break;
                    }
                }
                if (oops)
                {
                    break;
                }
                for (int x = 0; x < platform.X; x++)
                {
                    for (int y = 0; y < platform.Y; y++)
                    {
                        //check for other platforms first;
                        Tiles[sizeX + x, sizeY + y] = platform.Tiles[x, y];




                    }
                }
                // Debug.Log("Placed Room");
                break;
            }

        }

        //Spawn some stuff;

        AddQuestItem(Tiles, (vec) =>
        {
            // Offsets that Bryce told me to use
            return new Vector2(vec.x + 1F, vec.y + 1.5F);
        });

        while (CountToSpawn > 0)
        {
            int sizeX = Random.Range(1, (int)GameSize.x - 1);
            int sizeY = Random.Range(1, (int)GameSize.y - 1);

            if (Tiles[sizeX, sizeY] == 1 && FindOnlyOneCardinalNeighbor(Tiles, sizeX, sizeY, out Vector2 Direction))
            {
                Direction *= -1;
                GameObject spawn = Instantiate(Spawner, new Vector3(sizeX, sizeY), transform.rotation);
                spawn.GetComponent<SpitterLogic>().SpitDirection = Direction;
                CountToSpawn--;

            }
        }


        

        for (int x = 0; x < GameSize.x; x++)
        {
            for (int y = 0; y < GameSize.y; y++)
            {
                if (Tiles[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x - 1, y - 1, 0), Tile);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x - 1, y - 1, 0), null);
                }

            }
        }


    }

    private void AddQuestItem(int[,] tiles, System.Func<Vector2Int, Vector2> tileToPositionTranslator)
    {
        // Find a good spot to put it

        System.Func<Vector2Int, bool> isTileFilled = (spot) =>
        {
            return tiles[spot.x, spot.y] != 0;
        };

        System.Func<Vector2Int, bool> isGoodSpot = (candidateMiddleFloor) =>
        {
            if (candidateMiddleFloor.y < 0 /* below the bottom */ ||
                candidateMiddleFloor.y + 1 > tiles.GetLength(1) /* above the top */ ||
                candidateMiddleFloor.x - 1 < 0 /* left of the leftmost range */ ||
                candidateMiddleFloor.x + 1 > tiles.GetLength(0) /* right of rightmost */)
            {
                return false;  // Out of bounds of the tilemap
            }

            // Algorithm: find at least 3 blocks in a row with empty space above it and put the item there
            //
            //  *
            // ###

            return
            isTileFilled(candidateMiddleFloor + Vector2Int.left) &&
            isTileFilled(candidateMiddleFloor) &&
            isTileFilled(candidateMiddleFloor + Vector2Int.right) &&
            !(isTileFilled(candidateMiddleFloor + Vector2Int.left + Vector2Int.up)) &&
            !(isTileFilled(candidateMiddleFloor + Vector2Int.up)) &&
            !(isTileFilled(candidateMiddleFloor + Vector2Int.right + Vector2Int.up));
        };

        System.Func<Vector2Int> positionPicker = () =>
        {
            // Look at minimum 5 tiles above the ground, so start there
            int minimumTilesAboveGround = 5;

            for (int y = minimumTilesAboveGround; y < tiles.GetLength(1); ++y)
            {
                for (int x = 1; x < tiles.GetLength(0); ++x)
                {
                    if (isGoodSpot(new Vector2Int(x, y)))
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }

            return new Vector2Int(0, 0);  // Oh no!
        };

        Vector2Int foundTilePosition = positionPicker();

        Vector2 foundWorldPosition = tileToPositionTranslator(foundTilePosition);

        // Add it to the scene
        GameObject quest = Object.Instantiate(this.questObject);
        quest.transform.position = new Vector3(foundWorldPosition.x, foundWorldPosition.y);

        CircleCollider2D collider = quest.AddComponent<CircleCollider2D>();
        collider.radius = 1F;
        collider.transform.position = quest.transform.position;
        collider.isTrigger = true;

        QuestPickupBehaviour behaviour = quest.AddComponent<QuestPickupBehaviour>();
        behaviour.ItemPickedUp += () =>
        {
            ((GameManager)this.gameManager).SetQuestItemPickedUp();
            Object.Destroy(quest);
        };
    }

    private class QuestPickupBehaviour : MonoBehaviour
    {
        public event System.Action ItemPickedUp;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("OnTriggerEnter2D MGN");

            if (this.ItemPickedUp != null)
            {
                this.ItemPickedUp();
            }
        }
    }

    // How many spaces around it are empty
    private int checkNeighbors(int[,] tiles, int x, int y)
    {
        int count = 0;
        for (int xOffest = -1; xOffest < 2; xOffest++)
        {
            for (int yOffset = -1; yOffset < 2; yOffset++)
            {
                if (xOffest == 0 && yOffset == 0)
                {
                    continue;
                }
                if (x + xOffest < 0 || y + yOffset < 0)
                {
                    count++;
                    continue;
                }

                if (tiles[x + xOffest, y + yOffset] == 1)
                {
                    count++;
                }
            }
        }
        return count;
    }
    private bool FindOnlyOneCardinalNeighbor(int[,] tiles, int x, int y, out Vector2 direction)
    {
        int count = 0;

        if (tiles[x - 1, y] == 1)
        {
            count++;
            direction = new Vector2(-1, 0);
        }
        if (tiles[x + 1, y] == 1)
        {
            count++;
            direction = new Vector2(1, 0);
        }
        if (tiles[x, y - 1] == 1)
        {
            count++;
            direction = new Vector2(0, -1);
        }
        if (tiles[x, y + 1] == 1)
        {
            count++;
            direction = new Vector2(0, 1);
        }
        else direction = Vector2.zero;
        if (count == 1)
        {
            return true;
        }
        else
        {
            return false;

        }
    }
}
struct Chunk
{
    public int[,] Tiles;
    public int X;
    public int Y;
}
