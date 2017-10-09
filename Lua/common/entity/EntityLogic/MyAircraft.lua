local Aircraft = RequireCommon("entity.EntityLogic.Aircraft")
local MyAircraft = class("MyAircraft",Aircraft)

function MyAircraft:ctor( ... )
	MyAircraft.super.ctor(self, ...)
    self.m_TargetPosition = Vector3.zero
end

function MyAircraft:OnShow( userData )
	MyAircraft.super.OnShow( self, userData )
    self.m_MyAircraftData = self.UserData
	if not self.m_MyAircraftData or not self.m_MyAircraftData.IsMyAircraft then
        logError("My aircraft data is invalid.")
        return
	end

	local sceneBackground = Object.FindObjectOfType(ScrollableBackground)
    if not sceneBackground then    
        logWarning("Can not find scene background.")
        return
    end

    self.m_PlayerMoveBoundary = Rect(sceneBackground.PlayerMoveBoundary.bounds.min.x, sceneBackground.PlayerMoveBoundary.bounds.min.z,
        sceneBackground.PlayerMoveBoundary.bounds.size.x, 
        sceneBackground.PlayerMoveBoundary.bounds.size.z)
end

function MyAircraft:OnUpdate( elapseSeconds, realElapseSeconds )
	MyAircraft.super.OnUpdate(self, elapseSeconds, realElapseSeconds)
	if Input.GetMouseButton(0) then    
        local point = Camera.main:ScreenToWorldPoint(Input.mousePosition)
        self.m_TargetPosition = Vector3(point.x, 0, point.z)

        for k,v in pairs(self.m_Weapons) do
        	v:TryAttack()
        end
    end

    local direction = self.m_TargetPosition - self:CachedTransform().localPosition
    if direction.sqrMagnitude <= Vector3.kEpsilon then    
        return
    end
    
    local v = direction.normalized * self.m_MyAircraftData:Speed()
    v = v *  elapseSeconds
    local speed = Vector3.ClampMagnitude(v , direction.magnitude)

    self:CachedTransform().localPosition = Vector3
    (
        math.clamp(self:CachedTransform().localPosition.x + speed.x, self.m_PlayerMoveBoundary.xMin, self.m_PlayerMoveBoundary.xMax),
        0,
        math.clamp(self:CachedTransform().localPosition.z + speed.z, self.m_PlayerMoveBoundary.yMin, self.m_PlayerMoveBoundary.yMax)
    )
end

function MyAircraft:IsMyAircraft( ... )
    return true
end

return MyAircraft