﻿using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFM.Bot.Interfaces
{
    public interface ICommand
    {
        void Run(IActivity activity);
    }
}
