namespace DDDSample1.Domain.Predictions
{
    public class PredictionSearchParamsDto
    {
        public string UserId { get; set; }
        public string ModelName { get; set; }
        public string UserName { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}