using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Responses
{
	public class BaseResponse
	{
		public bool Success { get; set; }

		public string Message { get; set; }

		public string ExceptionMessage { get; set; }

		public bool Reset { get; set; }

		public string RedirectUrl { get; set; }
	}
}