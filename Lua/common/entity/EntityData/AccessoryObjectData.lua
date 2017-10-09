local EntityData = RequireCommon("entity.EntityData.EntityData")
local AccessoryObjectData = class("AccessoryObjectData", EntityData)

function AccessoryObjectData:ctor(entityId, typeId, camp, ownerId)
	AccessoryObjectData.super.ctor(self, entityId, typeId, camp, ownerId)
end

function AccessoryObjectData:IsAccessoryObject( ... )
	return true
end

return AccessoryObjectData