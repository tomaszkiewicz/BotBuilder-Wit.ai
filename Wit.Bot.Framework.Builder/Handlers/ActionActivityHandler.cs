using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Wit.Bot.Framework.Builder.Models;

namespace Wit.Bot.Framework.Builder.Handlers
{
    public delegate Task ActionActivityHandler(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult);
}