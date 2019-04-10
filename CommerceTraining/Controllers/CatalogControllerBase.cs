using CommerceTraining.Models.Media;
using CommerceTraining.Models.Pages;
using CommerceTraining.SupportingClasses;
using EPiServer;
using EPiServer.Commerce.Catalog; // AssetUrlResolver
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Security;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Controllers
{
    public class CatalogControllerBase<T> 
        : ContentController<T> where T : CatalogContentBase
    {
        public readonly IContentLoader _contentLoader;
        public readonly UrlResolver _urlResolver;
        public readonly AssetUrlResolver _assetUrlResolver; // ECF
        public readonly ThumbnailUrlResolver _thumbnailUrlResolver; // ECF

        public CatalogControllerBase(
            IContentLoader contentLoader
            , UrlResolver urlResolver
            , AssetUrlResolver assetUrlResolver
            , ThumbnailUrlResolver thumbnailUrlResolver
            )
        {
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
            _assetUrlResolver = assetUrlResolver;
            _thumbnailUrlResolver = thumbnailUrlResolver;
        }

        #region Media/Assets in 9 +

        public string GetDefaultAsset(IAssetContainer contentInstance) 
        {
            return _assetUrlResolver.GetAssetUrl(contentInstance);
        }

        public string GetNamedAsset(IAssetContainer contentInstance, string propName) 
        {
            return _thumbnailUrlResolver.GetThumbnailUrl(contentInstance,propName);
        }
        
        #endregion

        #region ...asset-work in ECF 8
        
        //public string GetDefaultAsset(EntryContentBase contentInstance)
        //{
        //    return _assetUrlResolver.GetAssetUrl(contentInstance);
        //}

        //public string GetDefaultAsset(NodeContent contentInstance)
        //{
        //    return _assetUrlResolver.GetAssetUrl(contentInstance);
        //}

        //public string GetNamedAsset(EntryContentBase contentInstance, string propName) // propName = "Group"
        //{
        //    return _thumbnailUrlResolver.GetThumbnailUrl(contentInstance, propName); // could have a better name
        //}

        //public string GetNamedAsset(NodeContent contentInstance, string propName)
        //{
        //    return _thumbnailUrlResolver.GetThumbnailUrl(contentInstance, propName);
        //}
        
        #endregion

        public string GetUrl(ContentReference contentReference)
        {
            return _urlResolver.GetUrl(contentReference);
        }

        public List<NameAndUrls> GetNodes(ContentReference contentReference)
        {
            // FilterForVisitor/FilterContentForVisitor is nice
            IEnumerable<IContent> things = FilterForVisitor.Filter
                (_contentLoader.GetChildren<NodeContent>(contentReference, new LoaderOptions()));

            List<NameAndUrls> comboList = new List<NameAndUrls>(); // ...to send back to the view

            foreach (NodeContent item in things)
            {
                NameAndUrls comboListitem = new NameAndUrls();
                comboListitem.name = item.Name;
                comboListitem.url = GetUrl(item.ContentLink);

                // Get from default group, "named" in Adv.
                comboListitem.imageUrl = GetDefaultAsset(item);
                comboListitem.imageThumbUrl = GetNamedAsset(item, "Thumbnail");

                #region NoDemo

                //n.imageUrl = GetUrl(item.CommerceMediaCollection.FirstOrDefault().AssetLink); 
                //n.imageTumbUrl = GetThumbUrl(item.CommerceMediaCollection.FirstOrDefault().AssetLink);
                //n.imageCustomThumbUrl = GetCustomThumbUrl(item.CommerceMediaCollection.FirstOrDefault().AssetLink);

                // Takes the group set to default
                // group is the model-prop
                //n.resolverThumb = GetNamedAsset(_contentLoader.Get<NodeContent>(daRef), "CustomLargeImage");

                #endregion

                comboList.Add(comboListitem);
            }

            return comboList;
        }
        
        public List<NameAndUrls> GetEntries(ContentReference contentReference)
        {
            // IContent
            IEnumerable<IContent> things = FilterForVisitor.Filter
                (_contentLoader.GetChildren<EntryContentBase>(contentReference, new LoaderOptions()));

            List<NameAndUrls> comboList = new List<NameAndUrls>();

            //foreach (var item in entries)
            foreach (EntryContentBase item in things)
            {
                NameAndUrls listItems = new NameAndUrls();
                listItems.name = item.Name;
                listItems.url = GetUrl(item.ContentLink);
                listItems.imageUrl = GetDefaultAsset(item);
                listItems.imageThumbUrl = GetNamedAsset(item, "Thumbnail");

                #region NoDemo

                //n.imageUrl = GetUrl(item.CommerceMediaCollection.FirstOrDefault().AssetLink);
                //n.imageTumbUrl = GetThumbUrl(item.CommerceMediaCollection.FirstOrDefault().AssetLink);
                //n.imageCustomThumbUrl = GetCustomThumbUrl(item.CommerceMediaCollection.FirstOrDefault().AssetLink);

                #endregion

                comboList.Add(listItems);
            }

            return comboList;
        }

    }
}