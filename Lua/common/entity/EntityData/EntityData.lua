local EntityData = class("EntityData")

function EntityData:ctor( entityId, typeId ,camp, ownerId)
	self.Id = entityId
	self.TypeId = typeId
	self.Camp = camp
	self.OwnerId = ownerId

	self.Position = Vector3.zero
	self.Rotation = Quaternion.identity
end

function EntityData:IsEntity( ... )
	return true
end

return EntityData