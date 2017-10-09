Game = {}

local EntityConfig = RequireCommon("config.Entity")

function Game.ShowLuaEntity(logicName, data, groupName)
	data.logicName = string.format("return RequireCommon('entity.EntityLogic.%s')", logicName)
    GameEntry.Entity:ShowLuaEntity(data.Id, EntityConfig[data.TypeId].AssetName, groupName, data)
end

return Game