﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabylonJs.Framework.Converters
{
    public interface IBabylonConverter
    {
        Task<string> ToJsonAsync();
    }
}
