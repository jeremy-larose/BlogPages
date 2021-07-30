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
        private BlogUser _defaultUser;

        public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            await _dbContext.Database.MigrateAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedBlogsAsync( _defaultUser );
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

            await _userManager.CreateAsync(adminUser, "Abc123!");
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());
            _defaultUser = adminUser;

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
            var defaultBlog = new Blog()
            {
                BlogUser = defaultUser,
                BlogUserId = defaultUser.Id,
                Created = DateTime.Now,
                Description = "The default blog build.",
            };

            if( _dbContext.Posts.Any() ) return;
            var defaultPost = new Post()
            {
                Blog = defaultBlog,
                BlogUser = defaultUser,
                BlogUserId = defaultUser.Id,
                Abstract = "This is the default blog post.",
                Content = "This is the content of the default blog post.",
                Created = DateTime.Now,

            };

            if (_dbContext.Tags.Any()) return;
            var defaultTag = new Tag()
            {
                BlogUser = defaultUser,
                BlogUserId = defaultUser.Id,
                PostId = defaultPost.Id,
                Post = defaultPost,
                Text = "Hair"
            };
            
            await _dbContext.AddAsync(defaultBlog);
            await _dbContext.AddAsync(defaultPost);
            await _dbContext.AddAsync(defaultTag);
        }
    }
}