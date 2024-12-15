namespace Homework
{
    public class ConversionRule
    {
        public Resource InputResource { get; }
        public Resource OutputResource { get; }
        public int InputAmount { get; }
        public int OutputAmount { get; } 
        public float Duration { get; }
        public ConversionRule(Resource inputResource, Resource outputResource, int inputAmount, int outputAmount, float duration)
        {
            InputResource = inputResource;
            OutputResource = outputResource;
            InputAmount = inputAmount;
            OutputAmount = outputAmount;
            Duration = duration;
        }
    }
}
