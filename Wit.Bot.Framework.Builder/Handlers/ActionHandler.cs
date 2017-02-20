using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Wit.Bot.Framework.Builder.Models;

namespace Wit.Bot.Framework.Builder.Handlers
{
    public delegate Task ActionHandler(IDialogContext context, WitResult witResult);
}