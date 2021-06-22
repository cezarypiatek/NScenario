using System.IO;
using System.Runtime.CompilerServices;
using NScenario.StepExecutors;

namespace NScenario
{
    public static class TestScenarioFactory
    {
        public static ITestScenario Default(TextWriter outputWriter = null, string scenarioPrefix = null, string stepPrefix = null, [CallerMemberName] string testMethodName = "")
        {
            var stepExecutor = BuildScenarioStepExecutor(outputWriter, scenarioPrefix, scenarioPrefix);
            return new TestScenario(stepExecutor, testMethodName);
        }

        private static IScenarioStepExecutor BuildScenarioStepExecutor(TextWriter outputWriter,string scenarioPrefix = null, string stepPrefix = null)
        {
            var stepExecutorBuilder = new DecoratorBuilder<IScenarioStepExecutor>();
            stepExecutorBuilder.WrapWith(_ => new OutputScenarioStepExecutor(outputWriter));
            stepExecutorBuilder.WrapWith(u => new IndentionScenarioStepExecutor(u));
            stepExecutorBuilder.WrapWith(u => new PrefixedScenarioStepExecutor(u, scenarioPrefix, stepPrefix));
            stepExecutorBuilder.WrapWith(u => new OrderedScenarioStepExecutor(u));
            var stepExecutor = stepExecutorBuilder.Build();
            return stepExecutor;
        }
    }
}