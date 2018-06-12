using SpaceMe.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceMe.Repository
{
    public interface IRepository : IDisposable
    {
        IEnumerable<Post> GetPostList();            // Get all posts
        Task<Post> GetPost(int id);                 // Get particulat post by id
        Task<int> Create(Post post);                // Create post
        Task<int> Update(Post post, bool updImg);   // Update post and image
        Task<bool> Delete(int id);                  // Delete post by id
    }
}
