#import <Foundation/Foundation.h>


typedef void(*IntCallBack)(int);
@interface NetServiceBrowserDelegate : NSObject

@property IntCallBack payStatuCallback;
@end

