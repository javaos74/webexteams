using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace WebexTeams
{

    public sealed class CreateRoom : CodeActivity
    {
        // Access Token.
        [Category("Input")]
        [DisplayName("Access Token")]
        [RequiredArgument]
        [Description("Input your webex team api access token here")]
        public InArgument<string> AccessToken { get; set; }

        // room title 
        [Category("Input")]
        [DisplayName("Room Title")]
        [RequiredArgument]
        [Description("Input your room title ")]
        public InArgument<string> RoomTitle { get; set; }

        // team id 
        [Category("Input")]
        [DisplayName("Team Id")]
        [Description("Input your team id which associates with room ")]
        public InArgument<string> TeamId { get; set; }


        // error code 
        [Category("Output")]
        [DisplayName("Error Code")]
        [Description("Error code if success 200 otherwise error code ")]
        public OutArgument<int> ErrorCode { get; set; }

        // error message 
        [Category("Output")]
        [DisplayName("Error Message")]
        [Description("Error message if error code is not 0 ")]
        public OutArgument<string> ErrorMessage { get; set; }

        [Category("Output")]
        [DisplayName("WebexTeams Room")]
        [Description("New Room is created")]
        public OutArgument<WebexTeamsRoom> Room { get; set; }

        // 작업 결과 값을 반환할 경우 CodeActivity<TResult>에서 파생되고
        // Execute 메서드에서 값을 반환합니다.
        protected override void Execute(CodeActivityContext context)
        {
            string accessToken = context.GetValue(this.AccessToken);
            string title = context.GetValue(this.RoomTitle);
            string teamId = context.GetValue(this.TeamId);

            WebexTeamsRoom room;
            Int32 errorCode;
            string errorMessage;

            WebexTeamsClient client = WebexTeamsClient.getInstance(accessToken);
            bool success = client.CreateRoom(teamId, title, out room, out errorCode, out errorMessage);

            context.SetValue(this.ErrorCode, errorCode);
            context.SetValue(this.ErrorMessage, errorMessage);
            context.SetValue(this.Room, room);
        }
    }
}
