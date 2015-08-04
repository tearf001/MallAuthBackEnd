using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MallAuth.Models
{
    /// <summary>
    /// 资源管理系统
    /// </summary>
    public class ContentManagerSystem
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
        public string Uploader { get; set; }
        public string OrigName { get; set; }
        [ForeignKey("Directory")]
        public int DirectoryId { get; set; }
        public virtual Directory Directory { get; set; }
    }

    public class Directory
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Parent")]
        public int? ParentId { get; set; }
        public string Owner { get; set; }
        public virtual Directory Parent { get; set; }
    }
}
