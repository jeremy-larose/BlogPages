using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BlogProject.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;

        public BlogsController(ApplicationDbContext context, IImageService imageService, UserManager<BlogUser> userManager)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Blogs.Include(b => b.BlogUser).Include(b=>b.Tags);
            var applicationDbContext = _context.Blogs.Include(b => b.BlogUser);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        [Authorize(Roles="Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Image")] Blog blog, List<string> tagValues )
        {
            var authorId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                blog.BlogUserId = authorId;
                blog.Created = DateTime.Now;
                blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                blog.ContentType = _imageService.ContentType(blog.Image);
                
                foreach (var tagText in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        Text = tagText,
                    });
                }
                
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["TagValues"] = string.Join(",", blog.Tags.Select(t => t.Text));
            ViewData["BlogUserId"] = new SelectList(_context.Set<BlogUser>(), "Id", "Id", blog.BlogUserId);
            return View(blog);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var blog = await _context.Blogs.Include(b=>b.Tags).FirstOrDefaultAsync(b=>b.Id == id );
            var blog = await _context.Blogs.FirstOrDefaultAsync(b=>b.Id == id );

            if (blog == null)
            {
                return NotFound();
            }
            //ViewData["TagValues"] = string.Join(",", blog.Tags.Select(t => t.Text));
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Blog blog, IFormFile newImage, List<string> tagValues)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //var newBlog = await _context.Blogs.Include(b=>b.Tags).FirstOrDefaultAsync(b=>b.Id == blog.Id );
                    var newBlog = await _context.Blogs.FirstOrDefaultAsync(b=>b.Id == blog.Id );

                    newBlog.Updated = DateTime.Now;
                    
                    if( newBlog.Name != blog.Name )
                        newBlog.Name = blog.Name;

                    if (newBlog.Description != blog.Description)
                        newBlog.Description = blog.Description;

                    if (newImage is not null)
                        newBlog.ImageData = await _imageService.EncodeImageAsync(newImage);
                    
                    // Remove all tags previously associated with this post
                    //_context.Tags.RemoveRange(newBlog.Tags);

                    // Add in new tags from the edit form
                    foreach (var tagText in tagValues)
                    {
                        _context.Add(new Tag()
                        {
                            Text = tagText,
                        });
                    }
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //ViewData["TagValues"] = string.Join(",", blog.Tags.Select(t => t.Text));
            ViewData["BlogUserId"] = new SelectList(_context.Set<BlogUser>(), "Id", "Id", blog.BlogUserId);
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
