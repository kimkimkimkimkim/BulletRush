using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameWindowFactory
{
    public static IObservable<GameWindowResponse> Create(GameWindowRequest request)
    {
        return Observable.Create<GameWindowResponse>(observer => {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("simulationResultText", request.simulationResultText);
            var window = (GameObject)Resources.Load("UI/Window/GameWindow");
            UIManager.Instance.OpenWindow<GameWindowUIScript>(window, param);
            return Disposable.Empty;
        });
    }
}
