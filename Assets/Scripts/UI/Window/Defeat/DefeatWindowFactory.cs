using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DefeatWindowFactory:MonoBehaviour
{
    public static IObservable<DefeatWindowResponse> Create(DefeatWindowRequest request)
    {
        return Observable.Create<DefeatWindowResponse>(observer => {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("onClickContinue", new Action(() => {
                observer.OnNext(new DefeatWindowResponse() { isContinue = true });
                observer.OnCompleted();
            }));
            param.Add("onClickNoThanks", new Action(() => {
                observer.OnNext(new DefeatWindowResponse() { isContinue = false});
                observer.OnCompleted();
            }));

            var window = (GameObject)Resources.Load("UI/Window/DefeatWindow");
            UIManager.Instance.OpenWindow<DefeatWindowUIScript>(window,param);
            return Disposable.Empty;
        });
    }
}
