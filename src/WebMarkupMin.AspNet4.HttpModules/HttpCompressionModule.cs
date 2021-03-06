﻿using System;
using System.IO;
using System.Web;

using WebMarkupMin.AspNet.Common;
using WebMarkupMin.AspNet.Common.Compressors;
using WebMarkupMin.AspNet4.Common;

namespace WebMarkupMin.AspNet4.HttpModules
{
	/// <summary>
	/// Compresses the content using GZIP or Deflate
	/// </summary>
	public sealed class HttpCompressionModule : IHttpModule
	{
		/// <summary>
		/// WebMarkupMin configuration
		/// </summary>
		private readonly WebMarkupMinConfiguration _configuration;

		/// <summary>
		/// HTTP compression manager
		/// </summary>
		private readonly IHttpCompressionManager _compressionManager;


		/// <summary>
		/// Constructs a instance of HTTP module for compressesion
		/// </summary>
		public HttpCompressionModule()
			: this(WebMarkupMinConfiguration.Instance, HttpCompressionManager.Current)
		{ }

		/// <summary>
		/// Constructs a instance of HTTP module for compressesion
		/// </summary>
		/// <param name="configuration">WebMarkupMin configuration</param>
		/// <param name="compressionManager">HTTP compression manager</param>
		public HttpCompressionModule(WebMarkupMinConfiguration configuration,
			IHttpCompressionManager compressionManager)
		{
			_configuration = configuration;
			_compressionManager = compressionManager;
		}


		/// <summary>
		/// Initializes a module and prepares it to handle requests
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpApplication"></see>
		/// that provides access to the methods, properties, and events common to
		/// all application objects within an ASP.NET application
		/// </param>
		public void Init(HttpApplication context)
		{
			context.PreRequestHandlerExecute += PreRequestHandlerExecute;
			context.Error += ProcessError;
		}

		/// <summary>
		/// Handles the BeginRequest event of the context control.
		/// </summary>
		/// <param name="sender">The source of the event</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data</param>
		private void PreRequestHandlerExecute(object sender, EventArgs e)
		{
			if (!_configuration.IsCompressionEnabled())
			{
				return;
			}

			HttpContext context = ((HttpApplication)sender).Context;
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			string mediaType = response.ContentType;

			if (request.HttpMethod == "GET" && response.StatusCode == 200
				&& _compressionManager.IsSupportedMediaType(mediaType))
			{
				context.Items["originalResponseFilter"] = response.Filter;

				string acceptEncoding = request.Headers["Accept-Encoding"];

				ICompressor compressor = _compressionManager.CreateCompressor(acceptEncoding);
				response.Filter = compressor.Compress(response.Filter);
				compressor.AppendHttpHeaders((key, value) =>
				{
					response.Headers[key] = value;
				});
			}
		}

		/// <summary>
		/// Handles the Error event of the context control
		/// </summary>
		/// <param name="sender">The source of the event</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data</param>
		private void ProcessError(object sender, EventArgs e)
		{
			HttpContext context = ((HttpApplication)sender).Context;
			if (context.Error != null && context.Items.Contains("originalResponseFilter"))
			{
				var originalResponseFilter = context.Items["originalResponseFilter"] as Stream;
				if (originalResponseFilter != null)
				{
					context.Response.Filter = originalResponseFilter;
				}
			}
		}

		/// <summary>
		/// Destroys object
		/// </summary>
		public void Dispose()
		{
			// Nothing to destroy
		}
	}
}