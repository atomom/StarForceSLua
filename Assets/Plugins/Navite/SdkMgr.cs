using System;
using System.Runtime.InteropServices;
using Native;
using Umeng;
using SimpleJson;
using SLua;

namespace Native
{
    public class SdkMgr
    {
        public static void JTPayInit(string pay_systemname, string pay_code, string pay_appid, string pay_comkey, string pay_key, string pay_vector, string pay_return_url, string pay_notice_url)
        {
            JTPayTool.SYSTEM_NAME = pay_systemname;
            JTPayTool.CODE = pay_code;
            JTPayTool.APPID = pay_appid;
            JTPayTool.COM_KEY = pay_comkey;
            JTPayTool.KEY = pay_key;
            JTPayTool.VECTOR = pay_vector;
            JTPayTool.RETURN_URL = pay_return_url;
            JTPayTool.NOTICE_URL = pay_notice_url;
        }

        public static void BuglyInit(string iOS_ID, string android_ID, bool isDebug)
        {
            //控制是否开启SDK的调试信息打印，接入调试时可设置为true，正式发布版本请设置为false或注释掉
#if UNITY_IPHONE || UNITY_IOS
            BuglyAgent.ConfigDebugMode(isDebug);
    	    BuglyAgent.InitWithAppId (iOS_ID);
            BuglyAgent.EnableExceptionHandler();
#elif UNITY_ANDROID
            BuglyAgent.ConfigDebugMode(isDebug);
            BuglyAgent.InitWithAppId(android_ID);
            BuglyAgent.EnableExceptionHandler();
#endif
        }

        /// <summary>
        /// 友盟
        /// </summary>
        /// <param name="iOS_ID"></param>
        /// <param name="android_ID"></param>
        public static void UmengInit(string iOS_ID, string android_ID)
        {
            //控制是否开启SDK的调试信息打印，接入调试时可设置为true，正式发布版本请设置为false或注释掉

#if UNITY_IPHONE || UNITY_IOS
            GA.SetCrashReportEnabled(false);
            GA.StartWithAppKeyAndChannelId(iOS_ID, "ios");
#elif UNITY_ANDROID
            GA.StartWithAppKeyAndChannelId(android_ID, "ios");
#endif
            GA.SetLogEnabled(false);

        }
        #region 微信

        public static void AppStoreInit()
        {
#if UNITY_IPHONE || UNITY_IOS
            WeChatTool.InitIAPManager();
#endif
        }

        public static void WeChatInit(string App_ID,string Schemes)
        {
            WeChatTool.AppID = App_ID;
            WeChatTool.Schemes = Schemes;
            WeChatTool.getInstance();
        }

        public static bool isWeChatInstalled()
        {
            return WeChatTool.isWeChatInstalled();
        }

        public static string getShemesArgs()
        {
            return WeChatTool.getShemesArgs();
        }

        public static void WeChatLogin(Action<string> callback)
        {
            WeChatTool.getInstance().Login(callback);
        }

        public static void WeChatShareText(string text, WechatShareType type, Action<int> callback)
        {
            WeChatTool.getInstance().ShareText(text, type, callback);
        }
        public static void WeChatShareImage(string img_path, string icon_path, WechatShareType type, Action<int> callback)
        {
            WeChatTool.getInstance().ShareImage(img_path, icon_path, type, callback);
        }

        public static void WeChatShareWepPage(string url, string title, string des, string icon_path, WechatShareType type, Action<int> callback)
        {
            WeChatTool.getInstance().ShareWepPage(url, title, des, icon_path, type, callback);
        }

        public static void GamePay(string json, GamePayType type, Action<int> callback)
        {
            //JsonObject obj = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(json);
            WeChatTool.getInstance().Payment(json, (int)type, callback);
        }

        public static void CopyToClipboard(string json)
        {
            WeChatTool.getInstance().CopyToClipboard(json);
        }

        public static int GetTeleSignalStrength()
        {

#if UNITY_EDITOR || UNITY_IOS
            return 3;
#elif UNITY_ANDROID
            return AndroidNative.GetTeleSignalStrength();
#else
            return 0;
#endif
        }

        public static int GetWIFISignalStrength(int lv)
        {
#if UNITY_EDITOR || UNITY_IOS
            return lv - 2;
#elif UNITY_ANDROID
            return AndroidNative.GetWIFISignalStrength(lv);
#else
            return 0;
#endif
        }

        public static float BatteryLevel()
        {
#if UNITY_EDITOR
            return 3;
#elif UNITY_IOS
            return iOSBatteryLevel();
#elif UNITY_ANDROID
            return AndroidNative.BatteryLevel();
#else
            return 0;
#endif
        }

        public static string getIpv6(string mHost)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
		    string mIPv6 = iOSGetIPv6(mHost);
		    return mIPv6;
#else
            return mHost + "&&ipv4";
#endif
        }
        
     
#endregion

#if UNITY_IOS
#region IOS Native
        private const string Native = "__Internal";

        [DllImport(Native)]
        private static extern float iOSBatteryLevel();
        [DllImport(Native)]
        private static extern string iOSGetIPv6(string ip);

        //[DllImport(Native)]
        //private static extern int iOSGetWIFISignalStrength();

        //[DllImport(Native)]
        //private static extern int iOSGetTeleSignalStrength();

#endregion
#endif
    }
}
