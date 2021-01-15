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

#import "CBNativeDialog.h"

extern "C" {
void _CBNativeDialog_show(char *cGameObject,
                         char *cTitle,
                         char *cMessage,
                         char *cPositiveButtonTitle,
                         char *cPositiveButtonAction,
                         char *cNegativeButtonTitle,
                         char *cNegativeButtonAction) {
    NSString *gameObject = [NSString stringWithCString:cGameObject
                                         encoding:NSUTF8StringEncoding];
    NSString *title = [NSString stringWithCString:cTitle
                                         encoding:NSUTF8StringEncoding];
    NSString *message = [NSString stringWithCString:cMessage
                                           encoding:NSUTF8StringEncoding];
    NSString *positiveButtonTitle = [NSString stringWithCString:cPositiveButtonTitle
                                                       encoding:NSUTF8StringEncoding];
    NSString *positiveButtonAction = nil;
    if (cPositiveButtonAction != NULL) {
        positiveButtonAction = [NSString stringWithCString:cPositiveButtonAction
                                                           encoding:NSUTF8StringEncoding];
    }
    NSString *negativeButtonTitle = nil;
    if (cNegativeButtonTitle != NULL) {
        negativeButtonTitle = [NSString stringWithCString:cNegativeButtonTitle
                                                           encoding:NSUTF8StringEncoding];
    }
    NSString *negativeButtonAction = nil;
    if (cNegativeButtonAction != NULL) {
        negativeButtonAction = [NSString stringWithCString:cNegativeButtonAction
                                                           encoding:NSUTF8StringEncoding];
    }
    CBNativeDialog *nativeDialog = [[CBNativeDialog alloc] init];
    return [nativeDialog
            show:gameObject
            title:title
            message:message
            positiveButtonTitle:positiveButtonTitle
            positiveButtonAction:positiveButtonAction
            negativeButtonTitle:negativeButtonTitle
            negativeButtonAction:negativeButtonAction];
}
}

@implementation CBNativeDialog : NSObject

- (void)show:(NSString *)gameObject title:(NSString *)title message:(NSString *)message positiveButtonTitle:(NSString *)positiveButtonTitle positiveButtonAction:(NSString *)positiveButtonAction negativeButtonTitle:(NSString *)negativeButtonTitle negativeButtonAction:(NSString *)negativeButtonAction {
    UIAlertController *alertContoroller = [UIAlertController
                                           alertControllerWithTitle:title
                                           message:message
                                           preferredStyle:UIAlertControllerStyleAlert];
    if (negativeButtonTitle != nil) {
        [alertContoroller addAction:[UIAlertAction actionWithTitle:negativeButtonTitle
                                                             style:UIAlertActionStyleDefault
                                                           handler:^(UIAlertAction * _Nonnull action) {
            UnitySendMessage([gameObject UTF8String], [negativeButtonAction UTF8String], "");
        }]];
    }
    [alertContoroller addAction:[UIAlertAction actionWithTitle:positiveButtonTitle
                                                         style:UIAlertActionStyleDefault
                                                       handler:^(UIAlertAction * _Nonnull action) {
        UnitySendMessage([gameObject UTF8String], [positiveButtonAction UTF8String], "");
    }]];
    
    UIViewController *viewController = UnityGetGLViewController();
    [viewController presentViewController:alertContoroller animated:true completion:nil];
}
@end
