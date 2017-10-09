local BaseEntity = RequireCommon("entity.EntityLogic.BaseEntity")
local  AttachPoint = "Thruster Point"

local Thruster = class("Thruster",BaseEntity)

function Thruster:OnShow( userData )
	Thruster.super.OnShow(self, userData)
	self.m_ThrusterData = self.UserData
	if not self.m_ThrusterData or not self.m_ThrusterData.IsThruster then
        logError("Thruster data is invalid.")
        return
	end

    GameEntry.Entity:AttachLuaEntity(self.CSEntity, self.m_ThrusterData.OwnerId, AttachPoint)
end

function Thruster:OnAttachTo( parentEntity, parentTransform, userData )
	Thruster.super.OnAttachTo(self, parentEntity, parentTransform, userData)

	self.CSEntity.Name = string.format("Thruster of %s", parentEntity.Name)
	self:CachedTransform().localPosition = Vector3.zero
end

return Thruster