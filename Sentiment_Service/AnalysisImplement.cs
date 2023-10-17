using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sentiment_Service
{
    public class AnalysisImplement
    {
        public int PerformSentimentAnalysis(string comment)
        {
            MLModel1.ModelInput sampleData = new MLModel1.ModelInput()
            {
                Col0 = comment,
            };

            var predictionResult = MLModel1.Predict(sampleData);

            return (int)predictionResult.PredictedLabel;
        }
    }
}
