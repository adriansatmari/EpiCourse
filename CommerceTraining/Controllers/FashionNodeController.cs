using CommerceTraining.Models.Catalog;
using CommerceTraining.SupportingClasses;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    public class FashionNodeController : CatalogControllerBase<FashionNode>
    {
        public FashionNodeController(IContentLoader contentLoader, UrlResolver urlResolver, AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver) 
            : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
        }

        public ActionResult Index(FashionNode currentContent)
        {
            var model = new NodeEntryCombo
            {
                CurrentContent = currentContent,
                Nodes = GetNodes(currentContent.ContentLink),
                Entries = GetEntries(currentContent.ContentLink)
            };

            return View(model);
        }
    }
}