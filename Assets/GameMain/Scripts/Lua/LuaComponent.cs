//using GameFramework;
//using GameFramework.Resource;
//using SLua;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityGameFramework.Runtime;

//namespace StarForce.Lua
//{
//    /// <summary>
//    /// Lua 组件。将 ToLua 插件集成到 UnityGameFramework 中。本类的实现参考 ToLua 中的 <see cref="LuaClient"/> 类。
//    /// </summary>
//    public class LuaComponent : GameFrameworkComponent
//    {

//        private LuaSvr luaSvr;
//        private LuaState luaState;

//        #region MonoBehaviour

//        private void Start()
//        {
//            Init();
//        }

//        private void OnDestroy()
//        {
//            Deinit();
//        }

//        #endregion MonoBehaviour

//        private void Init()
//        {
//            luaSvr = new LuaSvr();
//            luaState = LuaSvr.mainState;
//            luaState.loaderDelegate = LuaHelper.DoFile; //自定义加载Lua的文件

//#if UNITY_EDITOR && UNITY_STANDALONE
//            FileUtils.getInstance().genStreamPath();
//#else
//            FileUtils.getInstance().genSearchPath();
//#endif
//        }

//        public void Start(string main, bool updateSuccess, Action<int> cb)
//        {
//            luaSvr.init(cb, () =>
//            {
//                if (cb != null) cb(101);
//                luaSvr.start(main);
//            }, LuaSvrFlag.LSF_BASIC | LuaSvrFlag.LSF_3RDDLL | LuaSvrFlag.LSF_EXTLIB);
//        }

//        private void Deinit()
//        {
//            LuaSvr.mainState = null;
//            luaState.Dispose();
//            luaState = null;
//            luaSvr = null;
//            LuaHelper.Clear();
//        }

//        #region 执行lua代码
//        public object DoString(string script)
//        {
//            return luaState.doString(script);
//        }

//        public object DoBuffer(byte[] bytes)
//        {
//            object obj;
//            if (luaState.doBuffer(bytes, "temp buffer", out obj))
//                return obj;
//            return null;
//        }

//        public object DoFile(string path)
//        {
//            return luaState.doFile(path);
//        }

//        public bool IsMain()
//        {
//            return LuaSvr.mainState != null;
//        }
//        #endregion

//    }
//}
