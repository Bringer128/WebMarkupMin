﻿using System;

using Microsoft.AspNet.Builder;
using Microsoft.Extensions.Internal;

using WebMarkupMin.AspNet5.Internal;

namespace WebMarkupMin.AspNet5
{
	/// <summary>
	/// Extension methods for <see cref="IApplicationBuilder"/> to add
	/// WebMarkupMin optimization features to the request execution pipeline
	/// </summary>
	public static class BuilderExtensions
	{
		/// <summary>
		/// Adds a WebMarkupMin optimization features to the <see cref="IApplicationBuilder"/> request execution pipeline
		/// </summary>
		/// <param name="app">The <see cref="IApplicationBuilder"/></param>
		/// <returns>The <paramref name="app"/></returns>
		public static IApplicationBuilder UseWebMarkupMin([NotNull] this IApplicationBuilder app)
		{
			// Verify if `AddWebMarkupMin` was done before calling `UseWebMarkupMin`.
			// We use the `WebMarkupMinMarkerService` to make sure if all the services were added.
			IServiceProvider services = app.ApplicationServices;
			WebMarkupMinServicesHelper.ThrowIfWebMarkupMinNotRegistered(services);

			return app.UseMiddleware<WebMarkupMinMiddleware>();
		}
	}
}