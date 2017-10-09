function math.clamp(v, minValue, maxValue)  
    if v < minValue then
        return minValue
    end
    if( v > maxValue) then
        return maxValue
    end
    return v 
end

function Handler(func,target, ...)
    local argtemp1 = table.pack(...)
    return function(...)
        local argtemp2 = table.pack(...) 
        table.merge(argtemp2, argtemp1)
        return func(target, table.unpack(argtemp2)) 
    end
end

function isNull(str) 
    return str == nil  or str == ""
end

function string.trim(s) 
    return (string.gsub(s, "^%s*(.-)%s*$", "%1"))
end  

function string.split(str, delimiter)
	if str==nil or str=='' or delimiter==nil then
		return nil
	end
	
    local result = {}
    for match in (str..delimiter):gmatch("(.-)"..delimiter) do
        table.insert(result, match)
    end
    return result
end

--http请求
function httpRes(url,cb)
    local request = HttpManager.instance:CreateHTTPRequest(url, "GET",
        function(response)
            if not isNull(response.Error) then
                logWarn(response.Error)
                cb(-1)
                return
            end

            local ret = response:GetResponseText()

            log(ret)
            cb(0,ret)
        end)
    request:Start()
end

local function getSettingPath()
    if not Utils.Tool.isNative then
        return Application.dataPath .."/Z_Test/"
    end
    return FileUtils.getInstance():getWritablePath(nil)
end

--加密保存
function save_setting(file,data)
    LuaHelper.SaveFileEncode(getSettingPath(),file,data)
end

--解密获取
function get_setting(file)
    return LuaHelper.GetFileDeCode(getSettingPath(),file)
end

--移除
function remove_setting(file)
    LuaHelper.RemoveFile(getSettingPath(),file)
end

function createSign(tab)	
	local str = ''
	local strKey = ''
	local key_table = {}
    local val = ''
	
	--取出所有的键  
	for key,_ in pairs(tab) do  
		table.insert(key_table,key)  
	end  

	--对所有键进行排序  
	table.sort(key_table)  
	for _,key in pairs(key_table) do  
        val = tab[key] or ''
		str = key ..'='.. val ..'&'
		strKey = strKey..str 
	end
	
	strKey = strKey..'key='..sdk.AppKey
    local fileUtils = FileUtils.getInstance()	
	strKey = fileUtils:GenMd5(strKey)
	return string.upper(strKey)
end

function log(msg)
	if(setting.isDebug == true) then
		if type(msg) ~= 'string' then
			msg = JSON.stringify(msg)
		end		
		Debug.Log('[INFO] '..msg)
	end
end

function logError(msg)	
	if(setting.isDebug == true) then
		if type(msg) ~= 'string' then
			msg = JSON.stringify(msg)
		end		
		Debug.LogWarning('[ERROR] '..msg)
	end
end

function logWarn(msg)
	if(setting.isDebug == true) then	
		if type(msg) ~= 'string' then
			msg = JSON.stringify(msg)
		end		
		Debug.Log('[WARN] '..msg)
	end
end

function PathsInToTable(paths, t)
    for i,v in ipairs(paths) do
        local ps = string.split(v,"/")
        local p = ps[#ps]
        t[p] = v
    end
end

function getSystemInfo()
    local sysinfo = {}
--[[       
    sysinfo.os = SystemInfo.operatingSystem -- 操作系统类型  
    sysinfo.deviceModel = SystemInfo.deviceModel -- 设备型号 
    sysinfo.deviceName = SystemInfo.deviceName -- 设备名称 
    sysinfo.deviceType = SystemInfo.deviceType -- 设备类型 
    sysinfo.processorType = SystemInfo.processorType -- 处理器的名称
    sysinfo.userid = get_setting(OPENID) or '' -- 玩家游戏id 
--]]
    sysinfo.graphicsDeviceID = SystemInfo.graphicsDeviceID -- 显卡唯一标识符 
    sysinfo.deviceid = get_setting(DEVICEID)  -- 设备唯一标识符 
    sysinfo.timestamp = os.time() -- 精确到秒的时间戳
    return sysinfo
end

function formatUrl(tab)
    local str = ''
    for i,v in pairs(tab) do
        str = str .. i .. '=' .. v ..'&'
    end
    return string.sub(str, 1, #str-1)
end

function getBackGroundSize()
    local standarc_aspect = 1334 / 750;
    local device_aspect =  Screen.width / Screen.height; 
    local scale = 1.0;--适配缩放因子  
    
    if(device_aspect > standarc_aspect) then--按宽度适配
        scale = device_aspect / standarc_aspect;
    else--按高度适配
        scale = standarc_aspect / device_aspect;
    end
    return scale
end

function MathPow(v1,v2)
    --5 6
    local value = v1
    for i=1,v2-1 do
        value = value*v1
    end
    return value
end




--深度拷贝  
function deepcopy(object)      
    local lookup_table = {}  
    local function _copy(object)  
        if type(object) ~= "table" then  
            return object  
        elseif lookup_table[object] then  
  
            return lookup_table[object]  
        end  -- if          
        local new_table = {}  
        lookup_table[object] = new_table  
  
        for index, value in pairs(object) do  
            new_table[_copy(index)] = _copy(value)  
        end   
        return setmetatable(new_table, getmetatable(object))      
    end
    return _copy(object)  
end


function logShow(data,name)
    logError(".............."..name..".............")
    log(data)
end   

function FindKeyInTable(dataTable,findIdx)
    local result = false
    for i,v in pairs(dataTable) do
        if i == findIdx then
            return true
        end
        -- if type(v) == "table" then
        --     result = FindKeyInTable(v,findIdx)
        --     if result == true then
        --         return result
        --     end
        -- end
    end
    return result
end


local __g = _G

exports = {}

setmetatable(exports, {
    __newindex = function (_, name,value )
        rawset(__g, name, value)
    end,

    __index = function ( _, name)
        return rwaget(__g, name)
    end
})

function disable_global()
    setmetatable(__g, {
        __newindex = function(_, name, value)
            error(string.format("CAN NOT USE GLOBAL VARIABLE", name), 0)
        end
    })
end

