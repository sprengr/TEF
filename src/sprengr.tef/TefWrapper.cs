using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sprengr.Tef
{
    public class TefWrapper<TDb> where TDb : DbContext, new()
    {
        private readonly Dictionary<Type, dynamic> _inMemoryDbSets;

        public TefWrapper(TDb dbContext)
        {
            _inMemoryDbSets = new Dictionary<Type, dynamic>();

            //TODO: Initialize with values from dbContext parm
            InitializeSets();
        }

        private void InitializeSets()
        {
            var dbSetProperties = GetDbSetPropertyInfos();

            foreach (var dbSetProperty in dbSetProperties)
            {
                var entityType = dbSetProperty.PropertyType.GetGenericArguments().First();
                var emptyDbSet = CreateEmptyDbSet(entityType);
                _inMemoryDbSets[entityType] = emptyDbSet;
            }
        }

        private IEnumerable<PropertyInfo> GetDbSetPropertyInfos()
        {
            var properties = (typeof (TDb)).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var dbSetProperties = properties.Where(pi => pi.PropertyType.IsGenericType
                                                      && GetPropertyName(pi).Equals("DbSet`1"))
                                            .AsEnumerable();
            return dbSetProperties;
        }
        private string GetPropertyName(PropertyInfo pi)
        {
            return pi.PropertyType.GetGenericTypeDefinition().Name;
        }

        private dynamic CreateEmptyDbSet(Type entityType)
        {
            var dbSetType = typeof (InMemoryDbSet<>);
            var dbSetGenericType = dbSetType.MakeGenericType(entityType);
            dynamic emptyDbSet = Activator.CreateInstance(dbSetGenericType);
            return emptyDbSet;
        }

        public IDbSet<T> AddSetList<T>(IEnumerable<T> entities)
            where T : class
        {
            var dbSet = new InMemoryDbSet<T>();
            dbSet.AddRange(entities);
            return AddSet(dbSet);
        }

        public IDbSet<TEntity> AddSet<TEntity>(params TEntity[] entities)
            where TEntity : class
        {
            var dbSet = new InMemoryDbSet<TEntity>();
            if (entities.Length == 1 && entities[0] is IEnumerable)
            {
                //TODO: Some casting magic possible?
                throw new InvalidOperationException("Please use arrays, as the type inference can't distinguish \"params\" from enumerables");
            }
            dbSet.AddRange(entities);
            return AddSet(dbSet);
        }

        public IDbSet<TEntity> AddSet<TEntity>(InMemoryDbSet<TEntity> dbSet)
            where TEntity : class
        {
            _inMemoryDbSets[typeof (TEntity)] = dbSet;

            return dbSet;
        }

        public TDb GetDb()
        {
            var dbContext = new TDb();

            InitializeWithInMemoryDbSets(dbContext);

            return dbContext;
        }

        private void InitializeWithInMemoryDbSets(TDb dbContext)
        {
            foreach (var setType in _inMemoryDbSets.Keys)
            {
                var name = GetPropertNameByType(typeof (TDb), setType);
                SetPropertyValue(dbContext, name, _inMemoryDbSets[setType]);
            }
        }

        private string GetPropertNameByType(Type dbContextType, Type dbSetType)
        {
            var properties = GetDbSetPropertyInfos(dbContextType, dbSetType);
            if (!properties.Any())
            {
                throw new InvalidOperationException(string.Format("Type {0} has no property of type {1}.", dbContextType.FullName, dbSetType.FullName));
            }
            return properties.First().Name;
        }

        private PropertyInfo[] GetDbSetPropertyInfos(IReflect dbType, Type setType)
        {
            return dbType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(pi => pi.PropertyType.GenericTypeArguments.Contains(setType)).ToArray();
        }

        private void SetPropertyValue(object propertyContainer, string propertyName, object value)
        {
            var a = propertyContainer.GetType().GetProperty(propertyName);
            a.SetValue(propertyContainer, value);
        }

        public DbSet<TSet> Set<TSet>() where TSet : class
        {
            return _inMemoryDbSets[typeof(TSet)];
        }
    }

}
