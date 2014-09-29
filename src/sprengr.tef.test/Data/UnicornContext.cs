using sprengr.tef.test.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sprengr.tef.test.Data
{
    public class UnicornContext : DbContext
    {
        public virtual DbSet<Unicorn> Unicorns { get; set; }
        public virtual DbSet<UnicornType> Types { get; set; }
    }
}
