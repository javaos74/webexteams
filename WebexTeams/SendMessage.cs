using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebexTeams
{

    public sealed class SendMessage : CodeActivity
    {
        // Access Token.
        [Category("Input")]
        [DisplayName("Access Token")]
        [RequiredArgument]
        [Description("Input your webex team api access token here")]
        public InArgument<string> AccessToken { get; set; }

        // Room id 
        [Category("Input")]
        [DisplayName("Room Id")]
        [RequiredArgument]
        [Description("Input your room id")]
        public InArgument<string> RoomId { get; set; }

        // Input message  
        [Category("Input")]
        [DisplayName("Input message")]
        [Description("Input your message id")]
        public InArgument<string> Text { get; set; }

        // Images 
        [Category("Input")]
        [DisplayName("Local file path")]
        [Description("Input your file path")]
        public InArgument<string> FilePath { get; set; }

        // error code 
        [Category("Output")]
        [DisplayName("Error code")]
        [Description("Error code if success 200 otherwise error code ")]
        public OutArgument<int> ErrorCode { get; set; }

        // error message 
        [Category("Output")]
        [DisplayName("Error message")]
        [Description("Error message if error code is not 0 ")]
        public OutArgument<string> ErrorMessage { get; set; }

        [Category("Output")]
        [DisplayName("Created message")]
        [Description("New message")]
        public OutArgument<WebexTeamsMessage> Message { get; set; }


        // 작업 결과 값을 반환할 경우 CodeActivity<TResult>에서 파생되고
        // Execute 메서드에서 값을 반환합니다.
        protected override void Execute(CodeActivityContext context)
        {
            string accessToken = context.GetValue(this.AccessToken);
            string roomId = context.GetValue(this.RoomId);
            string text = context.GetValue(this.Text);
            string filePath = context.GetValue(this.FilePath);

            WebexTeamsMessage message;
            Int32 errorCode;
            string errorMessage;

            WebexTeamsClient client = WebexTeamsClient.getInstance(accessToken);
            bool success = client.SendMessage(roomId, text, filePath, out message, out errorCode, out errorMessage);

            context.SetValue(this.ErrorCode, errorCode);
            context.SetValue(this.ErrorMessage, errorMessage);
            context.SetValue(this.Message, message);
        }
    }
}
