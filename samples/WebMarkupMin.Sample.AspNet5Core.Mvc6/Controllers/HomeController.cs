﻿using System.Xml.Linq;
using System.Collections.Generic;

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

using WebMarkupMin.Sample.Logic.Models;
using WebMarkupMin.Sample.Logic.Services;

namespace WebMarkupMin.Sample.AspNet5Core.Mvc6.Controllers
{
	public class HomeController : Controller
	{
		private readonly FileContentService _fileContentService;

		private readonly SitemapService _sitemapService;


		public HomeController(
			IConfigurationRoot configuration,
			IApplicationEnvironment applicationEnvironment,
			SitemapService sitemapService)
		{
			string textContentDirectoryPath = configuration
				.GetSection("webmarkupmin")
				.GetSection("Samples")["TextContentDirectoryPath"]
				;

			_fileContentService = new FileContentService(textContentDirectoryPath, applicationEnvironment);
			_sitemapService = sitemapService;
		}


		public IActionResult Index()
		{
			ViewBag.Body = new HtmlString(_fileContentService.GetFileContent("index.html"));

			return View();
		}

		[Route("minifiers")]
		public IActionResult Minifiers()
		{
			return View();
		}

		[Route("change-log")]
		public IActionResult ChangeLog()
		{
			ViewBag.Body = new HtmlString(_fileContentService.GetFileContent("change-log.html"));

			return View();
		}

		[Route("contact")]
		public IActionResult Contact()
		{
			ViewBag.Body = new HtmlString(_fileContentService.GetFileContent("contact.html"));

			return View();
		}

		[Route("sitemap")]
		public IActionResult Sitemap()
		{
			var sitemapItems = new List<SitemapItem>
			{
				new SitemapItem(GetAbsoluteUrl("Home", "Index"), null, SitemapChangeFrequency.Hourly, 0.9),
				new SitemapItem(GetAbsoluteUrl("Home", "Minifiers"), null, SitemapChangeFrequency.Daily, 0.7),
				new SitemapItem(GetAbsoluteUrl("HtmlMinifier", "Index"), null, SitemapChangeFrequency.Daily, 0.5),
				new SitemapItem(GetAbsoluteUrl("XhtmlMinifier", "Index"), null, SitemapChangeFrequency.Daily, 0.5),
				new SitemapItem(GetAbsoluteUrl("XmlMinifier", "Index"), null, SitemapChangeFrequency.Daily, 0.5),
				new SitemapItem(GetAbsoluteUrl("Home", "Changelog"), null, SitemapChangeFrequency.Daily, 0.8),
				new SitemapItem(GetAbsoluteUrl("Home", "Contact"), null, SitemapChangeFrequency.Weekly, 0.4)
			};

			XDocument xmlSitemap = _sitemapService.GenerateXmlSiteMap(sitemapItems);

			return new ContentResult
			{
				Content = xmlSitemap.ToString(),
				ContentType = new MediaTypeHeaderValue("text/xml")
			};
		}

		[NonAction]
		private string GetAbsoluteUrl(string controllerName, string actionName)
		{
			var urlHelper = new UrlHelper(
				(IActionContextAccessor)Resolver.GetService(typeof(IActionContextAccessor)),
				(IActionSelector)Resolver.GetService(typeof(IActionSelector)));
			string url = urlHelper.Action(actionName, controllerName);

			string absoluteUrl = string.Empty;
			if (url != null)
			{
				HttpRequest request = ActionContext.HttpContext.Request;
				absoluteUrl = request.Scheme + "://" + request.Host + url;
			}

			return absoluteUrl;
		}
	}
}