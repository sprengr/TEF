using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Sprengr.Tef.Test.Data;
using Sprengr.Tef.Test.Data.Entity;

namespace Sprengr.Tef.Test
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

        [Test]
        public void Save()
        {
            var dataModel = new TefContext<UnicornContext>();
            var repo = new UnicornRepository(dataModel.GetDataModel);
            var unicorn = new Unicorn()
            {
                Name = "Rose Misty Horse",
                IsAlive = true,
                Type = new UnicornType()
                {
                    Id = 0,
                    Name = "Pegacorn",
                    IsExtinct = true
                }
            };

            dataModel.AddSet(unicorn.Type);
            repo.Save(unicorn);
            //TODO: nicer syntax
            dataModel.GetDataModel().Unicorns.First().Should().ShouldBeEquivalentTo(new { Id = 0 }, o => o.ExcludingMissingProperties());
        }

    }
}
