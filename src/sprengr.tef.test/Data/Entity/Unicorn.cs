using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sprengr.tef.test.Data.Entity
{
    public class Unicorn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAlive { get; set; }
        public UnicornType Type { get; set; }
    }
}
