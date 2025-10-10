using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    private string[] difficulty = {"easy", "normal", "hard", "asian"};

    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _newBestText;
    [SerializeField] private TMP_Text _bestScoreText;
    [SerializeField] private TMP_Text _difficultyText;
    [SerializeField] private TMP_Text _pointText;

    [SerializeField] private Button[] _selectiveButton;    // _selectiveButton is plus(+) and _selectiveButton[1] is minus(-)

    [SerializeField] private TMP_Text[] _rank;
    [SerializeField] private Sprite[] _spriteBackGround;
    [SerializeField] private GameObject _backGround;

    [SerializeField] private float _animationTime;
    [SerializeField] private AnimationCurve _speedCurve;
    [SerializeField] private TMP_Text[] _useBack_text;

    private int[] _priceBackGround = { 2000, 2000, 3000, 3000, 8000, 8000, 8000, 8000, 8000, 8000, 8000, 8000, 8000, 8000, 8000, 8000, 8000, 8000, 6000, 6000, 6000, 6000, 10000, 12000 };

   
    private void Awake()
    {

        if (!PlayerPrefs.HasKey("Difficulty"))  // Khởi tạo dữ liệu game
        {
            PlayerPrefs.SetInt("Difficulty", 0);
            PlayerPrefs.SetInt("Easy", 0);
            PlayerPrefs.SetInt("Normal", 0);
            PlayerPrefs.SetInt("Hard", 0);
            PlayerPrefs.SetInt("Asian", 0);

            // BackGround
            PlayerPrefs.SetInt("00", 1);
            for (int i = 1; i <= 23; i++)
            {
                string key = i.ToString("D2"); // Chuyển số thành chuỗi có 2 chữ số (01, 02, 03, ...)
                PlayerPrefs.SetInt(key, -1);
            }

            // Point
            PlayerPrefs.SetInt("Point", 1000000);
        }
        _pointText.text = PlayerPrefs.GetInt("Point").ToString();
        for (int i = 0; i <= 23; i++)
        {
            string key = i.ToString("D2"); // Chuyển số thành chuỗi có 2 chữ số (01, 02, 03, ...)
            if(PlayerPrefs.GetInt(key) == 1)
            {
                ChangeBackGround(_backGround, i);
                break;
            }
        }

        PriceUpdate();

        ArrayLimit();

        _rank[0].text = "x" + PlayerPrefs.GetInt("Easy").ToString();
        _rank[1].text = "x" + PlayerPrefs.GetInt("Normal").ToString();
        _rank[2].text = "x" + PlayerPrefs.GetInt("Hard").ToString();
        _rank[3].text = "x" + PlayerPrefs.GetInt("Asian").ToString();

        _difficultyText.text = difficulty[PlayerPrefs.GetInt("Difficulty")];
        _bestScoreText.text = GameManager.Instance.HighScore.ToString();
        if (!GameManager.Instance.IsInitialized)
        {
            _scoreText.gameObject.SetActive(false);
            _newBestText.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(ShowScore());
        }
    }
     public void Use(string id)
    {
        int intId = int.Parse(id);
        if (PlayerPrefs.GetInt(id) == -1 && PlayerPrefs.GetInt("Point") >= _priceBackGround[intId])
        {
            PlayerPrefs.SetInt(id, 0);
            PlayerPrefs.SetInt("Point", PlayerPrefs.GetInt("Point") - _priceBackGround[intId]);
            _pointText.text = PlayerPrefs.GetInt("Point").ToString();
        }
        else if(PlayerPrefs.GetInt(id) == 0)
        {
            for (int i = 0; i <= 23; i++)
            {
                string key = i.ToString("D2"); // Chuyển số thành chuỗi có 2 chữ số (01, 02, 03, ...)
                if(PlayerPrefs.GetInt(key) == 1)
                {
                    PlayerPrefs.SetInt(key, 0);
                    break;
                }
            }
            PlayerPrefs.SetInt(id, 1);
            ChangeBackGround(_backGround, intId);
        }
        PriceUpdate();
        PlayerPrefs.Save();
     }

    public void PriceUpdate()
    {
        for (int i = 0; i <= 23; i++)
        {
            string key = i.ToString("D2"); // Chuyển số thành chuỗi có 2 chữ số (01, 02, 03, ...)
            if (PlayerPrefs.GetInt(key) == -1)
                _useBack_text[i].text = _priceBackGround[i].ToString();
            else if (PlayerPrefs.GetInt(key) == 0)
                _useBack_text[i].text = "Use";
            else
                _useBack_text[i].text = "Used";
        }
    }

    public void ChangeBackGround(GameObject backGround, int id)
    {
        Image panelImage = backGround.GetComponent<Image>();
            panelImage.sprite = _spriteBackGround[id];
    }

    private IEnumerator ShowScore()
    {
        int tempScore = 0;
        _scoreText.text = tempScore.ToString();

        int currentScore = GameManager.Instance.CurrentScore;
        int highScore = GameManager.Instance.HighScore;

        if(highScore < currentScore)
        {
            _newBestText.gameObject.SetActive(true);
            GameManager.Instance.HighScore = currentScore;
        }
        else
        {
            _newBestText.gameObject.SetActive(false);
        }

        _bestScoreText.text = GameManager.Instance.HighScore.ToString();
        float speed = 1 / _animationTime;
        float timeElapsed = 0f;

        while(timeElapsed < 1f)
        {
            timeElapsed += speed * Time.deltaTime;
            tempScore = (int)(_speedCurve.Evaluate(timeElapsed) * currentScore);
            _scoreText.text = tempScore.ToString();
            yield return null;
        }

        tempScore = currentScore;
        _scoreText.text = tempScore.ToString();

    }

    [SerializeField] private AudioClip _clickClip;

    public void ClickedPlay()
    {
        SoundManager.Instance.PlaySound(_clickClip);
        GameManager.Instance.GotoGameplay();
    }

    private void ArrayLimit()
    {
        _selectiveButton[0].interactable = true;
        _selectiveButton[1].interactable = true;

        if (PlayerPrefs.GetInt("Difficulty") == 0)
            _selectiveButton[1].interactable = false;
        if (PlayerPrefs.GetInt("Difficulty") == difficulty.Length - 1)
            _selectiveButton[0].interactable = false;
    }
    public void SelectiveDifficulty(int a)
    {
        PlayerPrefs.SetInt("Difficulty", PlayerPrefs.GetInt("Difficulty") + a);        // a để thể hiện +-1 trong chọn difficulty
        ArrayLimit();
        _difficultyText.text = difficulty[PlayerPrefs.GetInt("Difficulty")];
    }
}
