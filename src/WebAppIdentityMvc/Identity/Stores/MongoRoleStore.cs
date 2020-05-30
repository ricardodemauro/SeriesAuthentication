using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebAppIdentityMvc.Identity.Stores
{
    //based on https://github.com/g0t4/aspnet-identity-mongo/
    public class MongoRoleStore<TRole, TKey> : IRoleStore<TRole>, IQueryableRoleStore<TRole>
        where TRole : IdentityRole<TKey>
         where TKey : IEquatable<TKey>
    {
        private readonly IMongoCollection<TRole> _Roles;

        public MongoRoleStore(MongoProxyTable proxyMongo)
        {
            _Roles = proxyMongo.GetCollection<TRole>(MongoProxyTable.TABLE_ROLES);
        }

        public virtual void Dispose()
        {
            // no need to dispose of anything, mongodb handles connection pooling automatically
        }

        public virtual async Task<IdentityResult> CreateAsync(TRole role, CancellationToken token)
        {
            await _Roles.InsertOneAsync(role, cancellationToken: token);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken token)
        {
            var result = await _Roles.ReplaceOneAsync(r => r.Id.Equals(role.Id), role, cancellationToken: token);

            // todo low priority result based on replace result
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken token)
        {
            var result = await _Roles.DeleteOneAsync(r => r.Id.Equals(role.Id), token);

            // todo low priority result based on delete result
            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.Id.ToString());

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.Name);

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        // note: can't test as of yet through integration testing because the Identity framework doesn't use this method internally anywhere
        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.NormalizedName);

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public virtual Task<TRole> FindByIdAsync(string roleId, CancellationToken token)
            => _Roles.Find(r => r.Id.Equals(roleId))
                .FirstOrDefaultAsync(token);

        public virtual Task<TRole> FindByNameAsync(string normalizedName, CancellationToken token)
            => _Roles.Find(r => r.NormalizedName == normalizedName)
                .FirstOrDefaultAsync(token);

        public virtual IQueryable<TRole> Roles
            => _Roles.AsQueryable();
    }
}
