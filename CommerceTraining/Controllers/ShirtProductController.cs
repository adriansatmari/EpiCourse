using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using CommerceTraining.Models.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.ServiceLocation;
using CommerceTraining.Models.Pages;
using EPiServer.Web.Routing;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.Linking;
using CommerceTraining.SupportingClasses;
using CommerceTraining.Models.ViewModels;

namespace CommerceTraining.Controllers
{
    
    [TemplateDescriptor(Default =true)]
    public class ShirtProductController : CatalogControllerBase<ShirtProduct>
    {
        
        private readonly IRelationRepository _relationRepository;

        public ShirtProductController(IContentLoader contentLoader, UrlResolver urlResolver, AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver, IRelationRepository relationRepository)
            : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
            _relationRepository = relationRepository;
        }

        public ActionResult Index(ShirtProduct currentContent, string size = null, string color = null)
        {
            var variants = currentContent.GetVariants(_relationRepository)
                .Select(contentLink => _contentLoader.Get<ShirtVariation>(contentLink));
            var sizes = variants.Select(variant => variant.Size).Distinct();
            var colors = variants.Select(variant => variant.Color).Distinct();
            string imgUrl = _assetUrlResolver.GetAssetUrl(currentContent);

            if (!string.IsNullOrEmpty(size))
            {
                variants = variants.Where(variant => variant.Size == size);
            }

            if (!string.IsNullOrEmpty(color))
            {
                variants = variants.Where(variant => variant.Color == color);
            }

            var model = new ShirtProductViewModel
            {
                CurrentContent = currentContent,
                VariantLinks = variants.Select(variant => variant.ContentLink),
                ImageUrl = _assetUrlResolver.GetAssetUrl(currentContent),
                Sizes = sizes.Select(s => new SelectListItem { Text = s, Value = s, Selected = s == size }),
                Colors = colors.Select(c => new SelectListItem { Text = c, Value = c, Selected = c == color })
            };

            return View(model);
        }

    }


}