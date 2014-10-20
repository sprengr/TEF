using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprengr.Tef.Test.Data
{
    public class ContextTwoEntities : DbContext
    {
        public virtual DbSet<TestEntityWithId> EntityWithId { get; set; }
        public virtual DbSet<TestEntityWithIdAndName> EntityWithIdAndName { get; set; }

    }

    public class TestEntityWithId
    {
        public int Id { get; set; }
    }

    public class TestEntityWithIdAndName
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
}
