using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using PFM.Bot.Services;
using PFM.Bot.Utils;
using System;
using System.Threading.Tasks;

namespace PFM.Bot.Dialogs
{
    [Serializable]
    public class IntentDialog : LuisDialog<object>
    {
        public IntentDialog(string modelId, string subscriptionKey) : base(new LuisService(new LuisModelAttribute(modelId, subscriptionKey, domain: "eastus2.api.cognitive.microsoft.com"))) { }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Eu ainda estou aprendendo, não sei responder a esta pergunta!");
        }

        [LuisIntent("Resumo")]
        public async Task HandleResumo(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var service = new CoreService();
            var summarizer = new Summarizer();

            await context.PostAsync("OK! Aqui vai o seu resumo da semana passada:");
            var lastWeekEntries = (await service.LastWeekSummary(Settings.AccountId)).Entries;
            await context.PostAsync(summarizer.GetSummary(lastWeekEntries, "Você recebeu:", "Porém, você gastou com:"));

            await Task.Delay(2000);

            await context.PostAsync("Para a próxima semana, posso ver que você tem agendado:");
            var nextWeekEntries = (await service.NextWeekSummary(Settings.AccountId)).Entries;
            await context.PostAsync(summarizer.GetSummary(nextWeekEntries, "Para receber:", "Mas também será debitado:"));
        }

        [LuisIntent("Comer")]
        public async Task HandleComer(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            context.Call(new DinnerDialog(result), ResumerAfterExecution);
        }

        [LuisIntent("Comprar")]
        public async Task HandleCompras(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            context.Call(new ProductDialog(), ResumerAfterExecution);
        }

        [LuisIntent("Greetings")]
        public async Task HandleGreetings(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Oi!");
        }

        [LuisIntent("Despedidas")]
        public async Task HandleDespedidas(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Tchau e até logo, obrigada!");
        }

        private Task ResumerAfterExecution(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(base.MessageReceived);
            return Task.CompletedTask;
        }
    }
}