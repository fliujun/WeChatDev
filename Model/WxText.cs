﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class WxText
    {
        public virtual int ID { get; set; }
        public virtual string Content { get; set; }

        public virtual DateTime UpdateTime { get; set; }
    }
}
