Testable Entity Framework
=========================

Easy and straightforward way to test EntityFramework queries.

##Example
Add "using" to your test class

    using Sprengr.Tef;

Now write your tests starting with following extension method

	var testableContext = new UnicornContext().CreateTef();

Add your test data

    testableContext.AddSet(new[] {
        new Unicorn() { Name = "Nightshade Lovely Reins", Id = 0 },
        new Unicorn() { Name = "Poppy Dainty Hooves", Id = 1 },
        new Unicorn() { Name = "Marigold Misty Lashes", Id = 2 }
    });

Write queries as you would against a db

    List<Unicorn> sortedUnicorns;
    using (var db = testableContext.GetDb())
    {
        sortedUnicorns = (from b in db.Unicorns
                          orderby b.Name
                          select b).ToList();
    }

Test the result

    var unicornIDs = sortedUnicorns.Select(u => u.Id).ToArray();
    unicornIDs.Should().BeEquivalentTo(new [] { 2, 0, 1 });

For should assertion style you can use the excellent [FluentAssertion](https://github.com/dennisdoomen/fluentassertions) library.

Find more examples [here](examples/sprengr.tef.test/UnicornRepositoryTest.cs)

##Installation
use NuGet:

    Install-Package sprengr.tef