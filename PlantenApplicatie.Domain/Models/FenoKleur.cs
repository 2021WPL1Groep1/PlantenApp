﻿using System;
using System.Collections.Generic;

namespace PlantenApplicatie.Domain.Models
{
    public partial class FenoKleur
    {
        public int Id { get; set; }
        public string NaamKleur { get; set; }
        public byte[] HexWaarde { get; set; }
    }
}