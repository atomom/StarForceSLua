using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace StarForce.Lua
{
    public static class LuaHelper
    {
        private static string m_Key = "";

        public static string Key
        {
            get
            {
                return m_Key;
            }
            set
            {
                m_Key = value;
            }
        }
        static Dictionary<string, byte[]> mCacheLuaFile = new Dictionary<string, byte[]>();
        static public void Init()
        {

        }

        /// <summary>
        /// 清除脚本文件缓存
        /// </summary>
        public static void Clear()
        {
            mCacheLuaFile.Clear();
        }
        public static void RemoveFile(string path, string file)
        {
            string md5 = FileUtils.getInstance().GenMd5(file);
            FileUtils.getInstance().removeFile(path + "/" + md5);

        }
        public static void SaveFileEncode(string path, string file, string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            string md5 = FileUtils.getInstance().GenMd5(file);
            byte[] by = UTF8Encoding.UTF8.GetBytes(data);
            var encode = AecEnCode(by);
            FileUtils.getInstance().writeBytes(path + "/" + md5, encode);
        }

        public static string GetFileDeCode(string path, string file)
        {
            string md5 = FileUtils.getInstance().GenMd5(file);
            byte[] by = FileUtils.getInstance().getBytes(path + "/" + md5);
            if (by == null || by.Length == 0) return "";
            var decode = AecDeCode(by);
            if (decode == null) return "";
            return UTF8Encoding.UTF8.GetString(decode);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Data">明文</param>
        /// <returns></returns>
        public static byte[] AecEnCode(byte[] bs)
        {
            byte[] keys = System.Text.Encoding.UTF8.GetBytes(Key);
            for (int i = 0; i < bs.Length; i++)
            {
                bs[i] = (byte)(bs[i] ^ keys[i % keys.Length]);
            }
            return bs;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Data">密文</param>
        /// <returns></returns>
        public static byte[] AecDeCode(byte[] bs)
        {
            byte[] keys = System.Text.Encoding.UTF8.GetBytes(Key);
            for (int i = 0; i < bs.Length; i++)
            {
                bs[i] = (byte)(bs[i] ^ keys[i % keys.Length]);
            }
            return bs;
        }

        public static string LoadProtoFile(string path)
        {
            var fileUtils = FileUtils.getInstance();
            byte[] data = null;
            if (string.IsNullOrEmpty(path))
            {
                //加载AssetBundle
                Debug.LogError("读不到文件--->" + path);
                return null;
            }
            else
            {
                data = fileUtils.getBytes(path);
            }

#if UNITY_EDITOR && UNITY_STANDALONE
            if (StarForce.GameEntry.Base.EditorResourceMode)
            {
                if (data != null)
                {
                    data = AecDeCode(data);
                }
            }
#elif UNITY_EDITOR
            string relativePath = System.Environment.CurrentDirectory.Replace("\\", "/");
            if (data != null && path.IndexOf(relativePath) == -1)
            {
                data = AecDeCode(data);
            }
#else
            if (data != null)
            {
                data = AecDeCode(data);
            }
#endif
            if (data == null) return "";
            return UTF8Encoding.UTF8.GetString(data);
        }


        /// <summary>
        /// 读lua脚本文件
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static byte[] DoFile(string fn)
        {
            try
            {
                fn = fn.Replace(".", "/");

                byte[] data = null;
                var fileUtils = FileUtils.getInstance();
                if (!fn.EndsWith(".lua")) fn += ".lua";
                if (mCacheLuaFile.ContainsKey(fn))
                {
                    return mCacheLuaFile[fn];
                }
                string path = fileUtils.getFullPath("lua/" + fn);
                if (string.IsNullOrEmpty(path))
                {
                    //加载AssetBundle
                    Debug.LogError("读不到文件--->" + fn);
                    return null;
                }
                else
                {
                    data = fileUtils.getBytes(path);
                }

#if UNITY_EDITOR && UNITY_STANDALONE
                if (StarForce.GameEntry.Base.EditorResourceMode)
                {
                    if (data != null)
                    {
                        data = AecDeCode(data);
                        mCacheLuaFile.Add(fn, data);
                    }
                }
#elif UNITY_EDITOR
                string relativePath = System.Environment.CurrentDirectory.Replace("\\", "/");
                if (data != null && path.IndexOf(relativePath) == -1)
                {
                    data = AecDeCode(data);
                    mCacheLuaFile.Add(fn, data);
                }
#else
                    if (data != null)
                    {
                        data = AecDeCode(data);
                        mCacheLuaFile.Add(fn, data);
                    }
#endif
                return data;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

    }
}