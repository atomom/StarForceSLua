//
//  IAPManager.h
//  Unity-iPhone
//
//  Created by MacMini on 14-5-16.
//
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

@interface IAPManager : NSObject<SKProductsRequestDelegate, SKPaymentTransactionObserver>{
    SKProduct *proUpgradeProduct;
    SKProductsRequest *productsRequest;
}

-(void)attachObserver;

-(BOOL)CanMakePayment;
//请求产品数据
-(void)requestProductData:(NSString *)productIdentifiers;
//下单购买
-(void)buyRequest:(NSString *)productIdentifier;
-(NSString *)encode:(const uint8_t *)input length:(NSInteger) length;
-(NSString *)transactionInfo:(SKPaymentTransaction *)transaction;
@end
