using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CommonDialogFactory : MonoBehaviour
{
    public static IObservable<CommonDialogResponse> Create(CommonDialogRequest request)
    {
        return Observable.Create<CommonDialogResponse>(observer => {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("onClickYes", new Action(() => {
                observer.OnNext(new CommonDialogResponse() { responseType = DialogUtil.DialogResponseType.Yes});
                observer.OnCompleted();
            }));
            param.Add("onClickNo", new Action(() => {
                observer.OnNext(new CommonDialogResponse() { responseType = DialogUtil.DialogResponseType.No });
                observer.OnCompleted();
            }));
            param.Add("title", request.title);
            param.Add("content", request.content);

            var dialog = (GameObject)Resources.Load("UI/Dialog/CommonDialog");
            UIManager.Instance.OpenDialog<CommonDialogUIScript>(dialog, param);
            return Disposable.Empty;
        });
    }
}
