using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    void Start()
    {
        CBNativeDialog.Instance.Show(title: "title",
            message: "message",
            positiveButtonTitle: "OK",
            positiveButtonAction: () => { Debug.Log("OK"); },
            negativeButtonTitle: "CANCEL",
            negativeButtonAction: () => { Debug.Log("CANCEL"); });
    }
}
