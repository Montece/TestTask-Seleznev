using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int RenderRadius; //������ ��������� ������
    [Range(0, 100)]
    [SerializeField] private int WallChance; //���� ��������� ����� (�� 0% �� 100%)
    [Space]
    [SerializeField] private GameObject ChunkPrefab; //������ �����
    [SerializeField] private GameObject[] ChunkDecorationsPrefabs; //������� �������� ����� (��� ���������)
    [SerializeField] private GameObject[] ChunkObstaclesPrefabs; //������� �������� ����� (� ����������)
    [SerializeField] private GameObject ChunkWallPrefab; //������ ����� �����

    private readonly Dictionary<WorldPos, GameObject> worldChunks = new(); //������� ������. �������� ������� ��������� �������, ������ ��� ��� HashMap
    private Transform chunksParent; //��������� ������
    private PlayerMovement playerMovement; //�����

    private void Start()
    {
        //��������� ������
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        //��������� ���� ������
        chunksParent = new GameObject("ChunksParent").transform;
    }

    private void Update()
    {
        //������� ������
        WorldPos playerPos = playerMovement.GetPosition();
        //������������� ������
        RenderChunks(playerPos, RenderRadius);
    }

    ///<summary> ������������� ������ � ����������� �� ������� ������ </summary>
    private void RenderChunks(WorldPos playerPosition, int renderRadius)
    {
        //��������� ��� ������������ �����, �������� ��, ������� �� � ������� � ���������� ��, ������� � �������
        foreach (WorldPos chunkPosition in worldChunks.Keys)
        {
            if (Mathf.Abs(chunkPosition.X - playerPosition.X) <= renderRadius && Mathf.Abs(chunkPosition.Y - playerPosition.Y) <= renderRadius) worldChunks[chunkPosition].SetActive(true);
            else worldChunks[chunkPosition].SetActive(false);
        }

        //������������ ������ ������ ������ (��������� ������ �� �����������)
        for (int x = playerPosition.X - renderRadius; x <= playerPosition.X + renderRadius; x++)
        {
            for (int y = playerPosition.Y - renderRadius; y <= playerPosition.Y + renderRadius; y++)
            {
                WorldPos chunkPosition = new(x, y);
                //���� ���� ������ ������ ��� �� ������������, �� ������� ��� � ����� ����������
                if (!worldChunks.ContainsKey(chunkPosition))
                {
                    GenerateChunk(chunkPosition);
                    worldChunks[chunkPosition].SetActive(true);
                }    
            }
        }
    }

    ///<summary> ��������� ������ ����� �� ������� </summary>
    private void GenerateChunk(WorldPos position)
    {
        //��������� �����
        GameObject chunk = Instantiate(ChunkPrefab);
        chunk.name = $"Chunk X:{position.X} Y:{position.Y}";
        chunk.transform.position = new(position.X * WorldPos.CHUNK_SIZE, 0f, position.Y * WorldPos.CHUNK_SIZE);
        chunk.transform.SetParent(chunksParent);
        worldChunks.Add(position, chunk);

        //��������� ��������� �����
        GameObject decoration = Instantiate(GetRandomDecoration());
        decoration.transform.SetParent(chunk.transform);
        decoration.transform.localPosition = new(Random.Range(-0.4f, 0.4f), 0.5f, Random.Range(-0.4f, 0.4f));

        //��������� ����������� �����
        GameObject obstacle = Instantiate(GetRandomObstacle());
        obstacle.transform.SetParent(chunk.transform);
        obstacle.transform.localPosition = new(Random.Range(-0.4f, 0.4f), 0f, Random.Range(-0.4f, 0.4f));

        //��������� ������� �����
        bool chanceTopWall = Random.Range(1, 100 + 1) <= WallChance;
        if (chanceTopWall)
        {
            GameObject topWall = Instantiate(ChunkWallPrefab);
            topWall.transform.SetParent(chunk.transform);
            topWall.transform.localPosition = new(0.475f, 0f, 0f);
            topWall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }

        //��������� ������ �����
        bool chanceBottomWall = Random.Range(1, 100 + 1) <= WallChance;
        if (chanceBottomWall)
        {
            GameObject bottomWall = Instantiate(ChunkWallPrefab);
            bottomWall.transform.SetParent(chunk.transform);
            bottomWall.transform.localPosition = new(-0.475f, 0f, 0f);
            bottomWall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }

        //��������� ����� �����
        bool chanceLeftWall = Random.Range(1, 100 + 1) <= WallChance;
        if (chanceLeftWall)
        {
            GameObject leftWall = Instantiate(ChunkWallPrefab);
            leftWall.transform.SetParent(chunk.transform);
            leftWall.transform.localPosition = new(0.475f, 0f, 0f);
            leftWall.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        //��������� ������ �����
        bool chanceRightWall = Random.Range(1, 100 + 1) <= WallChance;
        if (chanceRightWall)
        {
            GameObject rightWall = Instantiate(ChunkWallPrefab);
            rightWall.transform.SetParent(chunk.transform);
            rightWall.transform.localPosition = new(-0.475f, 0f, 0f);
            rightWall.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    
    ///<summary> ��������� ��������� ��������� ��� ����� </summary>
    private GameObject GetRandomDecoration()
    {
        return ChunkDecorationsPrefabs[Random.Range(0, ChunkDecorationsPrefabs.Length)];
    }

    ///<summary> ��������� ���������� ����������� ��� ����� </summary>
    private GameObject GetRandomObstacle()
    {
        return ChunkObstaclesPrefabs[Random.Range(0, ChunkObstaclesPrefabs.Length)];
    }
}