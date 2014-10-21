using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Sprengr.Tef;
using Sprengr.Tef.Test.Data;

namespace sprengr.tef.test
{
    public class TefWrapperTest
    {

        [Test]
        public void ItShouldRetainDbOverMultipleContexts()
        {
            var dataModelMock = new SimpleTestContext().CreateTef();

            using (var db = dataModelMock.GetDb())
            {
                db.Entity.Add(new SimpleEntityWithId {Id = 1});
            }
            using (var db = dataModelMock.GetDb())
            {
                db.Entity.Add(new SimpleEntityWithId {Id = 2});
            }

            dataModelMock.Set<SimpleEntityWithId>().Select(u => u.Id).ShouldBeEquivalentTo(new[] { 1, 2 });
        }

        [Test]
        public void ItShouldCreateEmptyListsForAllEntites()
        {
            var simpleTestContextTef = new SimpleTestContext().CreateTef();
            simpleTestContextTef.Set<SimpleEntityWithId>().Should().BeEmpty();

            var contextTwoEntitiesTef = new ContextTwoEntities().CreateTef();
            contextTwoEntitiesTef.Set<TestEntityWithId>().Should().BeEmpty();
            contextTwoEntitiesTef.Set<TestEntityWithIdAndName>().Should().BeEmpty();
        }

        [Test]
        public void ItShouldAddEntitiesWhenAttachIsCalled()
        {
            var simpleTestContextTef = new SimpleTestContext().CreateTef();
            using (var db = simpleTestContextTef.GetDb())
            {
                db.Entity.Count().Should().Be(0);
                db.Entity.Attach(new SimpleEntityWithId {Id = 3});
                db.Entity.Count().Should().BeGreaterThan(0);
            }
        }

        [Test]
        public void ItShouldThrowIfAddingNonSets()
        {
            var simpleTestContextTef = new SimpleTestContext().CreateTef();
            simpleTestContextTef.Invoking(t => t.AddSet("addingAString")).ShouldThrow<Exception>();
        }
    }
}
