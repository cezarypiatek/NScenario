namespace NScenario.StepExecutors
{
    public class OrderedScenarioStepExecutor : LevelTrackingScenarioStepExecutor
    {
        public OrderedScenarioStepExecutor(IScenarioStepExecutor scenarioStepExecutorImplementation) : base(scenarioStepExecutorImplementation)
        {
        }

        protected override string DecorateDescription(string description, Level level)
        {
            var stepNumberDescription = string.Join(".", level.LevelPath);
            return $"{stepNumberDescription}: {description}";
        }
    }
}