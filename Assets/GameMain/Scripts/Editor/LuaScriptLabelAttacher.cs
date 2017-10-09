using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace StarForce.Editor
{
    public static class LuaScriptLabelAttacher
    {
        private const string LuaScriptExtension = ".lua";
        public const string LuaScriptLabel = "LuaScript";

        [MenuItem("Star Force/Attach LuaScript Label")]
        public static void AttachLuaScriptLabel()
        {
            string[] luaScriptLabels = new string[] { LuaScriptLabel };
            var luaScriptAssetNames = AssetDatabase.GetAllAssetPaths().Where(luaScriptAssetName => luaScriptAssetName.EndsWith(LuaScriptExtension));
            foreach (string luaScriptAssetName in luaScriptAssetNames)
            {
                Object asset = AssetDatabase.LoadAssetAtPath(luaScriptAssetName, typeof(Object));
                AssetDatabase.SetLabels(asset, new HashSet<string>(AssetDatabase.GetLabels(asset)).Union(luaScriptLabels).ToArray());
            }

            Debug.Log("Attach LuaScript label complete.");
        }
    }
}
