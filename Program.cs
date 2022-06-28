using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;

namespace Function
{

    public class AliceRequestBase
    {


        [JsonPropertyName("meta")]
        public AliceMetaModel Meta { get; set; }

        [JsonPropertyName("session")]
        public AliceSessionModel Session { get; set; }

        [JsonPropertyName("request")]
        public AliceRequestModel Request { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
    public class AliceMetaModel
    {
        [JsonPropertyName("locale")]
        public string Locale { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("interfaces")]
        public AliceInterfacesModel Interfaces { get; set; }
    }

    public class AliceInterfacesModel
    {
        [JsonPropertyName("screen")]
        public object Screen { get; set; }

        [JsonPropertyName("payments")]
        public object Payments { get; set; }

        [JsonPropertyName("account_linking")]
        public object AccountLinking { get; set; }


    }

    public class AliceSessionModel
    {
        [JsonPropertyName("new")]
        public bool New { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        [JsonPropertyName("message_id")]
        public int MessageId { get; set; }

        [JsonPropertyName("skill_id")]
        public string SkillId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("user")]
        public AliceSessionUserModel User { get; set; }

        [JsonPropertyName("application")]
        public AliceSessionApplicationModel Application { get; set; }


    }
    public class AliceSessionUserModel
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
    public class AliceSessionApplicationModel
    {
        [JsonPropertyName("application_id")]
        public string ApplicationId { get; set; }
    }

    public class AliceRequestModel
    {
        [JsonPropertyName("command")]
        public string Command { get; set; }

        [JsonPropertyName("original_utterance")]
        public string OriginalUtterance { get; set; }

        [JsonPropertyName("payload")]
        public object Payload { get; set; }

        [JsonPropertyName("markup")]
        public AliceMarkupModel Markup { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        //nlu
        [JsonPropertyName("nlu")]
        public AliceNluModel Nlu { get; set; }
    }
    public class AliceMarkupModel
    {
        [JsonPropertyName("dangerous_context")]
        public bool DangerousContext { get; set; }
    }
    //nlu
    public class AliceNluModel
    {
        [JsonPropertyName("tokens")]
        public IEnumerable<string> Tokens { get; set; }

        [JsonPropertyName("entities")]
        public IEnumerable<string> Entities { get; set; }

        [JsonPropertyName("intents")]
        public object Intents { get; set; }
    }

    public class BaseRequest
    {
        public string httpMethod { get; set; }
        public string body { get; set; }
    }

    public class Response
    {
        public int StatusCode { get; set; }
        public string Body { get; set; }

        public Response(int statusCode, string body)
        {
            StatusCode = statusCode;
            Body = body;
        }
    }
    public class AliceResponse
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("response")]
        public AliceResponseModel Response { get; set; }

        public AliceResponse(AliceRequestBase request)
        {
            Version = request.Version;
            Response = new AliceResponseModel();
            Response.EndSession = false;
            var sh = new ShowMeta();
            //some random guid
            sh.content_id = "88f83f54-1135-4238-85c4-5e45959a64d0";
            sh.id = "88f83f54-1135-4238-85c4-5e45959a64d0";
            sh.publication_date = DateTime.UtcNow.ToString("o");

            Response.ShowItemMeta = sh;

            if (request.Request.Command == "привет")
                request.Request.Command = request.Request.Command + " медвед ";

            Response.Text = "ПРИВЕТ МАТВЕЙ ЭТО ЯНДЕКС ФУНКЦИЯ! " + request.Request.Command;
            Response.Tts = Response.Text;

        }
    }
    public class AliceResponseModel
    {
        private string _tts;
        private const int _textMaxLength = 1024;
        private const int _ttsMaxLength = 1024;

        private string _text;

        [JsonPropertyName("text")]
        public string Text
        {
            get => _text;

            set
            {
                _text = value;
            }
        }
        [JsonPropertyName("tts")]
        public string Tts
        {
            get => _tts;

            set
            {
                _tts = value;
            }
        }

        [JsonPropertyName("end_session")]
        public bool EndSession { get; set; }

        [JsonPropertyName("show_item_meta")]
        public ShowMeta ShowItemMeta { get; set; }

    }
    public class ShowMeta
    {
        [JsonPropertyName("publication_date")]
        public string publication_date { get; set; }
        [JsonPropertyName("id")]
        public string id { get; set; }
        [JsonPropertyName("content_id")]
        public string content_id { get; set; }
    }

    public class Handler
    {
        public AliceResponse FunctionHandler(AliceRequestBase request)
        {
            var response = new AliceResponse(request);
            return response;
        }
    }
}