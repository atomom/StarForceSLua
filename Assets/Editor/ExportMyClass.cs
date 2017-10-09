using System.Collections.Generic;
using SLua;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using YunvaIM;
using Umeng;
using System;
using UnityGameFramework.Runtime;
using GameFramework;
using StarForce;
using GameFramework.ObjectPool;

public class ExportMyClass
{
    static public void OnAddCustomClass(LuaCodeGen.ExportGenericDelegate add)
    {
        add(typeof(Tween), null);
        add(typeof(Tweener), null);

        add(typeof(ABSAnimationComponent), null);
        add(typeof(DOTweenAnimation), null);

        add(typeof(Ease), null);

        add(typeof(YunVaImSDK), null);
        add(typeof(YunvaMsgBase), null);
        add(typeof(ImThirdLoginResp), null);
        add(typeof(ImChannelLoginResp), null);
        add(typeof(ImChannelMotifyResp), null);
        add(typeof(ImRecordStopResp), null);
        add(typeof(ImRecordFinishPlayResp), null);
        add(typeof(ImChannelSendMsgResp), null);
        add(typeof(ImChannelMessageNotify), null);
        add(typeof(EventListenerManager), null);
        add(typeof(ImChannelPushMsgNotify), null);
        add(typeof(YunVaImSDK), null);

        add(typeof(ProtocolEnum), null);

        add(typeof(System.DateTime), null);

        add(typeof(GA), null);

        add(typeof(DG.Tweening.Sequence), null);
        add(typeof(DG.Tweening.DOTween), null);

        add(typeof(Native.JTPayTool), null);


        add(typeof(StarForce.GameEntry), "GameEntry");
        add(typeof(BaseComponent), null);
        add(typeof(DataNodeComponent), null);
        add(typeof(DebuggerComponent), null);
        add(typeof(DownloadComponent), null);
        add(typeof(EntityComponent), null);
        add(typeof(FsmComponent), null);
        add(typeof(LocalizationComponent), null);
        add(typeof(NetworkComponent), null);
        add(typeof(ObjectPoolComponent), null);
        add(typeof(ProcedureComponent), null);
        add(typeof(ResourceComponent), null);
        add(typeof(SceneComponent), null);
        add(typeof(SettingComponent), null);
        add(typeof(SoundComponent), null);
        add(typeof(UIComponent), null);
        add(typeof(WebRequestComponent), null);
        add(typeof(EventComponent), null);

        add(typeof(StarForce.ProcedureLuaBase), null);

        add(typeof(VarInt), "VarInt");

        add(typeof(OpenUIFormSuccessEventArgs), "OpenUIFormSuccessEventArgs");

        add(typeof(SceneId), "SceneId");
        add(typeof(Constant), "Constant");
        add(typeof(Constant.ProcedureData), null);

        add(typeof(ShowEntitySuccessEventArgs), "ShowEntitySuccessEventArgs");
        add(typeof(ShowEntityFailureEventArgs), "ShowEntityFailureEventArgs");

        add(typeof(LuaEntity), "LuaEntity");
        add(typeof(EntityLogic), "EntityLogic");

        add(typeof(ScrollableBackground), "ScrollableBackground");
        add(typeof(Utility.Random), "Utility.Random");
        add(typeof(LuaBehavior), "LuaBehavior");
        add(typeof(LuaGFComponent), "LuaGFComponent");
        add(typeof(LuaObjectBase), "LuaObjectBase");
        add(typeof(ObjectBase), null);
    }

    internal static void OnGetAssemblyToGenerateExtensionMethod(ref List<string> list)
    {
        list.Add("DOTween");
        list.Add("DOTween43");
        list.Add("DOTween46");
        list.Add("DOTween50");
    }

