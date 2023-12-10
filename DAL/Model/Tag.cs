using System;
using System.Collections.Generic;

namespace DAL.Model
{
    public partial class Tag
    {
        public Tag()
        {
            VideoTags = new HashSet<VideoTag>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<VideoTag> VideoTags { get; set; }
    }
}
