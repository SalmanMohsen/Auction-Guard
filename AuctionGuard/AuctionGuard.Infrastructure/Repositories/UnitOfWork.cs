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
using System.Transactions;

namespace AuctionGuard.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuctionGuardDbContext _appContext;
        private readonly AuctionGuardIdentityDbContext _identityContext;
        private Hashtable _repositories;

        public UnitOfWork(AuctionGuardDbContext appContext, AuctionGuardIdentityDbContext identityContext)
        {
            _appContext = appContext;
            _identityContext = identityContext;
        }

        public async Task<int> CommitAsync()
        {
            // Use TransactionScope to handle distributed transactions across two DbContexts.
            // This ensures that both SaveChanges calls either succeed together or fail together.
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Attempt to save changes in the first context
                    var appChanges = await _appContext.SaveChangesAsync();

                    // Attempt to save changes in the second context
                    var identityChanges = await _identityContext.SaveChangesAsync();

                    // If both saves are successful, complete the scope to commit the transaction.
                    scope.Complete();

                    return appChanges + identityChanges;
                }
                catch (Exception)
                {
                    // If an exception occurs in either SaveChangesAsync, scope.Complete() is not
                    // called, and the transaction is automatically rolled back when the scope is disposed.
                    throw; // Re-throw the exception to be handled by the service layer.
                }
            }
        }

        public Task RollbackAsync()
        {
            // This method is now effectively handled by the TransactionScope's implicit rollback on error.
            // It can be left empty as it's part of the interface contract.
            return Task.CompletedTask;
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
                if (typeof(TEntity) == typeof(Domain.Entities.User) ||
                    typeof(TEntity) == typeof(Domain.Entities.Role) ||
                    typeof(TEntity) == typeof(Domain.Entities.Permission))
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
            _appContext.Dispose();
            _identityContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
