using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eBooksApp.Data;
using eBooksApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eBooksApp.Controllers
{
    [Route("api/[controller]")]
    public class eBooksController : ControllerBase
    {
        private const string sessionId_ = "SessionId";
        private readonly ApplicationDbContext context_;
        private readonly IHostingEnvironment hostingEnvironment_;
        private string webRootPath = null;
        private string ebookPath = null;
        //private string ebookPathAdmin = null;
        // private string baseurl = "https://localhost:44348/eBooksApp\eBooksApp\wwwroot\FileStorage\";
        // private string baseurl = "https://localhost:44348/";

        public eBooksController(IHostingEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            context_ = context;
            hostingEnvironment_ = hostingEnvironment;
            webRootPath = hostingEnvironment_.WebRootPath;
            ebookPath = Path.Combine(webRootPath, "FileStorage");
            //ebookPathAdmin = Path.Combine(webRootPath, "FileStorageAdmin");
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            List<string> ebooks = null;
            try
            {
                ebooks = Directory.GetFiles(ebookPath).ToList<string>();
                for (int i = 0; i < ebooks.Count; ++i)
                    ebooks[i] = Path.GetFileName(ebooks[i]);
            }
            catch
            {
                ebooks = new List<string>();
                ebooks.Add("404 - Not Found");
            }
            return ebooks;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Download(int id)
        {
            List<string> ebooks = null;
            string ebook = "";
            try
            {
                ebooks = Directory.GetFiles(ebookPath).ToList<string>();
                if (0 <= id && id < ebooks.Count)
                    ebook = Path.GetFileName(ebooks[id]);
                else
                    return NotFound();
            }
            catch
            {
                return NotFound();
            }
            var memory = new MemoryStream();
            ebook = ebooks[id];
            using (var stream = new FileStream(ebook, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(ebook), Path.GetFileName(ebook));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
        {
        {".cs", "application/C#" },
        {".txt", "text/plain"},
        {".pdf", "application/pdf"},
        {".doc", "application/vnd.ms-word"},
        {".docx", "application/vnd.ms-word"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
        {".png", "image/png"},
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".gif", "image/gif"},
        {".csv", "text/csv"}
        };
        }
        

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            var request = HttpContext.Request;
            foreach (var ebook in request.Form.Files)
            {
                if (ebook.Length > 0)
                {
                    var path = Path.Combine(ebookPath, ebook.FileName);
                    using (var ebookStream = new FileStream(path, FileMode.Create))
                    {
                        await ebook.CopyToAsync(ebookStream);
                        //return RedirectToAction("eBooks", "Home");
                    }
                    //return RedirectToAction("eBooks", "Home");

                }
                else
                {
                    return BadRequest();
                    //Console.WriteLine("File not uploaded!!!!!!!!!!!!");
                    //string field = "eBook";
                    //return RedirectToAction("Error", "Home", new { field = field });
                }
            }

            return Ok();

        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            // ToDo
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //var ebook = await context_.eBooks.FindAsync(id);
            var ebook = await context_.eBooks.FindAsync(id);
            try { 
            if (ebook != null)
            {
                
                    var path = Path.Combine(ebookPath, ebook.Name);
                    //var path1 = "/FileStorage/" + ebook.Name;
                    FileInfo file = new FileInfo(path);
                    //Console.WriteLine(path);
                    if(file.Exists)
                    {
                        file.Delete();
                        
                    }
                return Ok();
                //return RedirectToAction("Index", "Home");
            }
            else
            {
                string field = "eBook";
                return RedirectToAction("NullError", "Home", new { field = field});
            }
            }
            catch
            {
                return BadRequest();
            }


        }

        
    }
}
