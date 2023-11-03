using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int RenderRadius; //Радиус отрисовки чанков
    [Range(0, 100)]
    [SerializeField] private int WallChance; //Шанс появления стены (от 0% до 100%)
    [Space]
    [SerializeField] private GameObject ChunkPrefab; //Префаб чанка
    [SerializeField] private GameObject[] ChunkDecorationsPrefabs; //Префабы объектов чанка (без плотности)
    [SerializeField] private GameObject[] ChunkObstaclesPrefabs; //Префабы объектов чанка (с плотностью)
    [SerializeField] private GameObject ChunkWallPrefab; //Префаб стены чанка

    private readonly Dictionary<WorldPos, GameObject> worldChunks = new(); //Словарь чанков. Скорость доступа элементов высокая, потому что это HashMap
    private Transform chunksParent; //Держатель чанков
    private PlayerMovement playerMovement; //Игрок

    private void Start()
    {
        //Получение игрока
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        //Держатель всех чанков
        chunksParent = new GameObject("ChunksParent").transform;
    }

    private void Update()
    {
        //Позиция игрока
        WorldPos playerPos = playerMovement.GetPosition();
        //Переотрисовка чанков
        RenderChunks(playerPos, RenderRadius);
    }

    ///<summary> Переотрисовка чанков в зависимости от позиции игрока </summary>
    private void RenderChunks(WorldPos playerPosition, int renderRadius)
    {
        //Проверяем все существующие чанки, скрываем те, которые не в радиусе и показываем те, которые в радиусе
        foreach (WorldPos chunkPosition in worldChunks.Keys)
        {
            if (Mathf.Abs(chunkPosition.X - playerPosition.X) <= renderRadius && Mathf.Abs(chunkPosition.Y - playerPosition.Y) <= renderRadius) worldChunks[chunkPosition].SetActive(true);
            else worldChunks[chunkPosition].SetActive(false);
        }

        //Сканирование чанков вокруг игрока (двумерный массив по координатам)
        for (int x = playerPosition.X - renderRadius; x <= playerPosition.X + renderRadius; x++)
        {
            for (int y = playerPosition.Y - renderRadius; y <= playerPosition.Y + renderRadius; y++)
            {
                WorldPos chunkPosition = new(x, y);
                //Если чанк вокруг игрока еще не сгенерирован, то создаем его и сразу показываем
                if (!worldChunks.ContainsKey(chunkPosition))
                {
                    GenerateChunk(chunkPosition);
                    worldChunks[chunkPosition].SetActive(true);
                }    
            }
        }
    }

    ///<summary> Генерация нового чанка по позиции </summary>
    private void GenerateChunk(WorldPos position)
    {
        //Генерация чанка
        GameObject chunk = Instantiate(ChunkPrefab);
        chunk.name = $"Chunk X:{position.X} Y:{position.Y}";
        chunk.transform.position = new(position.X * WorldPos.CHUNK_SIZE, 0f, position.Y * WorldPos.CHUNK_SIZE);
        chunk.transform.SetParent(chunksParent);
        worldChunks.Add(position, chunk);

        //Генерация декорации чанка
        GameObject decoration = Instantiate(GetRandomDecoration());
        decoration.transform.SetParent(chunk.transform);
        decoration.transform.localPosition = new(Random.Range(-0.4f, 0.4f), 0.5f, Random.Range(-0.4f, 0.4f));

        //Генерация препятствия чанка
        GameObject obstacle = Instantiate(GetRandomObstacle());
        obstacle.transform.SetParent(chunk.transform);
        obstacle.transform.localPosition = new(Random.Range(-0.4f, 0.4f), 0f, Random.Range(-0.4f, 0.4f));

        //Генерация верхней стены
        bool chanceTopWall = Random.Range(1, 100 + 1) <= WallChance;
        if (chanceTopWall)
        {
            GameObject topWall = Instantiate(ChunkWallPrefab);
            topWall.transform.SetParent(chunk.transform);
            topWall.transform.localPosition = new(0.475f, 0f, 0f);
            topWall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }

        //Генерация нижней стены
        bool chanceBottomWall = Random.Range(1, 100 + 1) <= WallChance;
        if (chanceBottomWall)
        {
            GameObject bottomWall = Instantiate(ChunkWallPrefab);
            bottomWall.transform.SetParent(chunk.transform);
            bottomWall.transform.localPosition = new(-0.475f, 0f, 0f);
            bottomWall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }

        //Генерация левой стены
        bool chanceLeftWall = Random.Range(1, 100 + 1) <= WallChance;
        if (chanceLeftWall)
        {
            GameObject leftWall = Instantiate(ChunkWallPrefab);
            leftWall.transform.SetParent(chunk.transform);
            leftWall.transform.localPosition = new(0.475f, 0f, 0f);
            leftWall.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        //Генерация правой стены
        bool chanceRightWall = Random.Range(1, 100 + 1) <= WallChance;
        if (chanceRightWall)
        {
            GameObject rightWall = Instantiate(ChunkWallPrefab);
            rightWall.transform.SetParent(chunk.transform);
            rightWall.transform.localPosition = new(-0.475f, 0f, 0f);
            rightWall.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    
    ///<summary> Получение случайной декорации для чанка </summary>
    private GameObject GetRandomDecoration()
    {
        return ChunkDecorationsPrefabs[Random.Range(0, ChunkDecorationsPrefabs.Length)];
    }

    ///<summary> Получение случайного препятствия для чанка </summary>
    private GameObject GetRandomObstacle()
    {
        return ChunkObstaclesPrefabs[Random.Range(0, ChunkObstaclesPrefabs.Length)];
    }
}