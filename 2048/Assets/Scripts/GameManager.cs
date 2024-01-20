using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Board board;
    Cell[] cells;
    public Text scoreText;
    int scoreTotal = 0;
    void Start()
    {
        scoreText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Application.Quit();
    }

    public void AddScore(int x)
    {
        scoreTotal += x;
        scoreText.text = scoreTotal.ToString();
    }

    public void ResetScore()
    {
        scoreTotal = 0;
        scoreText.text = scoreTotal.ToString();
    }
}
