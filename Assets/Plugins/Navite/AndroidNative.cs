using System;
using UnityEngine;
namespace Native
{

    public class AndroidNative
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    static AndroidJavaObject _PhoneState;
    static AndroidJavaObject PhoneState
    {
        get
        {
            if (_PhoneState == null)
            {
                _PhoneState = new AndroidJavaObject("unity.plugins.androidnative.PhoneState");
            }
            return _PhoneState;
        }
    }
#endif
        /// <summary>
        /// 返回应该显示的格子数（共5个）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int GetSignalLevel(int value)
        {
            //1、当信号大于等于 - 85dBm时候，信号显示满格
            //2、当信号大于等于 - 95dBm时候，而小于 - 85dBm时，信号显示4格
            //3、当信号大于等于 - 105dBm时候，而小于 - 95dBm时，信号显示3格，不好捕捉到。
            //4、当信号大于等于 - 115dBm时候，而小于 - 105dBm时，信号显示2格，不好捕捉到。
            //5、当信号大于等于 - 140dBm时候，而小于 - 115dBm时，信号显示1格，不好捕捉到。
            if (value > -85)
            {
                return 5;
            }
            else if (value < -85 && value > -95)
            {
                return 4;
            }
            else if (value < -95 && value > -105)
            {
                return 3;
            }
            else if (value < -105 && value > -115)
            {
                return 2;
            }
            else if (value < -115 && value > -140)
            {
                return 1;
            }

            return -1;
        }

        public static int CallStatic(string methodName)
        {
#if UNITY_ANDROID && !UNITY_EDITOR

        try
        {
           
            return PhoneState.CallStatic<int>(methodName);
            
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
#endif
            return -1;
        }
        public static int CallStatic(string methodName, int num)
        {
#if UNITY_ANDROID && !UNITY_EDITOR

        try
        {
            return PhoneState.CallStatic<int>(methodName,num);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
#endif
            return -1;
        }


        // 返回状态   是枚举-1,0,1,2   如下面
        public static int BatteryState()
        {
            return CallStatic("BatteryState");
        }

        /*
                switch (status) {
            case BatteryManager.BATTERY_STATUS_FULL:      // 满电
                // Full
                nowState = 2;
                break;
            case BatteryManager.BATTERY_STATUS_CHARGING:      // 正在充电
                // Charging
                nowState = 1;
                break;
            case BatteryManager.BATTERY_STATUS_DISCHARGING:
                // Unplugged
                nowState = 0;
                break;
            case BatteryManager.BATTERY_STATUS_NOT_CHARGING:
                // Unplugged
                nowState = 0;
                break;
            case BatteryManager.BATTERY_STATUS_UNKNOWN:
                // Unknown
                nowState = -1;
                break;
        }
        */

        // 返回剩余电量，  满电是100
        public static int BatteryLevel()
        {
            return CallStatic("BatteryLevel");
        }

        // 返回WIFI      返回的是负值  越靠近0 越强
        public static int GetWIFISignalStrength(int level)
        {
            return CallStatic("GetWIFISignalStrength", level);
        }

        // 返回Telephone
        public static int GetTeleSignalStrength()
        {
            return CallStatic("GetTeleSignalStrength");
        }
    }
}
