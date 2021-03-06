﻿using WebMarkupMin.AspNet.Common;
using WebMarkupMin.AspNet4.Common;

namespace WebMarkupMin.AspNet4.WebForms
{
	/// <summary>
	/// Web Forms page with support of HTML minification and HTTP compression
	/// </summary>
	public class MinifiedAndCompressedHtmlPage : MinifiedAndCompressedPageBase
	{
		/// <summary>
		/// Constructs a instance of Web Forms page with support of HTML minification and HTTP compression
		/// </summary>
		public MinifiedAndCompressedHtmlPage()
			: this(WebMarkupMinConfiguration.Instance, HtmlMinificationManager.Current, HttpCompressionManager.Current)
		{ }

		/// <summary>
		/// Constructs a instance of Web Forms page with support of HTML minification and HTTP compression
		/// </summary>
		/// <param name="configuration">WebMarkupMin configuration</param>
		/// <param name="minificationManager">HTML minification manager</param>
		/// <param name="compressionManager">HTTP compression manager</param>
		public MinifiedAndCompressedHtmlPage(WebMarkupMinConfiguration configuration,
			IHtmlMinificationManager minificationManager,
			IHttpCompressionManager compressionManager)
			: base(configuration, minificationManager, compressionManager)
		{ }
	}
}