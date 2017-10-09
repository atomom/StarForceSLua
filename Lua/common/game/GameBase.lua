local GameBase = class("GameBase")
local EntityConfig = RequireCommon("config.Entity")

local MyAircraft = RequireCommon("entity.EntityLogic.MyAircraft")
local MyAircraftData = RequireCommon("entity.EntityData.MyAircraftData")
local BaseBehavior = RequireCommon("utility.BaseBehavior")

function GameBase:Initialize( )
	self.m_OnShowEntitySuccess = Handler(self.OnShowEntitySuccess, self)
	self.m_OnShowEntityFailure = Handler(self.OnShowEntityFailure, self)
    GameEntry.Event:Subscribe(ShowEntitySuccessEventArgs.EventId, self.m_OnShowEntitySuccess)
    GameEntry.Event:Subscribe(ShowEntityFailureEventArgs.EventId, self.m_OnShowEntityFailure)

    self.SceneBackground = Object.FindObjectOfType(ScrollableBackground)
    if not self.SceneBackground then
        logWarn("Can not find scene background.")
        return
    end

    local vg = self.SceneBackground.VisibleBoundary.gameObject
    vg.name = "Scene_HideByBoundary"
    vg:GetOrAddComponent(LuaBehavior)
    
    local Id = 10000
    local typeId = 10000
    local mydata = MyAircraftData.new(Id, typeId)

    Game.ShowLuaEntity("MyAircraft", mydata, "Aircraft")

    self.GameOver = false
    self.m_MyAircraft = nil
end

function GameBase:Shutdown( )
    GameEntry.Event:Unsubscribe(ShowEntitySuccessEventArgs.EventId, self.m_OnShowEntitySuccess)
    GameEntry.Event:Unsubscribe(ShowEntityFailureEventArgs.EventId, self.m_OnShowEntityFailure)
end

function GameBase:Update( elapseSeconds, realElapseSeconds )
    if self.m_MyAircraft and self.m_MyAircraft:IsDead() then
    	self.GameOver = true
    	return
    end
end

function GameBase:OnShowEntitySuccess( sender, e )
    local csharpLogic = e.Entity.Logic
    local luaLogic = csharpLogic.UserTable
    if luaLogic.IsMyAircraft and luaLogic.IsMyAircraft() then
        self.m_MyAircraft = luaLogic
    end
end

function GameBase:OnShowEntityFailure( sender, e )
    logWarn("Show entity failure with error message %s.", e.ErrorMessage)
end

return GameBase