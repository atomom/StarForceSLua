using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Native
{
    public class JTPayTool
    {
        public static string SYSTEM_NAME = "jft";
        public static string CODE = "10218732";
        public static string APPID = "20170901110705627876";
        public static string COM_KEY = "0472CDF28838A88619FB73A142B32569";
        public static string KEY     = "41f6b3842378b3ee2cbb1ccdfb0e872e";
        public static string VECTOR = "5a5e6ee8ef106441";
        public static string RETURN_URL = "http://shangjia.jtpay.com/Form/TestReturn";
        public static string NOTICE_URL = "http://shangjia.jtpay.com/Form/TestNotice";
        private AndroidJavaObject jo;

        JTPayTool()
        {
        }

#if UNITY_IOS
        #region IOS Native
        [DllImport("__Internal")]
        private static extern void _jftPay(string orderId, string price, string payType, string userCode, string key, string iv, string comeKey, string appId, string systemName, string returnurl, string notifyurl, IntCallBack cb);

		/* Public interface for use inside C# / JS code */

		// Starts lookup for some bonjour registered service inside specified domain
		public static void jftPay(string orderId, string price, string payType, string userCode, string key, string iv, string comeKey, string appId, string systemName, string returnurl, string notifyurl, IntCallBack cb)
		{
			// Call plugin only when running on real device
			if (Application.platform != RuntimePlatform.OSXEditor)
                _jftPay(orderId, price, payType, userCode, key, iv, comeKey, appId, systemName, returnurl, notifyurl, cb);
		}

        #endregion
#endif

        public delegate void IntCallBack(int p);
        static Action<int> mPayStatusCallback;

        [AOT.MonoPInvokeCallback(typeof(IntCallBack))]
        static public void payStatusCallback(int data)
        {
            Debug.Log(data);
            if (mPayStatusCallback != null)
            {
                UnityThreadHelper.Dispatcher.Dispatch(() =>
                {
                    mPayStatusCallback(data);
                    mPayStatusCallback = null;
                });
            }
        }

        private static JTPayTool _ins  = null;
        static public JTPayTool getInstance()
        {
            if (_ins == null)
            {
                _ins = new JTPayTool();
            }
            return _ins;
        }

        //one step pay
        private void getToken(string orderId, string money, string payType, Action<int> cb)
        {
            mPayStatusCallback = cb;
#if UNITY_EDITOR
            cb(2);
#elif UNITY_ANDROID
            IntPtr fn = Marshal.GetFunctionPointerForDelegate((IntCallBack)payStatusCallback);
            jo.Call("getToken", new object[] { orderId, money, payType ,(int)fn});
#elif UNITY_IOS
            jftPay(orderId, money, payType, CODE, KEY, VECTOR, COM_KEY, APPID, SYSTEM_NAME, RETURN_URL, NOTICE_URL, payStatusCallback);
#endif
        }

        //初始化PayUtil
        private void initPayUtil()
        {
#if UNITY_EDITOR

#elif UNITY_ANDROID
            jo.Call("initPayUtil", new object[] { SYSTEM_NAME, CODE, APPID, COM_KEY, KEY, VECTOR, RETURN_URL, NOTICE_URL });
#endif
		}

        private void startActivity()
        {
#if UNITY_EDITOR

#elif UNITY_ANDROID
            using (AndroidJavaClass jc = new AndroidJavaClass("com.pay.sdk.usage.JTPayActivity"))
            {
                jo = jc.CallStatic<AndroidJavaObject>("GetInstance");
            }
#endif
		}

        //pay method  3 wexin; 4 ali; 11 qq ;12 jd
        public void Pay(string orderId, string pay, string money, Action<int> cb)
        {
            startActivity();
            initPayUtil();
            getToken(orderId, money, pay, cb);
        }
    }
    
}
