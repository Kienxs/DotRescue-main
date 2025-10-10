using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private AudioClip _moveClip, _loseClip;

    [SerializeField] private GameplayManager _gm;
    [SerializeField] private GameObject _explosionPrefab;

    private float[] _rotateSpeed = { 125f, 145f, 165f, 185f };

    private void Update()
    {
        if (!GameplayManager._gameplayManager.Started()) return;

#if UNITY_EDITOR
        // Sử dụng chuột khi trong Unity Editor
        if (Input.GetMouseButtonDown(0))
        {
            SoundManager.Instance.PlaySound(_moveClip);
            _rotateSpeed[PlayerPrefs.GetInt("Difficulty")] *= -1f;
        }
#else 
    if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    {
        SoundManager.Instance.PlaySound(_moveClip);
        _rotateSpeed[PlayerPrefs.GetInt("Difficulty")] *= -1f;
    }
#endif
    }

    private void FixedUpdate()
    {
        if (GameplayManager._gameplayManager.Started())
        {
            float baseSpeed = Mathf.Abs(_rotateSpeed[PlayerPrefs.GetInt("Difficulty")]);        // Lấy tốc độ quay hiện tại
            float totalSpeed = baseSpeed + GameplayManager._gameplayManager.PlayerSpeed();        // Cộng thêm playerSpeed mà không làm thay đổi tốc độ quay giữa âm và dương

            if (_rotateSpeed[PlayerPrefs.GetInt("Difficulty")] < 0)       // Kiểm tra hướng quay để xác định dấu của tốc độ
                totalSpeed = -totalSpeed;  // Quay ngược chiều nếu _rotateSpeed đang âm

            transform.Rotate(0, 0, totalSpeed * Time.fixedDeltaTime);        // Áp dụng tốc độ quay lên đối tượng
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            Instantiate(_explosionPrefab, transform.GetChild(0).position, Quaternion.identity);
            SoundManager.Instance.PlaySound(_loseClip);
            _gm.GameEnded();
            Destroy(gameObject);
        }
    }
}
