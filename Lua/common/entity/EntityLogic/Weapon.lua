local BaseEntity = RequireCommon("entity.EntityLogic.BaseEntity")
local Bullet = RequireCommon("entity.EntityLogic.Bullet")
local BulletData = RequireCommon("entity.EntityData.BulletData")
local EntityConfig = RequireCommon("config.Entity")

local  AttachPoint = "Weapon Point"

local Weapon = class("Weapon",BaseEntity)

function Weapon:OnShow( userData )
	Weapon.super.OnShow(self, userData)
	self.m_NextAttackTime = 0
	self.m_WeaponData = self.UserData
	if not self.m_WeaponData or not self.m_WeaponData.IsWeapon then
        logError("Weapon data is invalid.")
        return
	end

    GameEntry.Entity:AttachLuaEntity(self.CSEntity, self.m_WeaponData.OwnerId, AttachPoint)

end

function Weapon:OnAttachTo( parentEntity, parentTransform, userData )
	Weapon.super.OnAttachTo(self, parentEntity, parentTransform, userData)

	self.CSEntity.Name = string.format("Weapon of %s", parentEntity.Name)
	self:CachedTransform().localPosition = Vector3.zero
end

function Weapon:TryAttack( ... )
	if Time.time < self.m_NextAttackTime then
		return
	end

	self.m_NextAttackTime = Time.time + self.m_WeaponData.AttackInterval
	local bulletData = BulletData.new(GameEntry.Entity:GenerateSerialId(),self.m_WeaponData.BulletId, self.m_WeaponData.Camp,self.m_WeaponData.OwnerId, self.m_WeaponData.Attack, self.m_WeaponData.BulletSpeed)

	bulletData.Position = self:CachedTransform().position

    Game.ShowLuaEntity("Bullet", bulletData, "Bullet")

    GameEntry.Sound:PlaySound1(self.m_WeaponData.BulletSoundId)
end

return Weapon