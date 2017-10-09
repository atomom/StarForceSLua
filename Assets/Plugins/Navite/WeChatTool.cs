using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using LitJson;

namespace Native
{
    public enum WechatShareType
    {
        Wechat,
        Friend
    }

    public enum GamePayType
    {
        none,
        Wxpay,
        Alipay
    }

    public class WeChatTool
    {
        public delegate void IntCallBack(int p);
        public delegate void StringCallBack(string p);

        public static string AppID;
        static WeChatTool _ins;

        /// <summary>
        /// 游戏登陆回调
        /// </summary>
        static Action<string> mLoginCallBack;
        /// <summary>
        /// 分享游戏回调
        /// </summary>
        static Action<int> mShareCallBack;
        /// <summary>
        /// 游戏支付回调
        /// </summary>
        static Action<int> mGamePayCallBack;

        public static string Schemes { get; internal set; }
#if UNITY_EDITOR
#elif UNITY_ANDROID
        AndroidJavaObject wechat;
#endif
        public WeChatTool()
        {
            UnityThreadHelper.EnsureHelper();
#if UNITY_EDITOR
#elif UNITY_ANDROID
        using (var mWeChatTool = new AndroidJavaClass("com.sean.wechat.WeChatTool"))
            {
                wechat = mWeChatTool.CallStatic<AndroidJavaObject>("Create", AppID);
            }
#elif UNITY_IOS
            WechatInit(AppID,Schemes);
#endif

        }
        static public WeChatTool getInstance()
        {
            if (_ins == null)
            {
                _ins = new WeChatTool();
            }
            return _ins;
        }
        static public bool isWeChatInstalled()
        {
#if !UNITY_EDITOR && UNITY_IOS
            return isWXAppInstalled();
#else
            return true;
#endif
        }

        /// <summary>
        /// 获取从别的应用跳转过来的传参
        /// </summary>
        /// <returns></returns>
        static public string getShemesArgs()
        {
#if UNITY_EDITOR         
            
            return "";
#elif UNITY_IOS
            var args = GetShemesArgs();
            return args;
#elif UNITY_ANDROID
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject intent = jo.Call<AndroidJavaObject>("getIntent");
                using (AndroidJavaClass Intent = new AndroidJavaClass("android.content.Intent"))
                {
                    string action = intent.Call<string>("getAction");
                    string action_view = Intent.GetStatic<string>("ACTION_VIEW");
                    if (action == action_view)
                    {
                        AndroidJavaObject uri = intent.Call<AndroidJavaObject>("getData");
                        if (uri == null) return "";
                        string path = uri.Call<string>("getPath");
                        string query = uri.Call<string>("getQuery");
                        if (!string.IsNullOrEmpty(path))
                        {
                            jo.Call("setIntent",new AndroidJavaObject("android.content.Intent"));
                            Dictionary<string, string> node = new Dictionary<string, string>();
                            node.Add("path",path);
                            node.Add("query", query);
                            string v = LitJson.JsonMapper.ToJson(node);
                            return v;
                        }
                    }
                }
            }
            return "";
#else
            return "";
#endif
        }


        [AOT.MonoPInvokeCallback(typeof(StringCallBack))]
        static public void loginCallback(string data)
        {
            if (mLoginCallBack != null)
            {
                UnityThreadHelper.Dispatcher.Dispatch(() =>
                {
                    mLoginCallBack(data);
                    mLoginCallBack = null;
                });
            }
        }
        public void Login(Action<string> callback)
        {
            mLoginCallBack = callback;
#if UNITY_EDITOR
            loginCallback("{}");
#elif UNITY_ANDROID
           
            IntPtr fn = Marshal.GetFunctionPointerForDelegate((StringCallBack)loginCallback);
            
            wechat.Call("login", (int)fn);
#elif UNITY_IOS
            WechatLogin(loginCallback);
#endif

        }

        [AOT.MonoPInvokeCallback(typeof(IntCallBack))]
        static public void shareCallback(int err)
        {
            if (mShareCallBack != null)
            {
                UnityThreadHelper.Dispatcher.Dispatch(() =>
                {
                    mShareCallBack(err);
                    mShareCallBack = null;
                });
            }
        }

        public void ShareText(string text, WechatShareType type, Action<int> callback)
        {
            mShareCallBack = callback;
#if UNITY_EDITOR
            shareCallback(0);
#elif UNITY_ANDROID
            IntPtr fn = Marshal.GetFunctionPointerForDelegate((IntCallBack)shareCallback);
            wechat.Call("ShareText", text, (int)type, (int)fn);
           
#elif UNITY_IOS
            WechatShareText(text,(int)type, shareCallback);
#endif
        }

