local BaseEntity = class("BaseEntity")

function BaseEntity:ctor()
end

function BaseEntity:CachedTransform( ... )
	return self.CachedTransforme
end

function BaseEntity:CachedAnimation( ... )
	return self.CachedAnimatione
end

function BaseEntity:OnInit( userData )
end

function BaseEntity:OnShow( userData )
	self.UserData = userData
	if not self.UserData or not self.UserData.IsEntity then
        logError("Entity data is invalid.")
		return
	end
	
	self.CachedTransforme = self.CSEntity.CachedTransform
	self.CachedAnimatione = self.CSEntity.CachedAnimation

	self.CSEntity.Name = string.format("[Entity %s]", self.CSEntity.Id)
    self.CachedTransforme.localPosition = self.UserData.Position
    self.CachedTransforme.localRotation = self.UserData.Rotation
    self.CachedTransforme.localScale = Vector3.one
end

function BaseEntity:OnHide( ... )
	-- body
end

function BaseEntity:OnAttached( ... )
	-- body
end

function BaseEntity:OnDetached( ... )
	-- body
end

function BaseEntity:OnAttachTo( ... )
	-- body
end

function BaseEntity:OnDetachFrom( ... )
	-- body
end

function BaseEntity:OnUpdate( ... )
	-- body
end

function BaseEntity:OnTriggerEnter( ... )
	-- body
end

function BaseEntity:OnTriggerExit( ... )
	-- body
end

return BaseEntity