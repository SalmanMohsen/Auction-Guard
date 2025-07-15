using AuctionGuard.Domain.Interfaces;
using AuctionGuard.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuctionGuardDbContext _appContext;
        private readonly AuctionGuardIdentityDbContext _identityContext;
        private Hashtable _repositories;
        private IDbContextTransaction _transaction;

        public UnitOfWork(AuctionGuardDbContext appContext, AuctionGuardIdentityDbContext identityContext)
        {
            _appContext = appContext;
            _identityContext = identityContext;
        }

        public async Task<int> CommitAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _appContext.Database.BeginTransactionAsync();
                await _identityContext.Database.UseTransactionAsync(_transaction.GetDbTransaction());
            }

            try
            {
                var appChanges = await _appContext.SaveChangesAsync();
                var identityChanges = await _identityContext.SaveChangesAsync();

                await _transaction.CommitAsync();

                return appChanges + identityChanges;
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                object repositoryInstance;
                // Check if the entity belongs to the Identity context
                if (typeof(TEntity) == typeof(Domain.Entities.User) || typeof(TEntity) == typeof(Domain.Entities.Role) || typeof(TEntity) == typeof(Domain.Entities.Permission))
                {
                    repositoryInstance = new IdentityGenericRepository<TEntity>(_identityContext);
                }
                else
                {
                    repositoryInstance = new AppGenericRepository<TEntity>(_appContext);
                }
                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<TEntity>)_repositories[type];
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _appContext.Dispose();
            _identityContext.Dispose();
        }
    }
}
