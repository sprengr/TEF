using System.Data.Entity;

namespace Sprengr.Tef
{
    public static class TestableDbContext
    {
        public static TefWrapper<TDb> CreateTef<TDb>(this TDb dbContext) where TDb : DbContext, new()
        {
            return new TefWrapper<TDb>(dbContext);
        } 
    }
}
