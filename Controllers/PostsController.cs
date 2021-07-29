using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogProject.Data;
using BlogProject.Enums;
using BlogProject.Models;
using BlogProject.Services;
using BlogProject.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using X.PagedList;

namespace BlogProject.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;
        private readonly BlogSearchService _blogSearchService;
        private readonly UserManager<BlogUser> _userManager;
        private const int ItemsPerPage = 5;


        public PostsController(ApplicationDbContext context, ISlugService slugService, IImageService imageService, UserManager<BlogUser> userManager, BlogSearchService blogSearchService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _userManager = userManager;
            _blogSearchService = blogSearchService;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Blog).Include(p => p.BlogUser);
            return View(await applicationDbContext.ToListAsync());
        }
        
        public async Task<IActionResult> BlogPostIndex( int? id, int? page )
        {
            if (id is null)
                return NotFound();

            var pageNumber = page ?? 1;
            
            var posts = await _context.Posts
                .Where(p => p.BlogId == id && p.ReadyStatus == ReadyStatus.ProductionReady )
                .OrderByDescending( p=> p.Created )
                .ToPagedListAsync( pageNumber, ItemsPerPage );

            return View( posts );
        }

        public async Task<IActionResult> TagIndex(string tag)
        {
            // Get all posts that contain this tag
            var allPostIds = _context.Tags.Where(t => t.Text == tag).Select(t => t.PostId);
            var posts = await _context.Posts.Where(p => allPostIds.Contains(p.Id)).ToListAsync();
            return View("Index", posts);
        }

        public async Task<IActionResult> SearchIndex( int? page, string searchTerm )
        {
            ViewData["SearchTerm"] = searchTerm;
            var pageNumber = page ?? 1;

            var posts = _blogSearchService.Search(searchTerm);

            return View( await posts.ToPagedListAsync( pageNumber, ItemsPerPage ));
        }

        public async Task<IActionResult> Details(string slug)
        {
            ViewData["Title"] = "Post Details Page";
            if (string.IsNullOrEmpty(slug)) return NotFound();

            var post = await _context.Posts
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .ThenInclude(c => c.BlogUser)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Moderator)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null) return NotFound();

            var dataViewModel = new PostDetailViewModel
            {
                Post = post,
                Tags = _context.Tags.Select(t => t.Text.ToLower()).Distinct().ToList()
            };

            ViewData["HeaderImage"] = _imageService.DecodeImage(post.ImageData, post.ContentType);
            ViewData["MainText"] = post.Title;
            ViewData["SubText"] = post.Abstract;
            return View(dataViewModel);
        }
        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            ViewData["BlogUserId"] = new SelectList(_context.Set<BlogUser>(), "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image")]
            Post post, List<string> tagValues)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.Now;

                var authorId = _userManager.GetUserId(User);
                post.BlogUserId = authorId;

                // Use the _imageService to store the incoming user-specified image
                post.ImageData = await _imageService.EncodeImageAsync(post.Image);
                post.ContentType = _imageService.ContentType(post.Image);

                // Create the slug and determine if it is unique
                var slug = _slugService.URLFriendly(post.Title);

                // Create a variable to store whether an error has occurred.
                var validationError = false;

                if (string.IsNullOrEmpty(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("", "The title you provided cannot be used as it results in an empty slug.");
                }
                // Detect incoming duplicate Slugs
                else if (!_slugService.IsUnique(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("", "The title you provided cannot be used as it results in a duplicate slug.");
                }
                else if (slug.Contains("test"))
                {
                    validationError = true;
                    ModelState.AddModelError("Title", "The slug you provided cannot contain the word 'test'.");
                }

                if (validationError)
                {
                    ViewData["TagValues"] = string.Join(",", tagValues);
                    return View(post);
                }

                post.Slug = slug;

                _context.Add(post);
                await _context.SaveChangesAsync();

                foreach (var tagText in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        BlogUserId = authorId,
                        Text = tagText,
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadyStatus")]
            Post post, IFormFile newImage, List<string> tagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Create a copy of the original post for validation
                    var newPost = await _context.Posts
                        .Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == post.Id);

                    newPost.Updated = DateTime.Now;
                    newPost.Title = post.Abstract;
                    newPost.Abstract = post.Abstract;
                    newPost.Content = post.Content;
                    newPost.ReadyStatus = post.ReadyStatus;

                    var newSlug = _slugService.URLFriendly(newPost.Title);
                    if (newSlug != newPost.Slug)
                    {
                        if (_slugService.IsUnique(newSlug))
                        {
                            newPost.Title = post.Title;
                            newPost.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "This title cannot be used as it results in a duplicate slug.");
                            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
                            return View(post);
                        }
                    }

                    if (newImage is not null)
                    {
                        newPost.ImageData = await _imageService.EncodeImageAsync(newImage);
                        newPost.ContentType = _imageService.ContentType(newImage);
                    }

                    // Remove all tags previously associated with this post
                    _context.Tags.RemoveRange(newPost.Tags);

                    // Add in new tags from the edit form
                    foreach (var tagText in tagValues)
                    {
                        _context.Add(new Tag()
                        {
                            PostId = post.Id,
                            BlogUserId = newPost.BlogUserId,
                            Text = tagText,
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["BlogUserId"] = new SelectList(_context.Set<BlogUser>(), "Id", "Id", post.BlogUserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        //    public IActionResult TagIndex( List<string> tagValues )
        //      {
//            var post = _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(t => t.Tags == tagValues);

        //return View(post);
        //var tag = await _context.Posts.Where(p => p.Tags).Contains( );
        //return View(tag);
        //}
    }
}