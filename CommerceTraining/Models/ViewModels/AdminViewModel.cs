using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Models.ViewModels
{
    public class AdminViewModel
    {
        public XhtmlString MainBody { get; set; }

        public ContentArea ProductArea { get; set; }

    }
}