using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Tomaszkiewicz.WitAi.SampleBot.Handlers;

namespace Tomaszkiewicz.WitAi.SampleBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {
                if (activity.Type == ActivityTypes.Message)
                {
                    var stateClient = activity.GetStateClient();
                    var botData = await stateClient.BotState.GetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id);

                    var persistence = new BotStateServicePersistence(botData);

                    var dispatcher = CreateDispatcher(activity, persistence);

                    await dispatcher.Dispatch(activity.Text);

                    await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);
                }
                else
                {
                    HandleSystemMessage(activity);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private static WitDispatcher CreateDispatcher(Activity activity, IWitPersistence persistence)
        {
            var dispatcher = new WitDispatcher("VZQQJVTB2E5DBMXL2RXSS73CX75H4ABO", persistence);

            dispatcher.SetDefaultHandler(new DefaultBotHandler(activity));
            dispatcher.RegisterIntentHandler(new GreetingsHandler(activity));
            dispatcher.RegisterIntentHandler(new WeatherHandler(activity));
            dispatcher.RegisterIntentHandler(new ThanksHandler(activity));

            return dispatcher;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}