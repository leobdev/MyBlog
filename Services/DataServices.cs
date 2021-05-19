using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyBlog.Data;
using MyBlog.Enums;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Services
{

    public class DataServices
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBlogImageService _imageService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<BlogUser> _userManager;

        public DataServices(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, IBlogImageService imageService, IConfiguration configuration, UserManager<BlogUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _imageService = imageService;
            _configuration = configuration;
            _userManager = userManager;
        }

       

        public async Task ManageDataAsync()
        {
            //Task 0: Make sure the DB is present by running through the migrations
            await _context.Database.MigrateAsync();

            //Task 1: Seed Roles - Creatuing Roles and entering them into the system
            await SeedRolesAsync();

            //task 2: Seed a few users in the system (AspNetUsers)

            await SeedUsersAsync();
        }


        private async Task SeedRolesAsync()
        {
            //Are there any roles already in the system
            if (_context.Roles.Any())
                return;

            //Spin through an enum and do stuff
            foreach (var role in Enum.GetNames(typeof(BlogRole)))
            {               
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsersAsync()
        {
            if (_context.Users.Any())
            {
                return;
            }

            var adminUser = new BlogUser()
            {
                Email = "LeoB@Mailinator.com",
                UserName = "LeoB@Mailinator.com",
                FirstName = "Leo",
                LastName = "Ba",
                DisplayName = "Account Info",
                PhoneNumber = "555-1212",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeFileAsync("leob.png"),
                ContentType = "png"

            };
            

            await _userManager.CreateAsync(adminUser, _configuration["AdminPassword"]);

            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

        }
    }
}

   