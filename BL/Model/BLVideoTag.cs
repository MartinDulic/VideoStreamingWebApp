﻿using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model
{
    public class BLVideoTag
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; } = null!;
        public virtual Video Video { get; set; } = null!;
    }
}
