﻿using Bst.Fx.WarningData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.IWarning
{
    public interface IWarningService
    {
        void Execute();

        void SendMessage();

        void SendWarning(string messagekey, string businessId);
    }
}
