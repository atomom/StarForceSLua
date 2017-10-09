---
--- Created by atomom.
--- DateTime: 2017/9/28 12:06
---
local CampType = RequireCommon("Definition.Enum.CampType")
local RelationType = RequireCommon("Definition.Enum.RelationType")

return {
    { CampType.Player, CampType.Player, RelationType.Friendly },
    { CampType.Player, CampType.Enemy, RelationType.Hostile },
    { CampType.Player, CampType.Neutral, RelationType.Neutral },
    { CampType.Player, CampType.Player2, RelationType.Hostile },
    { CampType.Player, CampType.Enemy2, RelationType.Hostile },
    { CampType.Player, CampType.Neutral2, RelationType.Neutral },
    { CampType.Enemy, CampType.Enemy, RelationType.Friendly },
    { CampType.Enemy, CampType.Neutral, RelationType.Neutral },
    { CampType.Enemy, CampType.Player2, RelationType.Hostile },
    { CampType.Enemy, CampType.Enemy2, RelationType.Hostile },
    { CampType.Enemy, CampType.Neutral2, RelationType.Neutral },
    { CampType.Neutral, CampType.Neutral, RelationType.Neutral },
    { CampType.Neutral, CampType.Player2, RelationType.Neutral },
    { CampType.Neutral, CampType.Enemy2, RelationType.Neutral },
    { CampType.Neutral, CampType.Neutral2, RelationType.Hostile },
    { CampType.Player2, CampType.Player2, RelationType.Friendly },
    { CampType.Player2, CampType.Enemy2, RelationType.Hostile },
    { CampType.Player2, CampType.Neutral2, RelationType.Neutral },
    { CampType.Enemy2, CampType.Enemy2, RelationType.Friendly },
    { CampType.Enemy2, CampType.Neutral2, RelationType.Neutral },
    { CampType.Neutral2, CampType.Neutral2, RelationType.Neutral }
}
