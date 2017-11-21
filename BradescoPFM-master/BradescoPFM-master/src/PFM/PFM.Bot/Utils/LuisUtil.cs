using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PFM.Bot.Utils
{
    public class LuisUtil
    {
        public string GetEntity(LuisResult result, string entityName)
        {
            EntityRecommendation entity;
            result.TryFindEntity(entityName, out entity);
            return entity?.Entity.ToUpper();
        }
    }
}