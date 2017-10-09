local BaseBehavior = RequireCommon("utility.BaseBehavior")
local HPBar = class("HPBar", BaseBehavior)

function HPBar:Start( ... )
	HPBar.super.Start(self, ...)
	self.m_InstancePoolCapacity = 16
	if not self.m_HPBarInstanceRoot then
		logError("You must set HP bar instance root first.")
	end

	self.m_CachedCanvas = self.HPBarInstancesCavans
	self.m_HPBarItemObjectPool = GameEntry.ObjectPool:CreateSingleSpawnObjectPool(LuaObjectBase,"HPBarItem", self.m_InstancePoolCapacity)

	self.m_ActiveHPBarItems = {}
end


function HPBar:Update( ... )

	for i=#self.m_ActiveHPBarItems , 1, -1 do
		local hpBarItem = self.m_ActiveHPBarItems[i]
		if hpBarItem:Refresh() then

		else
			self:HideHPBar(i, hpBarItem)	
		end
	end
end

function HPBar:HideHPBar(i, hpBarItem )
	hpBarItem:Reset()
	table.remove(self.m_ActiveHPBarItems, i)
	GameEntry.ObjectPool:Unspawn(self.m_HPBarItemObjectPool, hpBarItem.CSEntity)
end

function HPBar:ShowHPBar( entity, fromHPRatio , toHPRatio)
	if not entity then
		logWarn("Entity is invalid.")
		return
	end

	local hpBarItem =  self:GetActiveHPBarItem(entity)
	if not hpBarItem then
		hpBarItem = self:CreateHPBarItem(entity)
		table.insert(self.m_ActiveHPBarItems, hpBarItem)		
	end

	hpBarItem:Init(entity, self.m_CachedCanvas, fromHPRatio, toHPRatio)

end

function HPBar:GetActiveHPBarItem(entity)
	if not entity then
		return
	end

	for i=1,#self.m_ActiveHPBarItems do
		local item = self.m_ActiveHPBarItems[i]
		if item.Owner == entity then
			return item
		end
	end

end

function HPBar:CreateHPBarItem( entity )
	local hpBarItem = nil
	local hpBarItemObject = GameEntry.ObjectPool:Spawn(self.m_HPBarItemObjectPool)
	if hpBarItemObject then
		local luaB = hpBarItemObject.Target
		hpBarItem = luaB.UserTable
	else
		local o = Object.Instantiate(self.HPBarItem)
		o.name = "game_HPBarItem"
		local luaB = o:AddComponent(LuaBehavior)
		hpBarItem = luaB.UserTable
		local transform = hpBarItem.transform
		transform:SetParent(self.m_HPBarInstanceRoot)
		transform.localScale = Vector3.one

		GameEntry.ObjectPool:Register(self.m_HPBarItemObjectPool, LuaObjectBase(luaB), true)
	end

	return hpBarItem
end

return HPBar
