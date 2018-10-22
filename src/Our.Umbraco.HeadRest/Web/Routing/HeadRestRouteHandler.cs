﻿using Our.Umbraco.HeadRest.Web.Models;
using System;
using System.Web.Routing;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.HeadRest.Web.Routing
{
    internal class HeadRestRouteHandler : UmbracoVirtualNodeRouteHandler
    {
        private HeadRestConfig _config;

        public HeadRestRouteHandler(HeadRestConfig config)
        {
            _config = config;
        }

        protected override IPublishedContent FindContent(RequestContext requestContext, UmbracoContext umbracoContext)
        {
            var nodeXPath = _config.RootNodeXPath;

            if (requestContext?.RouteData?.Values != null)
            { 
                if (requestContext.RouteData.Values.ContainsKey("path")
                    && requestContext.RouteData.Values["path"] != null)
                {
                    var path = requestContext.RouteData.Values["path"].ToString();
                    if (_config.CustomRouteMappings != null)
                    {
                        var match = _config.CustomRouteMappings.GetRouteMapFor(path);
                        if (match != null)
                        {
                            path = match.Target;

                            requestContext.RouteData.Values["HeadRestRouteMapMatch"] = match;
                        }
                    }
                    
                    var pathParts = path.Trim('/').Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var pathPart in pathParts)
                    {
                        nodeXPath += $"/*[@urlName='{pathPart}'][1]";
                    }
                }
            }

            var node = umbracoContext.ContentCache.GetSingleByXPath(nodeXPath);

            return node ?? new NotFoundPublishedContent();
        }
    }
}
