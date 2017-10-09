using UnityEngine;
using System.Collections;
using YunvaIM;
public class ImThirdLoginResp : YunvaMsgBase
{
    public int result;//返回结果0为成功，非0是失败
    public string msg;//错误描述
    public int userId;//云娃ID
    public string nickName;//昵称
    public string iconUrl;//用户图像地址
    public string thirdUserId;//第三方用户ID
    public string thirdUseName;//第三方用户名
    public string level;//用户等级
    public string vip;//用户vip等级
    public string ext;//扩展字段
    public int sex;//性别

    public ImThirdLoginResp(object Parser)
    {
        uint parser = (uint)Parser;
        result = YunVaImInterface.parser_get_integer(parser, 1, 0);
        msg = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 2, 0));
        userId = YunVaImInterface.parser_get_integer(parser, 3, 0);
        nickName = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 4, 0));
        iconUrl = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 5, 0));
		thirdUserId = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 6, 0));
        thirdUseName = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 7, 0));
        level = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 8, 0));
        vip = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 9, 0));
        ext = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 10, 0));
         sex = YunVaImInterface.parser_get_integer(parser, 11, 0);
        InvokeEventClass cpData = new InvokeEventClass(ProtocolEnum.IM_THIRD_LOGIN_RESP, this);
        YunVaImInterface.eventQueue.Enqueue(cpData);    

		YunvaLogPrint.YvDebugLog ("ImThirdLoginResp", string.Format ("result:{0},msg:{1},userId:{2},nickName:{3},iconUrl:{4},thirdUserId:{5},thirdUseName:{6}",result,msg,userId,nickName,iconUrl,thirdUserId,thirdUseName));
        //YunvaLogPrint.YvDebugLog("ImThirdLoginResp", string.Format("result:{0},msg:{1},userId:{2},nickName:{3},iconUrl:{4},thirdUserId:{5},thirdUseName:{6},level:{7},vip:{8},ext:{9},sex:{10}", result, msg, userId, nickName, iconUrl, thirdUserId, thirdUseName, level, vip, ext, sex));
    }
    public ImThirdLoginResp() { }

}
