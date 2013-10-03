using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Patterson.Domain.Entities
{
    public class FileUpload
    {
        [Key]
        public int fileID { get; set; }
        public byte[] fileContent { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public int ClassID { get; set; }
        public int CourseID { get; set; }

    }
}
