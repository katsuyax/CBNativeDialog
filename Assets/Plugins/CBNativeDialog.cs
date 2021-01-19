//
//  The MIT License (MIT)
//
//  Copyright (c) 2021 CrossBridge(Katsuya Kato)
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using System;
#if !UNITY_EDITOR && UNITY_IOS
using System.Runtime.InteropServices;
#endif
using UnityEngine;

public class CBNativeDialog : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _CBNativeDialog_show(
        string gameObject,
        string title,
        string message,
        string positiveButtonTitle,
        string positiveButtonAction,
        string negativeButtonTitle,
        string negativeButtonAction);
#endif

    private static CBNativeDialog _instance;

    public static CBNativeDialog Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<CBNativeDialog>();
                if (_instance == null)
                {
                    _instance = new GameObject("CBNativeDialog").AddComponent<CBNativeDialog>();
                }

                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    private Action _positiveButtonAction;
    private Action _negativeButtonAction;
    private Action _cancelAction;
    private bool _isShow = false;

    /// <summary>
    /// Generate dialog
    /// positiveButtonTitle / negativeButtonTitle Display buttons by entering values for each
    /// positiveButtonTitle is required
    /// Action can be null (e.g. just to show information to the user)
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="positiveButtonTitle"></param>
    /// <param name="positiveButtonAction"></param>
    /// <param name="negativeButtonTitle"></param>
    /// <param name="negativeButtonAction"></param>
    /// <param name="isCancelable">Android only</param>
    /// <param name="cancelAction">Android only</param>
    public bool Show(
        string title,
        string message,
        string positiveButtonTitle,
        Action positiveButtonAction = null,
        string negativeButtonTitle = null,
        Action negativeButtonAction = null,
        bool isCancelable = false,
        Action cancelAction = null)
    {
#if UNITY_EDITOR
        Debug.LogWarning("Editor is not supported");
        return false;
#endif        
        
        if (_isShow)
        {
            return false;
        }
        
        _positiveButtonAction = positiveButtonAction;
        _negativeButtonAction = negativeButtonAction;
        _cancelAction = cancelAction;
        _isShow = true;

#if !UNITY_EDITOR && UNITY_IOS
        _CBNativeDialog_show(
            gameObject.name, 
            title, 
            message, 
            positiveButtonTitle, 
            "PositiveButtonAction", 
            negativeButtonTitle,
            "NegativeButtonAction");
#elif !UNITY_EDITOR && UNITY_ANDROID
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            AndroidJavaObject alertDialogBuilder =
                new AndroidJavaObject("android.app.AlertDialog$Builder", activity);
            alertDialogBuilder.Call<AndroidJavaObject>("setTitle", title);
            alertDialogBuilder.Call<AndroidJavaObject>("setMessage", message);
            alertDialogBuilder.Call<AndroidJavaObject>("setCancelable", isCancelable);
            alertDialogBuilder.Call<AndroidJavaObject>("setPositiveButton", positiveButtonTitle,
                new PositiveButtonListener(this));
            if (negativeButtonTitle != null)
            {
                alertDialogBuilder.Call<AndroidJavaObject>("setNegativeButton", negativeButtonTitle,
                    new NegativeButtonListener(this));
            }

            if (isCancelable)
            {
                alertDialogBuilder.Call<AndroidJavaObject>("setOnCancelListener",
                    new CancelListener(this));
            }

            AndroidJavaObject dialog = alertDialogBuilder.Call<AndroidJavaObject>("create");
            dialog.Call("show");
        }));
#endif

        return true;
    }

    // for iOS
    public void PositiveButtonAction()
    {
        _positiveButtonAction?.Invoke();
        _isShow = false;
    }

    public void NegativeButtonAction()
    {
        _negativeButtonAction?.Invoke();
        _isShow = false;
    }

    // for Android
    private class PositiveButtonListener : AndroidJavaProxy
    {
        private readonly CBNativeDialog _parent;

        public PositiveButtonListener(CBNativeDialog dialog) : base("android.content.DialogInterface$OnClickListener")
        {
            _parent = dialog;
        }

        public void onClick(AndroidJavaObject obj, int value)
        {
            _parent._positiveButtonAction?.Invoke();
            _parent._isShow = false;
        }
    }

    private class NegativeButtonListener : AndroidJavaProxy
    {
        private readonly CBNativeDialog _parent;

        public NegativeButtonListener(CBNativeDialog dialog) : base("android.content.DialogInterface$OnClickListener")
        {
            _parent = dialog;
        }

        public void onClick(AndroidJavaObject obj, int value)
        {
            _parent._negativeButtonAction?.Invoke();
            _parent._isShow = false;
        }
    }

    private class CancelListener : AndroidJavaProxy
    {
        private readonly CBNativeDialog _parent;

        public CancelListener(CBNativeDialog dialog) : base("android.content.DialogInterface$OnCancelListener")
        {
            _parent = dialog;
        }

        public void onCancel(AndroidJavaObject obj)
        {
            _parent._cancelAction?.Invoke();
            _parent._isShow = false;
        }
    }
}
