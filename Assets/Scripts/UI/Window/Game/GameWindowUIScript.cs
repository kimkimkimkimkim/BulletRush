using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using TMPro;

public class GameWindowUIScript : WindowBase
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Slider _scoreSlider;

    public override void Init(Dictionary<string, object> param)
    {
        base.Init(param);

        SetInitialScore();
    }

    private void Update()
    {
        //SetScore();
    }

    private void SetInitialScore()
    {
        _scoreText.text = "0";
        _scoreSlider.maxValue = GameManager.Instance.stageClearScore;
        _scoreSlider.value = 0;
    }

    public void SetScore(int score)
    {
        _scoreText.text = score.ToString();
        _scoreSlider.value = score;
    }
}
