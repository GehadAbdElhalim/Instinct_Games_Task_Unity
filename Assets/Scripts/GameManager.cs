using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public string fileName;

    public MapGenerator mapGenerator;
    public UIManager uiManager;
    
    //keep reference to the player
    public GameObject player;

    //To change the bullet speed inside the prefab
    public GameObject bulletPrefab;

    //Viewing these variables in the inspector to make sure that they have been read correctly
    [Header("Just to check the values")]
    [SerializeField] int mapHeight;
    [SerializeField] int mapWidth;
    [SerializeField] List<int> rows;
    [SerializeField] List<int> cols;
    [SerializeField] float projectileSpeed;

    [HideInInspector] public int score;

    bool gameEnded = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        HealthBehaviour.OnPlayerDead.AddListener(EndGame);

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

                    for (int i = 0; i < cells.Length; i++)
                    {
                        cells[i] = cells[i].Trim('(', ')');
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

    private void Update()
    {
        if (gameEnded)
        {
            if(Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
            {
                RestartGame();
            }
        }
    }

    public void EndGame()
    {
        uiManager.ShowEndScreen();
        gameEnded = true;
        Time.timeScale = 0;
    }

    void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
