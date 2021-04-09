﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Drawing
{
    public interface ITypeface
    {
        string FamilyName { get; }
        FontSlant Slant { get; }
        int Weight { get; }
        object NativeObject { get; }
    }
}
