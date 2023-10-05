using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment_Analysis_Service
{
    public class AnalysisImplement
    {
        public int PerformSentimentAnalysis(string comment)
        {
            // Create a single instance of sample data for model input
            MLModel1.ModelInput sampleData = new MLModel1.ModelInput()
            {
                Col0 = comment,
            };

            // Make a single prediction on the sample data
            var predictionResult = MLModel1.Predict(sampleData);

            // Return the predicted sentiment label (0 or 1)
            return (int)predictionResult.PredictedLabel;
        }

    }
}
