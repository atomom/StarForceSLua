using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using Sean.Editor;
using StarForce.Lua;
namespace StarForce.Editor
{
    [CustomEditor(typeof(LuaBehavior))]
    public class LuaBehaviorInspector : UnityEditor.Editor
    {
        LuaBehavior mUlua;
        private static GUIContent
            //insertContent = new GUIContent("+", "添加变量"),
            deleteContent = new GUIContent("-", "删除变量");
        private static GUILayoutOption buttonWidth = GUILayout.MaxWidth(20f);
        private static GUIContent pointContent = GUIContent.none;
        //private SerializedProperty KeyMap;
        private SerializedProperty mVariables;

        private SerializedObject binding;

        GameObject mGo;
        string mLastLuaFile;

        bool mAdd = false;
        void OnEnable()
        {
            binding = new SerializedObject(target);
            mVariables = binding.FindProperty("mVariables");
            //KeyMap = binding.FindProperty("KeyMap");
        }

        void OnDisable()
        {
            binding = null;
            mVariables = null;
            //KeyMap = null;
        }

        /// <summary>
        /// 获得对象加载了多少脚本
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Component[] GetComponents(GameObject go)
        {
            return go.GetComponents<Component>();
        }
        /// <summary>
        /// 获得对象加载了多少脚本
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public Component[] GetComponents(Component go)
        {
            return go.GetComponents<Component>();
        }
        /// <summary>
        /// 获取脚本列表名
        /// </summary>
        /// <param name="comps"></param>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string[] GetTypeNames(Component[] comps, string type, out int index)
        {
            index = 0;
            string[] names = new string[comps.Length + 1];
            names[0] = "GameObject";
            for (int i = 0; i < comps.Length; i++)
            {
                string tp = comps[i].GetType().ToString();
                int find = tp.LastIndexOf(".");
                if (find > -1) tp = tp.Substring(find + 1);
                names[i + 1] = tp;
                if (index == 0 && !string.IsNullOrEmpty(type) && type.Equals(tp)) index = i + 1;
            }
            return names;
        }
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="file"></param>
        void SelectLuaFile(string file)
        {
            binding.Update();
            SerializedProperty sp = binding.FindProperty("mPath");
            sp.stringValue = file;
            binding.ApplyModifiedProperties();
            EditorTools.SetDirty(binding.targetObject);
        }

        public override void OnInspectorGUI()
        {
            if (mVariables == null)
                mVariables = binding.FindProperty("mVariables");

            mUlua = target as LuaBehavior;
            EditorTools.DrawSeparator();
            BindAttrValue();
            //EditorTools.DrawSeparator();
            //BindConstValue();
            binding.ApplyModifiedProperties();
        }

        public bool KeyIsNumber(string key)
        {
            var reg = new System.Text.RegularExpressions.Regex("^[0-9]+$");//判断是不是数据，要不是就表示没有选择，则从隐藏域里读出来
            System.Text.RegularExpressions.Match ma = reg.Match(key);
            if (ma.Success) EditorUtility.DisplayDialog("错误", "变量名不能为纯数字", "确定");
            return ma.Success;
        }

        /// <summary>
        /// 绑定变量
        /// </summary>

