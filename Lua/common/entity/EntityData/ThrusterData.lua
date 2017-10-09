local AccessoryObjectData = RequireCommon("entity.EntityData.AccessoryObjectData")
local ThrusterData = class("ThrusterData", AccessoryObjectData)

local Thruster = RequireCommon("config.Thruster")

function ThrusterData:ctor(entityId, typeId, camp, ownerId)
	ThrusterData.super.ctor(self, entityId, typeId, camp, ownerId)
	self.Speed = 0
	local drThruster = Thruster[self.TypeId]
	if not drThruster then
		return
	end

	self.Speed = drThruster.Speed
end
function ThrusterData:IsThruster( ... )
	return true
end
return ThrusterData