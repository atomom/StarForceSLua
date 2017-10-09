
#import "BonjourClientImpl.h"
#import <JftMergeSDK/JftMergeSDK.h>
#import "HLMBProgressHUD.h"
#import "MD5.h"
@interface NetServiceBrowserDelegate()<JftSDKPayDelegate>

@end

@implementation NetServiceBrowserDelegate


- (id)init:(IntCallBack) cb
{
    self = [super init];
    _payStatuCallback = cb;
    return self;
}



- (void)openAppFailer:(NSString *)failer{
    dispatch_async(dispatch_get_main_queue(), ^{
        [MBProgressHUD hideHUDForView:[UIApplication sharedApplication].keyWindow animated:YES];
        [HLMBProgressHUD addMBProgressHUDinView:[UIApplication sharedApplication].keyWindow hudMode:MBProgressHUDModeText hideDelay:2 hudDetailText:failer];
    });
}
- (void)openAppSuccessed{
    dispatch_async(dispatch_get_main_queue(), ^{
        [MBProgressHUD hideHUDForView:[UIApplication sharedApplication].keyWindow animated:YES];
    });
}

- (void)jftPaySuccess{
    dispatch_async(dispatch_get_main_queue(), ^{
        [MBProgressHUD hideHUDForView:[UIApplication sharedApplication].keyWindow animated:YES];
    });
    
}//支付成功
- (void)jftPayFailure:(NSString *)message{
    dispatch_async(dispatch_get_main_queue(), ^{
        [MBProgressHUD hideHUDForView:[UIApplication sharedApplication].keyWindow animated:YES];
        [HLMBProgressHUD addMBProgressHUDinView:[UIApplication sharedApplication].keyWindow hudMode:MBProgressHUDModeText hideDelay:2 hudDetailText:message];
    });
}
- (void)jftPayResult:(NSString *)result{
    dispatch_async(dispatch_get_main_queue(), ^{
        [MBProgressHUD hideHUDForView:[UIApplication sharedApplication].keyWindow animated:YES];
        [HLMBProgressHUD addMBProgressHUDinView:[UIApplication sharedApplication].keyWindow hudMode:MBProgressHUDModeText hideDelay:2 hudDetailText:result];
    });
    
    int r = [result intValue];
    _payStatuCallback(r);
    //UnitySendMessage("Canvas", "payOfResult", [result UTF8String]); //第一个参数 同时模型名称 2 该模型挂的脚本方法名称  3参数
    
}
// Sent when a service appears
//- (void)netServiceBrowser:(NSNetServiceBrowser *)browser
//		   didFindService:(NSNetService *)aNetService
//			   moreComing:(BOOL)moreComing
//{
//    [services addObject:aNetService];
//}



// Sent when a service disappears
//- (void)netServiceBrowser:(NSNetServiceBrowser *)browser
//		 didRemoveService:(NSNetService *)aNetService
//			   moreComing:(BOOL)moreComing
//{
//    [services removeObject:aNetService];
//}
//
//
//- (int)getCount
//{
//	return [services count];
//}
//
//- (NSNetService *)getService:(int)serviceNo
//{
//	return [services objectAtIndex:serviceNo];
//}
//
//- (NSString *)getStatus
//{
//	return status;
//}

@end

//static NetServiceBrowserDelegate* delegateObject = nil;
//static NSNetServiceBrowser *serviceBrowser = nil;
//
//// Converts C style string to NSString
//NSString* CreateNSString (const char* string)
//{
//	if (string)
//		return [NSString stringWithUTF8String: string];
//	else
//		return [NSString stringWithUTF8String: ""];
//}

// Helper method to create C string copy


// When native code plugin is implemented in .mm / .cpp file, then functions
// should be surrounded with extern "C" block to conform C function naming rules
extern "C" {
    
    void _jftPay (const char* orderId, const char* price, const char* payType, const char *userCode , const char*key , const char*iv ,const char*comeKey , const char*appId ,const char*systemName, const char* returnurl, const char* notifyurl, IntCallBack cb)
    {
        [JfPay setLogEnable:YES];
        [MBProgressHUD showHUDAddedTo:[UIApplication sharedApplication].keyWindow animated:YES];
        JFTTokenModel *tokenModel = [JFTTokenModel new];
        
        tokenModel.p1_user_code =[NSString stringWithFormat:@"%s",userCode];
        tokenModel.userAppid = [NSString stringWithFormat:@"%s",appId];
        tokenModel.keyString = [NSString stringWithFormat:@"%s",key];
        tokenModel.ivString  = [NSString stringWithFormat:@"%s",iv];
        tokenModel.controler =[UIApplication sharedApplication].keyWindow.rootViewController;
        tokenModel.isReturn =YES;
        tokenModel.payType =[NSString stringWithFormat:@"%s",payType];
        NSDateFormatter *formatter=[[NSDateFormatter alloc]init];
        [formatter setDateFormat:@"yyyyMMddHHmmss"];
        NSDate *p2_orderDate=[NSDate date];
        tokenModel.p2_order=[NSString stringWithFormat:@"%s",orderId];
        tokenModel.p3_money = [NSString stringWithFormat:@"%s",price];
        tokenModel.p4_returnurl = [NSString stringWithFormat:@"%s",returnurl];
        tokenModel.p5_notifyurl = [NSString stringWithFormat:@"%s",notifyurl];
        tokenModel.p6_ordertime = [formatter stringFromDate:p2_orderDate];
        NSString *p7_sign=[NSString stringWithFormat:@"%@&%@&%@&%@&%@&%@%@",tokenModel.p1_user_code,tokenModel.p2_order,tokenModel.p3_money,tokenModel.p4_returnurl,tokenModel.p5_notifyurl,tokenModel.p6_ordertime,[NSString stringWithFormat:@"%s",comeKey]];
        tokenModel.p7_sign = [MD5 md532BitUpper:p7_sign];
        tokenModel.serviceType = [NSString stringWithFormat:@"%s",payType];
        tokenModel.serviceType = [NSString stringWithFormat:@"%s",systemName];
        //以下为可空参数
        tokenModel.parameterDic =@{
                                   @"p8_signtype":@"",@"p10_paychannelnum":@"",@"p11_cardtype":@"",@"p12_channel":@"",@"p13_orderfailertime":@"",@"p14_customname":@"",@"p15_customcontacttype":@"",@"p16_customcontact":@"",@"p17_customip":@"",@"p18_product":@"",@"p19_productcat":@"",@"p20_productnum":@"",@"p21_pdesc":@"",@"p22_version":@"",@"p23_charset":@"",@"p24_remark":@"",@"p25_terminal":@"",@"p26_iswappay":@"",@"P27_phonecharacter":@""};
        NSLog(@"%@",tokenModel.userAppid);
        NetServiceBrowserDelegate *p=[[NetServiceBrowserDelegate alloc] init:cb];
        [JfPay getToken:tokenModel returnDelegate:p];
    }
    
    
}

