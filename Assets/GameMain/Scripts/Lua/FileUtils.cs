using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;
using System.Text.RegularExpressions;
using StarForce;

namespace StarForce.Lua
{
    public enum BuildPlatform
    {
        WebGL,
        Standalones,
        IOS,
        Android,
        WP8,
        uwp
    }

    public class FileUtils
    {
        static FileUtils s_sharedFileUtils;
        private List<string> _searchPathArray = new List<string>();
        private Dictionary<string, string> _pathCache = new Dictionary<string, string>();
        private string streamingAssetsPath;
        private string persistentDataPath;

        public FileUtils()
        {
            streamingAssetsPath = Application.streamingAssetsPath;
            persistentDataPath = Application.persistentDataPath;
        }
        static public FileUtils getInstance()
        {
            if (s_sharedFileUtils == null) s_sharedFileUtils = new FileUtils();
            return s_sharedFileUtils;
        }
        static public void destroyInstance()
        {
            if (s_sharedFileUtils != null)
            {
                s_sharedFileUtils._searchPathArray.Clear();
                s_sharedFileUtils._pathCache.Clear();
                s_sharedFileUtils = null;
            }
        }

        public void ClearCache()
        {
            _pathCache.Clear();
        }
        public List<string> getSearchPaths()
        {
            return _searchPathArray;
        }

        public void setSearchPaths(List<string> searchPaths)
        {
            _searchPathArray = searchPaths;
        }

        public void genStreamPath()
        {
            var _fileUtils = FileUtils.getInstance();
            _fileUtils.ClearCache();
            var searchPath = new List<string>();

            searchPath.Add(Application.streamingAssetsPath);

            searchPath.Add(Path.Combine(Application.streamingAssetsPath, Constant.GameData.CurrentName.ToLower()));

            _fileUtils.setSearchPaths(searchPath);
        }

        public void genSearchPath()
        {
            var _fileUtils = FileUtils.getInstance();
            _fileUtils.ClearCache();
            var searchPath = new List<string>();

            searchPath.Add(Application.streamingAssetsPath);

            searchPath.Add(Path.Combine(Application.streamingAssetsPath, Constant.GameData.CurrentName.ToLower()));

            var root = _fileUtils.getWritablePath(Constant.GameData.CurrentName);

            searchPath.Insert(0, Application.persistentDataPath);

            searchPath.Insert(0, root);

#if UNITY_EDITOR && UNITY_STANDALONE
            if (!StarForce.GameEntry.Base.EditorResourceMode)
            {
                string relativePath = System.Environment.CurrentDirectory.Replace("\\", "/");
                searchPath.Insert(0, relativePath);
            }
#else
            string relativePath = System.Environment.CurrentDirectory.Replace("\\", "/");
            searchPath.Insert(0, relativePath);
#endif
            _fileUtils.setSearchPaths(searchPath);
        }

        public void addSearchPath(string path)
        {
            addSearchPath(path, false);
        }

        public void addSearchPath(string path, bool front)
        {
            fixedPath(ref path);
            if (front)
            {
                var index = _searchPathArray.IndexOf(path);
                if (index == -1)
                    _searchPathArray.Insert(0, path);
                else if (index > 0)
                {
                    _searchPathArray.Remove(path);
                    _searchPathArray.Insert(0, path);
                }
            }
            else
            {
                var index = _searchPathArray.IndexOf(path);
                if (index == -1)
                    _searchPathArray.Add(path);
                else if (index > _searchPathArray.Count - 1)
                {
                    _searchPathArray.Remove(path);
                    _searchPathArray.Add(path);
                }
            }
        }


        public string GetMd5HashFromFile(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                    return "";
                FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                Debuger.LogError("GetMd5HashFromFile fail,error: " + e.Message);
            }
            return "";
        }
        public Texture2D writeTexture2D(string path, int width, int height, Color32[] color32)
        {
            var tx = new Texture2D(width, height);
            tx.SetPixels32(color32);
            tx.Apply();
            writeTexture2D(path, tx);
            return tx;
        }
        /// <summary>
        /// 保存图片到目录
        /// </summary>
        /// <param name="path">.jpg|.png</param>
        /// <param name="tx"></param>
        /// <returns>bool</returns>
        public bool writeTexture2D(string path, Texture2D tx)
        {
            if (Path.GetExtension(path) == ".jpg")
            {
                return writeBytes(path, tx.EncodeToJPG(60));
            }
            else if (Path.GetExtension(path) == ".png")
            {
                return writeBytes(path, tx.EncodeToPNG());
            }
            else
                throw new Exception("saving path is not jpg|png.");
        }

        private string GetMd5Hash(MD5 md5Hash, byte[] data)
        {
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// 获得hash值
        /// </summary>
        /// <param name="md5Hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            return GetMd5Hash(md5Hash, data);
        }
        public string GetMd5(string file)
        {
            string data = getString(file);
            return GenMd5(data);
        }

