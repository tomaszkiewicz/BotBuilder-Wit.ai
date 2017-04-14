using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tomaszkiewicz.WitAi.Handlers
{
    public delegate Task ActionHandler(Dictionary<string, object> witContext, WitResult witResult, ref bool requestReset);
}