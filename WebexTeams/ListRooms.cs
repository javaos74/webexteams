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
using WebexTeams.Properties;

namespace WebexTeams
{


    [LocalizedDisplayName(nameof(Resources.ActivityNameListRooms))]
    public sealed class ListRooms : CodeActivity
    {
        // Access Token.
        [LocalizedCategory(nameof(Resources.Input))]
        [LocalizedDisplayName(nameof(Resources.AccessToken))]
        [RequiredArgument]
        [LocalizedDescription(nameof(Resources.AccessTokenDesc))]
        public InArgument<string> AccessToken { get; set; }


        [LocalizedCategory(nameof(Resources.Output))]
        [LocalizedDisplayName(nameof(Resources.ErrorCode))]
        [LocalizedDescription(nameof(Resources.ErrorCodeDesc))]
        public OutArgument<int> ErrorCode { get; set; }

        [LocalizedCategory(nameof(Resources.Output))]
        [LocalizedDisplayName(nameof(Resources.ErrorMessage))]
        [LocalizedDescription(nameof(Resources.ErrorMessageDesc))]
        public OutArgument<string> ErrorMessage { get; set; }

        [LocalizedCategory(nameof(Resources.Output))]
        [LocalizedDisplayName(nameof(Resources.RoomList))]
        [LocalizedDescription(nameof(Resources.RoomListDesc))]
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
