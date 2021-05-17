using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyBlog.Data;
using MyBlog.Models;
using MyBlog.Services;
using X.PagedList;

namespace MyBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogImageService _imageService;
        private readonly IConfiguration _configuration;
        private readonly BasicSlugService _slugService;
        private readonly SearchService _searchService;


        public PostsController(ApplicationDbContext context, IBlogImageService imageService, IConfiguration configuration, BasicSlugService slugService, SearchService searchService)
        {
            _context = context;
            _configuration = configuration;
            _imageService = imageService;
            _slugService = slugService;
            _searchService = searchService;
        }

        public async Task<ActionResult> BlogPostIndex(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = _context.Blogs.Find(id);
            var blogPost = await _context.Posts.Where(p => p.BlogId == id).ToListAsync();
            ViewData["HeaderText"] = blog.Name;
            ViewData["SubText"] = blog.Description;
            ViewData["HeaderImage"] = _imageService.DecodeImage(blog.ImageData, blog.ContentType);

            return View("Index", blogPost);
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            ViewData["HeaderText"] = "MyBlog Posts";
            ViewData["SubText"] = "All things tech-related";

            var applicationDbContext = _context.Posts.Include(p => p.Blog);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string slug)//Takes slug, not id
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)//Acts on the previous include
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (post == null)
            {
                return NotFound();
            }

            ViewData["HeaderText"] = post.Title;
            ViewData["SubText"] = post.Abstract;
            ViewData["HeaderImage"] = _imageService.DecodeImage(post.ImageData, post.ContentType);
            ViewData["Authortext"] = $"Created by leo J. Barnuevo on {post.Created.ToString("MMM dd, yyy")}";
            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create(int? id)
        {
            var model = new Post();
            if (id is not null)
            {
                model.BlogId = (int)id;
            }
            else
            {
                ViewData["BlogId"] = new SelectList(_context.Posts, "Id", "Description");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchIndex(int? page, string searchString)
        {
            ViewData["SearchString"] = searchString;
            var posts = _searchService.SearchContent(searchString);
            var pageNumber = page ?? 1;
            var pageSize = 2;

            return View(await posts.ToPagedListAsync(pageNumber, pageSize));
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BlogId,Title,Abstract,Content,PublishState,ImageFile")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.Now;

                post.ImageData = (await _imageService.EncodeFileAsync(post.ImageFile)) ??
                   await _imageService.EncodeFileAsync(_configuration["DefaultPostImage"]);

                post.ContentType = post.ImageFile is null ?
                     _configuration["DefaultPostImage"].Split('.')[1] :
                     _imageService.ContentType(post.ImageFile);

                var slug = _slugService.UrlFriendly(post.Title);
                if (!_slugService.IsUnique(slug))
                {
                    ModelState.AddModelError("Title", "There is an issue with the Tittle, please thy again.");
                    ModelState.AddModelError("", "Where does this thing show up?");
                    return View(post);
                }

                post.Slug = slug;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("BlogPostIndex", new { id = post.BlogId });
            }
            ViewData["BlogId"] = new SelectList(_context.Posts, "Id", "Description", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Posts, "Id", "Description", post.BlogId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,Created,Updated,Slug,PublishState")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newSlug = _slugService.UrlFriendly(post.Title);
                    if (post.Slug != newSlug)
                    {
                        ModelState.AddModelError("tittle", "There is an issue with the Title. Please try again.");
                        return View(post);
                    }
                    post.Slug = newSlug;

                    if (post.ImageFile is not null)
                    {
                        post.ImageData = await _imageService.EncodeFileAsync(post.ImageFile);
                        post.ContentType = _imageService.ContentType(post.ImageFile);
                    }

                    post.Updated = DateTime.Now;

                    _context.Update(post);
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
            ViewData["BlogId"] = new SelectList(_context.Posts, "Id", "Description", post.BlogId);
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
    }
}
