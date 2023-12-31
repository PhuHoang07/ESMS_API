﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Entities.ExamTimeModel
{
    public class ExamTimeInfoModel
    {
        public int Idt { get; set; }
        public string Date { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public int Required { get; set; }
        public int Registered { get; set; }
        public string Amount { get; set; }
    }
}
