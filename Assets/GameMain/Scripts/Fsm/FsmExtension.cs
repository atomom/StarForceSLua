using UnityEngine;

using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public static class FsmExtension
    {
        public static void SetDataX(this ProcedureOwner owenr, string s, int i)
        {
            owenr.SetData(s, (VarInt)i);
        }
    }
}