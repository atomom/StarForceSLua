DEBUG = DEBUG or true
local lua_version = tonumber(_VERSION:match("%d%.*%d*"))
local bitsize = 32

function randomseed(seed)
  seed = math.floor(math.abs(seed))
  if seed >= (2^bitsize) then
    -- integer overflow, so reduce to prevent a bad seed
    seed = seed - math.floor(seed / 2^bitsize) * (2^bitsize)
  end
  if lua_version < 5.2 then
    -- 5.1 uses (incorrect) signed int
    math.randomseed(seed - 2^(bitsize-1))
  else
    -- 5.2 uses (correct) unsigned int
    math.randomseed(seed)
  end
  return seed
end

function seed()
  if package.loaded["socket"] and package.loaded["socket"].gettime then
    return randomseed(package.loaded["socket"].gettime()*10000)
  else
    return randomseed(os.time())
  end
end

--[[
    utf截断
]]
function truncateUTF8String(s, n)
    local dropping = string.byte(s, n+1)
    if not dropping then 
        return s 
    end
    if dropping >= 128 and dropping < 192 then
        return truncateUTF8String(s, n-1)
    end
    return string.sub(s, 1, n)
end
--[[
    utf8长度
]]
local function chsize(char)
    if not char then
        print("not char")
        return 0
    elseif char > 240 then
        return 4
    elseif char > 225 then
        return 3
    elseif char > 192 then
        return 2
    else
        return 1
    end
end
-- 截取utf8 字符串
-- str:         要截取的字符串
-- startChar:   开始字符下标,从1开始
-- numChars:    要截取的字符长度
function utf8sub(str, startChar, numChars)
    local startIndex = 1
    while startChar > 1 do
        local char = string.byte(str, startIndex)
        startIndex = startIndex + chsize(char)
        startChar = startChar - 1
    end

    local currentIndex = startIndex

    while numChars > 0 and currentIndex <= #str do
        local char = string.byte(str, currentIndex)
        currentIndex = currentIndex + chsize(char)
        numChars = numChars -1
    end
    return str:sub(startIndex, currentIndex - 1)
end

-- 计算utf8字符串字符数, 各种字符都按一个字符计算
-- 例如utf8len("1你好") => 3
function utf8len(str)
    local len = 0
    local currentIndex = 1
    while currentIndex <= #str do
        local char = string.byte(str, currentIndex)
        currentIndex = currentIndex + chsize(char)
        len = len +1
    end
    return len
end
--[[
    utf8长度
]]
local function lsize(char)
    if not char then
        print("not char")
        return 0
    elseif char > 240 then
        return 4
    elseif char > 225 then
        return 3
    elseif char > 192 then
        return 2
    elseif char > 127 then
        return 1
    elseif char > 122 then
        return 1
    --小写字母
    elseif char > 96 then
        if device.platform=="ios" then
            return 0.6
        else
            return 0.6
        end
    --字符[]
    elseif char > 90 then
        return 0.34
    --大写字母
    elseif char > 64 then
        if device.platform=="ios" then
            return 0.58
        else
            return 0.65
        end
    --其他 <
    elseif char > 58 then
        if device.platform=="ios" then
            return 0.65
        else
            return 0.65
        end
    --数字
    elseif char > 47 then
        if device.platform=="ios" then
            return 0.58
        else
            return 0.7
        end
    --其他
    else
        return 0.65
    end
end
--[[
    中英文混合长度
]]
function string.llen(str)
    local len = 0
    local currentIndex = 1
    while currentIndex <= #str do
        local char = string.byte(str, currentIndex)
        currentIndex = currentIndex + chsize(char)
        len = len + lsize(char)/chsize(char)
    end
    return len
end
-- --[[
--     中英文混合长度
-- ]]
-- function string.llen( str )
--     local len  = #str
--     local left = 0
--     local arr  = {0, 0xc0, 0xe0, 0xf0, 0xf8, 0xfc}
--     local t = 0
--     local start = 1
--     local wordLen = 0
--     --字符数量
--     local c = 0
--     while len ~= left do
--         local tmp = string.byte(str, start)
--         local i   = #arr
--         while arr[i] do
--             if tmp >= arr[i] then
--                 if i==1 then
--                     c = c + 1
--                 end
--                 break
--             end
--             i = i - 1
--         end
--         wordLen = i + wordLen

--         local tmpString = string.sub(str, start, wordLen)
--         t = t + 1

--         start = start + i
--         left = left + i
--     end
--     local ret = t - c + c/2
--     return ret
-- end

