using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using BulletRush.Debug;


public class DebugDialogFactory : MonoBehaviour
{
    public static IObservable<DebugDialogResponse> Create(DebugDialogRequest request)
    {
        return Observable.Create<DebugDialogResponse>(observer => {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("onClickClose", new Action(() => {
                observer.OnNext(new DebugDialogResponse());
                observer.OnCompleted();
            }));

            var dialog = (GameObject)Resources.Load("UI/Dialog/DebugDialog");
            UIManager.Instance.OpenDialog<DebugDialogUIScript>(dialog, param);
            return Disposable.Empty;
        });
    }
}
