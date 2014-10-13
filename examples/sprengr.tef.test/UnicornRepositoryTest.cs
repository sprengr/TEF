using NUnit.Framework;
using System.Linq;
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
            var dataModelMock = new UnicornContext().CreateTef();

            dataModelMock.AddSet(new[] {
                new Unicorn() { Name = "Nightshade Lovely Reins", Id = 0 },
                new Unicorn() { Name = "Poppy Dainty Hooves", Id = 1 },
                new Unicorn() { Name = "Marigold Misty Lashes", Id = 2 }
            });

            var repo = new UnicornRepository(dataModelMock.GetDb);
            var sorted = repo.GetSortedByName().Select(u => u.Id).ToArray();

            sorted.Should().BeEquivalentTo(new [] { 2, 0, 1 });
        }

        [Test]
        public void Save()
        {
            var dataModelMock = new UnicornContext().CreateTef();
            var repo = new UnicornRepository(dataModelMock.GetDb);
            var unicorn = new Unicorn()
            {
                Id = 1,
                Name = "Rose Misty Horse",
                IsAlive = true,
                Type = new UnicornType()
                {
                    Id = 0,
                    Name = "Pegacorn",
                    IsExtinct = true
                }
            };

            dataModelMock.AddSet(unicorn.Type);
            repo.Save(unicorn);
            //TODO: nicer syntax
            dataModelMock.Set<Unicorn>().First().ShouldBeEquivalentTo(new { IsAlive = false }, o => o.ExcludingMissingProperties());
        }

    }
}
