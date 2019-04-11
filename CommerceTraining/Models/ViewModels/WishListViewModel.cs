using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceTraining.SupportingClasses;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Order;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Security;

namespace CommerceTraining.Models.ViewModels
{
	public class WishListViewModel
	{
		public IEnumerable<ILineItem> LineItems { get; set; }
		public List<VariationContent> variationContentList { get; set; }
		public Money SubTotal { get; set; }
		public string WarningMessage { get; set; }
		public string PromotionMessage { get; set; } // not in startes


	}
}