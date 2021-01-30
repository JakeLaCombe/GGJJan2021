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
    public int PlacementTryMax;
    public GameObject questObject;
    public MonoBehaviour gameManager;

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
        AddQuestItem();

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

    private void AddQuestItem()
    {
        GameObject quest = Object.Instantiate(this.questObject);
        quest.transform.position = new Vector3(5F, 1F);

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
}
struct Chunk
{
    public int[,] Tiles;
    public int X;
    public int Y;
}
