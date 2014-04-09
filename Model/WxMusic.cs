using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class WxMusic
    {
        public virtual int ID { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string MusicURL { get; set; }
        public virtual string HQMusicUrl { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }
}