function delEscapedString(s)
    local endPos = 1
    local stringLen = string.len(s)
    local ret_str = ""
    repeat
    local curChar = string.sub(s,endPos,endPos)
    if curChar ~= [[\]] then
        ret_str = ret_str .. curChar
    end
    endPos = endPos + 1
    until endPos > stringLen + 1
    return ret_str
end

-----table转换字符串
function tostringex(v, len)
    if len == nil then 
        len = 0 
    end
    
    local pre = string.rep('  ', len+1)
    local pre1 = string.rep('  ', len)
    local ret = len==0 and "" or "  "
    if type(v) == "table" then
        if len > 10 then return "{ ... }" end
        local t = ""
        for k, v1 in pairs(v) do
            t = t .. "\n" .. pre .. (type(tonumber(k))=="number" and string.format("[%s]",k) or tostring(k) ) .. "  ="
            if type(v1) == "table" then
                t = t .. tostringex(v1, len+1) .. ","
            else
                t = t .. "  " .. (type(v1)=="string" and string.format("'%s'",v1) or tostring(v1) ) .. ","
            end
        end
        if t == "" then
            ret = ret .. "{}"
        else
            ret = ret .. "{" .. t .. "\n" .. pre1 .. "}"
        end
    else
        --value是string
        ret = ret .. (type(v)=="string" and string.format("'%s'",v) or tostring(v) ) .. ","
    end
    return ret
end

function traceback()
    local level = 3
    local ret = ""
    while true do
        local info = debug.getinfo(level, "Sln")
        if not info then
            break
        end
        if info.what == "C" then -- is a C function?
            ret = string.format("%s\n %s,%s",ret, level, "C function")
        else -- a Lua function
            ret = (string.format("%s\n [%s][%s]:%s",ret,info.short_src,info.name, info.currentline))
        end
        level = level + 1
    end
    return ret
end

function getLogStr(fmt, ...)    
    local list = {}
    local paramslist = {...}
    for i,v in ipairs(paramslist) do
        if type(v)=="table" then
            v = tostringex(v, len)
        end
        list[i] = v
    end

    if type(fmt)~="string" 
        or (type(fmt)=="string" and string.find(fmt,"%%s")==0) then

        if table.nums(paramslist)>0 then
            local fmts = fmt
            if type(fmt)=="table" then
                fmts = tostringex(fmt, len)
            end
            
            table.insert(list, 1, fmts)
            fmt = string.rep("%s ", table.nums(paramslist)+1)
        else
            list = {tostringex(fmt==nil and "nil" or fmt)}
            fmt = "%s"
        end
    end

    fmt = tostringex(fmt, len)
    local x = debug.traceback()
    local arr = string.split(x,"\n")
    --取真正的文件
    
    local r = arr[4]
    local times = os.date("%X", os.time())
    local str = string.format("["..times.."] " .. fmt,table.unpack(list))
    for i=4,#arr do 
        str = str  .. "\n" .. arr[i]
    end  
    return str
end


--[[
    保存log日志
]]
function saveLogFile(fileName,data)
    if DEBUG==0 or LOGTXT==false then
        return
    end
    str = tostringex(data)
    --目录名
    local dir = device.writablePath..("log/")
    BaseUtil:checkDir( dir )

    --文件名
    local filePath = string.format("%s%s.log",dir,fileName)

    LOG_DATAS = LOG_DATAS or {}
    LOG_DATAS[fileName] = LOG_DATAS[fileName] or {}
    --初始化和超过MAX条删掉
    local lognum = checkint(LOG_DATAS[fileName].current)
    local file = nil
    if lognum==0 or lognum>MAXLOG then
        file = io.open(filePath, "w+")
        lognum = 0
    else
        file = io.open(filePath, "a+")
    end

    --自增1
    LOG_DATAS[fileName].current = lognum + 1

    file:write(
        string.format("(%s) %s\n\n",
                        LOG_DATAS[fileName].current,
                        getLogStr(str)))
    file:close()
end

function LOG(fmt, ...)
    if DEBUG == true then
        local msg = getLogStr(fmt, ...) or "nil"
        Debug.Log('[INFO] '..msg)
    end
end

function WARN(fmt, ...)
    if DEBUG == true then
        local msg = getLogStr(fmt, ...) or "nil"
        Debug.LogWarning('[WARN] '..msg)
    end
end

function ERROR(fmt, ...)
    if DEBUG == true then
        local msg = getLogStr(fmt, ...) or "nil"
        Debug.LogError('[ERROR] '..msg)
    end
end

log = LOG
logWarn = WARN
logError = ERROR

