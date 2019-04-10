using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Models.ViewModels
{
    public class PageViewModel<T> where T : PageData
    {

        // ToDo: rootChildren nodes (lab C)


        public IEnumerable<CatalogContentBase> topLevelCategories { get; set; }
        public IEnumerable<IContent> myPageChildren { get; set; }
        public virtual XhtmlString MainBodyStartPage { get; set; }

        public PageViewModel(T currentPage)
        {
            CurrentPage = currentPage;
        }

        public T CurrentPage
        {
            get;
            set;
        }
    }
}