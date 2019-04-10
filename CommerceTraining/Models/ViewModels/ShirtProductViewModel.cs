using CommerceTraining.SupportingClasses;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceTraining.Models.ViewModels
{
    public class ShirtProductViewModel
    {
        public ProductContent CurrentContent { get; set; }

        public string ImageUrl { get; set; }

        public IEnumerable<ContentReference> VariantLinks { get; set; }

        public NameAndUrls SelectedVariant { get; set; }

        public IEnumerable<SelectListItem> Sizes { get; set; }

        public IEnumerable<SelectListItem> Colors { get; set; }
    }
}