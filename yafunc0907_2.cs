using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
    #region NLU
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
        [JsonConverter(typeof(AliceEntityModelEnumerableConverter))]
        public IEnumerable<AliceEntityModel> Entities { get; set; }



        [JsonPropertyName("intents")]
        public object Intents { get; set; }
    }
    #region nluModels
    public class AliceEntityNumberModel : AliceEntityModel
    {
        [JsonPropertyName("value")]
        public double Value { get; set; }
    }
    public class AliceEntityGeoModel : AliceEntityModel
    {
        [JsonPropertyName("value")]
        public AliceEntityGeoValueModel Value { get; set; }
    }
    public class AliceEntityGeoValueModel
    {
        [JsonPropertyName("house_number")]
        public string HouseNumber { get; set; }

        [JsonPropertyName("street")]
        public string Street { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("airport ")]
        public string Airport { get; set; }
    }
    public class AliceEntityFioModel : AliceEntityModel
    {
        [JsonPropertyName("value")]
        public AliceEntityFioValueModel Value { get; set; }
    }
    public class AliceEntityFioValueModel
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        [JsonPropertyName("patronymic_name")]
        public string PatronymicName { get; set; }
    }
    public class AliceEntityStringModel : AliceEntityModel
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
    public class AliceEntityDateTimeModel : AliceEntityModel
    {
        [JsonPropertyName("value")]
        public AliceEntityDateTimeValueModel Value { get; set; }
    }
    public class AliceEntityDateTimeValueModel
    {
        [JsonPropertyName("day")]
        public double Day { get; set; }

        [JsonPropertyName("day_is_relative")]
        public bool DayIsRelative { get; set; }

        [JsonPropertyName("hour")]
        public double Hour { get; set; }

        [JsonPropertyName("hour_is_relative")]
        public bool HourIsRelative { get; set; }

        [JsonPropertyName("minute")]
        public double Minute { get; set; }

        [JsonPropertyName("minute_is_relative")]
        public bool MinuteIsRelative { get; set; }

        [JsonPropertyName("month")]
        public double Month { get; set; }

        [JsonPropertyName("month_is_relative")]
        public bool MonthIsRelative { get; set; }

        [JsonPropertyName("year")]
        public double Year { get; set; }

        [JsonPropertyName("year_is_relative")]
        public bool YearIsRelative { get; set; }
    }
    #endregion
    public class AliceEntityModelEnumerableConverter : EnumerableConverter<AliceEntityModel>
    {
        private static readonly Dictionary<string, Type> _typeMap = new Dictionary<string, Type>
        {
            { "YANDEX.GEO", typeof(AliceEntityGeoModel) },
            { "YANDEX.FIO", typeof(AliceEntityFioModel) },
            { "YANDEX.NUMBER", typeof(AliceEntityNumberModel) },
            { "YANDEX.DATETIME", typeof(AliceEntityDateTimeModel) },
            { "YANDEX.STRING", typeof(AliceEntityStringModel) },
        };
        protected override AliceEntityModel ToItem(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            var readerAtStart = reader;

            string type = null;
            using (var jsonDocument = JsonDocument.ParseValue(ref reader))
            {
                var jsonObject = jsonDocument.RootElement;

                type = jsonObject
                    .EnumerateObject()
                    .FirstOrDefault(x => x.Name == "type")
                    .Value.GetString();
            }
            if (!string.IsNullOrEmpty(type) && _typeMap.TryGetValue(type, out var targetType))
            {
                return JsonSerializer.Deserialize(ref readerAtStart, targetType, options) as AliceEntityModel;
            }
            return null;
        }

        protected override void WriteItem(Utf8JsonWriter writer, AliceEntityModel item, JsonSerializerOptions options)
        {
            object newValue = null;
            if (item != null)
            {
                newValue = Convert.ChangeType(item, item.GetType(), CultureInfo.InvariantCulture);
            }

            JsonSerializer.Serialize(writer, newValue, options);
        }
    }
    public abstract class EnumerableConverter<TItem> : JsonConverter<IEnumerable<TItem>>
    {
        public override IEnumerable<TItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    var list = new List<TItem>();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            break;
                        }

                        list.Add(ToItem(ref reader, options));
                    }

                    return list.ToArray();
                case JsonTokenType.None:
                case JsonTokenType.StartObject:
                case JsonTokenType.EndObject:
                case JsonTokenType.EndArray:
                case JsonTokenType.PropertyName:
                case JsonTokenType.Comment:
                case JsonTokenType.String:
                case JsonTokenType.Number:
                case JsonTokenType.True:
                case JsonTokenType.False:
                case JsonTokenType.Null:
                default:
                    return Array.Empty<TItem>();
            }
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<TItem> value, JsonSerializerOptions options)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (value == null)
            {
                return;
            }

            writer.WriteStartArray();
            foreach (var item in value)
            {
                WriteItem(writer, item, options);
            }

            writer.WriteEndArray();
        }

        protected abstract TItem ToItem(ref Utf8JsonReader reader, JsonSerializerOptions options);

        protected virtual void WriteItem(Utf8JsonWriter writer, TItem item, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, item, options);
        }
    }
    public class AliceEntityModel
    {
        [JsonPropertyName("tokens")]
        public NluTokens Tokens { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }



    public class NluTokens
    {
        [JsonPropertyName("start")]
        public int start { get; set; }
        [JsonPropertyName("end")]
        public int end { get; set; }

    }



    #endregion
    #region audio
    public class AliceAudioResponse : IAliceResponse
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("response")]
        public AliceAudioResponseModel Response { get; set; }

        public AliceAudioResponse()
        {
            Response = new AliceAudioResponseModel();
            Version = "1.0";
        }
    }

    public class AliceAudioResponseModel
    {
        public AliceAudioResponseModel()
        {
            ShowItemMeta = new ShowMeta();       
                 ShowItemMeta.publication_date = DateTime.UtcNow.ToString("o");
            ShowItemMeta.content_id="1";
            ShowItemMeta.id="1";

            Directives = new AliceDirectives();
        }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("tts")]
        public string tts { get; set; }

        [JsonPropertyName("end_session")]
        public bool EndSession { get; set; }

        [JsonPropertyName("should_listen")]
        public bool ShouldListen { get; set; } = false;

        [JsonPropertyName("show_item_meta")]
        public ShowMeta ShowItemMeta { get; set; }

        [JsonPropertyName("directives")]
        public AliceDirectives Directives { get; set; }
    }
    public class AliceDirectives
    {
        [JsonPropertyName("audio_player")] 
        public AudioPlayerDirective audioPlayer { get; set; }

        public AliceDirectives()
        {
            audioPlayer = new AudioPlayerDirective();
        }
    }
    public class AudioPlayerDirective
    {
        [JsonPropertyName("action")] 
        public string Action { get; set; }="Play";
        [JsonPropertyName("item")]
        public AudioItem item { get; set; }

                [JsonPropertyName("type")]

        public string type { get;set;}="";
        
        public AudioPlayerDirective()
        {
            item = new AudioItem();
        }
    }
    public class AudioItem
    {
        [JsonPropertyName("stream")] 
        public AudioStream stream { get; set; }

        [JsonPropertyName("metadata")]
        public AudioMetadata metadata { get; set; }

        public AudioItem()
        {
            stream = new AudioStream();
            stream.token = Guid.NewGuid().ToString();
            metadata = new AudioMetadata();
            metadata.SongArtpic = new urlclass();
            metadata.SongBgimg = new urlclass();
        }
    }
    public class AudioStream
    {
        [JsonPropertyName("url")] 
        public string url { get; set; }

        [JsonPropertyName("token")] 
        public string token { get; set; }

        [JsonPropertyName("offset_ms")]
        public int offsetms { get; set; } = 0;

    }

    public class AudioMetadata
    {
        [JsonPropertyName("title")]
        public string SongTitle { get; set; }
        [JsonPropertyName("sub_title")] 
        public string SongArtist { get; set; }

        [JsonPropertyName("art")]
        public urlclass SongArtpic { get; set; }
        [JsonPropertyName("background_image")]
        public urlclass SongBgimg { get; set; }
    }
