﻿using ProyectoV1.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoV1.Interfaces
{
    internal interface IPromotionsBLL
    {
        void Create(Promotions pPromotions);
        void Update(Promotions pPromotions);
        void Delete(string pID);
        void Read(string pID);
        List<Promotions> ReadAlll();
    }
}
