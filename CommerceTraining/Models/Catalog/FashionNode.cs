using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Models.Catalog
{
    [CatalogContentType(GUID = "50A826FE-82A1-4C74-90F9-B706D5AD16BA", MetaClassName = "Fashion_Node")]
    public class FashionNode : NodeContent
    {
        [CultureSpecific]
        [IncludeInDefaultSearch]
        [Searchable]
        [Tokenize]
        public virtual XhtmlString MainBody { get; set; }
    }
}