local  ProcedureSceneBase = RequireCommon("procedure.ProcedureSceneBase")

local ProcedureScene_2 = class("ProcedureScene_2", ProcedureSceneBase)
local GameMode = RequireCommon("game.GameMode")
local SurvivalGame = RequireCommon("game.SurvivalGame")

local GameOverDelayedSeconds = 2

function ProcedureScene_2:GotoMenu( ... )
	self.m_GotoMenu = true
end

function ProcedureScene_2:OnDestroy( ... )
	ProcedureScene_2.super.OnDestroy(self, ...)

	self.m_Games = nil
end

function ProcedureScene_2:OnEnter( procedureOwner )
	ProcedureScene_2.super.OnEnter(self, procedureOwner)

	if not self.m_Games then
		self.m_Games = {[GameMode.Survival]=SurvivalGame.new()}
	end

	self.m_GotoMenu = false

	local gameMode = self.procedure:GetData(procedureOwner, StarForce.Constant.ProcedureData.GameMode)
	self.m_CurrentGame = self.m_Games[gameMode]
	self.m_CurrentGame:Initialize()
end

function ProcedureScene_2:OnLeave( ... )
	if self.m_CurrentGame then
		self.m_CurrentGame:Shutdown()
		self.m_CurrentGame = nil
	end
	ProcedureScene_2.super.OnLeave(self,...)
end

function ProcedureScene_2:OnUpdate( procedureOwner, elapseSeconds, realElapseSeconds )
	ProcedureScene_2.super.OnUpdate(self, procedureOwner, elapseSeconds, realElapseSeconds)
	if self.m_CurrentGame and not self.m_CurrentGame.GameOver then
		self.m_CurrentGame:Update(elapseSeconds, realElapseSeconds)
		return		
	end

	if not self.m_GotoMenu then
		self.m_GotoMenu = true
		self.m_GotoMenuDelaySeconds = 0
	end

	self.m_GotoMenuDelaySeconds = self.m_GotoMenuDelaySeconds + elapseSeconds

	if self.m_GotoMenuDelaySeconds >= GameOverDelayedSeconds then
	    self.procedure:SetData(procedureOwner, StarForce.Constant.ProcedureData.NextSceneId, VarInt(tonumber(SceneId.Menu)))
		self.procedure:ChangeState(procedureOwner)
	end
end

return ProcedureScene_2