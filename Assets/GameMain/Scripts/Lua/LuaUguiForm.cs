using GameFramework;
using SLua;
using UnityEngine;
using UnityEngine.UI;
using StarForce.Lua;

namespace StarForce
{
    [ExecuteInEditMode]
    public class LuaUguiForm : UGuiForm
    {
        [HideInInspector]
        [SerializeField]
        public LuaVariable[] mVariables = new LuaVariable[] { };
        #region 常量，lua中的事件名
        private const string ON_CLICK = "OnClick";
        private const string ON_ValueChange = "OnValueChange";
        private const string ON_PRESS = "OnPress";
        private const string INIT_FINISH = "Awake";
        private const string ON_INPUTFIELDEND = "InputFiledEnd";
        private const string ON_DESTROY = "OnDestroy";
        private const string ON_HOVER = "OnHover";
        #endregion

        #region 方法缓存
        LuaFunction onClick;
        LuaFunction onValueChange;
        LuaFunction onPress;
        LuaFunction initFinish;
        LuaFunction inputFieldEnd;
        LuaFunction onDestroyed;
        LuaFunction onHover;
        #endregion

        LuaTable table;
        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            LuaTable t = (LuaTable)userData;
            table = (LuaTable)t["uitable"];
            
            if (table == null) return;
            for (int i = 0; i < mVariables.Length; i++)
            {
                var val = mVariables[i];
                table[val.name] = val.val;
            }

            table["gameObject"] = gameObject;
            table["transform"] = transform;
            onClick = table[ON_CLICK] as LuaFunction;
            onHover = table[ON_HOVER] as LuaFunction;
            onPress = table[ON_PRESS] as LuaFunction;
            onValueChange = table[ON_ValueChange] as LuaFunction;
            initFinish = table[INIT_FINISH] as LuaFunction;
            inputFieldEnd = table[ON_INPUTFIELDEND] as LuaFunction;
            onDestroyed = table[ON_DESTROY] as LuaFunction;

            for (int i = 0; i < mVariables.Length; i++)
            {
                var val = mVariables[i];
                string name = val.name;
                if (onClick != null && (val.type == "StarForce.CommonButton" || val.type == "CommonButton"))
                {
                    CommonButton cb = (CommonButton)val.val;
                    cb.OnClick.AddListener(delegate
                    {
                        onClick.call(table, table[name], name);
                    });
                }

                if (onHover != null && (val.type == "StarForce.CommonButton" || val.type == "CommonButton"))
                {
                    CommonButton cb = (CommonButton)val.val;
                    cb.OnHover.AddListener(delegate
                    {
                        onHover.call(table, table[name], name);
                    });
                }

                if (onClick != null && (val.type == "UnityEngine.UI.Button" || val.type == "Button"))
                {
                    Button btn = val.val as Button;
                    btn.onClick.AddListener(delegate
                    {
                        onClick.call(table, table[name], name);
                    });
                }
                if (onValueChange != null && (val.type == "UnityEngine.UI.Toggle" || val.type == "Toggle"))
                {
                    Toggle btn = val.val as Toggle;
                    btn.onValueChanged.AddListener((ret) =>
                    {
                        onValueChange.call(table, table[name], name, ret);
                    });
                }

                if (inputFieldEnd != null && (val.type == "UnityEngine.UI.InputField" || val.type == "InputField"))
                {
                    InputField btn = val.val as InputField;
                    btn.onEndEdit.AddListener((ret) =>
                    {
                        inputFieldEnd.call(table, table[name], name, ret);
                    });
                }
                if (onPress != null && (val.type == "EventTips"))
                {
                    EventTips tips = val.val as EventTips;
                    tips.onPress = delegate (bool ret)
                    {
                        onPress.call(table, name, ret);
                    };
                }
            }
            if (initFinish != null)
            {
                initFinish.call(table);
            }

            table.invoke("OnInit", table, userData);
        }

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            table.invoke("OnOpen", table, userData);
        }

        protected internal override void OnClose(object userData)
        {
            base.OnClose(userData);         

            if(GameEntry.Lua && GameEntry.Lua.IsMain())
            {
                table.invoke("OnClose", table, userData);
            }
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            table.invoke("OnUpdate", table, elapseSeconds, realElapseSeconds);
        }
    }
}