function table.seekbyt(st,ct)
    for k1,v1 in pairs(st) do
        local ret = 0
        for k2,v2 in pairs(ct) do
            if v1[k2] == v2 then
                ret = ret + 1
            end
        end
        if ret==table.nums(ct) then
            return v1
        end
    end
end

function timeformat(exp_rest_time)
    local  off_hours = nil
    local  off_mins = nil
    local  off_secs = nil
    local  display_time_string = ""

    -- 转小时
    off_hours = math.floor(exp_rest_time / 3600)
    -- 转分钟
    off_mins  = math.floor((exp_rest_time % 3600) / 60)
    -- 转秒
    off_secs  = math.floor((exp_rest_time % 3600) % 60)

    if off_hours > 0 then
        display_time_string = off_hours .. _("SYSTEM.HOURS")
    end
    if off_mins > 0 then
        display_time_string = display_time_string .. off_mins .. _("SYSTEM.MINS")
    end
    if off_secs > 0 then
        display_time_string = display_time_string .. off_secs .. _("SYSTEM.SECS")
    end
    return display_time_string
end
--[[
    获取json转成lua后的数据文件
]]
function getNewDataFile(jsonfileName,luanewFile)
    local file = nil
    local data = nil
    newfile = luanewFile or jsonfileName
    if device.platform=="ios" then
        local filePath = cc.FileUtils:getInstance():fullPathForFilename(string.format("src/app/data/%s.json",jsonfileName))
        local filec = BaseUtil:readFile(filePath)
        data = json.decode(filec)
        file = cc.FileUtils:getInstance():fullPathForFilename(string.format("src/app/data/%s.lua",newfile))
    else
        local filePath = device.writablePath..(string.format("src/app/data/%s.json",jsonfileName))
        local filec = BaseUtil:readFile(filePath)
        data = json.decode(filec)
        file = device.writablePath..(string.format("src/app/data/%s.lua",newfile))
    end
    LOG(file)
    return file, data
end
--[[
    json保存为lua文件
]]
function changeJson(fileName,key,newfile)
    local file ,data= getNewDataFile(fileName, newfile)
    local f = io.open(file, "w+")
    f:write("return ")
    f:write("{\n")
    for i,v in ipairs(data) do
        local s = '['..(v[key or "f_id"]).."]="..tostringex(v)..",\n"
        f:write(s)
    end
    f:write("}")
    f:close()
    LOG("save ok")
end

--[[--

输出值的内容

### 用法示例

~~~ lua

local t = {comp = "chukong", engine = "quick"}

dump(t)

~~~

@param mixed value 要输出的值

@param [string desciption] 输出内容前的文字描述

@parma [integer nesting] 输出时的嵌套层级，默认为 3

]]
function dump(value, desciption, nesting)
    if type(nesting) ~= "number" then nesting = 3 end

    local lookupTable = {}
    local result = {}

    local function _v(v)
        if type(v) == "string" then
            v = "\"" .. v .. "\""
        end
        return tostring(v)
    end

    local traceback = string.split(debug.traceback("", 2), "\n")
    print("dump from: " .. string.trim(traceback[3]))

    local function _dump(value, desciption, indent, nest, keylen)
        desciption = desciption or "<var>"
        local spc = ""
        if type(keylen) == "number" then
            spc = string.rep(" ", keylen - string.len(_v(desciption)))
        end
        if type(value) ~= "table" then
            result[#result +1 ] = string.format("%s%s%s = %s", indent, _v(desciption), spc, _v(value))
        elseif lookupTable[value] then
            result[#result +1 ] = string.format("%s%s%s = *REF*", indent, desciption, spc)
        else
            lookupTable[value] = true
            if nest > nesting then
                result[#result +1 ] = string.format("%s%s = *MAX NESTING*", indent, desciption)
            else
                result[#result +1 ] = string.format("%s%s = {", indent, _v(desciption))
                local indent2 = indent.."    "
                local keys = {}
                local keylen = 0
                local values = {}
                for k, v in pairs(value) do
                    keys[#keys + 1] = k
                    local vk = _v(k)
                    local vkl = string.len(vk)
                    if vkl > keylen then keylen = vkl end
                    values[k] = v
                end
                table.sort(keys, function(a, b)
                    if type(a) == "number" and type(b) == "number" then
                        return a < b
                    else
                        return tostring(a) < tostring(b)
                    end
                end)
                for i, k in ipairs(keys) do
                    _dump(values[k], k, indent2, nest + 1, keylen)
                end
                result[#result +1] = string.format("%s}", indent)
            end
        end
    end
    _dump(value, desciption, "- ", 1)

    for i, line in ipairs(result) do
        print(line)
    end
end