public class urlclass {
            [JsonPropertyName("url")]

    public string url {get;set;}="https://ya.ru";
}
    #endregion
    
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
    public static class Storage
    {
        public static string Auth = "";
        public static string Code = "";
    }
    #region LOGIC
    public class AliceResponse : IAliceResponse
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("response")]
        public AliceResponseModel Response { get; set; }
        public AliceResponse()
        {
            Version = "1.0";
            Response = new AliceResponseModel();
        }
        public AliceResponse(AliceRequestBase request)
        {
            string funcurl = "https://functions.yandexcloud.net/d4entjkagd3fqn0hum1d";
            Version = request.Version;
            Response = new AliceResponseModel();
            Response.EndSession = false;
            var sh = new ShowMeta();
            //some random guid
            sh.content_id = "88f83f54-1135-4238-85c4-5e45959a64d0";
            sh.id = "88f83f54-1135-4238-85c4-5e45959a64d0";
            sh.publication_date = DateTime.UtcNow.ToString("o");
            string reqC = request.Request.Command;
            if (reqC == "привет")
            {
                reqC = reqC + " медвед ";
            }

            string help = " вот что я умею: скажи чегототам и я сделаю что-нибудь.";
            Response.ShowItemMeta = sh;
            if (reqC == string.Empty ||
                reqC.ToLower() == "помощь" ||
                reqC.ToLower().Contains("что ты умеешь"))
            {

                Response.Text = "Привет! " + help;
            }
            else if (reqC.ToLower().Contains("авторизация"))
            {
                string auth = "https://oauth.yandex.ru/authorize?response_type=code&client_id=e738f647b8b145948b7a5a622a59819d&redirect_uri="
                    + funcurl;

                Response.Text = "попробуй эту ссылку " + auth;

            }
            else if (reqC.ToLower().Contains("проверка"))
            {
                Response.Text = "code=" + Storage.Code + " токен =" + Storage.Auth;
            }
            else if (reqC.ToLower().Contains("проверка"))
            {

            }
            else
            {
                //извлечение чисел из входящего запроса
                var ints = request.Request.Nlu.Entities.Where(t => t.Type == "YANDEX.NUMBER").ToList();
                string sints = "";
                if (ints != null && ints.Count > 0)
                {
                    sints = ". А еще вот числа из запроса: ";
                    foreach (var item in ints)
                    {
                        if (item != null)

                            sints = sints + " " + (item as AliceEntityNumberModel).Value;


                    }

                }
                //попугай

                Response.Text = "ПРИВЕТ ПОВЕЛИТЕЛЬ! вот что прислали: " + reqC + sints;
                Response.Tts = Response.Text;
            }

        }
    }
    public interface IAliceResponse
    { }
    #region Cards
    public class Card
    {
        [JsonPropertyName("type")]
        public string type {get;set;}="BigImage";

        [JsonPropertyName("image_id")]
        public string imageid {get;set;}

        [JsonPropertyName("title")]
        public string title {get;set;}

        [JsonPropertyName("description")]
        public string description {get;set;}
        
        [JsonPropertyName("button")]
        public object button {get;set;}

        public Card()
        {
            type = "BigImage";
            imageid = "937455/d8bdb0e65cce413fcfd8";
            title = "anonimous";
            description = "anonimous";
            button = new object();
        }
    }
    public class AliceResponseCard : IAliceResponse
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("response")]
        public AliceResponseModelCard Response { get; set; }
        public AliceResponseCard()
        {
            Version = "1.0";
            Response = new AliceResponseModelCard();
        }
        
    }
    public class AliceResponseModelCard
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
        public Card card {get;set;}
    }
    #endregion

    #endregion

    public class Handler
    {
        public async Task<IAliceResponse> FunctionHandler(string requestJ)
        {

            if (requestJ.Contains("version") == false)
            {
                if (requestJ.Contains("queryStringParameters"))
                {
                    int startQ = requestJ.IndexOf("queryStringParameters");
                    int start2 = requestJ.IndexOf("code", startQ);
                    int startCode = requestJ.IndexOf(":", start2);
                    int endCode = requestJ.IndexOf("}", start2);
                    string code = requestJ.Substring(startCode, endCode - startCode);
                    code = code.Replace("'", "");
                    code = code.Replace(":", "");
                    code = code.Replace("}", "");
                    code = code.Replace("u0022", "");
                    code = code.Replace("\\", "");
                    code = code.Replace("\'", "");
                    code = code.Replace("\"", "");
                    Storage.Code = code;
                    //int icode = Convert.ToInt32(code);
                    //Приложение отправляет код, а также свой идентификатор и пароль в POST-запросе.
                    HttpClient httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri("https://oauth.yandex.ru");

                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    headers.Add("grant_type", "authorization_code");
                    headers.Add("code", code);
                    //e738f647b8b145948b7a5a622a59819d
                    //95992604fc3748659bb019212c3f30de
                    headers.Add("client_id", "23cabbbdc6cd418abb4b39c32c41195d");
                    headers.Add("client_secret", "53bc75238f0c4d08a118e51fe9203300");


                    var httpcontent = new FormUrlEncodedContent(headers);

                    var webRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth.yandex.ru/token")
                    {
                        Content = httpcontent
                    };

                    var res = await httpClient.PostAsync("https://oauth.yandex.ru/token",httpcontent);

                    var jsonBody = res.Content.ReadAsStringAsync().Result;
                    int st = jsonBody.IndexOf("access_token");
                    int st2 = jsonBody.IndexOf(":",st);
                    int st3 = jsonBody.IndexOf(",", st2);
                    string auf = jsonBody.Substring(st2, st3 - st2);
                    auf = auf.Replace(":", "");
                    auf = auf.Replace("\"", "");
                    auf = auf.Trim();
                    Storage.Auth = auf;
                    return new AliceResponse();
                    //throw new Exception("auf=" + auf);
                }
                else
                    //hmmm
                    throw new Exception("req=" + requestJ);

                
            }
            else
            {
                AliceRequestBase request = JsonSerializer.Deserialize<AliceRequestBase>(requestJ);
                if (request.Request.Command.ToLower().Contains("аудио"))
                {
                    var audio = new AliceAudioResponse();
                    audio.Response.Text = "послушай эту музыку";
                    audio.Response.tts = "послушай эту музыку";
                    //from api.Track.GetFileLink
                    audio.Response.Directives.audioPlayer.item.stream.url = "https://s53sas.storage.yandex.net/get-mp3/1659b4f005c147acbd5a601d8d0a4c5855992b59/0005e2c05c65f84d/rmusic/U2FsdGVkX1_GK5XMhybGzFpjubChT1YEso7Ai2Fl1iepxICrrCW0XvlkpeB9mwjXmkHCnwrQnLojkHnidVcc7nfbQcWw7hbuPHDNKdaIXso/6e384b312cdfe5fc7cbc23f094a9be3ca04ebac6d25bae22ab1d9debaed51094/31487";
                    audio.Response.Directives.audioPlayer.item.metadata.SongTitle = "question";
                    audio.Response.Directives.audioPlayer.item.metadata.SongArtist = "System of a down";
                    return audio;
                }
                else if (request.Request.Command.ToLower().Contains("картинка"))
                {
                    AliceResponseCard resp = new AliceResponseCard();
                    resp.Response.Text = "карта";
                    resp.Response.Tts = "карта";
                    resp.Response.card = new Card();
                    return resp;
                }
                else
                {
                    var response = new AliceResponse(request);
                    return response;
                }
            }
        }
        public string token(string access_token, string expires_in = "", string token_type = "")
        {
            string res = access_token;
            if (res.Contains("#"))
                res = res.Substring(res.IndexOf("#"));
            //hmmm
            throw new Exception(res);

            return res;
        }
    }
}