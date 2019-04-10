using CommerceTraining.SupportingClasses;
using EPiServer.Commerce.Marketing;
using EPiServer.Core;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.InventoryService;
//using EPiServer.Commerce.SpecializedProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Models.ViewModels
{
    public class ShirtVariationViewModel
    {
        //public IEnumerable<RewardDescription> rewards { get; set; }

        public string discountString { get; set; }
        public decimal discountPrice { get; set; }
        public string priceString { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public bool CanBeMonogrammed { get; set; }
        public XhtmlString MainBody { get; set; }

        // not in the Fund. course... check this
        public ContentArea ProductArea { get; set; }
        public IEnumerable<IWarehouseInventory> WHOldSchool { get; set; } // not using custom WarehouseInfo-class... yet
        public IEnumerable<InventoryRecord> WHNewSchool { get; set; }

    }
}