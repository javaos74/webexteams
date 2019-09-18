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
using WebexTeams.Properties;
using System.Activities.Validation;

namespace WebexTeams
{

    [LocalizedDisplayName(nameof(Resources.ActivityNameSendMessage))]
    public sealed class SendMessage : CodeActivity
    {
        // Access Token 
        [LocalizedCategory(nameof(Resources.Input))]
        [LocalizedDisplayName(nameof(Resources.AccessToken))]
        [RequiredArgument]
        [LocalizedDescription(nameof(Resources.AccessTokenDesc))]
        public InArgument<string> AccessToken { get; set; }

        // Room id 
        [LocalizedCategory(nameof(Resources.Input))]
        [LocalizedDisplayName(nameof(Resources.RoomId))]
        [RequiredArgument]
        [LocalizedDescription(nameof(Resources.RoomIdDesc))]
        public InArgument<string> RoomId { get; set; }

        // Input message  
        [LocalizedCategory(nameof(Resources.Input))]
        [LocalizedDisplayName(nameof(Resources.Message))]
        [LocalizedDescription(nameof(Resources.MessageDesc))]
        public InArgument<string> Text { get; set; }

        // Images 
        [LocalizedCategory(nameof(Resources.Input))]
        [LocalizedDisplayName(nameof(Resources.LocalFile))]
        [LocalizedDescription(nameof(Resources.LocalFileDesc))]
        public InArgument<string> FilePath { get; set; }

        // error code 
        [LocalizedCategory(nameof(Resources.Output))]
        [LocalizedDisplayName(nameof(Resources.ErrorCode))]
        [LocalizedDescription(nameof(Resources.ErrorCodeDesc))]
        public OutArgument<int> ErrorCode { get; set; }

        // error message 
        [LocalizedCategory(nameof(Resources.Output))]
        [LocalizedDisplayName(nameof(Resources.ErrorMessage))]
        [LocalizedDescription(nameof(Resources.ErrorMessageDesc))]
        public OutArgument<string> ErrorMessage { get; set; }

        [LocalizedCategory(nameof(Resources.Output))]
        [LocalizedDisplayName(nameof(Resources.NewMessageDesc))]
        [LocalizedDescription(nameof(Resources.NewMessageDesc))]
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


        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            if( this.Text == null && this.FilePath == null)
            {
                metadata.AddValidationError(new ValidationError(string.Format(Resources.ValidateErrorDesc, Resources.Message, Resources.LocalFile)));
            }
        }
    }
}
