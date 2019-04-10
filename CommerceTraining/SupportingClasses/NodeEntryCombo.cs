using EPiServer.Commerce.Catalog.ContentTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.SupportingClasses
{
    public class NodeEntryCombo
    {
        public IEnumerable<NameAndUrls> Nodes { get; set; }
        public IEnumerable<NameAndUrls> Entries { get; set; }
        public NodeContent CurrentContent { get; set; } // added afterwords - not in starters
    }
}