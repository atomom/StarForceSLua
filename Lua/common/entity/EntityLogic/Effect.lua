local BaseEntity = RequireCommon("entity.EntityLogic.BaseEntity")

local Effect = class("Effect",BaseEntity)

function Effect:ctor(...)
	Effect.super.ctor(self, ...)
	self.m_ElapseSeconds = 0
end

function Effect:OnInit( ... )
	Effect.super.OnInit(self, ...)
end

function Effect:OnShow( userData )
	Effect.super.OnShow(self, userData)
	self.m_EffectData = self.UserData
	if not self.m_EffectData or not self.m_EffectData.IsEffect then
        logError("Effect data is invalid.")
        return
	end

    self.m_ElapseSeconds = 0
end


function Effect:OnUpdate( elapseSeconds, realElapseSeconds )
	Effect.super.OnUpdate(self, elapseSeconds, realElapseSeconds)
	
    self.m_ElapseSeconds = self.m_ElapseSeconds + elapseSeconds
    if self.m_ElapseSeconds >= self.m_EffectData.KeepTime then
		GameEntry.Entity:HideLuaEntity(self.CSEntity)
    end
end

return Effect