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
    }
}