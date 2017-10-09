local TargetableObject = RequireCommon("entity.EntityLogic.TargetableObject")
local Aircraft = class("Aircraft",TargetableObject)
local Thruster = RequireCommon("entity.EntityLogic.Thruster")
local Weapon = RequireCommon("entity.EntityLogic.Weapon")
local Armor = RequireCommon("entity.EntityLogic.Armor")
local Effect = RequireCommon("entity.EntityLogic.Effect")

local EffectData = RequireCommon("entity.EntityData.EffectData")

local EntityConfig = RequireCommon("config.Entity")
local ImpactData = RequireCommon("entity.EntityData.ImpactData")

function Aircraft:ctor( ... )
	Aircraft.super.ctor(self, ...)
	self.m_Weapons = {}
	self.m_Armors = {}
end

function Aircraft:OnShow( userData )
	Aircraft.super.OnShow(self, userData)
	self.m_AircraftData = self.UserData

	if not self.m_AircraftData or not self.m_AircraftData.IsAircraft then
		logError("Aircraft data is invalid.")
		return
	end

	self.CSEntity.Name = string.format("Aircraft (%s)", self.CSEntity.Id)

	local thrusterData = self.m_AircraftData.ThrusterData
	Game.ShowLuaEntity("Thruster", thrusterData, "Thruster")


    local weaponDatas = self.m_AircraftData.WeaponDatas
    for k,v in pairs(weaponDatas) do
    	Game.ShowLuaEntity("Weapon",v, "Weapon")
    end

    local armorDatas = self.m_AircraftData.ArmorDatas
    for k,v in pairs(armorDatas) do
    	Game.ShowLuaEntity("Armor",v, "Armor")
    end
end

function Aircraft:OnHide( ... )
	Aircraft.super.OnHide(self, ...)
end

function Aircraft:OnAttached( childEntity, parentTransform, userData )
	Aircraft.super.OnAttached( self, childEntity, parentTransform, userData)

	if childEntity.UserTable.NAME == "Thruster" then
		self.m_Thruster = childEntity.UserTable
		return
	end

	if childEntity.UserTable.NAME == "Weapon" then
		self.m_Weapons[childEntity.Id] = childEntity.UserTable
		return
	end

	if childEntity.UserTable.NAME == "Armor" then
		self.m_Armors[childEntity.Id] = childEntity.UserTable
		return
	end

end

function Aircraft:OnDetached( childEntity, userData )
	Aircraft.super.OnDetached( self, childEntity, userData)

	if childEntity.UserTable.NAME == "Thruster" then
		self.m_Thruster = nil
		return
	end

	if childEntity.UserTable.NAME == "Weapon" then
		self.m_Weapons[childEntity.Id] = nil
		return
	end

	if childEntity.UserTable.NAME == "Armor" then
		self.m_Armors[childEntity.Id] = nil
		return
	end

end

function Aircraft:OnDead(...)
	Aircraft.super.OnDead(self, ...)

	local effectData = EffectData.new(GameEntry.Entity:GenerateSerialId(), self.m_AircraftData.DeadEffectId)

	effectData.Position = self:CachedTransform().localPosition

	Game.ShowLuaEntity("Effect", effectData, "Effect")

  	GameEntry.Sound:PlaySound1(self.m_AircraftData.DeadSoundId)
end

function Aircraft:GetImpactData( )
	return ImpactData.new(self.m_AircraftData.Camp, self.m_AircraftData.HP, 0, self.m_AircraftData.Defense)
end

return Aircraft