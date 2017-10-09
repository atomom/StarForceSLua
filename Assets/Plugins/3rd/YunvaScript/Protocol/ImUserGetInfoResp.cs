using UnityEngine;
using System.Collections;
namespace YunvaIM
{
    //获取个人信息返回
    public class ImUserGetInfoResp:YunvaMsgBase
    {
        public int userId;
        public int sex;
        public string nickName;
        public string iconUrl;
        public string userLevel;
        public string vipLevel;
        public string ext;
        public int result;
         public string msg;
        public string uid;
        public ImUserGetInfoResp(object Parser)
        {
            uint parser = (uint)Parser;
            userId = YunVaImInterface.parser_get_integer(parser, 1, 0);
            sex = YunVaImInterface.parser_get_integer(parser, 2, 0);
            nickName = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 3, 0));
            iconUrl = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 4, 0));
            userLevel = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 5, 0));
            vipLevel = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 6, 0));
            ext = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 7, 0));
             result = YunVaImInterface.parser_get_integer(parser, 8, 0);
            msg = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 9, 0));
            uid = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 10, 0));
            YunvaLogPrint.YvDebugLog("ImUserGetInfoResp", string.Format("userId:{0},sex:{1},nickName:{2},iconUrl:{3},userLevel:{4},vipLevel:{5},ext:{6},result:{7},msg:{8},uid:{9}", userId, sex, nickName, iconUrl, userLevel, vipLevel, ext, result, msg, uid));
            YunVaImInterface.eventQueue.Enqueue(new InvokeEventClass(ProtocolEnum.IM_USER_GETINFO_RESP, this));
        }
    }
}