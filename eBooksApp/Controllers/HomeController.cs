using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using eBooksApp.Models;
using eBooksApp.Data;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;

namespace eBooksApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context_;
        private const string sessionId_ = "SessionId";
        private readonly IHostingEnvironment hostingEnvironment_;
        public HttpClient client { get; set; }
        private string webRootPath = null;
        private string ebookPath = null;
        private string ebookPathAdmin = null;

        private string baseUrl_ = "https://localhost:5001/api/eBooks";

        public HomeController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            context_ = context;
            client = new HttpClient();
            hostingEnvironment_ = hostingEnvironment;
            webRootPath = hostingEnvironment_.WebRootPath;
            ebookPath = Path.Combine(webRootPath, "FileStorage");
            ebookPathAdmin = Path.Combine(webRootPath, "FileStorageAdmin");
        }

        //----< show list of courses >-------------------------------

        public IActionResult Index()
        {
            return View(context_.Publishers.ToList<Publisher>());
        }

        

        public IActionResult eBooks()
        {
            // fluent API
            var ebooks = context_.eBooks.Include(l => l.Publishers);
            //var ebooks = context_.eBooks.Include(l => l.eBookItems);
            var orderedLects = ebooks.OrderBy(l => l.Title)
              .OrderBy(l => l.Publishers)
              .Select(l => l);
            return View(orderedLects);
        }

        

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IActionResult CreatePublisher(int id)
        {
            var model = new Publisher();
            return View(model);
        }

        

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public IActionResult CreatePublisher(int id, Publisher crs)
        {
            if (crs.Identifier == null && crs.Identifier == null)
            {
                ViewData["Field"] = "Value of both field";
                return View("Error");
            }
            else if(crs.Name == null)
            {
                ViewData["Field"] = "Name field";
                return View("Error");
            }
            else if (crs.Identifier == null)
            {
                ViewData["Field"] = "Identifier field";
                return View("Error");
            }
            else
            { 
                context_.Publishers.Add(crs);
                context_.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        

        [Authorize(Roles = "Admin")]
        public IActionResult DeletePub(int? id)
        {
            //ViewBag.PublisherId = id;
            if (id == null)
            {
                ViewData["field"] = "Publisher";
                return View("NullError");
            }
            try
            {
                var publisher = context_.Publishers.Find(id);
                if (publisher != null)
                {
                    var publishers = context_.Publishers.Where(l => l.PublisherId == id);
                    var delpublishers = publishers.Select(l => l);
                    return View("DeletePublisher", delpublishers);
                }
                else
                {
                    ViewData["field"] = "Publisher";
                    return View("NullError");
                }
            }
            catch (Exception)
            {
                // nothing for now
                //ViewData["field"] = "Publisher";
                //return View("NullError");
            }
            return View("DeletePublisher");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeletePublisher(int? id)
        {
            if (id == null)
            {
                //return StatusCode(StatusCodes.Status400BadRequest);
                ViewData["field"] = "Publisher";
                return View("NullError");
            }
            try
            {
                var publisher = context_.Publishers.Find(id);
                if (publisher != null)
                {
                    context_.Remove(publisher);
                    context_.SaveChanges();
                }
                else
                {
                    ViewData["field"] = "Publisher";
                    return View("NullError");
                }
            }
            catch (Exception)
            {
                // nothing for now

                ViewData["child"] = "eBooks";
                ViewData["parent"] = "Publisher";
                return View("DeleteError");
            }
            return RedirectToAction("Index");
        }

        

        [Authorize(Roles = "Admin")]
        public ActionResult PublisherDetails(int? id)
        {
            if (id == null)
            {
                //return StatusCode(StatusCodes.Status400BadRequest);
                ViewData["field"] = "Publisher";
                return View("NullError");
            }
            Publisher publisher = context_.Publishers.Find(id);

            if (publisher == null)
            {
                //return StatusCode(StatusCodes.Status404NotFound);
                ViewData["field"] = "Publisher";
                return View("NullError");
            }
            var ebooks = context_.eBooks.Where(l => l.Publishers == publisher);

            publisher.eBooks = ebooks.OrderBy(l => l.Title).Select(l => l).ToList<eBook>();
            

            if (publisher.eBooks == null)
            {
                publisher.eBooks = new List<eBook>();
                eBook lct = new eBook();
                lct.Title = "none";
                lct.Author = "none";
                publisher.eBooks.Add(lct);
            }
            return View(publisher);
        }

        

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult EditPublisher(int? id)
        {
            if (id == null)
            {
                //return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
                ViewData["field"] = "Publisher";
                return View("NullError");
            }
            Publisher publisher = context_.Publishers.Find(id);
            if (publisher == null)
            {
                //return StatusCode(StatusCodes.Status404NotFound);
                ViewData["field"] = "Publisher";
                return View("NullError");
            }
            return View(publisher);
        }

        

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult EditPublisher(int? id, Publisher crs)
        {
            if (id == null)
            {
                //return StatusCode(StatusCodes.Status400BadRequest);
                ViewData["field"] = "Publisher";
                return View("NullError");
            }
            var publisher = context_.Publishers.Find(id);
            if (publisher != null)
            {
                publisher.Identifier = crs.Identifier;
                publisher.Name = crs.Name;
                try
                {
                    context_.SaveChanges();
                }
                catch (Exception)
                {
                    // do nothing for now
                    ViewData["field"] = "Publisher";
                    return View("NullError");
                }
            }
            else
            {
                ViewData["field"] = "Publisher";
                return View("NullError");
            }
            return RedirectToAction("Index");
        }
        

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IActionResult CreateeBook(int id)
        {
            var model = new eBook();
            return View(model);
        }

        

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public IActionResult CreateeBook(int id, eBook ebook)
        {
            context_.eBooks.Add(ebook);
            context_.SaveChanges();
            return RedirectToAction("eBooks");
        }

        

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IActionResult AddeBook(int id)
        {
            HttpContext.Session.SetInt32(sessionId_, id);
            //if (id == null)
            

            Publisher publisher = context_.Publishers.Find(id);
            if (publisher == null)
            {
                // return StatusCode(StatusCodes.Status404NotFound);
                ViewData["field"] = "Publisher";
                return View("NullError");
            }
            eBook lct = new eBook();
            return View(lct);
        }

        

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public IActionResult AddeBook(int? id, eBook lct)
        {
            if (id == null)
            {
                // return StatusCode(StatusCodes.Status400BadRequest);
                ViewData["field"] = "Publisher";
                return View("NullError");
            }
            

            int? publisherId_ = HttpContext.Session.GetInt32(sessionId_);

            var publisher = context_.Publishers.Find(publisherId_);

            if (publisher != null)
            {
                if (publisher.eBooks == null)
                {
                    List<eBook> ebooks = new List<eBook>();
                    publisher.eBooks = ebooks;
                }


                try
                {
                    IFormFile file = Request.Form.Files["MYeBook"];
                    if (Request.Form.Files["MYeBook"] == null)
                    {
                        //Console.WriteLine("File not uploaded!!!!!!!!!!!!");
                        string field = "eBook";
                        return RedirectToAction("Error", "Home", new { field = field });
                    }

                    var path = "/FileStorage/" + file.FileName;
                    //var path2 = "/FileStorageAdmin/" + file.FileName;
                    lct.Name = file.FileName;
                    lct.Path = path;
                    lct.UserName = User.Identity.Name;
                    //var path = "";
                    //if (User.IsInRole("Admin"))
                    //{
                    //    lct.Path = path2;
                    //    path = Path.Combine(ebookPathAdmin, file.FileName);
                    //}
                    //else
                    //{
                    //    lct.Path = path1;
                    //    path = Path.Combine(ebookPath, file.FileName);
                    //}

                    var path1 = Path.Combine(ebookPath, file.FileName);
                    FileInfo file1 = new FileInfo(path1);
                    //Console.WriteLine(path);
                    if (file1.Exists)
                    {
                        return RedirectToAction("FileExist");
                    }

                    MultipartFormDataContent multiContent = new MultipartFormDataContent();
                    byte[] data;
                    using (var br = new BinaryReader(file.OpenReadStream()))
                        data = br.ReadBytes((int)file.OpenReadStream().Length);
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    multiContent.Add(bytes, "file", file.FileName);
                    var result = client.PostAsync(baseUrl_, multiContent).Result;

                    if ((int)result.StatusCode == 200)
                    {
                        if (ModelState.IsValid)
                        {
                            publisher.eBooks.Add(lct);
                            context_.SaveChanges();
                            //return RedirectToAction("eBooks");
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                    //else
                    //{
                    //    string field = "eBook";
                    //    return RedirectToAction("Error", "Home", new { field = field });
                    //}
                }
                catch (Exception)
                {
                    // do nothing for now
                    ViewData["data"] = "eBook";
                    return View("UploadError");
                }
            }
            else
            {
                ViewData["field"] = "Publisher";
                return View("NullError");
            }
            return RedirectToAction("eBooks");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ReplaceeBook(int? id)
        {
            if (id == null)
            {
                ViewData["field"] = "eBook";
                return View("NullError");
            }
            if (context_.eBooks.Find(id) == null)
            {
                ViewData["field"] = "eBook";
                return View("NullError");
            }


            ViewBag.eBookId = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ReplaceeBook(int id)
        {
            if (id == null)
            {
                // return StatusCode(StatusCodes.Status400BadRequest);
                ViewData["field"] = "eBook";
                return View("NullError");
            }
            // retreive the target course from static field

            var ebook = context_.eBooks.Find(id);
            if (ebook == null)
            {
                ViewData["field"] = "eBook";
                return View("NullError");
            }
            try
            {
                IFormFile file = Request.Form.Files["MYeBook"];
                if (Request.Form.Files["MYeBook"] == null)
                {
                    //Console.WriteLine("File not uploaded!!!!!!!!!!!!");
                    string field = "eBook";
                    return RedirectToAction("Error", "Home", new { field = field });
                }

                var path = "/FileStorage/" + file.FileName;
                //var path2 = "/FileStorageAdmin/" + file.FileName;
                ebook.Name = file.FileName;
                ebook.Path = path;
                ebook.UserName = User.Identity.Name;

                var path1 = Path.Combine(ebookPath, file.FileName);
                FileInfo file1 = new FileInfo(path1);
                //Console.WriteLine(path);
                if (file1.Exists)
                {
                    return RedirectToAction("FileExist");
                }

                MultipartFormDataContent multiContent = new MultipartFormDataContent();
                byte[] data;
                using (var br = new BinaryReader(file.OpenReadStream()))
                    data = br.ReadBytes((int)file.OpenReadStream().Length);
                ByteArrayContent bytes = new ByteArrayContent(data);
                multiContent.Add(bytes, "file", file.FileName);
                var result = client.PostAsync(baseUrl_, multiContent).Result;

                if ((int)result.StatusCode == 200)
                {
                    if (ModelState.IsValid)
                    {
                        context_.SaveChanges();
                        //return RedirectToAction("eBooks");
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
                //else
                //{
                //    string field = "eBook";
                //    return RedirectToAction("Error", "Home", new { field = field });
                //}
            }
            catch (Exception)
            {
                // do nothing for now
                ViewData["data"] = "eBook";
                return View("UploadError");
            }
            return RedirectToAction("eBooks");
        }
            
            
    

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult EditeBook(int? id)
        {
            if (id == null)
            {
                //return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
                ViewData["field"] = "eBook";
                return View("NullError");
            }
            eBook ebook = context_.eBooks.Find(id);

            if (ebook == null)
            {
                //return StatusCode(StatusCodes.Status404NotFound);
                ViewData["field"] = "eBook";
                return View("NullError");
            }
            return View(ebook);
        }

       

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult EditeBook(int? id, eBook lct)
        {
            if (id == null)
            {
                //return StatusCode(StatusCodes.Status400BadRequest);
                ViewData["field"] = "eBook";
                return View("NullError");
            }
            var ebook = context_.eBooks.Find(id);

            if (ebook != null)
            {
                ebook.Title = lct.Title;
                ebook.Author = lct.Author;

                try
                {
                    context_.SaveChanges();
                }
                catch (Exception)
                {
                    // do nothing for now
                    ViewData["field"] = "eBook";
                    return View("NullError");
                }
            }
            else
            {
                ViewData["field"] = "eBook";
                return View("NullError");
            }
            return RedirectToAction("eBooks");
        }

        
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteeBook(int? id)
        {
            if (id == null)
            {
                //return StatusCode(StatusCodes.Status400BadRequest);
                ViewData["field"] = "eBook";
                return View("NullError");
            }
            try
            {
                var ebook = context_.eBooks.Find(id);

                if (ebook != null)
                {
                    //RedirectToAction("Delete", "eBooks", new { id = id });
                    if (ebook.Path != null)
                    {
                        var result = client.DeleteAsync(baseUrl_ + "/" + id.ToString()).Result;
                        if ((int)result.StatusCode == 200)
                        {
                            if (ModelState.IsValid)
                            {
                                context_.eBooks.Remove(ebook);
                                context_.SaveChanges();

                            }
                            //return RedirectToAction("Delete", "eBooks", new { id = ebook.eBookId});
                            else
                            {
                                return NotFound();
                            }
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        ViewData["field"] = "eBook";
                        return View("NullError");
                    }
                    //var comments = context_.Comments.Find(ebook.eBookId);

                    //context_.Remove(comments);
                    //context_.SaveChanges();

                    //var comments1 = context_.Comments.Include(l => l.eBooks);
                    //var ebooks = context_.eBooks.Include(l => l.eBookItems);
                    //var comments = comments1.OrderBy(l => l.Comment)
                    //  .OrderBy(l => l.eBooks)
                    //  .Select(l => l)
                    //  .Where(l => l.eBookId == id);
                    //var comments = context_.Comments.Where(l => l.eBooks == ebook);



                    //var comments = orderedComments.Where(l => l.eBookId == id);
                    // var comments = context_.Comments.Find(id);

                    //if (comments != null)
                    //{
                    //context_.Remove(context_.Comments.Where(l => l.eBookId == id));
                    //context_.SaveChanges();
                    //if(comments == null && ebook != null)
                    //{
                    //context_.eBooks.Remove(ebook);
                    //context_.SaveChanges();
                    //}
                    //}
                    //if (ebook != null && comments != null)
                    //{
                    //    context_.Remove(comments);
                    //    context_.SaveChanges();
                    //    context_.Remove(ebook);
                    //    context_.SaveChanges();
                    //}
                }
            }
            catch (Exception)
            {
                // nothing for now
                //Console.WriteLine("Error");

                //string errorMessage = "You need to first delete all the comments on this eBook. Then, proceed to delete the eBook.";
                ViewData["child"] = "Comments";
                ViewData["parent"] = "eBook";
                return View("DeleteError");

            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult UploadeBook(int? id)
        {
            if (id == null)
            {
                ViewData["field"] = "eBook";
                return View("NullError");
            }
            if(context_.eBooks.Find(id) == null)
            {
                ViewData["field"] = "eBook";
                return View("NullError");
            }


            ViewBag.eBookId = id;
            return View();
        }

        public IActionResult ReadeBook(int? id)
        {
            if (id == null)
            {
                ViewData["field"] = "eBook";
                return View("NullError");
            }

            var ebookid = context_.eBooks.Find(id);
            if (ebookid != null)
            {
                ViewBag.Path = ebookid.Path;
                ViewBag.eBookId = id;
                //ViewData["Path"] = "ghjgjh";
                //ViewBag.Name = name;

                if (ebookid.Path != null)
                {
                    if (User.IsInRole("Admin"))
                    {
                        var commentsAdmin = context_.Comments.Where(l => l.eBookId == id);
                        var comments = commentsAdmin.Select(l => l).ToList<Comments>();

                        return View(comments);
                    }
                    else
                    {

                        var commentslist = context_.Comments.Where(l => l.UserName == User.Identity.Name && l.eBookId == id);
                        var comments = commentslist.Select(l => l).ToList<Comments>();

                        return View(comments);
                    }
                }
            }
            if (ebookid == null)
            {
                ViewData["field"] = "eBook";
                return View("NullError");
            }

            return View();
            //return RedirectToAction("ReadeBook", new { id = id });
        }

        [Authorize(Roles = "Admin, User")]
        public IActionResult AddComment(int id, string comment)
        {
            //int id1 = id;
            if (id == null)
            {
                ViewData["field"] = "eBook";
                return View("NullError");
            }
            else
            { 
            if(comment != null)
            {
                context_.Comments.Add(new Comments { Comment = comment, eBookId = id, UserName = User.Identity.Name });
                context_.SaveChanges();
                return RedirectToAction("ReadeBook", new { id = id });
            }
            else
            {
                //ViewData["Field"] = "Comment";
                string field = "Comment";
                return RedirectToAction("Error", new { field = field });
            }

            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteComment(int? id)
        {
            if(id == null)
            {
                ViewData["field"] = "eBook";
                return View("NullError");
            }
            
            //var ebook = context_.eBooks.Find(id);
            var comments = context_.Comments.Find(id);
            var ebookid = comments.eBookId;
            context_.Remove(comments);
            context_.SaveChanges();
            //int id2 = Convert.ToInt32(id1);
            return RedirectToAction("ReadeBook", new { id = ebookid});
        }

        
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult DeleteError(string child, string parent)
        {
            ViewData["child"] = child;
            ViewData["parent"] = parent;
            return View();
        }

        public IActionResult NullError(string field)
        {
            ViewData["field"] = field;
            return View();
        }

        public IActionResult FileExist()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string field)
        {
            ViewData["Field"] = field;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
