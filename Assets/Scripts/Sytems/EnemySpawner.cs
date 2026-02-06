using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float spawnDistance = 12f;

    private Transform player;
    private float timer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        Vector2 spawnDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + (Vector3)(spawnDir * spawnDistance);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
