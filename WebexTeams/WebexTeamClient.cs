using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebexTeams
{
    
    public class WebexTeamsMessage
    {
        public string id { get; set; }
        public string roomId { get; set; }
        public string roomType { get; set; }
        public string text { get; set; }
    }

    public class FileParameter
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public FileParameter(byte[] file) : this(file, null) { }
        public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
        public FileParameter(byte[] file, string filename, string contenttype)
        {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }
    }

    public class WebexTeamsMemberships
    {
        public string roomId { get; set; }
        public string personEmail { get; set; }
        public string personId { get; set; }
        public string personDisplayName { get; set; }
    }
    public class WebexTeamsRoom
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public string TeamId { get; set; }
    }

    public class WebexTeamsClient
    {
        private static string ROOM_ENDPOINT = "https://api.ciscospark.com/v1/rooms";
        private static string MESSAGE_ENDPOINT = "https://api.ciscospark.com/v1/messages";
        private static string MEMBERSHIP_ENDPOINT = "https://api.ciscospark.com/v1/memberships";

        public static string accessToken { get; internal set; }

        private static WebexTeamsClient instance = null;
        private static readonly Encoding encoding = Encoding.UTF8;

        public static WebexTeamsClient getInstance( string token)
        {
            if( instance == null)
               instance =  new WebexTeamsClient(token);
            return instance;
        }

        private WebexTeamsClient( string token)
        {
            WebexTeamsClient.accessToken = token;
        }

        private static string getMediaType( string fileName)
        {
            switch( fileName.Split(".".ToCharArray()).Last().ToUpper())
            {
                case "PNG":
                    return "image/png";
                case "JPEG":
                    return "image/jpeg";
                case "JPG":
                    return "image/jpeg";
                case "GIF":
                    return "image/gif";
                case "BMP":
                    return "image/bmp";
                case "XLSX":
                case "XLS":
                    return "application/vnd.ms-excel";
                case "PPTX":
                case "PPT":
                    return "application/vnd.ms-powerpoint";
                case "DOCX":
                case "DOC":
                    return "application/vnd.ms-word";
                default:
                    return "unknwon";
            }
        }

        /**
         * multipart/form-data 형식으로 데이터를 생성 함 
         */
        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
        /**
         * 전체 Room List를 요청 함 
         */
        public bool ListRooms( out List<WebexTeamsRoom> roomList, out Int32 errorCode, out string errorMessage)
        {
            bool status = true;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ROOM_ENDPOINT);
            //request.Headers.Clear();
            request.Headers.Add("Authorization", string.Format("Bearer {0}", WebexTeamsClient.accessToken));
            request.ContentType = "application/json;charset=utf-8";
            request.Method = "GET";

            List<WebexTeamsRoom> rooms = new List<WebexTeamsRoom>();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject roomsResponse = JObject.Parse(reader.ReadToEnd());
                    IList<JToken> items = roomsResponse["items"].Children().ToList();
                    foreach (JToken item in items)
                    {
                        // JToken.ToObject is a helper method that uses JsonSerializer internally
                        WebexTeamsRoom room = item.ToObject<WebexTeamsRoom>();
                        rooms.Add(room);
                    }
                    errorCode = (int)HttpStatusCode.OK;
                    errorMessage = "OK";
                    roomList = rooms;
                }
                else
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject resp = JObject.Parse(reader.ReadToEnd());
                    errorCode = (int)response.StatusCode;
                    errorMessage = resp["message"].ToString();
                    roomList = null;
                    status = false;
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error Message : " + e.Message);
                errorCode = -1;
                errorMessage = e.Message;
                roomList = null;
                status = false;
            }
            return status;
        }


        /**
         * 새로운 대화방을 만듬 
         */
        public  bool CreateRoom( string teamId, string title, out WebexTeamsRoom room, out Int32 errorCode, out  string errorMessage)
        {
            bool status = true;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ROOM_ENDPOINT);
            request.Headers.Add("Authorization", string.Format("Bearer {0}", WebexTeamsClient.accessToken));
            request.ContentType = "application/json;charset=utf-8";
            request.Method = "POST";

            try
            {
                WebexTeamsRoom input = new WebexTeamsRoom();
                input.Title = title;
                if (!string.IsNullOrEmpty(teamId))
                    input.TeamId = teamId;
                string payload = JsonConvert.SerializeObject(input);
                Stream inputStream = request.GetRequestStream();
                byte[] buff = System.Text.Encoding.UTF8.GetBytes(payload);
                inputStream.Write(buff, 0, buff.Length);
                inputStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    WebexTeamsRoom result = JsonConvert.DeserializeObject<WebexTeamsRoom>(reader.ReadToEnd());

                    errorCode = (int)HttpStatusCode.OK;
                    errorMessage = "OK";
                    room = result;
                }
                else
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject resp = JObject.Parse(reader.ReadToEnd());
                    errorCode = (int)response.StatusCode;
                    errorMessage = resp["message"].ToString();
                    room = null;
                    status = false;
                }
            }
            catch ( System.Exception e)
            {
                System.Console.WriteLine("Error Message : " + e.Message);
                errorCode = -1;
                errorMessage = e.Message;
                room = null;
                status = false;
            }
            return status;
        }


        /**
         * 메세지를 전송한다. Text 메시지 및 로컬 파일을 포함한 이미지 전송이 가능하다. 
         */
        public bool SendMessage( string roomId, string text, string filePath, out WebexTeamsMessage message, out Int32 errorCode, out string errorMessage)
        {
            bool status = true;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MESSAGE_ENDPOINT);
            request.Headers.Add("Authorization", string.Format("Bearer {0}", WebexTeamsClient.accessToken));
            request.Method = "POST";

            try
            {
                Dictionary<string, object> postParameters = new Dictionary<string, object>();
                postParameters.Add("roomId", roomId);
                if( !string.IsNullOrEmpty(text))
                    postParameters.Add("text", text);
                string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
                string contentType = "multipart/form-data; boundary=" + formDataBoundary;
                request.ContentType = contentType;

                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        FileInfo imgFile = new FileInfo(filePath);
                        if (imgFile.Exists && imgFile.Length > 0)
                        {
                            byte[] imgData = new byte[imgFile.Length];
                            imgData = File.ReadAllBytes(imgFile.FullName);
                            if (imgData.Length > 0)
                            {
                                postParameters.Add("files", new FileParameter(imgData, imgFile.Name, WebexTeamsClient.getMediaType( imgFile.Name)));
                            }
                        }
                    }
                    catch (IOException ioe)
                    {
                        System.Console.WriteLine(ioe.Message);
                    }
                }

                byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);
                Stream inputStream = request.GetRequestStream();
                inputStream.Write(formData, 0, formData.Length);
                inputStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    WebexTeamsMessage result = JsonConvert.DeserializeObject<WebexTeamsMessage>(reader.ReadToEnd());
                    errorCode = (int)HttpStatusCode.OK;
                    errorMessage = "OK";
                    message = result;
                }
                else
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject resp = JObject.Parse(reader.ReadToEnd());
                    errorCode = (int)response.StatusCode;
                    errorMessage = resp["message"].ToString();
                    message = null;
                    status = false;
                }
            }
            catch ( System.Exception e)
            {
                System.Console.WriteLine("Error Message : " + e.Message);
                errorCode = -1;
                errorMessage = e.Message;
                message = null;
                status = false;
            }

            return status;
        }


        public bool AddMemberships( string roomId, string personEmail, out WebexTeamsMemberships membership, out Int32 errorCode, out string errorMessage)
        {
            bool status = true;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MEMBERSHIP_ENDPOINT);
            request.Headers.Add("Authorization", string.Format("Bearer {0}", WebexTeamsClient.accessToken));
            request.ContentType = "application/json;charset=utf-8";
            request.Method = "POST";

            try
            {
                WebexTeamsMemberships input = new WebexTeamsMemberships();
                input.roomId = roomId;
                input.personEmail = personEmail;
                string payload = JsonConvert.SerializeObject(input);
                Stream inputStream = request.GetRequestStream();
                byte[] buff = System.Text.Encoding.UTF8.GetBytes(payload);
                inputStream.Write(buff, 0, buff.Length);
                inputStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    WebexTeamsMemberships result = JsonConvert.DeserializeObject<WebexTeamsMemberships>(reader.ReadToEnd());

                    errorCode = (int)HttpStatusCode.OK;
                    errorMessage = "OK";
                    membership = result;
                }
                else
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject resp = JObject.Parse(reader.ReadToEnd());
                    errorCode = (int)response.StatusCode;
                    errorMessage = resp["message"].ToString();
                    membership = null;
                    status = false;
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error Message : " + e.Message);
                errorCode = -1;
                errorMessage = e.Message;
                membership = null;
                status = false;
            }
            return status;
        }
    }
}
