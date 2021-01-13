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

<img src="https://user-images.githubusercontent.com/539152/104406617-10834280-55a3-11eb-946f-1505997034fc.png" width=320> <img src="https://user-images.githubusercontent.com/539152/104406915-cbabdb80-55a3-11eb-8cac-5ac8e73dec24.png" width=320>
