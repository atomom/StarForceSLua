local BaseBehavior = RequireCommon("utility.BaseBehavior")
local HPBarItem = class("HPBarItem", BaseBehavior)

function HPBarItem:ctor( ... )
	HPBarItem.super.ctor(self, ...)
	self.AnimationSeconds = 0.3
    self.KeepSeconds = 0.4
    self.FadeOutSeconds = 0.3
    self.OwnerId = 0
end

function HPBarItem:Awake()
	self.m_CachedTransform = self.gameObject:GetComponent(RectTransform)
	if not self.m_CachedTransform then
        logError("RectTransform is invalid.")
        return
	end

	self.m_CachedCanvasGroup = self.gameObject:GetComponent(CanvasGroup)
	if not self.m_CachedCanvasGroup then
        logError("CanvasGroup is invalid.")
        return
    end

    self.m_HPBar = self.transform:Find("HPBar"):GetComponent(UI.Slider)
end

function HPBarItem:Init( owner, parentCavas, fromHPRatio, toHPRatio )
	if not owner then
        logError("Owner is invalid.")
        return
	end

	self.m_ParentCanvas = parentCavas
	self.gameObject:SetActive(true)

	self.CSEntity:StopAllCoroutines()

	self.m_CachedCanvasGroup.alpha = 1
	if self.Owner~=owner or self.OwnerId~=owner.Id then
		self.m_HPBar.value = fromHPRatio
		self.Owner = owner
		self.OwnerId = owner.CSEntity.Id
	end

	self:Refresh()

	self:HPBarCo(toHPRatio, self.AnimationSeconds, self.KeepSeconds, self.FadeOutSeconds)
end

function HPBarItem:Refresh( ... )
	if self.m_CachedCanvasGroup.alpha <= 0 then
		return false
	end

	if self.Owner and self.Owner.CSEntity.IsAvailable and self.Owner.CSEntity.Id == self.OwnerId then
		local worldPosition = self.Owner:CachedTransform().position + Vector3.forward
		local screenPosition = GameEntry.Scene.MainCamera:WorldToScreenPoint(worldPosition)

        self.m_CachedTransform.anchoredPosition = screenPosition
        -- local position = Vector3.zero
        -- if (RectTransformUtility.ScreenPointToLocalPointInRectangle(self.m_ParentCanvas.transform, screenPosition,
        --     self.m_ParentCanvas.worldCamera, position)) then
        --     self.m_CachedTransform.anchoredPosition = position
        -- end
	end
	return true
end

function HPBarItem:Reset( ... )
	self.CSEntity:StopAllCoroutines()

	self.m_CachedCanvasGroup.alpha = 1
	self.m_HPBar.value = 1
	self.gameObject:SetActive(false)
end

function HPBarItem:HPBarCo( value, animationDuration, keepDuration, fadeOutDuration )
	
	self.CSEntity:StartCoroutine(function ()

	    coroutine.yield(self.m_HPBar:SmoothValue(value, animationDuration))
	    
	    coroutine.yield(WaitForSeconds(keepDuration))

	    coroutine.yield(self.m_CachedCanvasGroup:FadeToAlpha(0, fadeOutDuration))
	
	end)
end

return HPBarItem