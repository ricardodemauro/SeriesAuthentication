﻿using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebAppIdentityMvc.Identity.Stores
{
    public class MongoUserStore<TUser, TKey> : IUserStore<TUser>,
                                               IUserPasswordStore<TUser>,
                                               IUserRoleStore<TUser>,
                                               IUserLoginStore<TUser>
        where TUser : IdentityUser<TKey>, IIdentityUserRole, IIdentityUserLogin
        where TKey : IEquatable<TKey>
    {
        private readonly IMongoCollection<TUser> _users;

        public MongoUserStore(MongoTablesFactory proxyTables)
        {
            _users = proxyTables.GetCollection<TUser>(MongoTablesFactory.TABLE_USERS);
        }

        public virtual void Dispose()
        {
            //do nothing here
        }

        public virtual async Task<IdentityResult> CreateAsync(TUser user, CancellationToken token)
        {
            await _users.InsertOneAsync(user, cancellationToken: token);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken token)
        {
            await _users.ReplaceOneAsync(u => u.Id.Equals(user.Id), user, cancellationToken: token);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken token)
        {
            await _users.DeleteOneAsync(u => u.Id.Equals(user.Id), token);
            return IdentityResult.Success;
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Id.ToString());

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.UserName);

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedUserName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedUserName;
            return Task.CompletedTask;
        }


        public virtual Task<TUser> FindByIdAsync(string userId, CancellationToken token)
            => IsObjectId(userId)
                ? _users.Find(u => u.Id.Equals(userId)).FirstOrDefaultAsync(token)
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

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken token)
            => Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
            => Task.FromResult(false);

        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            user.AddRole(roleName);
            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            user.RemoveRole(roleName);
            return Task.CompletedTask;
        }

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            var roles = user.Roles?.ToArray() ?? Array.Empty<string>();
            return await Task.FromResult(roles);
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

        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            user.AddUserLogin(login);
            return Task.CompletedTask;
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            user.RemoveUserLogin(loginProvider, providerKey);
            return Task.CompletedTask;
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.UserLogins);
        }

        public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return await Task.FromResult(default(TUser));
        }
    }
}
