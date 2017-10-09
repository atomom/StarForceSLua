//
//  WeChatTool.m
//  SDKSample
//
//  Created by ZengJiadong on 16/10/18.
//
//

#import <Foundation/Foundation.h>
#import "WeChatDelegate.h"
#import "AppDelegateListener.h"
#import "Foundation/NSJSONSerialization.h"
#import "UnityAppController.h"
extern "C"
{
    void WechatInit(const char* app_id,const char* openSchemes);
    void WechatLogin(StringCallBack cb);
    void WechatShareText(const char* text,int type,IntCallBack cb);
    void WechatShareImage(const char* img_path,const char* icon_path,int type,IntCallBack cb);
    void WechatShareWebPage(const char* url,const char* title,const char* desc, const char* icon_path,int type,IntCallBack cb);
    bool isWXAppInstalled();
    const char* GetShemesArgs();
    void GamePay(const char* text,int type, IntCallBack cb);
}
@implementation WeChatDelegate

+(instancetype)sharedWeChatTool
{
    static dispatch_once_t onceToken;
    static WeChatDelegate *instance;
    dispatch_once(&onceToken, ^{
        instance = [[WeChatDelegate alloc] init];
    });
    return instance;
}

/**
 * 解析URL参数的工具方法。
 */
+ (NSDictionary *)parseURLParams:(NSString *)query{
    NSArray *pairs = [query componentsSeparatedByString:@"&"];
    NSMutableDictionary *params = [[NSMutableDictionary alloc] init];
    for (NSString *pair in pairs) {
        NSArray *kv = [pair componentsSeparatedByString:@"="];
        if (kv.count == 2) {
            NSString *val =[[kv objectAtIndex:1] stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
            [params setObject:val forKey:[kv objectAtIndex:0]];
        }
    }
    return params;
}

- (void)onOpenURL:(NSNotification*)notification
{
    NSDictionary *userInfo = notification.userInfo;
    NSURL* url = [userInfo valueForKey:@"url"];
    if(url != nil)
    {
        if([url.scheme isEqualToString:_mSchemes]) {
            
            return;
        }
        [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
    }
}

-(void)initApp:(NSString*) app_id openSchemes:(NSString*) schemes
{
    _mSchemes = schemes;
    
    [WXApi registerApp:app_id];
    [WXApiManager sharedManager].delegate = self;
    [[NSNotificationCenter defaultCenter] removeObserver:self name:kUnityOnOpenURL object:nil];
    [[NSNotificationCenter defaultCenter]	addObserver:self
                                             selector:@selector(onOpenURL:)
                                                 name:kUnityOnOpenURL
                                               object:nil];
}
-(void)dealloc
{
    [[NSNotificationCenter defaultCenter] removeObserver:self name:kUnityOnOpenURL object:nil];
}

-(void)login:(StringCallBack) cb
{
    _loginCallback = cb;
    SendAuthReq* req = [[SendAuthReq alloc] init];
    req.scope = @"snsapi_userinfo";
    req.state = @"wechatlogin";
    [WXApi sendReq:req];
}
-(void)ShareText:(NSString*)text type:(int)type callback:(IntCallBack)cb
{
    _shareCallback = cb;
    SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
    req.bText = YES;
    req.scene = type;
    req.text = text;
    [WXApi sendReq:req];
}
-(void)ShareImage:(NSString*)img_path icon_path:(NSString*)icon type:(int)type callback:(IntCallBack)cb
{
    _shareCallback = cb;
    WXMediaMessage *message = [WXMediaMessage message];
    
    UIImage *thumbImage = [UIImage imageWithContentsOfFile:icon];
    WXImageObject *imageObject = [WXImageObject object];
    [imageObject setImageData:[NSData dataWithContentsOfFile: img_path]];
    message.mediaObject = imageObject;
    [message setThumbImage:thumbImage];
    
    SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
    req.bText = NO;
    req.message = message;
    req.scene = type;
    [WXApi sendReq:req];
}
-(void)ShareWebPage:(NSString*)url title:(NSString*)title desc:(NSString*)desc icon_path:(NSString*)icon type:(int)type callback:(IntCallBack)cb
{
    _shareCallback = cb;
    WXMediaMessage *message = [WXMediaMessage message];
    message.title = title;
    message.description = desc;
    UIImage *thumbImage = [UIImage imageWithContentsOfFile:icon];
    WXWebpageObject *webpageObject = [WXWebpageObject object];
    [webpageObject setWebpageUrl:url];
    message.mediaObject = webpageObject;
    [message setThumbImage:thumbImage];
    
    SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
    req.bText = NO;
    req.message = message;
    req.scene = type;
    [WXApi sendReq:req];
}

-(void)Pay:(NSString*)string type:(int)type callback:(IntCallBack)cb
{    
	_payCallback = cb;
    if (string && 0 != string.length)
    {
        NSError *error;
        NSData *jsonData = [string dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *resp = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&error];
        
        if (error)
        {
            NSLog(@"json解析失败：%@", error);
        }
        
        // 调起微信支付
        PayReq *request = [[PayReq alloc] init];
        request.partnerId = [resp objectForKey:@"partnerid"];
        request.prepayId = [resp objectForKey:@"prepayid"];
        request.package = @"Sign=WXPay";
        request.sign = [resp objectForKey:@"sign"];
        request.nonceStr = [resp objectForKey:@"noncestr"];
        request.timeStamp = [[resp objectForKey:@"timestamp"] intValue];
    
        [WXApi sendReq:request];
    }
}

void WechatInit(const char* app_id,const char* openSchemes)
{
    NSString* _id = [NSString stringWithUTF8String:app_id];
    NSString* schemes = [NSString stringWithUTF8String:openSchemes];
    [[WeChatDelegate sharedWeChatTool] initApp:_id openSchemes:schemes];
}

void WechatLogin(StringCallBack cb)
{
    [[WeChatDelegate sharedWeChatTool] login:cb];
}
void WechatShareText(const char* text,int type,IntCallBack cb)
{
    [[WeChatDelegate sharedWeChatTool] ShareText:[NSString stringWithUTF8String:text] type:type callback:cb];
}
void WechatShareImage(const char* img_path,const char* icon_path,int type,IntCallBack cb)
{
    [[WeChatDelegate sharedWeChatTool]ShareImage:[NSString stringWithUTF8String:img_path] icon_path:[NSString stringWithUTF8String:icon_path] type:type callback:cb];
}
void WechatShareWebPage(const char* url,const char* title,const char* desc, const char* icon_path,int type,IntCallBack cb)
{
    [[WeChatDelegate sharedWeChatTool]ShareWebPage:[NSString stringWithUTF8String:url] title:[NSString stringWithUTF8String:title]  desc:[NSString stringWithUTF8String:desc]  icon_path:[NSString stringWithUTF8String:icon_path]  type:type callback:cb];
}

bool isWXAppInstalled()
{
    return [WXApi isWXAppInstalled];
}

void GamePay(const char* json,int type,IntCallBack cb)
{
    [[WeChatDelegate sharedWeChatTool] Pay:[NSString stringWithUTF8String:json] type:type callback:cb];
}

const char* GetShemesArgs()
{
    UnityAppController* app = GetAppController();
    NSURL* url = app.mUrl;
    if(url == nil) return NULL;
    NSString* path = [url path];
    NSString* _mSchemesArgs;
    if(path != nil && path.length != 0)
    {
        NSString* json = @"{\"path\":\"%@\",\"query\":\"%@\"}";
        _mSchemesArgs = [NSString stringWithFormat:json,[url path],[url query]];
        
        NSLog(@"GetShemesArgs %@",_mSchemesArgs);
    }
    app.mUrl = nil;
    
    if(_mSchemesArgs == nil) return NULL;
    
    return strdup([_mSchemesArgs UTF8String]);
}

#pragma mark - WXApiManagerDelegate
- (void)managerDidRecvGetMessageReq:(GetMessageFromWXReq *)req {
//    // 微信请求App提供内容， 需要app提供内容后使用sendRsp返回
//    NSString *strTitle = [NSString stringWithFormat:@"微信请求App提供内容"];
//    NSString *strMsg = [NSString stringWithFormat:@"openID: %@", req.openID];
//    
//    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:strTitle
//                                                    message:strMsg
//                                                   delegate:self
//                                          cancelButtonTitle:@"OK"
//                                          otherButtonTitles:nil, nil];
//    alert.tag = 1000;
//    [alert show];
    
}

- (void)managerDidRecvShowMessageReq:(ShowMessageFromWXReq *)req {
//    WXMediaMessage *msg = req.message;
//    
//    //显示微信传过来的内容
//    WXAppExtendObject *obj = msg.mediaObject;
//    
//    NSString *strTitle = [NSString stringWithFormat:@"微信请求App显示内容"];
//    NSString *strMsg = [NSString stringWithFormat:@"openID: %@, 标题：%@ \n内容：%@ \n附带信息：%@ \n缩略图:%lu bytes\n附加消息:%@\n", req.openID, msg.title, msg.description, obj.extInfo, (unsigned long)msg.thumbData.length, msg.messageExt];
//    
//    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:strTitle
//                                                    message:strMsg
//                                                   delegate:self
//                                          cancelButtonTitle:@"OK"
//                                          otherButtonTitles:nil, nil];
//    [alert show];
   
}

- (void)managerDidRecvLaunchFromWXReq:(LaunchFromWXReq *)req {
//    WXMediaMessage *msg = req.message;
//    
//    //从微信启动App
//    NSString *strTitle = [NSString stringWithFormat:@"从微信启动"];
//    NSString *strMsg = [NSString stringWithFormat:@"openID: %@, messageExt:%@", req.openID, msg.messageExt];
//    
//    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:strTitle
//                                                    message:strMsg
//                                                   delegate:self
//                                          cancelButtonTitle:@"OK"
//                                          otherButtonTitles:nil, nil];
//    [alert show];
    
}

/**
 * 分享回调
 */
- (void)managerDidRecvMessageResponse:(SendMessageToWXResp *)response {
//    NSString *strTitle = [NSString stringWithFormat:@"发送媒体消息结果"];
//    NSString *strMsg = [NSString stringWithFormat:@"errcode:%d", response.errCode];
//    
//    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:strTitle
//                                                    message:strMsg
//                                                   delegate:self
//                                          cancelButtonTitle:@"OK"
//                                          otherButtonTitles:nil, nil];
//    [alert show];
    
    if(_shareCallback != NULL)
    {
        _shareCallback(response.errCode);
    }
}

- (void)managerDidRecvAddCardResponse:(AddCardToWXCardPackageResp *)response {
//    NSMutableString* cardStr = [[NSMutableString alloc] init];
//    for (WXCardItem* cardItem in response.cardAry) {
//        [cardStr appendString:[NSString stringWithFormat:@"cardid:%@ cardext:%@ cardstate:%u\n",cardItem.cardId,cardItem.extMsg,(unsigned int)cardItem.cardState]];
//    }
//    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"add card resp"
//                                                    message:cardStr
//                                                   delegate:self
//                                          cancelButtonTitle:@"OK"
//                                          otherButtonTitles:nil, nil];
//    [alert show];
   
}

/**
 * 登陆回调
 */
- (void)managerDidRecvAuthResponse:(SendAuthResp *)response {
  
    if(_loginCallback != NULL)
    {
        NSString* json = @"{\"code\":\"%@\",\"state\":\"%@\",\"errCode\":\"%d\",\"lang\":\"%@\",\"country\":\"%@\"}";
        
        json = [NSString stringWithFormat:json,response.code,response.state,response.errCode,response.lang,response.country];
        _loginCallback([json UTF8String]);
        json = nil;
    }
}


@end


