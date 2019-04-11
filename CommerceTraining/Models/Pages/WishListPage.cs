using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace CommerceTraining.Models.Pages
{
	[ContentType(DisplayName = "WishListPage", GUID = "4cdc256a-6ea4-4ee8-876b-1d0077180290"
		, Description = "Showing the wish list products")]
	public class WishListPage:PageData
	{
		[CultureSpecific]
		[Display(
			Name = "Main body",
			Description = "The main body Wist List Page can be used with the XHTML-editor for example text, images and tables.",
			GroupName = SystemTabNames.Content,
			Order = 1)]
		public virtual XhtmlString MainBodyWistListPage { get; set; }
	}
}