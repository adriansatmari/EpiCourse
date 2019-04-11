using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceTraining.Models.Pages;
using CommerceTraining.Models.ViewModels;
using CommerceTraining.Services.WishList;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Order;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Security;

namespace CommerceTraining.Controllers
{

	public class WishListController:PageController<WishListPage>
	{
		private readonly IOrderRepository _orderRepository;
		public WishListController(IOrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}

		public ActionResult Index(WishListPage currentPage)
		{
			WishListService wishListService=new WishListService();

			ICart myWishList = _orderRepository.LoadOrCreateCart<ICart>
				(PrincipalInfo.CurrentPrincipal.GetContactId(), "WishList");
			var myWishListItems = myWishList.GetAllLineItems();

			List<VariationContent> variationContentList = new List<VariationContent>();
			foreach (var item in myWishListItems)
			{
			     variationContentList= wishListService.GetContentLink(item.Code, myWishListItems);
			}

			WishListViewModel model = new WishListViewModel
			{
				LineItems = myWishListItems,
				variationContentList = variationContentList
			};
		
			
			return View("index", model);
		}
	}
}