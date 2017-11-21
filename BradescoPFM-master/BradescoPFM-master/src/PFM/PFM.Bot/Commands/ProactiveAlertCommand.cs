using PFM.Bot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;
using PFM.Bot.Services;
using PFM.Bot.Utils;

namespace PFM.Bot.Commands
{
    public class ProactiveAlertCommand : ICommand
    {
        public void Run(IActivity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            Task task = new Task(async () =>
            {
                var reply = ((Activity)activity).CreateReply("Um momento, vou pesquisar!");
                await connector.Conversations.SendToConversationAsync(reply);

                var service = new CoreService();
                var alert = await service.CheckNegativeBalance(Settings.AccountId);

                reply = ((Activity)activity).CreateReply(alert.Message);
                await connector.Conversations.SendToConversationAsync(reply);
            });
            task.Start();
        }
    }
}