using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tomaszkiewicz.WitAi
{
    public class InMemoryWitPersistence : IWitPersistence
    {
        private WitContext _witContext;
        private string _sessionId;
        private string _intent;
        private bool _resetRequested;
        private readonly Dictionary<string, object> _privateData = new Dictionary<string, object>();

        public Task<WitContext> GetWitContext()
        {
            return Task.FromResult(_witContext);
        }

        public Task<string> GetSessionId()
        {
            return Task.FromResult(_sessionId);
        }

        public Task SetIntent(string intent)
        {
            _intent = intent;
            return Task.CompletedTask;
        }

        public Task<string> GetIntent()
        {
            return Task.FromResult(_intent);
        }

        public Task<bool> GetResetRequested()
        {
            return Task.FromResult(_resetRequested);
        }

        public Task SetResetRequested(bool resetRequested)
        {
            _resetRequested = resetRequested;
            return Task.CompletedTask;
        }

        public Task SetSessionId(string sessionId)
        {
            _sessionId = sessionId;
            return Task.CompletedTask;
        }

        public Task SetWitContext(WitContext witContext)
        {
            _witContext = witContext;
            return Task.CompletedTask;
        }

        public Task<bool> TryGetPrivateConversationData(string name, out object data)
        {
            return Task.FromResult(_privateData.TryGetValue(name, out data));
        }

        public Task SetPrivateConversationData(string name, object data)
        {
            _privateData[name] = data;
            return Task.CompletedTask;
        }
    }
}