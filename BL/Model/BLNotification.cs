using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model
{
    public class BLNotification
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ReceiverEmail { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public DateTime? SentAt { get; set; }
    }
}
