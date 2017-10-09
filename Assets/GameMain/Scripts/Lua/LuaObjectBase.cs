using GameFramework.ObjectPool;

namespace StarForce
{
    public class LuaObjectBase : ObjectBase
    {
        public LuaObjectBase(object target)
            : base(target)
        {

        }

        protected override void Release()
        {
            LuaBehavior objItem = (LuaBehavior)Target;
            if (objItem == null)
            {
                return;
            }

            UnityEngine.Object.Destroy(objItem.gameObject);
        }
    }

}