using global::MongoDB.Bson;
using global::MongoDB.Driver;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebAppIdentityMvc.Identity.Stores
{
    //based on https://github.com/g0t4/aspnet-identity-mongo/
    public class MongoUserStore<TUser> :
                IUserPasswordStore<TUser>,
                IUserStore<TUser>,
                IUserRoleStore<TUser>
            where TUser : IdentityUser
    {
        private readonly IMongoCollection<TUser> _users;
        private readonly IMongoCollection<IdentityUserRole<string>> _userRoles;

        public MongoUserStore(MongoProxyTable proxyMongo)
        {
            _users = proxyMongo.GetCollection<TUser>(MongoProxyTable.TABLE_USERS);
            _userRoles = proxyMongo.GetCollection<IdentityUserRole<string>>(MongoProxyTable.TABLE_USER_ROLES);
        }

        public virtual void Dispose()
        {
        }

        public virtual async Task<IdentityResult> CreateAsync(TUser user, CancellationToken token)
        {
            await _users.InsertOneAsync(user, cancellationToken: token);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken token)
        {
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: token);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken token)
        {
            await _users.DeleteOneAsync(u => u.Id == user.Id, token);
            return IdentityResult.Success;
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedUserName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedUserName;
            return Task.CompletedTask;
        }

        public virtual Task<TUser> FindByIdAsync(string userId, CancellationToken token)
            => IsObjectId(userId)
                ? _users.Find(u => u.Id == userId).FirstOrDefaultAsync(token)
                : Task.FromResult<TUser>(null);

        private bool IsObjectId(string id)
        {
            ObjectId temp;
            return ObjectId.TryParse(id, out temp);
        }

        public virtual Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken token)
            => _users.Find(u => u.NormalizedUserName == normalizedUserName).FirstOrDefaultAsync(token);

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken token)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken token) => Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            var userRole = new IdentityUserRole<string>()
            {
                RoleId = roleName,
                UserId = user.Id
            };
            await _userRoles.InsertOneAsync(userRole, cancellationToken: cancellationToken);
        }

        public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            await _userRoles.DeleteOneAsync(u => u.UserId == user.Id && u.RoleId == roleName, cancellationToken: cancellationToken);
        }

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            var userRoles = await _userRoles.Find(x => x.UserId == user.Id)
                .ToListAsync(cancellationToken);

            return userRoles?.Select(x => x.RoleId).ToList() ?? Array.Empty<string>().ToList();
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            var roles = await GetRolesAsync(user, cancellationToken);
            return roles.Contains(roleName);
        }

        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
