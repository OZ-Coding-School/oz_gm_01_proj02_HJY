
using UnityEngine;
using System.Collections.Generic;

public class MapSpawner : MonoBehaviour
{
    [Header("청크 프리팹들")]
    public GameObject startChunkPrefab;   // 첫 번째 시작 그리드는 고정
    public GameObject[] chunkPrefabs;     // 랜덤으로 반복될 청크들

    [Header("플레이어")]
    public Transform player;

    [Header("청크 설정")]
    public float chunkHeight = 10;    // 청크 높이 (빈틈없이 프리팹이 생성되는 간격)
    public int initialChunks = 3;     // 시작 시 생성할 총 청크 개수

    private List<GameObject> activeChunks = new List<GameObject>();
    private float spawnY = 0f;
    private int lastIndex = -1; // 마지막으로 사용한 프리팹 인덱스 기억

    void Start()
    {
        // 첫 번째 청크는 고정된 프리팹으로 생성
        SpawnStartChunk();

        // 나머지 초기 청크는 랜덤으로 생성
        for (int i = 1; i < initialChunks; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        // 플레이어가 내려가면 새로운 청크 생성
        if (player.position.y < spawnY + chunkHeight)
        {
            SpawnChunk();
            RemoveOldChunk();
        }
    }

    void SpawnStartChunk()
    {
        GameObject chunk = Instantiate(startChunkPrefab, new Vector3(0, spawnY, 0), Quaternion.identity);
        activeChunks.Add(chunk);
        spawnY -= chunkHeight;
    }

    void SpawnChunk()
    {
        // 랜덤 인덱스 선택 (직전과 다른 값이 나오도록)
        int index;
        do
        {
            index = Random.Range(0, chunkPrefabs.Length);
        } while (index == lastIndex && chunkPrefabs.Length > 1);

        lastIndex = index;

        GameObject prefab = chunkPrefabs[index];
        GameObject chunk = Instantiate(prefab, new Vector3(0, spawnY, 0), Quaternion.identity);

        activeChunks.Add(chunk);
        spawnY -= chunkHeight;
    }

    void RemoveOldChunk()
    {
        if (activeChunks.Count > initialChunks + 2) // 화면 위로 벗어난 청크 제거
        {
            GameObject oldChunk = activeChunks[0];
            activeChunks.RemoveAt(0);

            // 시작 청크는 파괴하지 않음
            if (oldChunk.name.Contains(startChunkPrefab.name)) return;

            Destroy(oldChunk);
        }
    }
}