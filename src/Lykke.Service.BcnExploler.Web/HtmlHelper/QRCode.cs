using System;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace Lykke.Service.BcnExploler.Web.HtmlHelper
{
    public static class QRCodeHtmlHelper
    {
        public static IHtmlContent QRCode(this IHtmlHelper htmlHelper, string data, int size = 80, object htmlAttributes = null)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (size < 1)
                throw new ArgumentOutOfRangeException("size", size, "Must be greater than zero.");

            var url = $"https://lykke-qr.azurewebsites.net/QR/{HttpUtility.UrlEncode(data)}.gif";

            var tag = new TagBuilder("img");
            if (htmlAttributes != null)
                tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            tag.Attributes.Add("src", url);
            tag.Attributes.Add("width", size.ToString());
            tag.Attributes.Add("height", size.ToString());

            return tag.RenderSelfClosingTag();
        }
    }
}