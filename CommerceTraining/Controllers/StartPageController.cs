using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using CommerceTraining.Models.Pages;
using EPiServer.ServiceLocation;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Web.Routing;
//using CommerceTraining.SupportingClasses;

namespace CommerceTraining.Controllers
{
    public class StartPageController : PageController<StartPage>
    {
       
        public readonly IContentLoader _contentLoader;
        public readonly UrlResolver _urlResolver;

        ContentReference topCategory { get; set; } // used for listing of nodes at the start-page

        public StartPageController(
            IContentLoader contentLoader
            , UrlResolver urlResolver)
        {
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;

            // uncomment the below when the catalog is modelled
            topCategory = contentLoader.Get<StartPage>(PageReference.StartPage).Settings.topCategory;
        }

        public string GetUrl(ContentReference contentReference)
        {
            return _urlResolver.GetUrl(contentReference);
        }

        public ActionResult Index(StartPage currentPage)
        {
            var model = new CommerceTraining.Models.ViewModels.PageViewModel<StartPage>(currentPage)
            {
                MainBodyStartPage = currentPage.MainBody,
                myPageChildren = _contentLoader.GetChildren<IContent>(currentPage.ContentLink),
                
                // uncomment the below when the catalog is modelled
                topLevelCategories = _contentLoader.GetChildren<CatalogContentBase>(topCategory).OfType<NodeContent>(),
            };

            return View(model);
        }
    }
}