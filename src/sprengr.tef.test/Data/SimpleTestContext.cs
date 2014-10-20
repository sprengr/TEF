using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprengr.Tef.Test.Data
{
    public class SimpleTestContext : DbContext
    {
        public virtual DbSet<SimpleEntityWithId> Entity { get; set; }
    }

    public class SimpleEntityWithId
    {
        public int Id { get; set; }
    }
}
