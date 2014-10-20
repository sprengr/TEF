using Sprengr.Tef.Test.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprengr.Tef.Test.Data
{
    public class UnicornRepository
    {
        private Func<UnicornContext> _contextFactory;

        public UnicornRepository(Func<UnicornContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Save(Unicorn unicorn)
        {
            using (var db = _contextFactory())
            {
                var type = db.Types.First(t => t.Id == unicorn.Type.Id);
                unicorn.IsAlive = unicorn.IsAlive && !type.IsExtinct;

                db.Unicorns.Add(unicorn);
                db.SaveChanges();
            }
        }

        public List<Unicorn> GetSortedByName()
        {
            using (var db = _contextFactory())
            {
                return (from b in db.Unicorns
                            orderby b.Name
                            select b).ToList();
            }
        }

    }
}
