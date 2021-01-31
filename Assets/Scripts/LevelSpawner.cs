using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelSpawner : MonoBehaviour
{

    public Vector2 GameSize;
    private Tilemap tilemap;
    public TileBase Tile;
    public TileBase Vending;
    public Vector2 platformSize;
    public int PlatformGenerateCount;
    public int PlacementTryMax = 10;

    public GameObject[] questObjectPossibilities;
    public GameObject questGiverSpeechBubble;
    public GameObject homeBase;  // The spot to return your found item from your quest
    public MonoBehaviour gameManager;
    public AudioSource backgroundMusic;
    public AudioClip[] backgroundMusicPossibilities;
    private int backgroundMusicPossibilityCurrentlyPlayingIndex = -1;  // -1 = not playing

    public int CountToSpawn;
    public List<GameObject> Spawners;

    List<GameObject> spawnedobjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {


        tilemap = GetComponent<Tilemap>();

        GameObject questObjectPrefab = questObjectPossibilities[Random.Range(0, questObjectPossibilities.Length)];

        ChangeMap(questObjectPrefab);
        SetUpHomeBaseToReturnItem(homeBase, (GameManager)gameManager);
        GiveQuest(this.questGiverSpeechBubble, questObjectPrefab);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetUpHomeBaseToReturnItem(GameObject homeBase, GameManager gameManager)
    {
        //BoxCollider2D collider = homeBase.GetComponent<BoxCollider2D>();

        HomeBaseBehaviour behaviour = homeBase.AddComponent<HomeBaseBehaviour>();
        behaviour.HomeBaseEntered += () =>
        {
            bool result = gameManager.TryReturnHeldQuestItem();

            if (result)
            {
                GameObject questObjectPrefab = questObjectPossibilities[Random.Range(0, questObjectPossibilities.Length)];
                ChangeMap(questObjectPrefab);
                GiveQuest(this.questGiverSpeechBubble, questObjectPrefab);
            }
        };
    }

    private class HomeBaseBehaviour : MonoBehaviour
    {
        public event System.Action HomeBaseEntered;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (this.HomeBaseEntered != null)
            {
                this.HomeBaseEntered();
            }
        }
    }

    public void ChangeMap(GameObject questObjectPrefab)
    {
        for (int i = 0; i < spawnedobjects.Count; i++)
        {
            Destroy(spawnedobjects[i].gameObject);
        }
        spawnedobjects.Clear();
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
        }, questObjectPrefab, (GameManager)this.gameManager);


        int lefttoSpawn = CountToSpawn;
        while (lefttoSpawn > 0)
        {
            int sizeX = Random.Range(1, (int)GameSize.x - 1);
            int sizeY = Random.Range(1, (int)GameSize.y - 1);

            if (Random.value >= .35f)
            {
                GameObject Spitter = Spawners.Find(x => x.name == "Spitter");
                if ((Tiles[sizeX, sizeY] == 1 || Tiles[sizeX, sizeY] == 0) && FindOnlyOneCardinalNeighbor(Tiles, sizeX, sizeY, out Vector2 Direction))
                {
                    Direction *= -1;
                    GameObject spawn = Instantiate(Spitter, new Vector3(sizeX + 1f + Direction.x * 0.75f, sizeY + .5f + Direction.y * 0.75f), transform.rotation);
                    spawn.GetComponent<SpitterLogic>().SpitDirection = Direction;
                    Tiles[sizeX, sizeY] = 2;
                    lefttoSpawn--;
                    spawnedobjects.Add(spawn);
                }
            }
            else
            {
                GameObject Confusion = Spawners.Find(x => x.name == "Confusion");
                if (Tiles[sizeX, sizeY] == 0 && Tiles[sizeX + 1, sizeY] == 0 && Tiles[sizeX, sizeY + 1] == 0 && Tiles[sizeX + 1, sizeY + 1] == 0)
                {
                    GameObject spawn = Instantiate(Confusion, new Vector3(sizeX + 1.5f, sizeY + 1f), transform.rotation);
                    Tiles[sizeX, sizeY] = -1;
                    Tiles[sizeX + 1, sizeY] = -1;
                    Tiles[sizeX, sizeY + 1] = -1;
                    Tiles[sizeX + 1, sizeY + 1] = -1;
                    lefttoSpawn--;
                    spawnedobjects.Add(spawn);
                }



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
                else if (Tiles[x, y] == 2)
                {
                    tilemap.SetTile(new Vector3Int(x - 1, y - 1, 0), Vending);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x - 1, y - 1, 0), null);
                }

            }
        }


    }

    private void GiveQuest(GameObject questGiverSpeechBubble, GameObject questObjectPrefab)
    {
        QuestItem itemScript = questObjectPrefab.GetComponent<QuestItem>();

        // Clear out any previous objects inside the speech bubble
        QuestItem previousItem = questGiverSpeechBubble.GetComponentInChildren<QuestItem>();
        if (previousItem != null)
        {
            Object.Destroy(previousItem.gameObject);
        }

        // Display the object inside the speech bubble

        GameObject spriteToDisplayInSpeechBubble = Object.Instantiate(questObjectPrefab);
        spriteToDisplayInSpeechBubble.transform.parent = questGiverSpeechBubble.transform;
        spriteToDisplayInSpeechBubble.transform.localPosition = new Vector3(0, -0.3F, -1 /* so it shows up on top */);

        // Change the text of the quest
        var tm = questGiverSpeechBubble.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        tm.text = $"Find my {itemScript.displayableName}!";

        // Play some background music during this quest
        this.backgroundMusicPossibilityCurrentlyPlayingIndex++; // Play next one in line
        int clipToPlayIndex = this.backgroundMusicPossibilityCurrentlyPlayingIndex % this.backgroundMusicPossibilities.Length;
        this.backgroundMusic.clip = this.backgroundMusicPossibilities[clipToPlayIndex];
        this.backgroundMusic.Play();
    }

    private static void AddQuestItem(int[,] tiles, System.Func<Vector2Int, Vector2> tileToPositionTranslator, GameObject questObjectPrefab, GameManager manager)
    {
        // Look at minimum 5 tiles above the ground, so start there
        int minimumTileHeightToPutQuestItem = 5;

        // But it gets harder/higher every time you return a quest item!
        const int heightIncreaseEveryReturnedItem = 4;
        minimumTileHeightToPutQuestItem += (manager.NumberOfQuestItemsReturned * heightIncreaseEveryReturnedItem);

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
            for (int y = minimumTileHeightToPutQuestItem; y < tiles.GetLength(1); ++y)
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
        GameObject quest = Object.Instantiate(questObjectPrefab);
        quest.transform.position = new Vector3(foundWorldPosition.x, foundWorldPosition.y);

        CircleCollider2D collider = quest.AddComponent<CircleCollider2D>();
        collider.radius = 1F;
        collider.transform.position = quest.transform.position;
        collider.isTrigger = true;

        QuestPickupBehaviour behaviour = quest.AddComponent<QuestPickupBehaviour>();
        behaviour.ItemPickedUp += () =>
        {
            QuestItem questItemScript = quest.GetComponentInChildren<QuestItem>();

            manager.SetQuestItemPickedUp(questItemScript);
            quest.SetActive(false);  // Make it disappear but do not "destroy" it so we can refer to it later (because it's being "held" by the player)
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
        direction = Vector2.zero;
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