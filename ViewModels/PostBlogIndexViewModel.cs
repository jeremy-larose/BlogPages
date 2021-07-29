using System.Linq;
using BlogProject.Models;
using X.PagedList;

namespace BlogProject.ViewModels
{
    public class PostBlogIndexViewModel
    {
        public IPagedList<Post> Posts { get; set; }
        public IQueryable<Blog> Blog { get; set; }
    }
}