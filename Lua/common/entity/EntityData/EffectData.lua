local EntityData = RequireCommon("entity.EntityData.EntityData")
local EffectData = class("EffectData", EntityData)


function EffectData:ctor(entityId, typeId , camp, owerId, attack, speed)
	EffectData.super.ctor(self, entityId, typeId, camp, owerId)
	self.KeepTime = 3
end

function EffectData:IsEffect( ... )
	return true
end

return EffectData