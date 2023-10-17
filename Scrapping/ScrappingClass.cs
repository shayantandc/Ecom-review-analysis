using HtmlAgilityPack;
using System;
using Sentiment_Service;
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
            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//head/following-sibling::*");

            if (titleNode != null)
            {
                return titleNode.InnerHtml;
            }

            return null;

        }


        public List<ListingDetail> HParse(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            List<ListingDetail> listingDetails = new List<ListingDetail>();
            ParseComments(htmlDoc, listingDetails);
            return listingDetails;
        }

        public void ParseComments(HtmlDocument htmlDoc, List<ListingDetail> listingDetails)
        {
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

        public List<ListingDetail> GetTweets(IFormFile htmlFile)
        {
            string htmlContent;
            using (var reader = new StreamReader(htmlFile.OpenReadStream()))
            {
                htmlContent = reader.ReadToEnd();
            }

            htmlContent = CleanUpHtml(htmlContent);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var tweetNodes = doc.DocumentNode.SelectNodes("//span[contains(@class, 'css-901oao css-16my406 r-poiln3 r-bcqeeo r-qvutc0')]");

            if (tweetNodes != null && tweetNodes.Any())
            {
                List<ListingDetail> listingDetails = new List<ListingDetail>();

                foreach (var tweetNode in tweetNodes)
                {
                    var tweetText = tweetNode.InnerText.Trim();
                    if (tweetText.Length > 35 && !tweetText.Contains("Rs") && !tweetText.Contains("%"))
                    {
                        if (!string.IsNullOrEmpty(tweetText))
                        {
                            var listingDetail = new ListingDetail
                            {
                                Comments = tweetText,
                                Score = 0
                            };
                            listingDetails.Add(listingDetail);
                        }
                    }
                }

                return listingDetails;
            }
            else
            {
                return new List<ListingDetail>();
            }
        }

        public string CleanUpHtml(string html)
        {
            html = Regex.Replace(html, @"\s+", " ");
            html = Regex.Replace(html, @">\s+<", "><");
            return html;
        }

        public List<ListingDetail> PerformSentimentAnalysisTweet(List<ListingDetail> listingDetails)
        {
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


    }
}
