local BaseEntity = RequireCommon("entity.EntityLogic.BaseEntity")
local ImpactData = RequireCommon("entity.EntityData.ImpactData")

local Bullet = class("Bullet",BaseEntity)

function Bullet:OnShow( userData )
	Bullet.super.OnShow(self, userData)
	self.m_BulletData = self.UserData
	if not self.m_BulletData or not self.m_BulletData.IsBullet then
        logError("Bullet data is invalid.")
        return
	end
end


function Bullet:OnUpdate( elapseSeconds, realElapseSeconds )
	Bullet.super.OnUpdate(self, elapseSeconds, realElapseSeconds)
	
	self:CachedTransform():Translate(
		(Vector3.forward * self.m_BulletData.Speed * elapseSeconds), Space.World)
end


function Bullet:GetImpactData( )
	return ImpactData.new(self.m_BulletData.Camp, 0, self.m_BulletData.Attack, 0)
end

function Bullet:IsBullet()
	return true
end
return Bullet