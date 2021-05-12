using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBlog.Data;
using MyBlog.Enums;


namespace MyBlog.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;

        public  SearchService(ApplicationDbContext context) 
        {
            _context = context;
        }

        public IQueryable<Post>SearchContent(string searchString) 
        {
            var result = _context.Post.Where(p => p.PublishState == Enums.PublishState.ProductionReady);

            if (string.IsNullOrEmpty(searchString))
            {
                result = result.Where(p => p.Title.Contains(searchString) ||
                                      p.Abstract.Contains(searchString) ||
                                      p.Content.Contains(searchString) ||
                                      p.Comments.Any(c => c.Body.Contains(searchString) ||
                                                          c.ModeratedBody.Contains(searchString) ||
                                                          c.Author.FullName.Contains(searchString)));
            }
        
            return result.OrderByDescending(p => p.Created);
        }
    }
}
