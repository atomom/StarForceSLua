using GameFramework.Event;
using SLua;
using UnityGameFramework.Runtime;
using StarForce.Lua;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityEngine;
using System.Collections.Generic;

namespace StarForce
{
    public class ProcedureLuaBase : ProcedureBase
    {
        [SerializeField]
        private string m_luaPath = "common/procedure/ProcedureScene_{0}";
        private Dictionary<int, LuaTable> luas = new Dictionary<int, LuaTable>();
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        public string LuaPath
        {
            get
            {
                return m_luaPath;
            }

            set
            {
                m_luaPath = value;
            }
        }

        private LuaTable table;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            int sceneid = procedureOwner.GetData<VarInt>(Constant.ProcedureData.NextSceneId);
            luas.TryGetValue(sceneid,out table);
            if (table==null)
            {
                var luaComp = GameEntry.Lua;
                object ret = luaComp.DoFile(string.Format(LuaPath, sceneid));
                table = (LuaTable)((LuaTable)ret).invoke("new", this);
                luas.Add(sceneid, table);
            }
            table.invoke("OnEnter", table, procedureOwner);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            if (GameEntry.Lua && GameEntry.Lua.IsMain())
            {
                table.invoke("OnLeave", table, procedureOwner, isShutdown);
            }
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            table.invoke("OnUpdate", table, procedureOwner, elapseSeconds, realElapseSeconds);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
            if (GameEntry.Lua && GameEntry.Lua.IsMain())
            {
                table.invoke("OnDestroy", table, procedureOwner);
            }
        }

        public void ChangeState(ProcedureOwner procedureOwner)
        {
            ChangeState<ProcedureChangeScene>(procedureOwner);
        }

        public void SetData(ProcedureOwner procedureOwner, string s, int i)
        {
            procedureOwner.SetData(s, (VarInt)i);
        }

        public int GetData(ProcedureOwner procedureOwner, string s)
        {
            return (int)procedureOwner.GetData(s).GetValue();
        }
    }
}
