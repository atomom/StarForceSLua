using UnityEngine;
using UnityEditor;
using UnityGameFramework.Runtime;
using StarForce.Lua;
using SLua;
using GameFramework;

namespace StarForce
{
    [ExecuteInEditMode]
    public class LuaGFComponent : GameFrameworkComponent
    {
        [HideInInspector]
        [SerializeField]
        public LuaVariable[] mVariables = new LuaVariable[] { };
#region 常量，lua中的事件名
        private const string ON_AWAKE = "Awake";
        private const string ON_START = "Start";
        private const string ON_DESTROY = "OnDestroy";
        private const string ON_UPDATE = "Update";
#endregion

        public LuaTable UserTable;
        protected override void Awake()
        {
            base.Awake();
            string path = gameObject.name;
            path = path.Replace("_", ".");
            object a = GameEntry.Lua.DoString(string.Format("return RequireCommon(\"{0}\")", path));
            LuaTable t = a as LuaTable;
            if (t == null)
            {
                Log.Warning("no usertable {0}", path);
                return;
            }
            UserTable = (LuaTable)t.invoke("new");


            if (!CanCall())
            {
                return;
            }
            for (int i = 0; i < mVariables.Length; i++)
            {
                var val = mVariables[i];
                UserTable[val.name] = val.val;
            }

            UserTable["gameObject"] = gameObject;
            UserTable["transform"] = transform;
            UserTable["CSEntity"] = this;
            InvokeTable(ON_AWAKE);
        }

        protected void Start()
        {
            InvokeTable(ON_START);
        }

        private void Update()
        {
            InvokeTable(ON_UPDATE);
        }

        private void OnDestroy()
        {
            InvokeTable(ON_DESTROY);
        }

        private void InvokeTable(string s)
        {

            if (!CanCall())
            {
                return;
            }
            UserTable.invoke(s, UserTable);
        }

        private bool CanCall()
        {
            if (!GameEntry.Lua || !GameEntry.Lua.IsMain())
            {
                return false;
            }
            if (UserTable == null)
            {
                return false;
            }
            return true;
        }
    }
}