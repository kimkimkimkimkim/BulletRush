using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [SerializeField] GameObject _canvas;
    [SerializeField] Joystick _joystick;

    private void Start()
    {
        SetUI(UIMode.Home);
    }

    public void SetUI(UIMode uiMode)
    {
        switch (uiMode)
        {
            case UIMode.Home:
                _joystick.gameObject.SetActive(true);
                OpenHomeWindow();
                break;
            case UIMode.Playing:
                _joystick.gameObject.SetActive(true);
                break;
            case UIMode.Defeat:
                _joystick.GetComponent<Joystick>().Initialize();
                _joystick.gameObject.SetActive(false);
                break;
            case UIMode.Win:
                _joystick.GetComponent<Joystick>().Initialize();
                _joystick.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    private void OpenHomeWindow()
    {
        HomeWindowFactory.Create(new HomeWindowRequest() { joystick = _joystick })
            .Subscribe();
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
