using UnityGameFramework.Runtime;

namespace StarForce
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry
    {
        public static ConfigComponent Config
        {
            get;
            private set;
        }

        public static LuaGFComponent LuaGF
        {
            get;
            private set;
        }

        private static void InitCustomComponents()
        {
            Config = UnityGameFramework.Runtime.GameEntry.GetComponent<ConfigComponent>();
            LuaGF = UnityGameFramework.Runtime.GameEntry.GetComponent<LuaGFComponent>();
        }

        public static LuaGFComponent GetLuaComponent(string name)
        {
            return UnityGameFramework.Runtime.GameEntry.GetComponent<LuaGFComponent>();
        }
    }
}
