﻿using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ClearWindowFactory : MonoBehaviour
{
    public static IObservable<ClearWindowResponse> Create(ClearWindowRequest request)
    {
        return Observable.Create<ClearWindowResponse>(observer => {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("onClickBonus", new Action(() => {
                observer.OnNext(new ClearWindowResponse());
                observer.OnCompleted();
            }));
            param.Add("onClickNext", new Action(() => {
                observer.OnNext(new ClearWindowResponse());
                observer.OnCompleted();
            }));

            var window = (GameObject)Resources.Load("UI/Window/ClearWindow");
            UIManager.Instance.OpenWindow<ClearWindowUIScript>(window, param);
            return Disposable.Empty;
        });
    }
}