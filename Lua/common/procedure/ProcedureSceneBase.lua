local ProcedureSceneBase = class("ProcedureSceneBase")

function ProcedureSceneBase:ctor( obj )
	 self.procedure = obj
end

function ProcedureSceneBase:OnInit( ... )
end

function ProcedureSceneBase:OnEnter( ... )
end

function ProcedureSceneBase:OnLeave( ... )
end

function ProcedureSceneBase:OnUpdate( ... )
end

function ProcedureSceneBase:OnDestroy( ... )
end


return ProcedureSceneBase