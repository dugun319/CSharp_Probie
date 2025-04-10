using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Softpower.SmartMaker.TopApp;

namespace Softpower.SmartMaker.TopSmartAtom.Components.ChatBot
{
	public class GoogleGeminiManager : ChatBotManager
	{		
		private readonly string URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
		private readonly string ContentType = "application/json";

		public GoogleGeminiManager () { }

		public override void SendMessage (string strAPIKey, string strMessage)
		{
			string strResponse = "";
			string fullUrl = $"{URL}?key={strAPIKey}";

			string body = JsonConvert.SerializeObject (new
			{
				contents = new[]
				{
					new
					{
						parts = new[]
						{
							new { text = strMessage }
						}
					}
				}
			});

			string contentType = "application/json";

			// PQAppBase에서 Content-Type을 중복 지정해주기 때문에 여기서 제거
			List<string[]> headerList = null;

			string strCookie = "";
			string strResult = PQAppBase.SetHttpAction (fullUrl, body, ref strCookie, contentType, headerList, Encoding.UTF8);

			if (!string.IsNullOrEmpty (strResult))
			{
				try
				{
					JToken obj = JToken.Parse (strResult);
					var token = obj.SelectToken ("candidates[0].content.parts[0].text");

					if (token is JValue valueToken)
					{
						strResponse = valueToken.Value.ToString ().Trim ();
					}
					else
					{
						strResponse = "Gemini 응답을 파싱할 수 없습니다.";
					}
				}
				catch (System.Exception ex)
				{
					strResponse = $"Gemini 응답 파싱 오류: {ex.Message}";
				}
			}
			else
			{
				strResponse = "Gemini API 호출 실패 또는 빈 응답입니다.";
			}

			// 완료 콜백 호출
			BaseCompletedSendMessage (strResponse);
		}
	}
}
