using GameFramework;
using SLua;
using StarForce;
using StarForce.Lua;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LuaClient : MonoBehaviour {

    private LuaSvr luaSvr;
    private LuaState luaState;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        Init();
        Start("common/Main", true, (i) =>
        {
            if (i == 101)
            {
                GameEntry.Lua = this;
                StartCoroutine(LoadScene());
            }
        });
    }

    private void OnDestroy()
    {
        Deinit();
    }

    IEnumerator LoadScene()
    {
        AsyncOperation a = SceneManager.LoadSceneAsync(0);
        yield return a;
        while (!a.isDone)
        {
            Log.Debug("loadscene ...{0}" ,a.progress);
        }
    }

    private void Init()
    {
        luaSvr = new LuaSvr();
        luaState = LuaSvr.mainState;
        luaState.loaderDelegate = LuaHelper.DoFile; //自定义加载Lua的文件

#if UNITY_EDITOR && UNITY_STANDALONE
        FileUtils.getInstance().genStreamPath();
#else
            FileUtils.getInstance().genSearchPath();
#endif
    }

    public void Start(string main, bool updateSuccess, Action<int> cb)
    {
        luaSvr.init(cb, () => {
            if (cb != null) cb(101);
            luaSvr.start(main);
        }, LuaSvrFlag.LSF_BASIC | LuaSvrFlag.LSF_3RDDLL | LuaSvrFlag.LSF_EXTLIB);
    }

    private void Deinit()
    {
        LuaSvr.mainState = null;
        luaState.Dispose();
        luaState = null;
        luaSvr = null;
        LuaHelper.Clear();
    }

    #region 执行lua代码
    public object DoString(string script)
    {
        return luaState.doString(script);
    }

    public object DoBuffer(byte[] bytes)
    {
        object obj;
        if (luaState.doBuffer(bytes, "temp buffer", out obj))
            return obj;
        return null;
    }

    public object DoFile(string path)
    {
        return luaState.doFile(path);
    }

    public bool IsMain()
    {
        return LuaSvr.mainState != null;
    }
    #endregion
}
