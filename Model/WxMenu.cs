using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class WxMenu
    {
        public virtual int ID { get; set; }
        public virtual int PID { get; set; }
        public virtual string BtnName { get; set; }
        public virtual string BtnKey { get; set; }
        public virtual string BtnType { get; set; }
        public virtual int ReplyType { get; set; }
        public virtual int ReplyID { get; set; }
        public virtual string ReplyUrl { get; set; }
        public virtual int OrderNum { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }
}