        public void ShareImage(string img_path, string icon_path, WechatShareType type, Action<int> callback)
        {
            mShareCallBack = callback;
#if UNITY_EDITOR
            shareCallback(0);
#elif UNITY_ANDROID
            
            IntPtr fn = Marshal.GetFunctionPointerForDelegate((IntCallBack)shareCallback);
            wechat.Call("ShareImage", img_path, icon_path, (int)type, (int)fn);
           
#elif UNITY_IOS
            WechatShareImage(img_path, icon_path, (int)type, shareCallback);
#endif

        }

        public void ShareWepPage(string url, string title, string des, string icon_path, WechatShareType type, Action<int> callback)
        {
            mShareCallBack = callback;
#if UNITY_EDITOR
            shareCallback(0);
#elif UNITY_ANDROID
            IntPtr fn = Marshal.GetFunctionPointerForDelegate((IntCallBack)shareCallback);
            wechat.Call("ShareWebPage", url, title, des, icon_path, (int)type, (int)fn);
           
#elif UNITY_IOS
            WechatShareWebPage(url, title, des, icon_path, (int)type, shareCallback);
#endif

        }
#if UNITY_IOS
        #region IOS Native

        private const string Native = "__Internal";

        [DllImport(Native)]
        private static extern void WechatInit(string app_id,string schemes);

        [DllImport(Native)]
        private static extern string GetShemesArgs();

        [DllImport(Native)]
        private static extern bool isWXAppInstalled();

        [DllImport(Native)]
        private static extern bool WechatLogin(StringCallBack cb);

        [DllImport(Native)]
        private static extern void WechatShareText(string text, int type, IntCallBack cb);

        [DllImport(Native)]
        private static extern void WechatShareImage(string img_path, string icon_path, int type, IntCallBack cb);

        [DllImport(Native)]
        private static extern void GamePay(string json, int type, IntCallBack cb);
        
        [DllImport (Native)]  
        private static extern void _copyTextToClipboard(string text);  

        /// <summary>
        /// 分享网页
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="title">标题</param>
        /// <param name="desc">描述</param>
        /// <param name="icon_path">icon路径</param>
        /// <param name="type">分享类型</param>
        /// <param name="cb">回调</param>
        [DllImport(Native)]
        private static extern void WechatShareWebPage(string url, string title, string desc, string icon_path, int type, IntCallBack cb);
        
         
        public List<string> productInfo = new List<string>();

        [DllImport (Native)] 
		public static extern void InitIAPManager();//初始化
	
        [DllImport (Native)] 
		public static extern bool IsProductAvailable();//判断是否可以购买
	
        [DllImport (Native)] 
		public static extern void RequstProductInfo(string s);//获取商品信息
	
        [DllImport (Native)] 
		public static extern void BuyProduct(string s);//购买商品

        #endregion
#endif

        [AOT.MonoPInvokeCallback(typeof(IntCallBack))]
        static public void gamePayCallback(int data)
        {
            if (mGamePayCallBack != null)
            {
                UnityThreadHelper.Dispatcher.Dispatch(() =>
                {
                    mGamePayCallBack(data);
                    mGamePayCallBack = null;
                });
            }
        }

        /// <summary>
        /// 游戏支付
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type">支付类型 1微信/ 2支付宝</param>
        /// <param name="callback"></param>
        public void Payment(string json, int type, Action<int> callback)
        {
            mGamePayCallBack = callback;
#if UNITY_EDITOR
            gamePayCallback(0);
#elif UNITY_ANDROID           
            IntPtr fn = Marshal.GetFunctionPointerForDelegate((IntCallBack)gamePayCallback);            
            wechat.Call("GamePay", json, type, (int)fn);
#elif UNITY_IOS
            //string json = SimpleJson.SimpleJson.SerializeObject(obj);
            GamePay(json, type, gamePayCallback);
#endif
        }


        public void CopyToClipboard(string input)
        {

#if UNITY_EDITOR
            TextEditor t = new TextEditor();
            //t.content = new GUIContent(input);
            t.text = input;
            t.OnFocus();
            t.Copy();
#elif UNITY_IPHONE
            _copyTextToClipboard(input);  
#elif UNITY_ANDROID
            AndroidJavaObject androidObject = new AndroidJavaObject("com.sean.wechat.ClipboardTools");
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            if (activity == null)
	            return ;

            // 复制到剪贴板
            androidObject.Call("copyTextToClipboard", activity, input);

            // 从剪贴板中获取文本
            String text =androidObject.Call<String>("getTextFromClipboard");
#endif
        }

        public List<string> GetProductList()
        {
            List<string> list = new List<string>();
#if UNITY_IOS
            //RequstProductInfo(list);
#endif
            return list;
        }

    }
}
