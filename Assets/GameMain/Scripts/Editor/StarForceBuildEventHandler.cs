using GameFramework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.AssetBundleTools;

namespace StarForce.Editor
{
    public sealed class StarForceBuildEventHandler : IBuildEventHandler
    {
        private static readonly IDictionary<string, string> s_LuaScriptNames = new Dictionary<string, string>();

        public void PreProcessBuildAll(string productName, string companyName, string gameIdentifier,
            string applicableGameVersion, int internalResourceVersion, string unityVersion, BuildAssetBundleOptions buildOptions, bool zip,
            string outputDirectory, string workingPath, string outputPackagePath, string outputFullPath, string outputPackedPath, string buildReportPath)
        {
            CleanStreamingAssets();
            RenameLuaScripts();
        }

        public void PostProcessBuildAll(string productName, string companyName, string gameIdentifier,
            string applicableGameVersion, int internalResourceVersion, string unityVersion, BuildAssetBundleOptions buildOptions, bool zip,
            string outputDirectory, string workingPath, string outputPackagePath, string outputFullPath, string outputPackedPath, string buildReportPath)
        {
            RevertRenameLuaScripts();
        }

        public void PreProcessBuild(BuildTarget buildTarget, string workingPath, string outputPackagePath, string outputFullPath, string outputPackedPath)
        {

        }

        public void PostProcessBuild(BuildTarget buildTarget, string workingPath, string outputPackagePath, string outputFullPath, string outputPackedPath)
        {
            CopyToStreamingAssets(buildTarget, outputPackagePath);
        }

        private static void CleanStreamingAssets()
        {
            string streamingAssetsPath = Utility.Path.GetCombinePath(Application.dataPath, "StreamingAssets");
            string[] fileNames = Directory.GetFiles(streamingAssetsPath, "*", SearchOption.AllDirectories);
            foreach (string fileName in fileNames)
            {
                if (fileName.Contains(".gitkeep"))
                {
                    continue;
                }

                File.Delete(fileName);
            }

            Utility.Path.RemoveEmptyDirectory(streamingAssetsPath);
        }

        private static void CopyToStreamingAssets(BuildTarget buildTarget, string assetPath)
        {
            if (buildTarget != BuildTarget.StandaloneWindows)
            {
                return;
            }

            string streamingAssetsPath = Utility.Path.GetCombinePath(Application.dataPath, "StreamingAssets");
            string[] fileNames = Directory.GetFiles(assetPath, "*", SearchOption.AllDirectories);
            foreach (string fileName in fileNames)
            {
                string destFileName = Utility.Path.GetCombinePath(streamingAssetsPath, fileName.Substring(assetPath.Length));
                FileInfo destFileInfo = new FileInfo(destFileName);
                if (!destFileInfo.Directory.Exists)
                {
                    destFileInfo.Directory.Create();
                }

                File.Copy(fileName, destFileName);
            }
        }

        private void RenameLuaScripts()
        {
            s_LuaScriptNames.Clear();

            var luaScriptAssetNames = AssetDatabase.FindAssets("l:LuaScript").ToList().ConvertAll(guid => AssetDatabase.GUIDToAssetPath(guid));
            foreach (string luaScriptAssetName in luaScriptAssetNames)
            {
                string luaScriptAssetMetaName = luaScriptAssetName + ".meta";
                string renamedLuaScriptAssetName = luaScriptAssetName + ".bytes";
                string renamedLuaScriptAssetMetaName = renamedLuaScriptAssetName + ".meta";

                File.Move(luaScriptAssetName, renamedLuaScriptAssetName);
                s_LuaScriptNames.Add(luaScriptAssetName, renamedLuaScriptAssetName);

                File.Move(luaScriptAssetMetaName, renamedLuaScriptAssetMetaName);
                s_LuaScriptNames.Add(luaScriptAssetMetaName, renamedLuaScriptAssetMetaName);
            }

            AssetDatabase.Refresh();
        }

        private void RevertRenameLuaScripts()
        {
            foreach (var luaScriptNames in s_LuaScriptNames)
            {
                File.Move(luaScriptNames.Value, luaScriptNames.Key);
            }

            s_LuaScriptNames.Clear();
            AssetDatabase.Refresh();
        }
    }
}
