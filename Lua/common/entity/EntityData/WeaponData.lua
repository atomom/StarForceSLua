local AccessoryObjectData = RequireCommon("entity.EntityData.AccessoryObjectData")
local WeaponData = class("WeaponData", AccessoryObjectData)

local Weapon = RequireCommon("config.Weapon")

function WeaponData:ctor(entityId, typeId, camp, ownerId)
	WeaponData.super.ctor(self, entityId, typeId, camp, ownerId)
	self.Speed = 0
	local drWeapon = Weapon[self.TypeId]
	if not drWeapon then
		return
	end

    self.Attack = drWeapon.Attack
    self.AttackInterval = drWeapon.AttackInterval
    self.BulletId = drWeapon.BulletId
    self.BulletSpeed = drWeapon.BulletSpeed
    self.BulletSoundId = drWeapon.BulletSoundId
end

function WeaponData:IsWeapon( ... )
	return true
end

return WeaponData