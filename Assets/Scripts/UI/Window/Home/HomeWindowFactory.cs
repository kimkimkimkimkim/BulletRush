using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class HomeWindowFactory : MonoBehaviour
{
    public static IObservable<HomeWindowResponse> Create(HomeWindowRequest request)
    {
        return Observable.Create<HomeWindowResponse>(observer => {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("onClickBonus", new Action(() => {
                observer.OnNext(new HomeWindowResponse());
                observer.OnCompleted();
            }));
            param.Add("onClickNext", new Action(() => {
                observer.OnNext(new HomeWindowResponse());
                observer.OnCompleted();
            }));
            param.Add("joystick", request.joystick);

            var window = (GameObject)Resources.Load("UI/Window/HomeWindow");
            UIManager.Instance.OpenWindow<HomeWindowUIScript>(window, param);
            return Disposable.Empty;
        });
    }
}