        public string GenMd5(string data)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return GetMd5Hash(md5Hash, data);
            }
        }

        public bool EqualsMd5(string oldPath, string newPath)
        {
            string _old = getString(oldPath);
            string _new = getString(newPath);
            if (_old == null || _new == null)
            {
                return false;
            }
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, _old);
                string hashOfInput = GetMd5Hash(md5Hash, _new);
                // Create a StringComparer an compare the hashes.
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                if (0 == comparer.Compare(hashOfInput, hash))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 从文件读字符串
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string getString(string fileName)
        {
            if (!isFileExist(fileName))
            {
                return null;
            }
#if !UNITY_EDITOR && UNITY_ANDROID
        return getStringAndroid(fileName);
#else
            return File.ReadAllText(fileName);
#endif
        }
        public string getString(string path, string fileName)
        {
            if (!path.EndsWith("/")) path += "/";
            return getString(path + fileName);
        }
        public byte[] getBytes(string path, string fileName)
        {
            if (!path.EndsWith("/")) path += "/";
            return getBytes(path + fileName);
        }

        /// <summary>
        /// 从文件读二进制
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] getBytes(string fileName)
        {
            if (!isFileExist(fileName))
            {
                return null;
            }
#if !UNITY_EDITOR && UNITY_ANDROID
        return getBytesAndroid(fileName);
#else
            return File.ReadAllBytes(fileName);
#endif
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool isFileExist(string filePath)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
        return isFileExistsAndroid(filePath);
#else
            return File.Exists(filePath);
#endif
        }

        public string getFullPathForWww(string path)
        {
            var url = getFullPath(path);
            if (string.IsNullOrEmpty(url)) return "";
            if (path == url && url.IndexOf(streamingAssetsPath) == -1) url = streamingAssetsPath + "/" + path;
            System.Uri u = new System.Uri(url);
            return u.AbsoluteUri;
        }

        /// <summary>
        /// 判断目录是否存在
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public bool isDirectoryExist(string dir)
        {
            return Directory.Exists(dir);
        }

        public bool Move(string src, string target)
        {
            try
            {
                if (isFileExist(target))
                {
                    removeFile(target);
                }
                string path = Path.GetDirectoryName(target);

                createDirectory(path);
                File.Move(src, target);
                return true;

            }
            catch (IOException e)
            {
                Debuger.LogError(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="oldFile">旧文件名</param>
        /// <param name="newFile">新文件名</param>
        /// <returns></returns>
        public bool renameFile(string path, string oldFile, string newFile)
        {
            string _old = path + oldFile;
            string _new = path + newFile;
            try
            {
                if (isFileExist(_old))
                {
                    removeFile(_new);
                }
                else
                    return false;
                File.Move(_old, _new);
                return true;
            }
            catch (IOException e)
            {
                Debuger.LogError(e.ToString());
            }
            Debuger.LogError("can't found " + _old);
            return false;
        }
        public static string getLinuxPath(string path)
        {
#if UNITY_EDITOR
        return Regex.Replace(path, "\\\\", "/");
#else
            return path;
#endif
        }
        public void movePath(string oldPath, string newPath)
        {
            oldPath = getLinuxPath(oldPath);
            newPath = getLinuxPath(newPath);
            ForEachDirectory(oldPath, (path) =>
            {
                path = getLinuxPath(path);
                var p = path.Replace(oldPath, newPath);
                var dir = Path.GetDirectoryName(newPath);
                createDirectory(dir);
                File.Move(path, p);
            });
        }

        public Int64 getLastWriteTime(string path)
        {
            path = getFullPath(path);
            if (string.IsNullOrEmpty(path))
                return 0;
            FileInfo fi = new FileInfo(path);
            return fi.LastWriteTime.Ticks;
        }
        public Texture2D getTexture2D(string path)
        {
            path = getFullPath(path);
            if (string.IsNullOrEmpty(path))
                return null;
            var mData = getBytes(path);
            Texture2D tx = new Texture2D(256, 256);
            tx.LoadImage(mData);
            return tx;
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public bool removeDirectory(string dir)
        {
            if (isDirectoryExist(dir))
            {
                Directory.Delete(dir, true);
                return true;
            }
            return false;
        }
        public bool removeFile(string file)
        {
            if (isFileExist(file))
            {
#if !UNITY_EDITOR
                if (file.IndexOf(streamingAssetsPath) == -1)
#endif
                {
                    File.Delete(file);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 可写目录
        /// </summary>
        /// <returns></returns>
        public string getWritablePath(string gameName)
        {
            if (string.IsNullOrEmpty(gameName))
            {
                return persistentDataPath + "/root/";
            }
            return persistentDataPath + "/root/" + gameName + "/";
        }

        public byte[] getFileDataFromZip(string zipFilePath, string fileName)
        {
            return null;
        }
        public bool writeFileWithCode(string filepath, string data, Encoding code)
        {
            try
            {
                string path = Path.GetDirectoryName(filepath);
                createDirectory(path);

                if (code != null)
                {
                    File.WriteAllText(filepath, data, code);
                }
                else
                {
                    File.WriteAllText(filepath, data);
                }
                return true;
            }
            catch (Exception e)
            {
                Debuger.LogError("writeFIle fail. " + filepath);
                throw e;
            }
        }
        public bool writeString(string filepath, string data)
        {
            return writeFileWithCode(filepath, data, Encoding.UTF8);
        }
        public bool writeBytes(string filePath, byte[] bytes)
        {
            try
            {
                string path = Path.GetDirectoryName(filePath);
                createDirectory(path);

                File.WriteAllBytes(filePath, bytes);
                return true;
            }
            catch (IOException e)
            {
                Debuger.LogError("writeFIle fail. " + filePath);
                throw e;
            }
        }
        private void Write(FileStream fs, byte[] data)
        {
            fs.Write(data, 0, data.Length);
        }
        public bool writeFileStream(string path, List<byte[]> dataes)
        {
            createDirectory(Path.GetDirectoryName(path));
            using (FileStream fs = new FileStream(path, System.IO.FileMode.Append))
            {
                for (int i = 0; i < dataes.Count; ++i)
                    Write(fs, dataes[i]);
            }
            return true;
        }
        public bool writeFileStream(string dir, string filename, List<byte[]> dataes)
        {
            return writeFileStream(Path.Combine(dir, filename), dataes);
        }

        public void createDirectory(string path)
        {
            if (!isDirectoryExist(path))
                Directory.CreateDirectory(path);
        }

        public void clearPath(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                return;
            }
            FileInfo[] files = info.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                files[i].Delete();
            }
            DirectoryInfo[] diries = info.GetDirectories();
            for (int j = 0; j < diries.Length; j++)
            {
                diries[j].Delete(true);
            }
        }

        /// <summary>
        /// 解压zip文件
        /// </summary>
        /// <param name="zip"></param>
        /// <returns></returns>
        public bool unZip(string zip)
        {
            ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
            string rootPath = System.IO.Path.GetDirectoryName(zip);
            if (!isFileExist(zip)) return false;
            // 开始解压
            //FastZipEvents events = new FastZipEvents();
            //events.Progress = onProgress;
            FastZip fast = new FastZip();
            fast.ExtractZip(zip, rootPath, "");
            return true;
        }

        public void ForEachDirectory(string path, Action<string> callBack)
        {
            ForEachDirectory(path, "*", callBack);
        }

        /// <summary>
        /// 遍历文件夹下所有文件。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="callBack"></param>
        public void ForEachDirectory(string path, string searchPattern, Action<string> callBack)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                return;
            }
            FileInfo[] files = info.GetFiles(searchPattern, SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                callBack(getLinuxPath(files[i].FullName));
            }

        }

        /// <summary>
        /// 获得目录下所有文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> getAllFileInPath(string path)
        {
            return getAllFileInPathWithSearchPattern(path, null);
        }

        /// <summary>
        /// 获得目录下所有后缀为{searchPattern}的文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public List<string> getAllFileInPathWithSearchPattern(string path, string searchPattern)
        {
            List<string> list = new List<string>();
            ForEachDirectory(path, searchPattern, (string file) =>
            {
                list.Add(file);
            });

            return list;
        }

        private bool isRoot(string path, string fileName)
        {
            bool ret = false;
            if (Path.GetDirectoryName(fileName).IndexOf(path) > -1)
            {
                ret = true;
            }
            return ret;
        }

        private void fixedPath(ref string path)
        {
            if (!path.EndsWith("/"))
            {
                path = path + "/";
            }
        }

        /// <summary>
        /// 获得文件存在的路径，目录从_searchPathArray中查找。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string getFullPath(string fileName)
        {
            if (_pathCache.ContainsKey(fileName)) return _pathCache[fileName];

            for (int i = 0; i < _searchPathArray.Count; i++)
            {
                string path = _searchPathArray[i];
                if (isRoot(path, fileName))
                    continue;
                fixedPath(ref path);
                var p = path + fileName;
                if (isFileExist(p))
                {
                    _pathCache.Add(fileName, p);
                    return p;
                }
            }
            if (isFileExist(fileName))
            {
                _pathCache.Add(fileName, fileName);
                return fileName;
            }
            return "";
        }


        public string getRuntimePlatform()
        {
            string pf = "";
#if UNITY_EDITOR
        switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
        {
            case UnityEditor.BuildTarget.StandaloneLinux:
            case UnityEditor.BuildTarget.StandaloneLinux64:
            case UnityEditor.BuildTarget.StandaloneLinuxUniversal:
            case UnityEditor.BuildTarget.StandaloneOSXIntel:
                pf = BuildPlatform.IOS.ToString();
                break;
            case UnityEditor.BuildTarget.StandaloneWindows:
            case UnityEditor.BuildTarget.StandaloneWindows64:
                pf = BuildPlatform.Standalones.ToString();
                //pf = BuildPlatform.Android.ToString();
                break;
            case UnityEditor.BuildTarget.WebGL:
                pf = BuildPlatform.WebGL.ToString();
                break;
#if UNITY_5
            case UnityEditor.BuildTarget.iOS:
#else
            case UnityEditor.BuildTarget.iPhone:
#endif
                pf = BuildPlatform.IOS.ToString();
                break;
            case UnityEditor.BuildTarget.Android:
                pf = BuildPlatform.Android.ToString();
                break;
            case UnityEditor.BuildTarget.WSAPlayer:
                pf = BuildPlatform.uwp.ToString();
                break;
            default:
                Debuger.LogError("Internal error. Bundle Manager dosn't support for platform " + UnityEditor.EditorUserBuildSettings.activeBuildTarget);
                pf = BuildPlatform.Standalones.ToString();
                break;
        }
#else
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXPlayer:
                    pf = BuildPlatform.Standalones.ToString();
                    break;
                case RuntimePlatform.OSXWebPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                    pf = BuildPlatform.WebGL.ToString();
                    break;
                case RuntimePlatform.IPhonePlayer:
                    //IOS
                    pf = BuildPlatform.IOS.ToString();
                    break;
                case RuntimePlatform.Android:
                    //安卓
                    pf = BuildPlatform.Android.ToString();
                    break;

                case RuntimePlatform.WSAPlayerARM:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerX86:
                    //Win10
                    // pf = BuildPlatform.Win10.ToString();
                    break;
                default:
                    Debuger.LogError("Platform " + Application.platform + " is not supported by BundleManager.");
                    pf = BuildPlatform.Standalones.ToString();
                    break;
            }
#endif
            return pf.ToLower();

        }


#if !UNITY_EDITOR && UNITY_ANDROID
    private AndroidJavaClass _helper;

    private AndroidJavaClass helper
    {
        get
        {
            if (_helper != null) return _helper;
            _helper = new AndroidJavaClass("sean.unity.helper.Unity3dHelper");
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                object jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                _helper.CallStatic("init", jo);
            }
            return _helper;
        }
    }

    private byte[] getBytesAndroid(string path)
    {
        if (path.IndexOf(streamingAssetsPath) > -1)
        {
            path = path.Replace(streamingAssetsPath + "/", "");
        }
        else if (path.IndexOf(persistentDataPath) > -1)
        {
            return File.ReadAllBytes(path);
        }
        return helper.CallStatic<byte[]>("getBytes", path);
    }
    private string getStringAndroid(string path)
    {
        if (path.IndexOf(streamingAssetsPath) > -1)
        {
            path = path.Replace(streamingAssetsPath + "/", "");
        }
        else if (path.IndexOf(persistentDataPath) > -1)
        {
            return File.ReadAllText(path);
        }
        return helper.CallStatic<string>("getString", path);
    }
    private bool isFileExistsAndroid(string path)
    {
        if(path.IndexOf(streamingAssetsPath) > -1)
        {
            path = path.Replace(streamingAssetsPath + "/", "");
        }
        else if(path.IndexOf(persistentDataPath) > -1)
        {
            return File.Exists(path);
        }
        return helper.CallStatic<bool>("isFileExists", path);
    }
#endif
        public string getAssetBundleFilePath(string path)
        {
            string name = path.Substring(0, path.IndexOf("/"));
            if (name.ToLower() == Constant.GameData.CommonName.ToLower() || name.ToLower() == "common")
            {
                name = Constant.GameData.CommonName;
            }
            else
            {
                name = Constant.GameData.CurrentName;
            }
            return getAssetBundleFilePathByGame(name, path);
        }

        string getRootAssetBundleFilePath(string path)
        {
            return getAssetBundleFilePathByGame(Constant.GameData.CurrentName, path);
        }

        string getAssetBundleFilePathByGame(string name, string path)
        {
            string n = name.ToLower() + "assetbundle/" + path;
            var p = getFullPath(n);
            if (string.IsNullOrEmpty(p))
            {
                Debug.LogError("can not find file " + n);
            }
#if !UNITY_EDITOR && UNITY_ANDROID
        if (n == p)
        {
            p = Application.dataPath + "!assets/" + p;
        }
#endif
            return p;
        }
        

    }
}