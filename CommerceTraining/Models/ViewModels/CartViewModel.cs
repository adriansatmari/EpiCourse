using EPiServer.Commerce.Order;
using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Models.ViewModels
{
    public class CartViewModel
    {
        public IEnumerable<ILineItem> LineItems { get; set; }
        public Money SubTotal { get; set; }
        public string WarningMessage { get; set; }
        public string PromotionMessage { get; set; } // not in startes
    }
}