using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace PFM.Bot.Utils
{
    public class MemoryDataStore : IBotDataStore<BotData>
    {
        Dictionary<IAddress, BotData> _dict1 = new Dictionary<IAddress, BotData>();
        Dictionary<IAddress, BotData> _dict2 = new Dictionary<IAddress, BotData>();
        Dictionary<IAddress, BotData> _dict3 = new Dictionary<IAddress, BotData>();

        public Task<bool> FlushAsync(IAddress key, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<BotData> LoadAsync(IAddress key, BotStoreType botStoreType, CancellationToken cancellationToken)
        {
            try
            {
                switch (botStoreType)
                {
                    case BotStoreType.BotConversationData:
                        return Task.FromResult(_dict1[key]);

                    case BotStoreType.BotPrivateConversationData:
                        return Task.FromResult(_dict2[key]);

                    case BotStoreType.BotUserData:
                        return Task.FromResult(_dict3[key]);
                }
            }
            catch { }

            return Task.FromResult<BotData>(null);
        }

        public Task SaveAsync(IAddress key, BotStoreType botStoreType, BotData data, CancellationToken cancellationToken)
        {
            switch(botStoreType)
            {
                case BotStoreType.BotConversationData:
                    _dict1[key] = data;
                    break;
                case BotStoreType.BotPrivateConversationData:
                    _dict2[key] = data;
                    break;
                case BotStoreType.BotUserData:
                    _dict3[key] = data;
                    break;

            }

            return Task.FromResult(true);
        }
    }
}