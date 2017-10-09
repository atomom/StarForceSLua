local AircraftData = RequireCommon("entity.EntityData.AircraftData")
local MyAircraftData = class("MyAircraftData", AircraftData)
local CampType = RequireCommon("Definition.Enum.CampType")

function MyAircraftData:ctor(entityId, typeId)
	MyAircraftData.super.ctor(self, entityId, typeId, CampType.Player)
	self.Position = Vector3.zero
end

function MyAircraftData:IsMyAircraft( ... )
	return true
end

return MyAircraftData