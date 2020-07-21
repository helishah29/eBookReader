using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eBooksApp.Models
{
    public class Publisher
    {
        [Key]
        public int PublisherId { get; set; }
        [Required]
        public string Identifier { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<eBook> eBooks { get; set; }
        //public ICollection<eBookItem> eBookItems { get; set; }
    }

    public class eBook
    {
        public int eBookId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string UserName { get; set; }

        public int PublisherId { get; set; }
        public Publisher Publishers { get; set; }

        public ICollection<Comments> Comments { get; set; }

       // public eBookItem eBookItems { get; set; }
    }

    public class Comments
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public string Comment { get; set; }
        
        public int eBookId { get; set; }
        public eBook eBooks { get; set; }
        public string UserName { get; set; }
    }

    //public class eBookItem
    //{
    //    public int eBookItemId { get; set; }
    //    public string Name { get; set; }
    //    public string Path { get; set; }

    //    public int? eBookId { get; set; }
    //    public eBook eBooks { get; set; }
    //}
}
