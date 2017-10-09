using UnityEngine;
using System.Collections;
namespace YunvaIM
{
	public class ImSetUserInfoResp : YunvaMsgBase
	{
		public int result;
		public string msg;
		public ImSetUserInfoResp(object Parser)
		{
			uint parser = (uint)Parser;
			result = YunVaImInterface.parser_get_integer(parser, 1, 0);
			msg = YunVaImInterface.IntPtrToString(YunVaImInterface.parser_get_string(parser, 2, 0));
			YunVaImInterface.eventQueue.Enqueue(new InvokeEventClass(ProtocolEnum.IM_SETUSERINFO_RESP, this));
			YunvaLogPrint.YvDebugLog ("ImSetUserInfoResp", string.Format ("result:{0},msg:{1}",result,msg));
		}
	}
}