namespace NScenario.StepExecutors
{
    public class IndentionScenarioStepExecutor : LevelTrackingScenarioStepExecutor
    {
        public IndentionScenarioStepExecutor(IScenarioStepExecutor scenarioStepExecutorImplementation) : base(scenarioStepExecutorImplementation)
        {
        }

        protected override string DecorateDescription(string description, Level level)
        {
            var indentation = new string('\t', level.Nesting);
            return $"{indentation}{description}";
        }
    }
}