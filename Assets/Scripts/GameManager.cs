using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MapGenerator mapGenerator;

    public GameObject player;

    public string fileName;

    [SerializeField] int mapHeight;
    [SerializeField] int mapWidth;
    [SerializeField] List<int> rows;
    [SerializeField] List<int> cols;
    [SerializeField] float projectileSpeed;

    public GameObject bulletPrefab;

    public int score;

    private void Awake()
    {
        instance = this;

        string filePath = Application.dataPath + "/StreamingAssets/" + fileName;

        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);

            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] lineParts = line.Split(':');

                if (lineParts[0] == "MapSize")
                {
                    lineParts[1] = lineParts[1].Replace(" ", "");
                    string[] parameters = lineParts[1].Split(new char[] { 'x', 'X' });
                    mapHeight = int.Parse(parameters[0]);
                    mapWidth = int.Parse(parameters[1]);

                    mapGenerator.GenerateMap(mapHeight, mapWidth);
                }

                if (lineParts[0] == "Turrets")
                {
                    lineParts[1] = lineParts[1].Replace(" ", "");
                    string[] cells = lineParts[1].Split(',');

                    for(int i = 0; i < cells.Length; i++)
                    {
                        cells[i] = cells[i].Trim('(', ')');
                        //cells[i] = cells[i].Replace(" ", "");
                        //cells[i] = cells[i].Replace("(", "");
                        //cells[i] = cells[i].Replace(")", "");
                        string[] cellIndices = cells[i].Split('-');
                        rows.Add(int.Parse(cellIndices[0]));
                        cols.Add(int.Parse(cellIndices[1]));
                    }

                    mapGenerator.SpawnTurrets(rows.ToArray(), cols.ToArray());
                }

                if (lineParts[0] == "ProjectileSpeed")
                {
                    lineParts[1] = lineParts[1].Replace(" ", "");
                    projectileSpeed = float.Parse(lineParts[1]);
                    bulletPrefab.GetComponent<BulletBehaviour>().speed = projectileSpeed;
                }
            }
        }

        mapGenerator.SpawnCollectibles();
    }
}
