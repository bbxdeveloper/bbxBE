﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AxegazMobileSrv.Attrib
{
    public class Table : Attribute
    {
        public string Name { get; set; }

        public Table(string p_name)
        {
            Name = p_name;
        }
    }
}
