using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NScenario.OutputWriters;
using NScenario.StepExecutors;

namespace NScenario
{
    public static class TestScenarioFactory
    {
        public static IScenarioOutputWriter DefaultScenarioOutputWriter { get; set; } = new StreamScenarioOutputWriter(Console.Out);

        public static ITestScenario Default(TextWriter outputWriter = null, string scenarioPrefix = null, string stepPrefix = null, [CallerMemberName] string testMethodName = "", string title = null)
        {
            title ??= GenerateScenarioTitle(testMethodName);
            EnsureTestScenarioTitleUniqueness(title);
            var selectedOutputWriter = outputWriter != null ? new StreamScenarioOutputWriter(outputWriter) : DefaultScenarioOutputWriter;
            var stepExecutor = BuildScenarioStepExecutor(selectedOutputWriter, scenarioPrefix, stepPrefix);
            return new TestScenario(stepExecutor, title);
        }
        public static ITestScenario Default(IScenarioOutputWriter outputWriter, string scenarioPrefix = null, string stepPrefix = null, [CallerMemberName] string testMethodName = "", string title = null)
        {
            title ??= GenerateScenarioTitle(testMethodName);
            var selectedOutputWriter = outputWriter;
            var stepExecutor = BuildScenarioStepExecutor(selectedOutputWriter, scenarioPrefix, stepPrefix);
            return new TestScenario(stepExecutor, title ?? testMethodName);
        }

        private static readonly Regex InWordBreakPattern = new Regex("(?<!(^|[A-Z]))(?=[A-Z])|(?<!^)(?=[A-Z][a-z])", RegexOptions.Compiled);

        private static readonly Regex WordSplitPattern = new Regex(@"[^a-zA-Z0-9]", RegexOptions.Compiled);

        private static string GenerateScenarioTitle(string scenario)
        {
            var parts = WordSplitPattern.Split(scenario).Select(word => InWordBreakPattern.Split(word)).SelectMany(x => x).Where(x => !string.IsNullOrWhiteSpace(x));
            var scenarioTitle = string.Join(" ", parts);
            return scenarioTitle;
        }

        private static readonly HashSet<string> ScenarioTitles = new HashSet<string>();

        private static readonly object titleUniquenessLock = new object();
        private static void EnsureTestScenarioTitleUniqueness(string scenarioTitle)
        {
            lock (titleUniquenessLock)
            {
                if (ScenarioTitles.Add(scenarioTitle) == false)
                {
                    throw new InvalidOperationException("Test scenario with a given title was already created. If you are using test method with parameters, please specify test scenario title explicitly by setting 'title' parameter.");
                }
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