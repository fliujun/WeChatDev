using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class WxArticle
    {
        public virtual int ID { get; set; }
        public virtual int PID{get;set;}
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }

        //图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
        public virtual string PicUrl { get; set; }
        public virtual string Url { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime UpdateTime { get; set; }
        //public virtual IList<WxArticle> SubArticles { get; set; }
    }
}
