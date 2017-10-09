local EntityData = RequireCommon("entity.EntityData.EntityData")
local TargetableObjectData = class("TargetableObjectData", EntityData)
local CampType = RequireCommon("Definition.Enum.CampType")


function TargetableObjectData:ctor(entityId, typeId , camp)
	TargetableObjectData.super.ctor(self, entityId, typeId, camp)
	self.HP = 0
	self.Camp = CampType.Unknown
end

function  TargetableObjectData:HPRatio( )
	return self.MaxHP >0 and self.HP/ self.MaxHP or 0
end

function TargetableObjectData:IsTargetableObject( ... )
	return true
end

return TargetableObjectData