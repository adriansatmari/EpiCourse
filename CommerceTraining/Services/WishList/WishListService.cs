using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Order;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;

namespace CommerceTraining.Services.WishList
{
	public class WishListService
	{
		public List<VariationContent> GetContentLink(string code, IEnumerable<ILineItem> myWishListItems)
		{
			var referenceConverter = ServiceLocator.Current.GetInstance<ReferenceConverter>();
			var variationContentList = new List<VariationContent>();

			foreach (var item in myWishListItems)
			{
				var variantLink = referenceConverter.GetContentLink(item.Code);
				var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
				var variant = repo.Get<VariationContent>(variantLink);
				variationContentList.Add(variant);
			}

			return variationContentList;
		}
	}
}