using HtmlAgilityPack;

namespace FlightFinder.Client.Test.TestHelpers
{
    public static class HtmlNodeExtensions
    {
        public static bool HasClass(this HtmlNode node, string cssClassName)
        {
            return node.GetAttributeValue("class", string.Empty).Contains(cssClassName);
        }
    }
}
