using HtmlAgilityPack;
using System.Threading.Tasks;

namespace Scrapping_Namesapce
{
    public interface IScrapping
    {
        Task<List<ListingDetail>> PerformSentimentAnalysisAsync(string url);
        Task<string> GetHtml(string url);
        public List<ListingDetail> HParse(string html);
        string AnalyzeAverageSentiment(List<ListingDetail> listingDetails);
        public List<ListingDetail> GetTweets(IFormFile htmlFile);
        public List<ListingDetail> PerformSentimentAnalysisTweet(List<ListingDetail> listingDetails);
    }
}
