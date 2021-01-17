using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Vector2 cellSize = new Vector2(1, 1);
    public float bordersHeight = 5;
    Vector2Int mapSize;
    [SerializeField] GameObject cell;
    [SerializeField] GameObject turret;
    [SerializeField] GameObject collectible;
    [SerializeField] GameObject border;

    [HideInInspector] public Vector3[,] cellPositions;

    [Header("Materials")]
    [SerializeField] Material[] cellMats;

    private void Awake()
    {
        collectibleBehaviour.OnCollectibleDestroyed.AddListener(SpawnOneCollectible);
    }

    public void GenerateMap(int mapHeight, int mapWidth)
    {
        mapSize = new Vector2Int(mapHeight, mapWidth);

        float cellHeight = cellSize.x;
        float cellWidth = cellSize.y;

        cellPositions = new Vector3[mapHeight, mapWidth];

        int matIndex = 0;

        for (int i = 0; i < mapHeight; i++)
        {
            matIndex = (matIndex + 1) % cellMats.Length;

            for (int j = 0; j < mapWidth; j++)
            {
                float x = -((mapHeight - 1) / 2.0f) + i;
                x *= cellHeight;
                float z = -((mapWidth - 1) / 2.0f) + j;
                z *= cellWidth;

                GameObject go = Instantiate(cell, new Vector3(x, transform.position.y, z), Quaternion.identity, transform);
                go.transform.localScale = new Vector3(cellHeight, 1, cellWidth);

                go.GetComponentInChildren<MeshRenderer>().material = cellMats[matIndex];
                matIndex = (matIndex + 1) % cellMats.Length;

                cellPositions[i, j] = new Vector3(x, transform.position.y, z);
            }
        }

        gameObject.AddComponent<BoxCollider>().size = new Vector3(mapHeight * cellHeight, 0.1f, mapWidth * cellWidth);

        SpawnBorders();
    }

    void SpawnBorders()
    {
        //Right Border
        float x1 = -((mapSize.x - 1) / 2.0f) + mapSize.x;
        x1 *= cellSize.x;
        x1 -= cellSize.x / 2;
        float z1 = 0;
        z1 *= cellSize.y;

        GameObject border1 = Instantiate(border, new Vector3(x1, 1, z1), Quaternion.identity, transform);
        border1.transform.localScale = new Vector3(0.5f, bordersHeight, mapSize.y * cellSize.y);

        //Left Border
        float x2 = -(-((mapSize.x - 1) / 2.0f) + mapSize.x);
        x2 *= cellSize.x;
        x2 += cellSize.x / 2;
        float z2 = 0;
        z2 *= cellSize.y;

        GameObject border2 = Instantiate(border, new Vector3(x2, 1, z2), Quaternion.identity, transform);
        border2.transform.localScale = new Vector3(0.5f, bordersHeight, mapSize.y * cellSize.y);

        //Up Border
        float x3 = 0;
        x3 *= cellSize.x;
        float z3 = -((mapSize.y - 1) / 2.0f) + mapSize.y;
        z3 *= cellSize.y;
        z3 -= cellSize.y / 2;

        GameObject border3 = Instantiate(border, new Vector3(x3, 1, z3), Quaternion.identity, transform);
        border3.transform.localScale = new Vector3(mapSize.x * cellSize.x, bordersHeight, 0.5f);

        //Down Border
        float x4 = 0;
        x4 *= cellSize.x;
        float z4 = -(-((mapSize.y - 1) / 2.0f) + mapSize.y);
        z4 *= cellSize.y;
        z4 += cellSize.y / 2;

        GameObject border4 = Instantiate(border, new Vector3(x4, 1, z4), Quaternion.identity, transform);
        border4.transform.localScale = new Vector3(mapSize.x * cellSize.x, bordersHeight, 0.5f);
    }

    public void SpawnTurrets(int[] rows, int[] cols)
    {
        for (int i = 0; i < rows.Length; i++)
        {
            GameObject go = Instantiate(turret, cellPositions[rows[i], cols[i]], Quaternion.identity);
            go.transform.Rotate(Vector3.up, Random.Range(0f, 360f));
        }
    }

    [ContextMenu("spawn collectibles")]
    public void SpawnCollectibles()
    {
        for (int i = 0; i < 2 * Mathf.Max(mapSize.x, mapSize.y); i++)
        {
            SpawnOneCollectible();
        }
    }

    void SpawnOneCollectible()
    {
        Vector3 randomCellCenter = cellPositions[Random.Range(0, mapSize.x), Random.Range(0, mapSize.y)];
        float angle = Random.Range(0, Mathf.PI * 2);

        float x = Mathf.Sin(angle) * (cellSize.x / 2 - 1) + randomCellCenter.x;
        float z = Mathf.Cos(angle) * (cellSize.y / 2 - 1) + randomCellCenter.z;

        //Instantiate(collectible, new Vector3(x, 0.5f, z), Quaternion.identity);
        ObjectPooler.instance.SpawnFromPool("Collectible", new Vector3(x, 0.25f, z), Quaternion.Euler(180,0,0));
    }
}
