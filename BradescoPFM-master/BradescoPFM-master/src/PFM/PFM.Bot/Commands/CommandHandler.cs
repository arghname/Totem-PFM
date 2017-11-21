using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Microsoft.Bot.Connector;

namespace PFM.Bot.Commands
{
    public class CommandHandler
    {
        private const string _commandWithArgumentPattern = @"\/command\=[a-z]+";
        private const string _commandPattern = @"\/command\=";

        public string Text { get; }


        public CommandHandler(string text)
        {
            Text = text;
        }

        public bool IsCommand()
        {
            return Regex.IsMatch(Text, _commandWithArgumentPattern, RegexOptions.IgnoreCase);
        }

        public void Execute(Activity activity)
        {
            if (IsCommand())
            {
                var command = Regex.Replace(Text, _commandPattern, String.Empty);
                var handler = CommandFactory.GetCommand(command);
                handler.Run(activity);
            }
            else
            {
                throw new InvalidProgramException("The message is not a command.");
            }
        }

    }
}