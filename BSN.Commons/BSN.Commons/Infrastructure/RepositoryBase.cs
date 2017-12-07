﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Commons.Infrastructure
{
	public abstract partial class RepositoryBase<T> : IRepository<T> where T : class
	{
		private DbContext _dataContext;
		protected readonly DbSet<T> dbSet;

		protected DbContext DataContext => _dataContext ?? (_dataContext = DatabaseFactory.Get());
		protected IDatabaseFactory DatabaseFactory { get; private set; }


		protected RepositoryBase(IDatabaseFactory databaseFactory)
		{
			DatabaseFactory = databaseFactory;
			dbSet = DataContext.Set<T>();
		}

		public virtual void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public virtual void AddRange(IEnumerable<T> entities)
		{
			dbSet.AddRange(entities);
		}

		public virtual void Update(T entity)
		{
			Update(entity, cfg => cfg.AutoDetectChangedProperties());
		}

		public virtual void Update(T entity, Action<IUpdateConfig<T>> configurer)
		{
			var updateConfig = new UpdateConfig();
			configurer.Invoke(updateConfig);

			if (!updateConfig.AutoDetectChangedPropertiesEnabled)
				_dataContext.Configuration.AutoDetectChangesEnabled = false;

			dbSet.Attach(entity);

			if (updateConfig.IncludeAllPropertiesEnabled)
			{
				_dataContext.Entry(entity).State = EntityState.Modified;
			}
			else if (!updateConfig.AutoDetectChangedPropertiesEnabled)
			{
				foreach (string propertyName in updateConfig.PropertyNames)
					_dataContext.Entry(entity).Property(propertyName).IsModified = true;
			}

			if (!updateConfig.AutoDetectChangedPropertiesEnabled)
				_dataContext.Configuration.AutoDetectChangesEnabled = true;
		}

		public virtual void UpdateRange(IEnumerable<T> entities)
		{
			UpdateRange(entities, cfg => cfg.IncludeAllProperties());
		}

		public virtual void UpdateRange(IEnumerable<T> entities, Action<IUpdateConfig<T>> configurer)
		{
			var updateConfig = new UpdateConfig();
			configurer.Invoke(updateConfig);

			if (!updateConfig.AutoDetectChangedPropertiesEnabled)
				_dataContext.Configuration.AutoDetectChangesEnabled = false;

			if (updateConfig.IncludeAllPropertiesEnabled)
			{
				foreach (T entity in entities)
				{
					dbSet.Attach(entity);
					_dataContext.Entry(entity).State = EntityState.Modified;
				}
			}
			else if (!updateConfig.AutoDetectChangedPropertiesEnabled)
			{
				foreach (T entity in entities)
				{
					dbSet.Attach(entity);
					foreach (string propertyName in updateConfig.PropertyNames)
						_dataContext.Entry(entity).Property(propertyName).IsModified = true;
				}
			}
			else
			{
				foreach (T entity in entities)
					dbSet.Attach(entity);
			}

			if (!updateConfig.AutoDetectChangedPropertiesEnabled)
				_dataContext.Configuration.AutoDetectChangesEnabled = true;
		}

		public virtual void Delete(T entity)
		{
			dbSet.Remove(entity);
		}

		public virtual void Delete(Expression<Func<T, bool>> where)
		{
			var objects = dbSet.Where(where).AsEnumerable();
			foreach (var obj in objects)
				dbSet.Remove(obj);
		}

		public virtual void DeleteRange(IEnumerable<T> entities)
		{
			dbSet.RemoveRange(entities);
		}

		public virtual T GetById(long id)
		{
			return dbSet.Find(id);
		}

		public virtual T GetById(string id)
		{
			return dbSet.Find(id);
		}

		public virtual IEnumerable<T> GetAll()
		{
			return dbSet.ToList();
		}

		public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
		{
			return dbSet.Where(where);
		}

		public T Get(Expression<Func<T, bool>> where)
		{
			return dbSet.Where(where).FirstOrDefault();
		}
	}
}