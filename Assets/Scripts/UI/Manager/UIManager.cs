﻿using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [SerializeField] GameObject _uiBase;
    [SerializeField] GameObject _dialogBase;
    [SerializeField] Joystick _joystick;

    private GameObject prevWindow;
    private GameObject nowWindow;

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
        prevWindow = null;
        foreach(Transform child in _uiBase.transform)
        {
            if (child.gameObject.tag == "Screen") {
                prevWindow = child.gameObject;
                child.gameObject.SetActive(false);
            }
        }

        var instance = (GameObject)Instantiate(window,_uiBase.transform);
        nowWindow = instance;

        T uiScript = instance.GetComponent<T>();
        uiScript.Init(param);
    }

    public IObservable<Unit> CloseWindowObservable(GameObject window)
    {
        if(prevWindow != null) prevWindow.SetActive(true);

        return Observable.ReturnUnit()
            .Do(_ => Destroy(window))
            .SelectMany(_ => Observable.NextFrame());
    }

    public GameObject GetNowWindow()
    {
        return nowWindow;
    }

    public void OpenDialog<T>(GameObject dialog, Dictionary<string, object> param) where T : DialogBase
    {
        var instance = (GameObject)Instantiate(dialog, _dialogBase.transform);

        T uiScript = instance.GetComponent<T>();
        uiScript.Init(param);
    }

    public IObservable<Unit> CloseDialogObservable(GameObject dialog)
    {
        return Observable.ReturnUnit()
            .Do(_ => Destroy(dialog))
            .SelectMany(_ => Observable.NextFrame());
    }

    public void PlaySpeedUpAnimationObservable()
    {
        if (nowWindow == null) return;
        var uiScript = nowWindow.GetComponent<GameWindowUIScript>();
        if (uiScript == null) return ;

        uiScript.PlaySpeedUpAnimationObservable();
    }

}

public enum UIMode {
    None,
    Home,
    Playing,
    Defeat,
    Win,
}
