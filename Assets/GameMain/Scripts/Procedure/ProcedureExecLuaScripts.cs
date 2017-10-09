using GameFramework.Fsm;
using GameFramework.Procedure;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityGameFramework.Runtime;
using StarForce.Lua;

namespace StarForce
{
    public class ProcedureExecLuaScripts : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            var luaComp = GameEntry.Lua;
            luaComp.Start("common/Main", true, (i) =>
            {
                if(i==101)
                {
                    ChangeState<ProcedureChangeScene>(procedureOwner);
                }
            });
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        }
    }
}
