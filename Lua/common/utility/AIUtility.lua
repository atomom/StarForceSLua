local CampPairToRelation = RequireCommon("config.CampPairToRelation")
local RelationType = RequireCommon("Definition.Enum.RelationType")
local CampType = RequireCommon("Definition.Enum.CampType")
local AIUtility = {}

local function GetKey(first, second)
    return string.format("%s_%s", first, second)
end

local function Relations()
    if not AIUtility.m_relations then
        AIUtility.m_relations = {}
        for i, v in pairs(CampPairToRelation) do
            local key = GetKey(v[1],v[2])
            table[key] = v[3]
        end
    end
    return AIUtility.m_relations
end

local function GetRelation(first, second)
    if first>second then
        local temp = first
        first = second
        second = temp
    end

    local relationType = Relations()[GetKey(first, second)] or RelationType.Unknown
    return relationType
end


local function CalcDamageHP(attack, defense)
    if (attack <= 0) then
        return 0
    end

    if (defense < 0) then
        defense = 0
    end

    return attack * attack / (attack + defense)
end

function AIUtility.GetCamps(camp, relation)
    local result
    if not AIUtility.m_camps then
        AIUtility.m_camps  = {}
    end

    result = AIUtility.m_camps[GetKey(camp, relation)]
    if result then
        return result
    end

    local camps = {}
    for i, v in pairs(CampType) do
        if GetRelation(camp, v) == relation then
            table.insert(camps, v)
        end
    end

    AIUtility.m_camps[GetKey(camp, relation)] = camps

    result = camps
    return result
end

function AIUtility.GetDistance(from, to)
    local ft = from.CachedTransform
    local tt = to.CachedTransform
    return (tt.position - ft.position).magnitude
end

function AIUtility.PerformCollision( entity, other )
	if not entity or not other then
		return
	end

	local target = other
	if target and target.IsTargetableObject and target:IsTargetableObject() then
		local entityImpactData = entity:GetImpactData()
		local targetImpactData = other:GetImpactData()

		if GetRelation(entityImpactData.Camp, targetImpactData.Camp) == RelationType.Friendly then
			return
		end

		local entityDamageHP = CalcDamageHP(targetImpactData.Attack, entityImpactData.Defense)
        local targetDamageHP = CalcDamageHP(entityImpactData.Attack, targetImpactData.Defense)

        local delta = math.min(entityImpactData.HP - entityDamageHP, targetImpactData.HP - targetDamageHP)
        if (delta > 0) then
            entityDamageHP = entityDamageHP + delta
            targetDamageHP = targetDamageHP + delta
        end

        entity:ApplyDamage(target, entityDamageHP)
        target:ApplyDamage(entity, targetDamageHP)
        return
	end

	local bullet = other
    if bullet and bullet.IsBullet and bullet:IsBullet() then
    
        local entityImpactData = entity:GetImpactData()
        local bulletImpactData = bullet:GetImpactData()
        if (GetRelation(entityImpactData.Camp, bulletImpactData.Camp) == RelationType.Friendly) then
            return
		end

        local entityDamageHP = CalcDamageHP(bulletImpactData.Attack, entityImpactData.Defense)

        entity:ApplyDamage(bullet, entityDamageHP)
        GameEntry.Entity:HideLuaEntity(bullet.CSEntity)
        return
    end

end


return AIUtility