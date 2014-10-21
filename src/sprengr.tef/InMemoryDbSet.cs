using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sprengr.Tef
{
    /// <summary>
    /// Mockable DbSet. Idea from:
    /// https://stackoverflow.com/questions/11787968/unit-testing-entity-framework-with-mock-idbset
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InMemoryDbSet<T> : DbSet<T>, IEnumerable<T>, IQueryable<T> where T : class
    {
        private readonly HashSet<T> _data;
        private readonly IQueryable _query;

        public InMemoryDbSet()
        {
            _data = new HashSet<T>();
            _query = _data.AsQueryable();
        }

        public override T Add(T entity)
        {
            _data.Add(entity);
            return entity;
        }

        public override IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
            return entities;
        }

        public override T Attach(T entity)
        {
            _data.Add(entity);
            return entity;
        }

        public new TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            throw new NotImplementedException();
        }

        public new T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public new virtual T Find(params object[] keyValues)
        {
            throw new NotImplementedException("Derive from InMemoryDbSet and override Find");
        }

        public new System.Collections.ObjectModel.ObservableCollection<T> Local
        {
            get { return new System.Collections.ObjectModel.ObservableCollection<T>(_data); }
        }

        public new T Remove(T entity)
        {
            _data.Remove(entity);
            return entity;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public Type ElementType
        {
            get { return _query.ElementType; }
        }

        public Expression Expression
        {
            get { return _query.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _query.Provider; }
        }
    }

}
