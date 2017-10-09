local ImpactData = class("ImpactData")

function ImpactData:ctor( Camp,HP,Attack,Defense )
	self.Camp = Camp
	self.HP = HP
	self.Attack = Attack
	self.Defense = Defense
end

function ImpactData:IsImpact( ... )
	return true
end

return ImpactData