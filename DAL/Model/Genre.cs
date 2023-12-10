using System;
using System.Collections.Generic;

namespace DAL.Model
{
    public partial class Genre
    {
        public Genre()
        {
            Videos = new HashSet<Video>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<Video> Videos { get; set; }
    }
}
