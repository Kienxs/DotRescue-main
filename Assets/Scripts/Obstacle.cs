using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
    private float currentRotateSpeed;

    private float rotateTime;
    private float currentRotateTime;

    private float[] _minRotateSpeed = { 115f, 135f, 155f, 175f };
    private float[] _maxRotateSpeed = { 125f, 145f, 165f, 185f };
    private float[] _minRotateTime = { 0.5f, 0.45f, 0.4f, 0.3f };
    private float[] _maxRotateTime = { 1.2f, 1.1f, 1f, 1f };

    private int number = 0;

    private void Awake()
    {
        number = PlayerPrefs.GetInt("Difficulty");
        currentRotateTime = 0f;
        currentRotateSpeed = _minRotateSpeed[number] + (_maxRotateSpeed[number] - _minRotateSpeed[number]) * 0.1f * Random.Range(0,11);
        rotateTime = _minRotateTime[number] + (_maxRotateTime[number] - _minRotateTime[number]) * 0.1f * Random.Range(0,11);
        currentRotateSpeed *= Random.Range(0, 2) == 0 ? 1f : -1f;
    }


    private void Update()
    {
        if(!GameplayManager._gameplayManager.Started()) return;

        currentRotateTime += Time.deltaTime;

        if(currentRotateTime > rotateTime)
        {
            currentRotateTime = 0f;
            currentRotateSpeed = _minRotateSpeed[number] + (_maxRotateSpeed[number] - _minRotateSpeed[number]) * 0.1f * Random.Range(0, 11);
            rotateTime = _minRotateTime[number] + (_maxRotateTime[number] - _minRotateTime[number]) * 0.1f * Random.Range(0, 11);
            currentRotateSpeed *= Random.Range(0, 2) == 0 ? 1f : -1f;
        }
    }

    private void FixedUpdate()
    {
        if (GameplayManager._gameplayManager.Started())
        {
            transform.Rotate(0, 0, currentRotateSpeed * Time.fixedDeltaTime);
        }
    }
}
