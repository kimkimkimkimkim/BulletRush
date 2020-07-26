using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [SerializeField] GameObject _canvas;
    [SerializeField] GameObject _joystick;

    private void Start()
    {
        SetUI(UIMode.Home);
    }

    public void SetUI(UIMode uiMode)
    {
        switch (uiMode)
        {
            case UIMode.Home:
                _joystick.SetActive(true);
                break;
            case UIMode.Playing:
                _joystick.SetActive(true);
                break;
            case UIMode.Defeat:
                _joystick.GetComponent<Joystick>().Initialize();
                _joystick.SetActive(false);
                break;
            case UIMode.Win:
                _joystick.GetComponent<Joystick>().Initialize();
                _joystick.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void OpenWindow<T>(GameObject window,Dictionary<string, object> param) where T : WindowBase
    {
        var instance = (GameObject)Instantiate(window,_canvas.transform);

        T uiScript = instance.GetComponent<T>();
        uiScript.Init(param);
    }

    public IObservable<Unit> CloseWindowObservable(GameObject window)
    {
        return Observable.ReturnUnit()
            .Do(_ => Destroy(window))
            .SelectMany(_ => Observable.NextFrame());
    }

}

public enum UIMode {
    None,
    Home,
    Playing,
    Defeat,
    Win,
}
