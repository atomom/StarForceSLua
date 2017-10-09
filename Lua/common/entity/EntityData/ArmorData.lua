local AccessoryObjectData = RequireCommon("entity.EntityData.AccessoryObjectData")
local ArmorData = class("ArmorData", AccessoryObjectData)

local Armor = RequireCommon("config.Armor")

function ArmorData:ctor(entityId, typeId, camp, ownerId)
	ArmorData.super.ctor(self, entityId, typeId, camp, ownerId)
	self.Speed = 0
	local drArmor = Armor[self.TypeId]
	if not drArmor then
		return
	end

    self.MaxHP = drArmor.MaxHP
    self.Defense = drArmor.Defense
end

function ArmorData:IsArmor( ... )
	return true
end

return ArmorData