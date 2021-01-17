using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject endGameScreen;

    private void Update()
    {
        scoreText.text = "SCORE : " + GameManager.instance.score.ToString();
    }

    public void ShowEndScreen()
    {
        endGameScreen.SetActive(true);
    }
}
