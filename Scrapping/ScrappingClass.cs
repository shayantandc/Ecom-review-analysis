using HtmlAgilityPack;
using Sentiment_Analysis_Service;
using System;
using System.Text.RegularExpressions;

namespace Scrapping_Namesapce
{
    public class ScrappingClass : IScrapping
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ScrappingClass(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }



        public async Task<string> GetHtml(string url)
        {
            //return _httpClient.GetStringAsync(BuildUrl(NormalizeAddress(address)));

            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//head/following-sibling::*");

            if (titleNode != null)
            {
                return titleNode.InnerHtml;
            }

            return null;

            //var httpClient = _httpClientFactory.CreateClient("MyHttpClient");
            //var response = await httpClient.GetAsync("https://www.amazon.in/Dell-R5-5500U-35-56cm-Spill-Resistant-Keyboard/dp/B0C1434CCN/ref=sr_1_1_sspa?crid=8XYSP7171OE9&keywords=dell+laptop&nsdOptOutParam=true&qid=1695794172&sprefix=dell+laptop%2Caps%2C259&sr=8-1-spons&sp_csd=d2lkZ2V0TmFtZT1zcF9hdGY&psc=1");

            //if (response.IsSuccessStatusCode)
            //{
            //    return await response.Content.ReadAsStringAsync();
            //}

            //return null;
        }


        public List<ListingDetail> HParse(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            List<ListingDetail> listingDetails = new List<ListingDetail>();
            ParseComments(htmlDoc, listingDetails);
            return listingDetails;
        }


        //private void ParseListingPrice(HtmlDocument htmlDoc, ListingDetail listingDetail)
        //{
        //    var listingPriceElement = htmlDoc.DocumentNode.SelectSingleNode("//span[@data-testid=\"price\"]/span[1]");
        //    if (listingPriceElement != null)
        //    {
        //        var listingPriceText = listingPriceElement.InnerHtml;
        //        listingDetail.Comments = decimal.Parse(listingPriceText.Replace("$", "")).ToString();
        //    }
        //}

        public void ParseComments(HtmlDocument htmlDoc, List<ListingDetail> listingDetails)
        {
            // Find all div elements with the specified class
            var elements = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'a-expander-content reviewText review-text-content a-expander-partial-collapse-content')]");

            if (elements != null)
            {
                foreach (var element in elements)
                {
                    string textContent = element.InnerText.Trim();

                    if (!string.IsNullOrEmpty(textContent))
                    {
                        var listingDetail = new ListingDetail
                        {
                            Comments = textContent,
                            Score = 0
                        };
                        listingDetails.Add(listingDetail);
                    }
                }
            }
        }

        public async Task<List<ListingDetail>> PerformSentimentAnalysisAsync(string url)
        {
            var html = await GetHtml(url);
            var listingDetails = HParse(html);

            var sentimentAnalyzer = new AnalysisImplement();
            var updatedListingDetail = new List<ListingDetail>();

            foreach (var detail in listingDetails)
            {
                string[] comments = detail.Comments.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                foreach (var comment in comments)
                {
                    int sentimentScore = sentimentAnalyzer.PerformSentimentAnalysis(comment);
                    string feedback = sentimentScore == 0 ? "negative" : "positive";

                    var updatedDetail = new ListingDetail
                    {
                        Comments = comment,
                        Score = sentimentScore,
                        Feedback = feedback
                    };

                    updatedListingDetail.Add(updatedDetail);
                }
            }

            return updatedListingDetail;
        }

        public string AnalyzeAverageSentiment(List<ListingDetail> listingDetails)
        {
            int countPositive = listingDetails.Count(detail => detail.Score == 1);
            int countNegative = listingDetails.Count(detail => detail.Score == 0);

            return countNegative > countPositive ? "Mostly it is negative" : "Mostly it is positive";
        }



    }
}
