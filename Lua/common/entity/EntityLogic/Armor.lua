local BaseEntity = RequireCommon("entity.EntityLogic.BaseEntity")
local  AttachPoint = "Armor Point"

local Armor = class("Armor",BaseEntity)

function Armor:OnShow( userData )
	Armor.super.OnShow(self, userData)
	self.m_ArmorData = self.UserData
	if not self.m_ArmorData or not self.m_ArmorData.IsArmor then
        logError("Armor data is invalid.")
        return
	end

    GameEntry.Entity:AttachLuaEntity(self.CSEntity, self.m_ArmorData.OwnerId, AttachPoint)

end

function Armor:OnAttachTo( parentEntity, parentTransform, userData )
	Armor.super.OnAttachTo(self, parentEntity, parentTransform, userData)

	self.CSEntity.Name = string.format("Armor of %s", parentEntity.Name)
	self:CachedTransform().localPosition = Vector3.zero
end

return Armor