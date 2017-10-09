//
//  IAPManager.m
//  Unity-iPhone
// -fno-objc-arc
//  Created by MacMini on 14-5-16.
//
//

#import "IAPManager.h"


@implementation IAPManager

-(void) attachObserver{
    NSLog(@"AttachObserver");
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
}

-(BOOL) CanMakePayment{
    return [SKPaymentQueue canMakePayments];
}

//接收商品信息
-(void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response{
    NSArray *products = response.products;
    if ([products count] == 0) {
        return;
    }
    // SKProduct对象包含了在App Store上注册的商品的本地化信息。
    for (SKProduct *p in products) {
        UnitySendMessage("Main", "ShowProductList", [[self productInfo:p] UTF8String]);
    }
    //无效产品
    for(NSString *invalidProductId in response.invalidProductIdentifiers){
        NSLog(@"Invalid product id:%@",invalidProductId);
    }
    //自动释放
    //[request autorelease];
}

//请求产品数据
- (void)requestProductData:(NSString *)type {
    //根据商品ID查找商品信息
    NSArray *product = [[NSArray alloc] initWithObjects:type, nil];
    NSSet *nsset = [NSSet setWithArray:product];
    //创建SKProductsRequest对象，用想要出售的商品的标识来初始化， 然后附加上对应的委托对象。
    //该请求的响应包含了可用商品的本地化信息。
    SKProductsRequest *request = [[SKProductsRequest alloc] initWithProductIdentifiers:nsset];
    request.delegate = self;
    [request start];
}

//购买商品
-(void)buyRequest:(NSString *)productIdentifier{
    //创建一个支付对象，并放到队列中
    SKPayment *payment = [SKPayment paymentWithProductIdentifier:productIdentifier];
    //设置购买的数量
    //payment.quantity = 1;
    [[SKPaymentQueue defaultQueue] addPayment:payment];
}

//商品信息
-(NSString *)productInfo:(SKProduct *)product{
    NSArray *info = [NSArray arrayWithObjects:product.localizedTitle,product.localizedDescription,product.price,product.productIdentifier, nil];
    return [info componentsJoinedByString:@"\t"];
}

-(NSString *)encode:(const uint8_t *)input length:(NSInteger) length{
    static char table[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    
    NSMutableData *data = [NSMutableData dataWithLength:((length+2)/3)*4];
    uint8_t *output = (uint8_t *)data.mutableBytes;
    
    for(NSInteger i=0; i<length; i+=3){
        NSInteger value = 0;
        for (NSInteger j= i; j<(i+3); j++) {
            value<<=8;
            
            if(j<length){
                value |=(0xff & input[j]);
            }
        }
        
        NSInteger index = (i/3)*4;
        output[index + 0] = table[(value>>18) & 0x3f];
        output[index + 1] = table[(value>>12) & 0x3f];
        output[index + 2] = (i+1)<length ? table[(value>>6) & 0x3f] : '=';
        output[index + 3] = (i+2)<length ? table[(value>>0) & 0x3f] : '=';
    }
    
    return [[NSString alloc] initWithData:data encoding:NSASCIIStringEncoding];
}

-(NSString *)transactionInfo:(SKPaymentTransaction *)transaction{
    
    return [self encode:(uint8_t *)transaction.transactionReceipt.bytes length:transaction.transactionReceipt.length];
    
    //return [[NSString alloc] initWithData:transaction.transactionReceipt encoding:NSASCIIStringEncoding];
}

-(void) provideContent:(SKPaymentTransaction *)transaction{
    UnitySendMessage("Main", "ProvideContent", [[self transactionInfo:transaction] UTF8String]);
}

//监听购买结果
-(void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions{
    for (SKPaymentTransaction *tran in transactions) {
        switch (tran.transactionState) {
            case SKPaymentTransactionStatePurchased: // 如果小票状态是购买完成
                [[SKPaymentQueue defaultQueue] finishTransaction:tran];
                // 更新界面或者数据，把用户购买得商品交给用户
                //返回购买的商品信息
                [self verifyPruchase];
                //商品购买成功可调用本地接口
                break;
            case SKPaymentTransactionStateFailed:
                // 支付失败
                // 将交易从交易队列中删除
                [[SKPaymentQueue defaultQueue] finishTransaction:tran];
                break;
            case SKPaymentTransactionStateRestored:
                // 将交易从交易队列中删除
                [[SKPaymentQueue defaultQueue] finishTransaction:tran];
                break;
            default:
                break;
        }
    }
}

//交易结束
- (void)completeTransaction:(SKPaymentTransaction *)transaction {
    NSLog(@"交易结束");
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
}

- (void)request:(SKRequest *)request didFailWithError:(NSError *)error {
    NSLog(@"请求商品失败%@", error);
}

- (void)requestDidFinish:(SKRequest *)request {
    NSLog(@"反馈信息结束调用");
}

#pragma mark 验证购买凭据

- (void)verifyPruchase {
    // 验证凭据，获取到苹果返回的交易凭据
    // appStoreReceiptURL iOS7.0增加的，购买交易完成后，会将凭据存放在该地址
    NSURL *receiptURL = [[NSBundle mainBundle] appStoreReceiptURL];
    // 从沙盒中获取到购买凭据
    NSData *receiptData = [NSData dataWithContentsOfURL:receiptURL];
    // 发送网络POST请求，对购买凭据进行验证
    //测试验证地址:https://sandbox.itunes.apple.com/verifyReceipt
    //正式验证地址:https://buy.itunes.apple.com/verifyReceipt
    NSURL *url = [NSURL URLWithString:@"https://sandbox.itunes.apple.com/verifyReceipt"];
    NSMutableURLRequest *urlRequest =
    [NSMutableURLRequest requestWithURL:url cachePolicy:NSURLRequestUseProtocolCachePolicy timeoutInterval:10.0f];
    urlRequest.HTTPMethod = @"POST";
    NSString *encodeStr = [receiptData base64EncodedStringWithOptions:NSDataBase64EncodingEndLineWithLineFeed];
    NSString *payload = [NSString stringWithFormat:@"{\"receipt-data\" : \"%@\"}", encodeStr];
    NSData *payloadData = [payload dataUsingEncoding:NSUTF8StringEncoding];
    urlRequest.HTTPBody = payloadData;
    // 提交验证请求，并获得官方的验证JSON结果 iOS9后更改了另外的一个方法
    NSData *result = [NSURLConnection sendSynchronousRequest:urlRequest returningResponse:nil error:nil];
    // 官方验证结果为空
    if (result == nil) {
        NSLog(@"验证失败");
        return;
    }
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:result options:NSJSONReadingAllowFragments error:nil];
    if (dict != nil) {
        // 比对字典中以下信息基本上可以保证数据安全
        // bundle_id , application_version , product_id , transaction_id
        NSLog(@"验证成功！购买的商品是：%@", @"_productName");
    }
}

@end
