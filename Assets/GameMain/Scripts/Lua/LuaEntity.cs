using UnityEngine;
using UnityGameFramework.Runtime;
using SLua;
using GameFramework;

namespace StarForce
{
    public class LuaEntity : EntityLogic
    {
        public int Id
        {
            get
            {
                return Entity.Id;
            }
        }
        private LuaTable m_UserTable = null;
        public Animation CachedAnimation
        {
            get;
            private set;
        }

        public LuaTable UserTable
        {
            get
            {
                return m_UserTable;
            }
        }

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            CachedAnimation = GetComponent<Animation>();
            LuaTable luadata = (LuaTable)userData;
            string logicName = (string)luadata["logicName"];
            LuaTable t = GameEntry.Lua.DoString(logicName) as LuaTable;
            m_UserTable = t.invoke("new", userData) as LuaTable;

            if (!CanCall())
            {
                return;
            }
            UserTable["CSEntity"] = this;
            UserTable.invoke("OnInit",UserTable, userData);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (!CanCall())
            {
                return;
            }
            UserTable.invoke("OnShow",UserTable, userData);
        }

        protected internal override void OnHide(object userData)
        {
            base.OnHide(userData);
            if (!CanCall())
            {
                return;
            }
            UserTable.invoke("OnHide", UserTable, userData);
        }

        protected internal override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);
            if (!CanCall())
            {
                return;
            }
            UserTable.invoke("OnAttached",UserTable, childEntity, parentTransform, userData);
        }

        protected internal override void OnDetached(EntityLogic childEntity, object userData)
        {
            base.OnDetached(childEntity, userData);
            if (!CanCall())
            {
                return;
            }
            UserTable.invoke("OnDetached", UserTable, childEntity, userData);
        }

        protected internal override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);
            if (!CanCall())
            {
                return;
            }
            UserTable.invoke("OnAttachTo", UserTable, parentEntity, parentTransform, userData);
        }

        protected internal override void OnDetachFrom(EntityLogic parentEntity, object userData)
        {
            base.OnDetachFrom(parentEntity, userData);
            if (!CanCall())
            {
                return;
            }
            UserTable.invoke("OnDetachFrom", UserTable, parentEntity, userData);
        }
        
        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (!CanCall())
            {
                return;
            }
            UserTable.invoke("OnUpdate", UserTable, elapseSeconds, realElapseSeconds);
        }

        private void OnTriggerEnter(Collider other)
        {
            InvokeTable("OnTriggerEnter", other);
        }
        
        private void OnTriggerExit(Collider other)
        {
            InvokeTable("OnTriggerExit", other);
        }
        
        private void InvokeTable(string s, Collider o)
        {
            if (!CanCall())
            {
                return;
            }

            UserTable.invoke(s, UserTable, o);
        }

        private void InvokeTable(string s, object o)
        {
            if (!CanCall())
            {
                return;
            }

            UserTable.invoke(s, UserTable, o);
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