using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sprengr.Tef
{
    public class TefContext<T> where T : DbContext
    {
        private Mock<T> _dbContext;

        public TefContext()
        {
            _dbContext = new Mock<T>();
            InitializeSets<T>();
        }

        private void InitializeSets<TDb>()
        {
            //TODO: refactor, cleanup
            var p = (typeof(TDb)).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition().Name.Equals("DbSet`1")).AsEnumerable();

            foreach (var pp in p)
            {
                var propertyExpression = GenerateExpressionForSetProperty(pp.Name);
                var setType = typeof(InMemoryDbSet<>);
                var entityType = pp.PropertyType.GetGenericArguments().First();
                var emptySet = Activator.CreateInstance(setType.MakeGenericType(entityType));
                _dbContext.Setup(propertyExpression).Returns(emptySet);
                //TODO: Generic set
                //_dbContext.Setup(d => d.Set<S>()).Returns(emptySet);   
            }
        }

        public IDbSet<T> AddSetList<T>(IEnumerable<T> entities)
            where T : class
        {
            var dbSet = new InMemoryDbSet<T>();
            dbSet.AddRange(entities);
            return AddSet(dbSet);
        }

        public IDbSet<S> AddSet<S>(params S[] entities)
            where S : class
        {
            var dbSet = new InMemoryDbSet<S>();
            if (entities.Length == 1 && entities[0] is IEnumerable)
            {
                //TODO: Some casting magic possible?
                throw new InvalidOperationException("Please use arrays, as the type inference can't distinguish \"params\" from enumerables");
            }
            dbSet.AddRange(entities);
            return AddSet(dbSet);
        }

        public IDbSet<S> AddSet<S>(InMemoryDbSet<S> dbSet)
            where S : class
        {
            var propertyExpression = GenerateExpressionForSetProperty<S>();
            AddExistingSet<S>(dbSet);

            _dbContext.Setup(propertyExpression).Returns(dbSet);
            _dbContext.Setup(d => d.Set<S>()).Returns(dbSet);
            return dbSet;
        }

        private Expression<Func<T, object>> GenerateExpressionForSetProperty<S>() where S : class
        {
            string propertNameInDataModel = GetPropertNameByType<InMemoryDbSet<S>>(typeof(T));
            return GenerateExpressionForSetProperty(propertNameInDataModel);
        }

        private Expression<Func<T, object>> GenerateExpressionForSetProperty(string propertyName) 
        {
            var propertyContainer = Expression.Parameter(typeof(T));
            var property = Expression.PropertyOrField(propertyContainer, propertyName);
            var propertyExpression = Expression.Lambda<Func<T, object>>(property, propertyContainer);
            return propertyExpression;
        }

        private void AddExistingSet<S>(InMemoryDbSet<S> dbSet) where S : class
        {
            var existingSet = _dbContext.Object.Set<S>();
            if (existingSet != null)
            {
                dbSet.AddRange(existingSet);
            }
        }

        private string GetPropertNameByType<S>(Type type)
        {
            //TODO: Meldung wenn Typ nicht gefunden
            var properties = GetSetPropertyInfos<S>(type);
            if (properties.Count() < 1)
            {
                throw new InvalidOperationException(string.Format("Type {0} has no property of type {1}.", type.FullName, typeof(S).FullName));
            }
            return properties.First().Name;
        }

        private PropertyInfo[] GetSetPropertyInfos<TSet>(Type dbType)
        {
            return dbType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(pi => pi.PropertyType == typeof(TSet) || pi.PropertyType == typeof(TSet).BaseType).ToArray();
        }

        public T GetDataModel()
        {
            return _dbContext.Object;
        }
    }

}
