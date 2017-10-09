local LayerMask = LayerMask
local t = {
	DefaultLayerName = "Default",
	UILayerName = "UI",
	TargetableObjectLayerName = "Targetable Object",
}

local l = {
	DefaultLayerId = LayerMask.NameToLayer(t.DefaultLayerName),
    UILayerId = LayerMask.NameToLayer(t.UILayerName),
    TargetableObjectLayerId = LayerMask.NameToLayer(t.TargetableObjectLayerName)
}

table.merge(t,l)

return t