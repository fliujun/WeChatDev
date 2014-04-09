using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class WxLink
    {
        public virtual int ID { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string Url { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }
}
