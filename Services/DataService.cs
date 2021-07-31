using System;
using System.Linq;
using System.Threading.Tasks;
using BlogProject.Data;
using BlogProject.Enums;
using BlogProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;
        private readonly ISlugService _slugService;

        public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager, ISlugService slugService)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _slugService = slugService;
        }

        public async Task ManageDataAsync()
        {
            await _dbContext.Database.MigrateAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            if (_dbContext.Roles.Any()) return;
            
            foreach (var role in Enum.GetNames(typeof(BlogRole)))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        
        private async Task SeedUsersAsync()
        {
            if (_dbContext.Users.Any()) return;

            var adminUser = new BlogUser
            {
                FirstName = "Team",
                LastName = "Maxirose",
                DisplayName = "The Team",
                Email = "maxirose@mailinator.com",
                UserName = "maxirose@mailinator.com",
                PhoneNumber = "(555) 555-5555",
                EmailConfirmed = true
            };

            await SeedBlogsAsync(adminUser);
            await _userManager.CreateAsync(adminUser, "Abc123!");
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            var modUser = new BlogUser
            {
                FirstName = "Macey",
                LastName = "LaRose",
                DisplayName = "The Owner",
                Email = "Maceylarose@mailinator.com",
                UserName = "Maceylarose@mailinator.com",
                PhoneNumber = "(989) 777-7777",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(modUser, "Abc123!");
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());

        }
        
        private async Task SeedBlogsAsync( BlogUser defaultUser )
        {
            if (_dbContext.Blogs.Any()) return;
            if( _dbContext.Posts.Any() ) return;
            if (_dbContext.Tags.Any()) return;


            var defaultBlog = new Blog
            {
                BlogUser = defaultUser,
                BlogUserId = defaultUser.Id,
                Created = DateTime.Now,
                Name = "Default Blog",
                Description = "The default blog build.",
            };

            var defaultPost = new Post
            {
                Blog = defaultBlog,
                BlogId = defaultBlog.Id,
                BlogUser = defaultUser,
                BlogUserId = defaultUser.Id,
                Title = "Default Post",
                Abstract = "This is the default blog post.",
                Content = "This is the content of the default blog post.",
                Created = DateTime.Now,
                ReadyStatus = ReadyStatus.ProductionReady,
                Slug = _slugService.URLFriendly( "Default Post ")
            };

            var defaultComment = new Comment()
            {
                Post = defaultPost,
                PostId = defaultPost.Id,
                Body = "This is the first default comment!",
                BlogUser = defaultUser,
                BlogUserId = defaultUser.Id,
                Created = DateTime.Now,
            };

            var defaultTag = new Tag()
            {
                PostId = defaultPost.Id,
                Post = defaultPost,
                Text = "Hair"
            };
            
            await _dbContext.AddAsync(defaultBlog);
            await _dbContext.AddAsync(defaultPost);
            await _dbContext.AddAsync(defaultTag);
            await _dbContext.AddAsync(defaultComment);
            
            defaultBlog.Posts.Add( defaultPost );
            //defaultBlog.Tags.Add( defaultTag );
            defaultPost.Tags.Add( defaultTag );
            defaultPost.Comments.Add(defaultComment);
        }
    }
}