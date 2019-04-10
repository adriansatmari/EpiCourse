using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using CommerceTraining.Models.Pages;
using CommerceTraining.Models.ViewModels;
using EPiServer.Commerce.Catalog.ContentTypes;
using Mediachase.Commerce.Website.Search;
using Mediachase.Search.Extensions;
using Mediachase.Search;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using System;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using CommerceTraining.Models.Catalog;
using EPiServer.Globalization;

namespace CommerceTraining.Controllers
{
    public class SearchController : PageController<SearchPage>
    {
        public IEnumerable<IContent> _localContent { get; set; }
        public readonly IContentLoader _contentLoader;
        public readonly ReferenceConverter _referenceConverter;
        public readonly UrlResolver _urlResolver;

        public SearchController(IContentLoader contentLoader, ReferenceConverter referenceConverter, UrlResolver urlResolver)
        {
            _contentLoader = contentLoader;
            _referenceConverter = referenceConverter;
            _urlResolver = urlResolver;
        }

        public ActionResult Index(SearchPage currentPage)
        {
            var model = new SearchPageViewModel
            {
                CurrentPage = currentPage,
            };

            return View(model);
        }

        public ActionResult Search(string keyWord)
        {
            // ToDo: SearchHelper and Criteria 
            SearchFilterHelper searchHelper = SearchFilterHelper.Current; // the easy way

            CatalogEntrySearchCriteria criteria = searchHelper.CreateSearchCriteria(keyWord
                , CatalogEntrySearchCriteria.DefaultSortOrder);

            criteria.RecordsToRetrieve = 25;
            criteria.StartingRecord = 0;
            //criteria.Locale = "en"; // needed
            criteria.Locale = ContentLanguage.PreferredCulture.Name;

            int count = 0; // "Out"
            bool cacheResult = true;
            TimeSpan timeSpan = new TimeSpan(0, 10, 0);

            // ToDo: Search 
            // One way of "doing it" ... retrieve it like ISearchResults (preferred, most certainly)
            ISearchResults searchResult = searchHelper.SearchEntries(criteria);
            ISearchDocument aDoc = searchResult.Documents.FirstOrDefault();
            int[] ints = searchResult.GetKeyFieldValues<int>();

            /* == ways of loading, keeping some old stuff for enjoying squiggles == */
            // ECF style Entries, old-school & legacy, not recommended at all... 
            // ...work with DTOs if not using the ContentModel
            Entries entries = CatalogContext.Current.GetCatalogEntries(ints // Note "ints"
                , new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));

            // still interesting
            CatalogContext.Current.GetCatalogEntriesDto(ints
                , new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
            
            // Same thing... ECF-old-style, not recommended... if not absolutely needed...
            // Use the helper and get the entries direct 
            // If entries are needed ... like for calculating discounts with legacy StoreHelper()
            Entries entriesDirect = searchHelper.SearchEntries(criteria, out count // Note the different return-types ... akward!
                , new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo)
                , cacheResult, new TimeSpan());

            // CMS style (better)... using ReferenceConverter and ContentLoader 
            List<ContentReference> refs = new List<ContentReference>();
            ints.ToList().ForEach(i => refs.Add(_referenceConverter.GetContentLink(i, CatalogContentType.CatalogEntry, 0)));
            
            // LoaderOptions() is new in CMS 8
            // ILanguageSelector selector = ServiceLocator.Current.GetInstance<ILanguageSelector>(); // obsolete
            _localContent = _contentLoader.GetItems(refs, new LoaderOptions()); // use this in CMS 8+

            // ToDo: Facets
            List<string> facetList = new List<string>();

            int facetGroups = searchResult.FacetGroups.Count();

            foreach (ISearchFacetGroup item in searchResult.FacetGroups)
            {
                foreach (var item2 in item.Facets)
                {
                    facetList.Add(String.Format("{0} {1} ({2})", item.Name, item2.Name, item2.Count));
                }
            }

            // Fill up the ViewModel
            var searchResultViewModel = new SearchResultViewModel();

            searchResultViewModel.totalHits = new List<string> { "" }; // change
            searchResultViewModel.nodes = _localContent.OfType<FashionNode>();
            searchResultViewModel.products = _localContent.OfType<ShirtProduct>();
            searchResultViewModel.variants = _localContent.OfType<ShirtVariation>();
            searchResultViewModel.allContent = _localContent;
            searchResultViewModel.facets = facetList;
            
            return View(searchResultViewModel);

            
        }
    }
}