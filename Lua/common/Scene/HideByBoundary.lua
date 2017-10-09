local BaseBehavior = RequireCommon("utility.BaseBehavior")
local HideByBoundary = class("HideByBoundary", BaseBehavior)


function HideByBoundary:ctor( ... )
	HideByBoundary.super.ctor(self,...)

end


function HideByBoundary:OnTriggerExit( other )
	HideByBoundary.super.OnTriggerExit(self, other)
    local go = other.gameObject
    local entity = go:GetComponent(LuaEntity)
    if not entity then
        logError("Unknown GameObject '%s', you must use LuaEntity only.", go.name);
        Object.Destroy(go)
        return
    end

    GameEntry.Entity:HideLuaEntity(entity)
end

return HideByBoundary