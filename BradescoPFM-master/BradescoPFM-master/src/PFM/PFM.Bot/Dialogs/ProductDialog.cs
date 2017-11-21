using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using PFM.Bot.Services;
using PFM.Bot.Utils;
using System.Globalization;

namespace PFM.Bot.Dialogs
{
    [Serializable]
    public class ProductDialog : IDialog
    {
        private CultureInfo _cultureInfo;

        public ProductDialog()
        {
            _cultureInfo = CultureInfo.GetCultureInfo("pt-BR");
        }

        public async Task StartAsync(IDialogContext context)
        {
            var text = ((Activity)context.Activity).Text.ToUpper();

            if (text.Contains("PHONE") || text.Contains("CELULAR"))
            {
                var recommendation = await new CoreService().GetAvailableMonthForPurchase(Settings.AccountId, "PHONE");
                var months = recommendation.AvailableMonth;

                if (months==0)
                    await context.PostAsync($"Você atualmente pode comprar um phone de {recommendation.Value.ToString("C", _cultureInfo)}.");
                else if (months==-1)
                    await context.PostAsync($"Você não pode comprar um phone de {recommendation.Value.ToString("C", _cultureInfo)} pelos próximos 4 meses. Economize mais!");
                else
                    await context.PostAsync($"Se você mantiver seu padrão de gastos, você pode comprar um phone de R$ {recommendation.Value} em {months} meses");
            }
            else
            {
                await context.PostAsync("Quer comprar um smartphone?");
            }
            context.Done<object>(null);
        }
    }
}