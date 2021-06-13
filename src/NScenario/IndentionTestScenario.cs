namespace NScenario
{
    public class IndentionTestScenario : LevelTrackingTestScenario
    {
        public IndentionTestScenario(ITestScenario testScenarioImplementation) : base(testScenarioImplementation)
        {
        }

        protected override string DecorateDescription(string description, Level level)
        {
            var indentation = new string('\t', level.Nesting);
            return $"{indentation}{description}";
        }
    }
}