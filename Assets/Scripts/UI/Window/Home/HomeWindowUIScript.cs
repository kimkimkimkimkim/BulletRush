using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UniRx.Triggers;

public class HomeWindowUIScript : WindowBase
{
    [SerializeField] private GameObject _dragIcon;


    public override void Init(Dictionary<string, object> param)
    {
        base.Init(param);

        var joystick = (Joystick)param["joystick"];
        joystick.homeWindow = gameObject;

        var sTime = Time.time;
        _dragIcon.UpdateAsObservable()
            .Do(_ => {
                var time = 3 * (Time.time - sTime);
                var x = 100 * Mathf.Cos(time);
                var y = 50 * Mathf.Sin(2*time);
                _dragIcon.transform.localPosition = new Vector3(x, y, 0);
            })
            .Subscribe();
    }
}
