using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using CommerceTraining.Models.Pages;
using CommerceTraining.Models.ViewModels;
using Mediachase.Commerce.Catalog;
using EPiServer.Commerce.Order;
using System;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Customers;
using EPiServer.Commerce.Catalog.ContentTypes;
using CommerceTraining.Models.Catalog;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Commerce.Catalog.Linking;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Pricing;

namespace CommerceTraining.Controllers
{
    public class AdminPageController : PageController<AdminPage>
    {
        private readonly ReferenceConverter _referenceConverter;
        private readonly IContentLoader _contentLoader;
        private readonly AssetUrlResolver _assetUrlResolver;

        public AdminPageController(
            ReferenceConverter referenceConverter
            , IContentLoader contentLoader
            , AssetUrlResolver assetUrlReslver)
        {
            _referenceConverter = referenceConverter;
            _contentLoader = contentLoader;
            _assetUrlResolver = assetUrlReslver;
        }

        // contains "ServiceLocator.Current.GetInstance<>"
        public ActionResult Index(AdminPage currentPage)
        {
            //...not the best choice...
            var ppp = ServiceLocator.Current.GetInstance<IPriceService>();
            ///ppp.

            //OrderContext.Current.RaiseOrderGroupUpdatedEvent()
            
            // short demo of ref-conv
            CheckReferenceConverter();
            GetProductStuff(currentPage);

            var model = new AdminViewModel
            {
                MainBody = currentPage.MainBody,
                ProductArea = currentPage.ProductArea
            };

            return View(model);
        }

        
        private void GetProductStuff(AdminPage currentPage)
        {
            // added using EPiServer.Commerce.Catalog.ContentTypes;
            // so we get all the nice extension methods
            string code = "Long-Sleeve-Shirt_1";
            var theThingRef = _referenceConverter.GetContentLink(code);

            var theThing = _contentLoader.Get<ShirtProduct>(theThingRef);
            string assetUrl = _assetUrlResolver.GetAssetUrl(theThing);

            CommerceMedia x = theThing.CommerceMediaCollection.First();
            ItemCollection<CommerceMedia> col = theThing.CommerceMediaCollection;

            Categories y = theThing.Categories;
            IEnumerable<ContentReference> z = theThing.GetCategories();

        }

        public void CheckReferenceConverter()
        {
            // variationController has demo code for "Loading examples"
            ContentReference theRef = _referenceConverter.GetContentLink("Shirts_1");
            int theInt = _referenceConverter.GetObjectId(theRef);

            CatalogContentType theType = _referenceConverter.GetContentType(theRef);

            ContentReference theSameRef = _referenceConverter.GetContentLink
                (theInt, CatalogContentType.CatalogNode, 0);

            string theCode = _referenceConverter.GetCode(theSameRef);

            ContentReference catalogRoot = _referenceConverter.GetRootLink();

            List<string> codes = new List<string>
            {
                "Shirts_1",
                "Men_1"
            };

            IDictionary<string, ContentReference> theDict;
            theDict = _referenceConverter.GetContentLinks(codes);
        }

        public void CheckPromos(string id)
        {

        }

        public void TestBF(string id)
        {

        }

        // examples of "Injected"
        Injected<IOrderGroupFactory> _orderGroupFactory;
        Injected<IOrderRepository> _orderRepository;

        public void SplitShip()
        {


        }

        public ActionResult TestLink()
        {
            //ContentReference theRef = new ContentReference(25);
            //object obj = new object();
            //return RedirectToAction("Index", new { page = new ContentReference(25) }.page.ToPageReference()); // ok
            return RedirectToAction("Index", new { node = ContentReference.StartPage }); // ok
            //return RedirectToAction("DeadEnd", new { node = theRef, passed = "Hello" });
        }


    }
}