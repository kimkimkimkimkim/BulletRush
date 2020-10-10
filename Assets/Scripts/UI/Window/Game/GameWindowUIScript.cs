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
    [SerializeField] private Text _simulationResultText;

    public override void Init(Dictionary<string, object> param)
    {
        var simulationResultText = (string)param["simulationResultText"];
        _simulationResultText.text = simulationResultText;

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

    public void SetScore(float score)
    {
        _scoreText.text = Math.Floor(score).ToString();
        _scoreSlider.value = score;
    }
}
