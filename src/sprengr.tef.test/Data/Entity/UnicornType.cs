﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sprengr.Tef.Test.Data.Entity
{
    public class UnicornType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Unicorn> Unicorns { get; set; }
        public bool IsExtinct { get; set; }
    }
}
