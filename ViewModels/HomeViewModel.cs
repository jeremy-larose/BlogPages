using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogProject.Data;
using BlogProject.Models;
using X.PagedList;

namespace BlogProject.ViewModels
{
    public class HomeViewModel
    {
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public IPagedList<Blog> Blogs { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}