WWW = UnityEngine.WWW
Yield = UnityEngine.Yield
RenderSettings = UnityEngine.RenderSettings
AmbientMode = UnityEngine.Rendering.AmbientMode
Color = UnityEngine.Color
SystemInfo = UnityEngine.SystemInfo
GameObject = UnityEngine.GameObject
Object = UnityEngine.Object
RectTransform = UnityEngine.RectTransform
Vector3 = UnityEngine.Vector3
Vector2 = UnityEngine.Vector2
Time = UnityEngine.Time
Input = UnityEngine.Input
Camera = UnityEngine.Camera
Rect = UnityEngine.Rect
Space = UnityEngine.Space
LayerMask = UnityEngine.LayerMask
Quaternion = UnityEngine.Quaternion
Random = UnityEngine.Random
Application = UnityEngine.Application
Debug = UnityEngine.Debug
RuntimePlatform = UnityEngine.RuntimePlatform
Screen = UnityEngine.Screen
CanvasGroup = UnityEngine.CanvasGroup
RectTransformUtility = UnityEngine.RectTransformUtility
UI = UnityEngine.UI
WaitForSeconds = UnityEngine.WaitForSeconds

cjson = require "cjson"
JSON = {}
JSON.parse = cjson.decode
JSON.decode = cjson.decode
JSON.encode = cjson.encode
JSON.stringify = cjson.encode

Luas = {}
COMMONNAME = "common"
function InitCommonLua( ... )
    if not Luas[COMMONNAME] then
        Luas[COMMONNAME] = {}
    end
end

function RequireCommon( path )
    InitCommonLua()
    local cpath = COMMONNAME .. "." .. path

    Luas[COMMONNAME][path] = cpath
    return require(cpath)
end

RequireCommon("tool.class")
RequireCommon("tool.define")
RequireCommon("tool.function")
RequireCommon("tool.functionEx1")
RequireCommon("tool.functionEx2")
RequireCommon("utility.game")

function main(updateSuccess,moduleName,data)
end

disable_global()