    internal static void OnAddUseClassList(ref List<string> list)
    {
        List<System.Type> types = new List<System.Type>()
        {
            typeof(UnityEngine.Object),
            typeof(Transform),
            typeof(RectTransform),
            typeof(YieldInstruction),
            typeof(WaitForEndOfFrame),
            typeof(WaitForSeconds),
            typeof(Screen),
            typeof(UnityEngine.Resolution),
            typeof(UnityEngine.Debug),
            typeof(UnityEngine.GameObject),
            typeof(UnityEngine.Component),
            typeof(UnityEngine.Transform),
            typeof(UnityEngine.Behaviour),
            typeof(UnityEngine.MonoBehaviour),
            typeof(AnimationState),
            typeof(Resources),
            typeof(Animation),
            typeof(Animator),
            typeof(AnimatorClipInfo),
            typeof(AnimatorControllerParameter),
            typeof(AnimationClip),
            typeof(AnimationClipPair),
            typeof(UnityEngine.Camera),
            typeof(UnityEngine.Color),
            typeof(UnityEngine.Input),
            typeof(UnityEngine.Physics),
            typeof(UnityEngine.RaycastHit),
            typeof(UnityEngine.Collider),
            typeof(UnityEngine.BoxCollider),
            typeof(UnityEngine.Application),
            typeof(Sprite),
            typeof(UnityEngine.AssetBundle),
            typeof(UnityEngine.EventSystems.UIBehaviour),
            typeof(UnityEngine.UI.Selectable),
            typeof(UnityEngine.UI.Graphic),
            typeof(UnityEngine.UI.MaskableGraphic),
            typeof(UnityEngine.UI.Image),
            typeof(UnityEngine.UI.RawImage),
            typeof(UnityEngine.UI.Button),
            typeof(UnityEngine.UI.Slider),
            typeof(UnityEngine.UI.Scrollbar),
            typeof(UnityEngine.UI.ScrollRect),
            typeof(UnityEngine.UI.Mask),
            typeof(UnityEngine.UI.Text),
            typeof(UnityEngine.UI.Toggle),
            typeof(UnityEngine.UI.InputField),
            typeof(UnityEngine.Color),
            typeof(UnityEngine.Color32),
            typeof(UnityEngine.LayerMask),
            typeof(UnityEngine.TextAsset),
            typeof(UnityEngine.Texture),
            typeof(UnityEngine.Texture2D),
            typeof(UnityEngine.TextureFormat),
            typeof(UnityEngine.Vector2),
            typeof(UnityEngine.Vector3),
            typeof(UnityEngine.Rect),
            typeof(UnityEngine.Quaternion),
            typeof(UnityEngine.SystemInfo),
            typeof(UnityEngine.Time),
            typeof(UnityEngine.WWW),
            typeof(UnityEngine.UI.LayoutGroup),
            typeof(UnityEngine.UI.GridLayoutGroup),
            typeof(UnityEngine.UI.VerticalLayoutGroup),
            typeof(UnityEngine.UI.HorizontalLayoutGroup),
            typeof(UnityEngine.UI.HorizontalOrVerticalLayoutGroup),
            typeof(AudioSource),
            typeof(GL),
            typeof(WWWForm),
            typeof(UnityEvent),
            typeof(PlayerPrefs),
            typeof(UnityEngine.UI.Slider.SliderEvent),
            typeof(UnityEngine.UI.Toggle.ToggleEvent),
            typeof(UnityEngine.UI.ToggleGroup),
            typeof(UnityEngine.EventSystems.EventTrigger),
            typeof(UnityEngine.CanvasGroup),
            typeof(SceneManager),
            typeof(Button.ButtonClickedEvent),
            typeof(UnityEngine.Renderer),
            typeof(UnityEngine.MeshRenderer),
            typeof(UnityEngine.Material),
            typeof(UnityEngine.TextEditor),
            typeof(UnityEngine.RenderSettings),
            typeof(UnityEngine.Rendering.AmbientMode),
            typeof(UserAuthorization),
            typeof(YieldInstruction),
            typeof(AsyncOperation),
            typeof(NetworkReachability),
            typeof(LocationService),
            typeof(LocationServiceStatus),
            typeof(LocationInfo),
            typeof(Space),

        };
        for (int i = 0; i < types.Count; i++)
        {
            list.Add(types[i].FullName);
        }
    }
}
