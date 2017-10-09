local EntityData = RequireCommon("entity.EntityData.EntityData")
local BulletData = class("BulletData", EntityData)


function BulletData:ctor(entityId, typeId , camp, owerId, attack, speed)
	BulletData.super.ctor(self, entityId, typeId, camp, owerId)
	self.Attack = attack
	self.Speed = speed
end

function BulletData:IsBullet( ... )
	return true
end

return BulletData