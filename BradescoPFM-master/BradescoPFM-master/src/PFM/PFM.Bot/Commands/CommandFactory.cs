using PFM.Bot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PFM.Bot.Commands
{
    public class CommandFactory
    {
        public static ICommand GetCommand(string command)
        {
            switch (command.ToLower())
            {
                case "proactive":
                    return new ProactiveAlertCommand();
                default:
                    return new DefaultCommand();
            }
        }
    }
}