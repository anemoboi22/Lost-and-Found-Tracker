using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test.Models;

namespace Test.Services
{
    public class DatabaseHelper
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseHelper(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<User>().Wait();
            _database.CreateTableAsync<Post>().Wait();
        }

        public Task<List<User>> GetUsersAsync()
        {
            return _database.Table<User>().ToListAsync();
        }
        public Task<User> GetUserAsync(int id)
        {
            return _database.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public Task<User> GetUserAsync(string username)
        {
            return _database.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();
        }

        public Task<int> SaveUserAsync(User user)
        {
            if (user.Id != 0)
            {
                return _database.UpdateAsync(user);
            }
            else
            {
                return _database.InsertAsync(user);
            }
        }

        public Task<int> DeleteUserAsync(User user)
        {
            return _database.DeleteAsync(user);
        }

        public Task<int> SavePostAsync(Post post)
        {
            if (post.Id != 0)
            {
                return _database.UpdateAsync(post);
            }
            else
            {
                return _database.InsertAsync(post);
            }
        }

        public Task<int> DeletePostAsync(Post post)
        {
            return _database.DeleteAsync(post);
        }

        public Task<List<Post>> GetPostsAsync()
        {
            return _database.Table<Post>().ToListAsync();
        }

    }
}
