This library makes it easy to display native iOS/Android dialogs in Unity.

## Installation

import [CBNativeDialog.unitypackage](https://github.com/katsuyax/CBNativeDIalog/releases) via Assets-Import Package

## Usage

```C#
StartCoroutine(CBNativeDialog.Instance.Show(title: "title",
            message: "message",
            positiveButtonTitle: "OK",
            positiveButtonAction: () => { Debug.Log("OK"); },
            negativeButtonTitle: "CANCEL",
            negativeButtonAction: () => { Debug.Log("CANCEL"); }));
```



