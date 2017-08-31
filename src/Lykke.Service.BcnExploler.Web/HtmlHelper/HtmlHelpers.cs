using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lykke.Service.BcnExploler.Web.HtmlHelper
{
    public static class HtmlHelpers
    {
        public static IHtmlContent Truncate(this IHtmlHelper htmlHelper, string value)
        {
            var truncate = new TagBuilder("span");
            truncate.Attributes["title"] = value;
            truncate.AddCssClass("truncate");

            truncate.InnerHtml.AppendHtml(value);

            return truncate;
        }


        public static IHtmlContent Loader(this IHtmlHelper htmlHelper, string htmlClass = null, string htmlId = null)
        {
            var wrapper = new TagBuilder("div");
            wrapper.Attributes["class"] = htmlClass;
            wrapper.Attributes["id"] = htmlId;

            var spinnerContainer = new TagBuilder("div");
            spinnerContainer.AddCssClass("spinner_container");

            var spinner = new TagBuilder("div");
            spinner.AddCssClass("spinner");

            var spinnerInside = new TagBuilder("div");
            spinnerInside.AddCssClass("spinner__inside");


            spinner.InnerHtml.AppendHtml(spinnerInside);
            spinnerContainer.InnerHtml.AppendHtml(spinner);
            wrapper.InnerHtml.AppendHtml(spinnerContainer);

            return wrapper;
        }

    }
}
