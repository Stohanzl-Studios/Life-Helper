﻿using SharedResources.Interfaces.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources
{
    public class Delegates
    {
        public delegate void ResponseCallback(IResponse response);
    }
}
