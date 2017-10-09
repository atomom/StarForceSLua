local GameBase = RequireCommon("game.GameBase")
local SurvivalGame = class("SurvivalGame", GameBase)
local GameMode = RequireCommon("game.GameMode")
local AsteroidConfig = RequireCommon("config.Asteroid")
local AsteroidData = RequireCommon("entity.EntityData.AsteroidData")
local Asteroid = RequireCommon("entity.EntityLogic.Asteroid")
local EntityConfig = RequireCommon("config.Entity")

function SurvivalGame:ctor( ... )
	self.GameMode = GameMode.Survival	
	self.m_ElapseSeconds = 0
end

function SurvivalGame:Update( elapseSeconds, realElapseSeconds )
	SurvivalGame.super.Update(self, elapseSeconds, realElapseSeconds)
	self.m_ElapseSeconds = self.m_ElapseSeconds + elapseSeconds

	if self.m_ElapseSeconds >= 1 then

		self.m_ElapseSeconds = 0

		local randomPositionX = self.SceneBackground.EnemySpawnBoundary.bounds.min.x + self.SceneBackground.EnemySpawnBoundary.bounds.size.x *Utility.Random.GetRandomDouble()
        local randomPositionZ = self.SceneBackground.EnemySpawnBoundary.bounds.min.z + self.SceneBackground.EnemySpawnBoundary.bounds.size.z * Utility.Random.GetRandomDouble()
        
        local asteroidData = AsteroidData.new(GameEntry.Entity:GenerateSerialId(), 60000 + Utility.Random.GetRandom(table.nums(AsteroidConfig)))

        asteroidData.Position = Vector3(randomPositionX, 0, randomPositionZ)
        

        Game.ShowLuaEntity("Asteroid",asteroidData, "Asteroid")
	end
end
return SurvivalGame