namespace NScenario
{
    public class OrderedTestScenario : LevelTrackingTestScenario
    {
        public OrderedTestScenario(ITestScenario testScenarioImplementation) : base(testScenarioImplementation)
        {
        }

        protected override string DecorateDescription(string description, Level level)
        {
            var stepNumberDescription = string.Join(".", level.LevelPath);
            return $"{stepNumberDescription}: {description}";
        }
    }
}