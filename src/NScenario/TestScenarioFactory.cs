using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using NScenario.OutputWriters;
using NScenario.StepExecutors;

namespace NScenario
{
    public static class TestScenarioFactory
    {
        public static IScenarioOutputWriter DefaultScenarioOutputWriter { get; set; } = new StreamScenarioOutputWriter(Console.Out);

        public static ITestScenario Default(TextWriter outputWriter = null, string scenarioPrefix = null, string stepPrefix = null, [CallerMemberName] string testMethodName = "", string title = null)
        {
            title ??= testMethodName;
            EnsureTestScenarioTitleUniqueness(title);
            var selectedOutputWriter = outputWriter != null ? new StreamScenarioOutputWriter(outputWriter) : DefaultScenarioOutputWriter;
            var stepExecutor = BuildScenarioStepExecutor(selectedOutputWriter, scenarioPrefix, stepPrefix);
            return new TestScenario(stepExecutor, title);
        }

        public static ITestScenario Default(IScenarioOutputWriter outputWriter, string scenarioPrefix = null, string stepPrefix = null, [CallerMemberName] string testMethodName = "", string title = null)
        {
            title ??= testMethodName;
            var selectedOutputWriter = outputWriter;
            var stepExecutor = BuildScenarioStepExecutor(selectedOutputWriter, scenarioPrefix, stepPrefix);
            return new TestScenario(stepExecutor, title ?? testMethodName);
        }

        private static readonly HashSet<string> ScenarioTitles = new HashSet<string>();

        private static void EnsureTestScenarioTitleUniqueness(string scenarioTitle)
        {
            if (ScenarioTitles.Add(scenarioTitle) == false)
            {
                throw new InvalidOperationException("Test scenario with a given title was already created. If you are using test method with parameters, please specify test scenario title explicitly by setting 'title' parameter.");
            }
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