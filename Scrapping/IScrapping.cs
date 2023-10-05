using HtmlAgilityPack;
using System.Threading.Tasks;

namespace Scrapping_Namesapce
{
    public interface IScrapping
    {
        Task<List<ListingDetail>> PerformSentimentAnalysisAsync(string url);
        Task<string> GetHtml(string url);

        public List<ListingDetail> HParse(string html);
        //ListingDetail ParseComments(HtmlDocument htmlDoc, ListingDetail listingDetail);
        string AnalyzeAverageSentiment(List<ListingDetail> listingDetails);
    }
}
