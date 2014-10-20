using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sprengr.Tef
{
    public class TefWrapper<T> where T : DbContext, new()
    {
        private T _dbContext;
        private readonly Dictionary<Type, dynamic> _inMemoryDbSets;

        public TefWrapper(T dbContext)
        {
            _inMemoryDbSets = new Dictionary<Type, dynamic>();
            _dbContext = dbContext;

            InitializeSets<T>();
        }

        public void InitializeSets<TDb>() where TDb : class 
        {
            var properties = (typeof (TDb)).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var dbSetProperties = properties.Where(pi => pi.PropertyType.IsGenericType 
                                                      && GetPropertyName(pi).Equals("DbSet`1"))
                                            .AsEnumerable();

            foreach (var dbSetProperty in dbSetProperties)
            {
                var dbSetType = typeof(InMemoryDbSet<>);
                var entityType = dbSetProperty.PropertyType.GetGenericArguments().First();
                var dbSetGenericType = dbSetType.MakeGenericType(entityType);
                dynamic emptyDbSet = Activator.CreateInstance(dbSetGenericType);
                //SetPropertyValue(dbSetProperty.Name, emptyDbSet);
                _inMemoryDbSets[entityType] = emptyDbSet;
            }
        }

        private static string GetPropertyName(PropertyInfo pi)
        {
            return pi.PropertyType.GetGenericTypeDefinition().Name;
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
            _inMemoryDbSets[typeof (S)] = dbSet;

            return dbSet;
        }

        private void SetPropertyValue(string propertyName, object emptySet)
        {
            var a = _dbContext.GetType().GetProperty(propertyName);
            a.SetValue(_dbContext, emptySet);
        }

        private string GetPropertNameByType(Type dbType, Type setType)
        {
            var properties = GetSetPropertyInfos(dbType, setType);
            if (!properties.Any())
            {
                throw new InvalidOperationException(string.Format("Type {0} has no property of type {1}.", dbType.FullName, setType.FullName));
            }
            return properties.First().Name;
        }

        private PropertyInfo[] GetSetPropertyInfos(IReflect dbType, Type setType)
        {
            return dbType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(pi => pi.PropertyType.GenericTypeArguments.Contains(setType)).ToArray();
        }

        public T GetDb()
        {
            _dbContext = new T();

            foreach (var setType in _inMemoryDbSets.Keys)
            {
                var name = GetPropertNameByType(typeof(T), setType);
                SetPropertyValue(name, _inMemoryDbSets[setType]);
            }

            return _dbContext;
        }
        
        public DbSet<TSet> Set<TSet>() where TSet : class
        {
            return _inMemoryDbSets[typeof(TSet)];
        }
    }

}
