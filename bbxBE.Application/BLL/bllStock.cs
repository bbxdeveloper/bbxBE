using bbxBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.BLL
{
    public static class bllStock
    {
        public static decimal GetNewAvgCost(decimal currAvgCost, decimal currQty, decimal chgQty, decimal newPrice)
        {
            //Ez tűnik a legjobb megközelítésnek (a negatív készlet miatt)
            //
            if ((Math.Abs(currQty) + chgQty) != 0)
            {
                return (Math.Abs(currAvgCost * currQty) + (chgQty * newPrice)) / (Math.Abs(currQty) + chgQty);
            }
            else
            {
                return Math.Abs(currAvgCost * currQty) + (chgQty * newPrice);
            }
        }
    }
}
