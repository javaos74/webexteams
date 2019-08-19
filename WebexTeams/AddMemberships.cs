using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;

namespace WebexTeams
{

    public sealed class AddMemberships : CodeActivity
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

        // input person email 
        [Category("Input")]
        [DisplayName("Person's email address")]
        [Description("Input email address")]
        public InArgument<string> PersonEmail { get; set; }


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

        // 작업 결과 값을 반환할 경우 CodeActivity<TResult>에서 파생되고
        // Execute 메서드에서 값을 반환합니다.
        protected override void Execute(CodeActivityContext context)
        {
            string accessToken = context.GetValue(this.AccessToken);
            string roomId = context.GetValue(this.RoomId);
            string personEmail = context.GetValue(this.PersonEmail);

            WebexTeamsMemberships membership;
            Int32 errorCode;
            string errorMessage;

            WebexTeamsClient client = WebexTeamsClient.getInstance(accessToken);
            bool success = client.AddMemberships(roomId, personEmail, out membership, out errorCode, out errorMessage);

            context.SetValue(this.ErrorCode, errorCode);
            context.SetValue(this.ErrorMessage, errorMessage);
            //context.SetValue(this.Message, message);

        }
    }
}
