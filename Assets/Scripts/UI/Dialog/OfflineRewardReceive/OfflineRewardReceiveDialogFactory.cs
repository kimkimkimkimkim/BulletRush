using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class OfflineRewardReceiveDialogFactory : MonoBehaviour
{
    public static IObservable<OfflineRewardReceiveDialogResponse> Create(OfflineRewardReceiveDialogRequest request)
    {
        return Observable.Create<OfflineRewardReceiveDialogResponse>(observer => {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("onClickReceive", new Action(() => {
                observer.OnNext(new OfflineRewardReceiveDialogResponse() { isBonus = false });
                observer.OnCompleted();
            }));
            param.Add("onClickBonus", new Action(() => {
                observer.OnNext(new OfflineRewardReceiveDialogResponse() { isBonus = true });
                observer.OnCompleted();
            }));
            param.Add("content", request.content);

            var dialog = (GameObject)Resources.Load("UI/Dialog/OfflineRewardReceiveDialog");
            UIManager.Instance.OpenDialog<OfflineRewardReceiveDialogUIScript>(dialog, param);
            return Disposable.Empty;
        });
    }
}
