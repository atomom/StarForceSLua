local ProcedureSceneBase = RequireCommon("procedure.ProcedureSceneBase")
local UIFormId = RequireCommon("ui.UIFormId")

local GameMode = RequireCommon("game.GameMode")
local ProcedureScene_1 = class("ProcedureScene_1", ProcedureSceneBase)

function ProcedureScene_1:StartGame( ... )
	self.m_StartGame = true
end

function ProcedureScene_1:OnEnter( ... )
	ProcedureScene_1.super.OnEnter(self, ...)

	self.m_StartGame = false

	self.func = Handler(self.OnOpenUIFormSuccess, self)

	GameEntry.Event:Subscribe(OpenUIFormSuccessEventArgs.EventId, self.func)

	local t = RequireCommon("ui.MenuForm").new()
	self.uitable = t
	GameEntry.UI:OpenUIForm(UIFormId.MenuForm, self)
end

function ProcedureScene_1:OnLeave( procedureOwner, isShutdown )
	ProcedureScene_1.super.OnLeave(self, procedureOwner, isShutdown)

    GameEntry.Event:Unsubscribe(OpenUIFormSuccessEventArgs.EventId, self.func)

    if self.m_MenuForm then
    	self.m_MenuForm:Close(isShutdown)
    	self.m_MenuForm = nil
    end

end

function ProcedureScene_1:OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds )
	ProcedureScene_1.super.OnUpdate(self, procedureOwner, elapseSeconds, realElapseSeconds)

	if self.m_StartGame then
		self.procedure:SetData(procedureOwner, StarForce.Constant.ProcedureData.GameMode,tonumber(GameMode.Survival))

	    self.procedure:SetData(procedureOwner, StarForce.Constant.ProcedureData.NextSceneId,tonumber(SceneId.Main))
		self.procedure:ChangeState(procedureOwner)
	end
end

function ProcedureScene_1:OnOpenUIFormSuccess( sender, ne )
    
    if (ne.UserData ~= self) then
    	return
    end

    self.m_MenuForm = ne.UIForm.Logic
end



return ProcedureScene_1