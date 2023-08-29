//
//  unity_ios_plugin_sample.m
//  BackDemoiOSApp
//
//  Created by Paras Modi on 29/08/23.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

// you can create diffrent file named omrlib.h but its not necessary. demo is simple.
@interface omrlib : NSObject

- (void) showMessage:(NSString *)title secondValue:(NSString *)message;
- (void) showShareDialog:(NSString *)text secondValue:(NSString *)url thirdValue:(NSString *)imagepath;
- (void) closeUnityAndReturnToiOS:(NSString *)description secondValue:(NSString *)msg;

@end

#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]
extern void _showMessage(const char* title, const char* message)
{
    omrlib* mylib = [[omrlib alloc] init];
    [mylib showMessage:GetStringParam(title) secondValue:GetStringParam(message)];
}
extern void _showShareDialog(const char* text, const char* url, const char* imagepath)
{
    omrlib* mylib = [[omrlib alloc] init];
    [mylib showShareDialog:GetStringParam(text) secondValue:GetStringParam(url) thirdValue:GetStringParam(imagepath)];
}
extern void _closeUnityAndReturnToiOS(const char* description, const char* msg)
{
    omrlib* mylib = [[omrlib alloc] init];
    [mylib closeUnityAndReturnToiOS:GetStringParam(description) secondValue:GetStringParam(msg)];
}

extern UIViewController *UnityGetGLViewController(); // Root view controller of Unity screen.

@implementation omrlib
- (void) showMessage:(NSString *)title secondValue:(NSString *)message
{
    UIAlertView * alert = [[UIAlertView alloc]initWithTitle:title
                                                    message:message
                                                   delegate:nil
                                          cancelButtonTitle:@"OK"
                                          otherButtonTitles:nil, nil];
    
    alert.alertViewStyle = UIAlertViewStyleDefault;
    [alert show];
}

- (void) showShareDialog:(NSString *)text secondValue:(NSString *)url thirdValue:(NSString *)imagepath
{
    UIImage *img = [[UIImage alloc] initWithContentsOfFile:imagepath];
    NSArray * activityItems = @[img, text, [NSURL URLWithString:url]];
    NSArray * applicationActivities = nil;
    NSArray * excludeActivities = @[UIActivityTypeAssignToContact, UIActivityTypePostToWeibo, UIActivityTypePrint];
    
    UIActivityViewController * activityController = [[UIActivityViewController alloc] initWithActivityItems:activityItems applicationActivities:applicationActivities];
    activityController.excludedActivityTypes = excludeActivities;
    
    [UnityGetGLViewController() presentViewController:activityController animated:YES completion:nil];
}

- (void) closeUnityAndReturnToiOS:(NSString *)description secondValue:(NSString *)msg
{
    NSLog(@"Description = %@ message = %@", description,msg);
    UIViewController *unityViewController = UnityGetGLViewController();
    [unityViewController dismissViewControllerAnimated:YES completion:nil];
}


@end
