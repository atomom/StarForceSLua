local BaseUGuiForm = RequireCommon("ui.BaseUGuiForm")

local MenuForm = class("MenuForm", BaseUGuiForm)

function MenuForm:OnInit( ... )
	MenuForm.super.OnInit(self, ...)
end

function MenuForm:OnOpen( userData )
	MenuForm.super.OnOpen(self, userData)

	self.m_ProcedureMenu = userData
	if not self.m_ProcedureMenu then
		logError("ProcedureMenu is invalid when open MenuForm.")
        return     
    end
	self.Quit.gameObject:SetActive(Application.platform ~= RuntimePlatform.IPhonePlayer)
end

function MenuForm:OnClose( ... )
	self.m_ProcedureMenu = nil
	MenuForm.super.OnClose(self, ...)
end

function MenuForm:OnUpdate( ... )
	MenuForm.super.OnUpdate(self, ...)
end

function MenuForm:OnClick( button, name )
	if self.Quit == button then
		log(name)
	elseif self.Start == button then
		self.m_ProcedureMenu:StartGame()
	end
end

return MenuForm