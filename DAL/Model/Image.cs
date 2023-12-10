using System;
using System.Collections.Generic;

namespace DAL.Model
{
    public partial class Image
    {
        public Image()
        {
            Videos = new HashSet<Video>();
        }

        public int Id { get; set; }
        public string Content { get; set; } = null!;

        public virtual ICollection<Video> Videos { get; set; }
    }
}
