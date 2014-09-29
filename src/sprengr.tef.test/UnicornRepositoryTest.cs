using NUnit.Framework;
using sprengr.tef.test.Data;
using sprengr.tef.test.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace sprengr.tef.test
{
    [TestFixture]
    public class UnicornRepositoryTest
    {
        [Test]
        public void GetSortedByName()
        {
            var dataModel = new TefContext<UnicornContext>();

            dataModel.AddSet(new[] {
                new Unicorn() { Name = "Nightshade Lovely Reins", Id = 0 },
                new Unicorn() { Name = "Poppy Dainty Hooves", Id = 1 },
                new Unicorn() { Name = "Marigold Misty Lashes", Id = 2 }
            });

            var repo = new UnicornRepository(dataModel.GetDataModel);
            var sorted = repo.GetSortedByName().Select(u => u.Id).ToArray();
            sorted.Should().BeEquivalentTo(new int[] { 2, 0, 1 });
        }
    }
}
