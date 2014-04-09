using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class WxBase
    {
        public virtual int ID{get;set;}
        public virtual string LoginName { get; set; }
        public virtual string LoginPwd { get; set; }
        public virtual string AppId { get; set; }
        public virtual string AppSecret { get; set; }
        public virtual int ReplyType { get; set; }
        public virtual int ReplyID { get; set; }
    }
}
