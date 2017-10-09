local TargetableObject = RequireCommon("entity.EntityLogic.TargetableObject")
local Asteroid = class("Asteroid",TargetableObject)
local EffectData = RequireCommon("entity.EntityData.EffectData")
local Effect = RequireCommon("entity.EntityLogic.Effect")
local ImpactData = RequireCommon("entity.EntityData.ImpactData")
local EntityConfig = RequireCommon("config.Entity")

function Asteroid:ctor( ... )
	Asteroid.super.ctor(self, ...)
	self.m_RotateSphere = Vector3.zero

end

function Asteroid:OnShow( userData )
	Asteroid.super.OnShow(self, userData)
	self.m_AsteroidData = self.UserData
	if not self.m_AsteroidData or not self.m_AsteroidData.IsAsteroid then
		logError("Asteroid data is invalid.")
		return
	end
	self.m_RotateSphere = Random.insideUnitSphere
end

function Asteroid:OnUpdate( elapseSeconds, realElapseSeconds)
	Asteroid.super.OnUpdate(self, elapseSeconds, realElapseSeconds)

    self:CachedTransform():Translate(Vector3.back * self.m_AsteroidData.Speed * elapseSeconds, Space.World)

    self:CachedTransform():Rotate(self.m_RotateSphere * self.m_AsteroidData.AngularSpeed * elapseSeconds, Space.Self)
end


function Asteroid:OnDead( attacker )
	Asteroid.super.OnDead(self, attacker)

	local effectData = EffectData.new(GameEntry.Entity:GenerateSerialId(), self.m_AsteroidData.DeadEffectId)

	effectData.Position = self:CachedTransform().localPosition

    Game.ShowLuaEntity("Effect", effectData, "Effect")

	GameEntry.Sound:PlaySound1(self.m_AsteroidData.DeadSoundId)
end

function Asteroid:GetImpactData( )
	return ImpactData.new(self.m_AsteroidData.Camp, self.m_AsteroidData.HP, self.m_AsteroidData.Attack, 0)
end

return Asteroid