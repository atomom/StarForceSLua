local BaseEntity = RequireCommon("entity.EntityLogic.BaseEntity")
local TargetableObject = class("TargetableObject",BaseEntity)
local Layer = RequireCommon("Definition.Constant.Layer")
local AIUtility = RequireCommon("utility.AIUtility")

function TargetableObject:ctor( ... )
	TargetableObject.super.ctor(self, ...)
end

function TargetableObject:ApplyDamage( attacker, damageHP )
	local fromHPRatio = self.m_TargetableObjectData:HPRatio()
	self.m_TargetableObjectData.HP = self.m_TargetableObjectData.HP - damageHP
	local toHPRatio = self.m_TargetableObjectData:HPRatio()
	if fromHPRatio > toHPRatio then

		if not self.lc then
			local lc = GameEntry.GetLuaComponent("")
			self.lct = lc.UserTable
		end

		if self.lct then
			self.lct:ShowHPBar(self, fromHPRatio, toHPRatio)
		end
	end

	if self.m_TargetableObjectData.HP <= 0 then
		self:OnDead(attacker)
	end
end

function TargetableObject:OnShow( userData )
	TargetableObject.super.OnShow(self, userData)
	self:CachedTransform():SetLayerRecursively(Layer.TargetableObjectLayerId)
	self.m_TargetableObjectData = self.UserData
	if not self.m_TargetableObjectData or not self.m_TargetableObjectData.IsTargetableObject then
		logError("Targetable object data is invalid.")
		return
	end
end

function TargetableObject:OnDead( attacker )
	GameEntry.Entity:HideLuaEntity(self.CSEntity)
end

function TargetableObject:OnTriggerEnter( other )
	local go = other.gameObject
	local entity = go:GetComponent(LuaEntity)
    
    if not entity then
    	return
    end
    local luaLogic = entity.UserTable
    
    if luaLogic.IsTargetableObject and luaLogic:IsTargetableObject() and entity.Id >= self.CSEntity.Id then
    	return
    end
    AIUtility.PerformCollision(self, luaLogic)
end

function TargetableObject:IsTargetableObject( )
	return true
end

function TargetableObject:GetImpactData( )
	
end

function TargetableObject:IsDead( ... )
	return self.m_TargetableObjectData.HP <= 0
end

return TargetableObject