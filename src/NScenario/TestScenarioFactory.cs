using System;
using System.IO;
using System.Runtime.CompilerServices;
using NScenario.OutputWriters;
using NScenario.StepExecutors;

namespace NScenario
{
    public static class TestScenarioFactory
    {
        public static IScenarioOutputWriter DefaultScenarioOutputWriter { get; set; } = new StreamScenarioOutputWriter(Console.Out);

        public static ITestScenario Default(TextWriter outputWriter = null, string scenarioPrefix = null, string stepPrefix = null, [CallerMemberName] string testMethodName = "")
        {
            var selectedOutputWriter = outputWriter != null ? new StreamScenarioOutputWriter(outputWriter) : DefaultScenarioOutputWriter;
            var stepExecutor = BuildScenarioStepExecutor(selectedOutputWriter, scenarioPrefix, stepPrefix);
            return new TestScenario(stepExecutor, testMethodName);
        }

        private static IScenarioStepExecutor BuildScenarioStepExecutor(IScenarioOutputWriter scenarioOutputWriter, string scenarioPrefix = null, string stepPrefix = null)
        {
            var stepExecutorBuilder = new DecoratorBuilder<IScenarioStepExecutor>();
            stepExecutorBuilder.WrapWith(_ => new OutputScenarioStepExecutor(scenarioOutputWriter));
            stepExecutorBuilder.WrapWith(u => new IndentionScenarioStepExecutor(u));
            stepExecutorBuilder.WrapWith(u => new PrefixedScenarioStepExecutor(u, scenarioPrefix, stepPrefix));
            stepExecutorBuilder.WrapWith(u => new OrderedScenarioStepExecutor(u));
            var stepExecutor = stepExecutorBuilder.Build();
            return stepExecutor;
        }
    }
}