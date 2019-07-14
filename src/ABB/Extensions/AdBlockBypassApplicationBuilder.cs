using System;
using System.IO;
using System.Text;
using ABB.KeyProvider;
using ABB.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ABB.Extensions
{
    public static class AdBlockBypassApplicationBuilder
    {
        private static string GetAbsolutePath(HttpRequest request)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Path = request.Path.ToString()
            };
            return uriBuilder.Uri.AbsolutePath;
        }
        public static IApplicationBuilder UseAdBlockBypass(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<ABBOptions>>();//get options

            var htmlReplacer = app.ApplicationServices.GetRequiredService<HtmlReplacer>();//get HtmlReplacer

            var fileProvider = app.ApplicationServices.GetRequiredService<FileProvider>();//get FileProvider

            var uniqueKeyProvider = app.ApplicationServices.GetRequiredService<IUniqueKeyProvider>();//get IUniqueKeyProvider

            if (options?.Value == null)
                throw new Exception("Define AdBlockBypassOptions with using (IServiceCollection).AddAdBlockBypass method");

            if (options.Value.Pages == null || options.Value.Pages.Count == 0)
                return app;//do nothing


            app.Use(async (context, next) =>
            {
                var uri = GetAbsolutePath(context.Request);
                if (options.Value.Pages.ContainsKey(uri))// if user add this request url to list
                {
                    var existingBody = context.Response.Body;

                    using (var newBody = new MemoryStream())
                    {
                        // We set the response body to our stream so we can read after the chain of middlewares have been called.
                        context.Response.Body = newBody;

                        await next();// after that newBody memory stream will have generated html of view

                        context.Response.Body = existingBody;
                        newBody.Seek(0, SeekOrigin.Begin);

                        var newContent = new StreamReader(newBody).ReadToEnd();
                        //replace keys in content
                        newContent = htmlReplacer.ReplaceBypassKeysInHtml(uri, newContent);

                        // Send our modified content to the response body.
                        await context.Response.WriteAsync(newContent);
                    }
                }
                else
                {
                    //when our pages dont contains url, check whether it is our css or js file

                    uri = uri.Substring(1, uri.Length - 1);//remove first \ 
                    var isUrlCanBeKey = uri
                                        .Split('\\').Length == 1;//check is url has another \

                    var keyInformation = uniqueKeyProvider.GetValueFromMatchedGuid(uri);

                    if (isUrlCanBeKey && keyInformation != null)
                    {
                        var fileString = fileProvider.ProvideCssJsFileString(uri);
                        await context.Response.WriteAsync(fileString, Encoding.UTF8);
                    }
                    else
                        await next.Invoke();
                }
            });
            return app;
        }
    }
}
