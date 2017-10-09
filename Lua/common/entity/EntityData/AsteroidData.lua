local TargetableObjectData = RequireCommon("entity.EntityData.TargetableObjectData")
local AsteroidData = class("AsteroidData", TargetableObjectData)
local Asteroid = RequireCommon("config.Asteroid")
local CampType = RequireCommon("Definition.Enum.CampType")

function AsteroidData:ctor(entityId, typeId)
	AsteroidData.super.ctor(self, entityId, typeId, CampType.Netural)
	local drAsteroid = Asteroid[self.TypeId]
	if not drAsteroid then
		return
	end

	self.MaxHP = drAsteroid.MaxHP
	self.HP = self.MaxHP
	self.Attack = drAsteroid.Attack
	self.Speed = drAsteroid.Speed
	self.AngularSpeed = drAsteroid.AngularSpeed
    self.DeadEffectId = drAsteroid.DeadEffectId
    self.DeadSoundId = drAsteroid.DeadSoundId
end

function AsteroidData:IsAsteroid( ... )
	return true
end

return AsteroidData