        public void BindAttrValue()
        {
            binding.Update();
            string reName = "";
            bool changed = false;
            List<LuaVariable> vals = new List<LuaVariable>(mUlua.mVariables);

            if (EditorTools.DrawHeader("绑定变量"))
            {
                #region 已有变量

                List<LuaVariable> removeList = new List<LuaVariable>();//要删除的变量
                for (int i = 0; i < mVariables.arraySize; i++)
                {
                    if (i % 2 == 0) GUI.backgroundColor = Color.cyan;
                    else GUI.backgroundColor = Color.white;
                    var val = mUlua.mVariables[i];

                    if (val.val == null)
                    {
                        removeList.Add(val);
                        changed = true;
                        continue;
                    }
                    EditorTools.BeginContents();
                    GUILayout.Space(5);
                    GUI.backgroundColor = Color.white;
                    //取出列表中的变量
                    SerializedProperty variable = mVariables.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                    //类型
                    SerializedProperty mType = variable.FindPropertyRelative("type");
                    //对象
                    SerializedProperty mVal = variable.FindPropertyRelative("variable");
                    var comps = GetComponents(val.gameObject);

                    int index = 0;
                    var names = GetTypeNames(comps, mType.stringValue, out index);
                    int choice = 0;
                    choice = EditorGUILayout.Popup(index, names, GUILayout.Width(100));
                    if (choice > 0 && choice != index)
                    {
                        mType.stringValue = names[choice];
                        //mVal.objectReferenceValue = comps[choice - 1];
                    }
                    else if (choice != index)
                    {
                        mType.stringValue = names[0];
                        //mVal.objectReferenceValue = val.gameObject;
                    }
                    EditorTools.DrawPadding();
                    GUILayout.Space(-16);

                    //描绘变量对象
                    EditorGUILayout.PropertyField(mVal, pointContent);
                    GUILayout.Space(6);

                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal();
                    var style = new GUIContent("变量名", "lua中访问的变量名");
                    GUILayout.Label(style, GUILayout.Width(40));

                    var mName = variable.FindPropertyRelative("name");
                    EditorGUILayout.PropertyField(mName, pointContent);

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth))
                    {
                        //mUlua.mVariables.Remove(val);
                        mVariables.DeleteArrayElementAtIndex(i);

                        EditorTools.SetDirty(mUlua);
                        EditorUtility.UnloadUnusedAssetsImmediate();
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    if (i % 2 == 0) GUI.backgroundColor = Color.cyan;
                    else GUI.backgroundColor = Color.white;
                    EditorTools.EndContents();
                }
                GUI.backgroundColor = Color.white;
                vals = new List<LuaVariable>(mUlua.mVariables);
                for (int j = 0; j < removeList.Count; j++)
                {
                    vals.Remove(removeList[j]);
                    removeList[j] = null;
                }
                removeList.Clear();

                mUlua.mVariables = vals.ToArray();
                for (int i = 0; i < vals.Count; i++)
                {
                    vals[i] = null;
                }
                vals.Clear();

                if (!string.IsNullOrEmpty(reName))
                {
                    EditorTools.BeginContents();

                    GUILayout.Space(6f);
                    reName = reName + "变量重名了……";

                    EditorGUILayout.HelpBox(reName, MessageType.Warning, true);
                    GUILayout.Space(6f);

                    EditorTools.EndContents();
                }
                #endregion

                #region 新变量
                EditorTools.BeginContents();
                GUILayout.Space(6f);
                GUILayout.BeginHorizontal();
                GUILayout.Label("新变量", GUILayout.Width(40));

                mGo = EditorGUILayout.ObjectField(mGo, typeof(GameObject), true) as GameObject;
                GUILayout.EndHorizontal();
                if (mAdd)
                {
                    int index = mVariables.arraySize - 1;
                    var variable = mVariables.GetArrayElementAtIndex(index);
                    mAdd = false;
                    var mName = variable.FindPropertyRelative("name");
                    string name = mName.stringValue;
                    mName.stringValue = "";
                    mName.stringValue = name;
                }
                if (mGo)
                {
                    string _name = mGo.name;
                    if (KeyIsNumber(_name))
                    {
                        _name = "name";
                    }
                    var val = new LuaVariable();
                    val.name = _name;
                    val.type = "GameObject";
                    //val.gameObject = mGo;
                    val.variable = mGo;
                    vals = new List<LuaVariable>(mUlua.mVariables);
                    vals.Add(val);

                    //mUlua.mVariables.Add(val);
                    mUlua.mVariables = vals.ToArray();
                    for (int i = 0; i < vals.Count; i++)
                    {
                        vals[i] = null;
                    }
                    vals.Clear();
                    EditorTools.SetDirty(mUlua);
                    mAdd = true;
                    mGo = null;

                }
                GUILayout.Space(6f);
                EditorTools.EndContents();
                #endregion

            }
            if (changed)
            {
                EditorTools.SetDirty(mUlua);
            }
        }
    }
}