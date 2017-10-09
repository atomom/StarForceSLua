using UnityEngine;
using UnityEditor;
using UnityGameFramework.Runtime;
using StarForce;
using GameFramework.ObjectPool;

namespace StarFroce
{
    public static class ObjectPoolBaseExtension
    {
        public static void Unspawn(this ObjectPoolComponent pc, ObjectPoolBase that, LuaBehavior obj)
        {
            IObjectPool<LuaObjectBase> pool = that as IObjectPool<LuaObjectBase>;
            if (pool == null)
            {
                return;
            }
            pool.Unspawn(obj);
        }

        public static LuaObjectBase Spawn(this ObjectPoolComponent pc, ObjectPoolBase that)
        {
            IObjectPool<LuaObjectBase> pool = that as IObjectPool<LuaObjectBase>;
            if (pool == null)
            {
                return null;
            }
            return pool.Spawn();
        }

        public static void Register(this ObjectPoolComponent pc, ObjectPoolBase that, LuaObjectBase luaB , bool ret)
        {
            IObjectPool<LuaObjectBase> pool = that as IObjectPool<LuaObjectBase>;
            if (pool == null)
            {
                return;
            }
            pool.Register(luaB, ret);
        }
    }
}