using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using TMPro;
using DG.Tweening;

public class GameWindowUIScript : WindowBase
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Slider _scoreSlider;
    [SerializeField] private Text _simulationResultText;
    [SerializeField] private Text _speedUpText;

    public override void Init(Dictionary<string, object> param)
    {
        var simulationResultText = (string)param["simulationResultText"];
        _simulationResultText.text = simulationResultText;

        SetInitialScore();
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

    public void PlaySpeedUpAnimationObservable()
    {
        var time = 1.5f;
        var defaultPosition = _speedUpText.transform.localPosition;
        var defaultColor = _speedUpText.color;
        DOTween.Sequence()
            .OnStart(() =>
            {
                _speedUpText.transform.localScale = Vector3.one * 0.6f;
                _speedUpText.gameObject.SetActive(true);
            })
            .Append(_speedUpText.transform.DOScale(1, time))
            .Join(_speedUpText.transform.DOLocalMoveY(defaultPosition.y + 15f, time))
            .Join(_speedUpText.DOFade(0, time / 2).SetDelay(time / 2))
            .OnCompleteAsObservable()
            .DoOnCompleted(() =>
            {
                _speedUpText.transform.localScale = defaultPosition;
                _speedUpText.color = defaultColor;
                _speedUpText.gameObject.SetActive(false);
            })
            .Subscribe()
            .AddTo(this);   
    }
}
