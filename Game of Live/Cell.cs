﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_of_Live {
    public class Cell
    {
        public enum Status { Alive, Dead, Unborn }

        public Status CurrentStatus { get; set; }
    }
}
