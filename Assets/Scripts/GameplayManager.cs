using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager _gameplayManager;

    private bool hasGameFinished;

    [SerializeField] private TMP_Text _scoreText;

    [SerializeField] private TMP_Text _countdownText;

    private float score;
    private float scoreSpeed;// tốc độ cộng điểm
    private float[] scoreSpeedLevel = { 1, 1.5f, 2.3f, 3.3f};// nhân điểm theo cấp độ khó
    private float playerSpeed;
    private int currentLevel;

    [SerializeField] private List<int> _levelSpeed, _levelMax, _playerSpeed;

    private bool _start;

    private void Awake()
    {
        _gameplayManager = this;

        GameManager.Instance.IsInitialized = true;

        score = 0;
        currentLevel = 0;
        _scoreText.text = ((int)score).ToString();

        scoreSpeed = _levelSpeed[currentLevel];
        _start = false;
        StartCoroutine(StartGame());
        StartCoroutine(Countdown(4));
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(5.2f);
        _start = true;
    }

    private void Update()
    {
        if (!_start) return;

        if (hasGameFinished) return;

        score += scoreSpeed * scoreSpeedLevel[PlayerPrefs.GetInt("Difficulty")] * Time.deltaTime;

        _scoreText.text = ((int)score).ToString();

        if (score > _levelMax[Mathf.Clamp(currentLevel, 0, _levelMax.Count - 1)])
        {
            currentLevel = Mathf.Clamp(currentLevel + 1, 0, _levelMax.Count - 1);
            scoreSpeed = _levelSpeed[currentLevel];
            playerSpeed = _playerSpeed[currentLevel];
        }
    }

    public void GameEnded()
    {
        hasGameFinished = true;
        GameManager.Instance.CurrentScore = (int)score;
        PlayerPrefs.SetInt("Point", PlayerPrefs.GetInt("Point") + (int)score);

        if((int)score >= 2500)
        {
            if(PlayerPrefs.GetInt("Difficulty") == 0)
                PlayerPrefs.SetInt("Easy", PlayerPrefs.GetInt("Easy") + 1);
            else if (PlayerPrefs.GetInt("Difficulty") == 1)
                PlayerPrefs.SetInt("Normal", PlayerPrefs.GetInt("Normal") + 1);
            else if (PlayerPrefs.GetInt("Difficulty") == 2)
                PlayerPrefs.SetInt("Hard", PlayerPrefs.GetInt("Hard") + 1);
            else
                PlayerPrefs.SetInt("Asian", PlayerPrefs.GetInt("Asian") + 1);
        }


        StartCoroutine(GameOver());
    }


    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.GotoMainMenu();
    }

    public bool Started()
    {
        return _start;
    }
    private IEnumerator Countdown(int countdownTime)
    {
        while (countdownTime > 0)
        {
            _countdownText.text = countdownTime.ToString(); // Hiển thị số giây còn lại
            yield return new WaitForSeconds(1f); // Đợi 1 giây
            countdownTime--;
        }

        _countdownText.text = "Go!"; // Hiển thị thông báo khi kết thúc đếm ngược
        yield return new WaitForSeconds(1f); // Hiển thị "Go!" trong 1 giây nữa

        _countdownText.text = ""; // Sau đó xóa văn bản đếm ngược
    }

    public float PlayerSpeed()
    {
        return playerSpeed;
    }
}
