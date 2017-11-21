using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis.Models;
using PFM.Bot.Utils;
using PFM.Bot.Models;
using PFM.Bot.Services;
using Microsoft.Bot.Builder.FormFlow;
using System.Globalization;

namespace PFM.Bot.Dialogs
{
    [Serializable]
    public class DinnerDialog : IDialog
    {
        private CultureInfo _cultureInfo;

        private string _restaurantName;
        private decimal _restaurantePrice;

        public DinnerDialog(LuisResult result)
        {
            _cultureInfo = CultureInfo.GetCultureInfo("pt-BR");
            _restaurantName = new LuisUtil().GetEntity(result, "Restaurante");
        }

        public async Task StartAsync(IDialogContext context) // Step 1: Check if it was identified the name
        {
            if (string.IsNullOrEmpty(_restaurantName))
            {
                await context.PostAsync("Desculpe! Não entendi muito bem onde você quer jantar. Vou te fazer algumas perguntas:");
                await context.PostAsync("Primeiro, onde você quer jantar?");
                context.Wait(RestaurantNameReceived);
            }
            else
            {
                await CheckRestaurantRegister(context);
            }
        }

        private async Task RestaurantNameReceived(IDialogContext context, IAwaitable<object> result) // Step 2: Get the restaurant name
        {
            _restaurantName = ((Activity)await result).Text.ToUpper();
            await CheckRestaurantRegister(context);
        }

        private async Task CheckRestaurantRegister(IDialogContext context) // Step 3: Check if the register exists on the database
        {
            var service = new ProductService();
            var product = await service.GetAsync(_restaurantName);

            if (product == null)
            {
                await context.PostAsync("Agora, quanto você acha que vai gastar lá?");
                context.Wait(RestaurantPriceReceived);
            }
            else
            {
                _restaurantePrice = product.Price;
                await GetRecommendation(context);
            }
        }

        private async Task RestaurantPriceReceived(IDialogContext context, IAwaitable<object> result) // Step 4: Get the restaurant price
        {
            var text = ((Activity)await result).Text;
            _restaurantePrice = decimal.Parse(text);

            //Insert on database:
            var product = new Product(_restaurantName, _restaurantePrice);
            await new ProductService().InsertAsync(product);

            await GetRecommendation(context);
        }

        private async Task GetRecommendation(IDialogContext context) // Step 5: Get the recommendation
        {
            var service = new CoreService();
            var recommendation = await service.GetAvailableMonthForPurchase(Settings.AccountId, _restaurantName);

            if (recommendation.AvailableMonth == 0)
            {
                await context.PostAsync($"Em média, um casal gasta {_restaurantePrice.ToString("C", _cultureInfo)} num jantar no {_restaurantName}. " +
                    "Considerando seus gastos até o dia de hoje, você não compromete seu padrão de gastos.");
            }
            else
            {
                await context.PostAsync($"Em média, um casal gasta {_restaurantePrice.ToString("C", _cultureInfo)} num jantar no {_restaurantName}. " +
                    "Nesse momento você poderia ir em um restaurante mais em conta. Quer recomendações?");
            }
            context.Done<object>(null);
        }
    }
}