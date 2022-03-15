namespace NScenario.StepExecutors
{
    public interface IScenarioOutputWriter
    {
        void WriteStepDescription(string description);
        void WriteScenarioTitle(string scenarioTitle);
    }
}