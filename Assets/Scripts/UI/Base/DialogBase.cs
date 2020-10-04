using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBase : MonoBehaviour
{
    [SerializeField] protected GameObject _frame;

    public virtual void Init(Dictionary<string, object> param)
    {

    }
}
