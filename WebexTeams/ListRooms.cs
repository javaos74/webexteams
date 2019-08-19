using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Net;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace WebexTeams
{
    public sealed class ListRooms : CodeActivity
    {
        // Access Token.
        [Category("Input")]
        [DisplayName("Access Token")]
        [RequiredArgument]
        [Description("Input your webex team api access token here")]
        public InArgument<string> AccessToken { get; set; }


        [Category("Output")]
        [DisplayName("Error Code")]
        [Description("Error code if success 200 otherwise error code ")]
        public OutArgument<int> ErrorCode { get; set; }

        [Category("Output")]
        [DisplayName("Error Message")]
        [Description("Error message if error code is not 0 ")]
        public OutArgument<string> ErrorMessage { get; set; }

        [Category("Output")]
        [DisplayName("Room List")]
        [Description("Room list return with name and id")]
        public OutArgument<List<WebexTeamsRoom>> Rooms { get; set; }

        // 작업 결과 값을 반환할 경우 CodeActivity<TResult>에서 파생되고
        // Execute 메서드에서 값을 반환합니다.
        protected override void Execute(CodeActivityContext context)
        {
            // 텍스트 입력 인수의 런타임 값을 가져옵니다.
            string accessToken = context.GetValue(this.AccessToken);
            List<WebexTeamsRoom> rooms;
            Int32 errorCode;
            string errorMessage;

            WebexTeamsClient client = WebexTeamsClient.getInstance(accessToken);

            bool success = client.ListRooms(out rooms, out errorCode, out errorMessage);
            context.SetValue(this.ErrorCode, errorCode);
            context.SetValue(this.ErrorMessage, errorMessage);
            context.SetValue(this.Rooms, rooms);
        }
    }
}
