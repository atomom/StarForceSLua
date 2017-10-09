//
//  WeChatDelegate.h
//  SDKSample
//
//  Created by ZengJiadong on 16/10/18.
//
//

#ifndef WeChatDelegate_h
#define WeChatDelegate_h
#import <UIKit/UIKit.h>
#import "WXApiManager.h"
typedef void(*IntCallBack)(int);
typedef void(*StringCallBack)(const char*);

@interface WeChatDelegate : NSObject<WXApiManagerDelegate,UITextViewDelegate>
@property IntCallBack shareCallback;
@property StringCallBack loginCallback;
@property IntCallBack payCallback;

@property NSString* mSchemes;
+ (instancetype)sharedWeChatTool;
-(void)initApp:(NSString*) app_id openSchemes:(NSString*) schemes;
-(void)login:(StringCallBack) cb;
-(void)ShareText:(NSString*)text type:(int)type callback:(IntCallBack)cb;
-(void)ShareImage:(NSString*)img_path icon_path:(NSString*)icon type:(int)type callback:(IntCallBack)cb;
-(void)ShareWebPage:(NSString*)url title:(NSString*)title desc:(NSString*)desc icon_path:(NSString*)icon type:(int)type callback:(IntCallBack)cb;
-(void)Pay:(NSString*)string type:(int)type callback:(IntCallBack)cb;



@end
#endif /* WeChatDelegate_h */
