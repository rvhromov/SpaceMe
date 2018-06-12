using SpaceMe.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SpaceMe.Repository
{
    public class PostRepository : IRepository
    {
        private ApplicationDbContext _db;
        public PostRepository()
        {
            _db = new ApplicationDbContext();
        }

        public IEnumerable<Post> GetPostList()
        {
            IEnumerable<Post> posts = new List<Post>();
            posts = _db.Posts;
            return posts;
        }

        public async Task<Post> GetPost(int id)
        {
            return await _db.Posts.FindAsync(id);
        }

        public async Task<int> Create(Post post)
        {
            _db.Posts.Add(post);
            return await _db.SaveChangesAsync();
        }

        public async Task<int> Update(Post post, bool updateImage)
        {
            _db.Entry(post).State = EntityState.Modified;
            _db.Entry(post).Property(p => p.Image).IsModified = updateImage;
            return await _db.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            Post post = await _db.Posts.FindAsync(id);
            if (post == null)
                return false;

            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();
            return true;
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}