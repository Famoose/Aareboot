using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public GameObject score;
    public GameObject start;
    public GameObject helper;
    public GridManager gridManager;
    public GameObject player;

    private Button startButton;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI helperText;
    private TextMeshProUGUI buttonText;

    private float playerDeadlineY = -10f;
    private float startTime;
    private bool gameRunning;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = score.GetComponent<TextMeshProUGUI>();
        helperText = helper.GetComponent<TextMeshProUGUI>();
        startButton = start.GetComponent<Button>();
        buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();

        score.SetActive(false);
        startButton.onClick.AddListener(StartGame);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning)
        {
            scoreText.text = $"Score: {Time.time - startTime:.00}";
            if (player.transform.position.y < playerDeadlineY)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        gameRunning = false;
        gridManager.gameRunning = gameRunning;

        helperText.text = "Game Over";
        buttonText.text = "Reset";
        start.SetActive(true);
        helper.SetActive(true);
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(Reset);

    }

    void StartGame(){
        startTime = Time.time;
        start.SetActive(false);
        helper.SetActive(false);
        score.SetActive(true);
        gameRunning = true;
        gridManager.gameRunning = gameRunning;
    }

    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
