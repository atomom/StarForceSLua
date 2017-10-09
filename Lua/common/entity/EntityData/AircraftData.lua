local TargetableObjectData = RequireCommon("entity.EntityData.TargetableObjectData")
local AircraftData = class("AircraftData", TargetableObjectData)
local Aircraft = RequireCommon("config.Aircraft")
local ThrusterData = RequireCommon("entity.EntityData.ThrusterData")
local WeaponData = RequireCommon("entity.EntityData.WeaponData")
local ArmorData = RequireCommon("entity.EntityData.ArmorData")

function AircraftData:ctor(entityId, typeId ,camp)
	AircraftData.super.ctor(self, entityId, typeId, camp)
	self.ArmorDatas = {}
	self.WeaponDatas = {}

	local drAircraft = Aircraft[self.TypeId]
	if not drAircraft then
        return
    end
	self.ThrusterData = ThrusterData.new(GameEntry.Entity:GenerateSerialId(), drAircraft.ThrusterId, self.Camp, self.Id)

	local keys = table.keys(drAircraft)
	for i=0,#keys do
		local wid = drAircraft[string.format("WeaponId%s",i)]
		if wid and wid>0 then
			self:AttachWeaponData(WeaponData.new(GameEntry.Entity:GenerateSerialId(), wid, self.Camp, self.Id))
		end
	end

	for i=0,#keys do
		local aid = drAircraft[string.format("ArmorId%s",i)]
		if aid and aid>0 then
			self:AttachArmorData(ArmorData.new(GameEntry.Entity:GenerateSerialId(), aid, self.Camp, self.Id))
		end
	end

	self.DeadEffectId = drAircraft.DeadEffectId
	self.DeadSoundId = drAircraft.DeadSoundId
	self.HP = self.MaxHP
end

function AircraftData:AttachWeaponData( weaponData )
	if not weaponData then
		return
	end

	if self.WeaponDatas[weaponData.Id] then
		return
	end

	self.WeaponDatas[weaponData.Id] = weaponData
end

function AircraftData:AttachArmorData( armorData )
	if not armorData then
		return
	end

	if self.ArmorDatas[armorData.Id] then
		return
	end

	self.ArmorDatas[armorData.Id] = armorData
	self:RefreshData()
end

function AircraftData:DetachArmorData( armorData )
	if not armorData then
		return
	end

	self.ArmorDatas[armorData.EntityId] = nil
	self:RefreshData()
end

function AircraftData:Speed()
	return  self.ThrusterData.Speed
end

function AircraftData:RefreshData( ... )
	self.MaxHP = 0
	self.Defense = 0

	for k,v in pairs(self.ArmorDatas) do
		self.MaxHP = self.MaxHP + v.MaxHP
		self.Defense = self.Defense + v.Defense
	end

    if self.HP > self.MaxHP then
        self.HP = self.MaxHP
	end
end

function AircraftData:IsAircraft()
	return true
end

return AircraftData