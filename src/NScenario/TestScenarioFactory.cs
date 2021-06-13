using System.IO;

namespace NScenario
{
    public static class TestScenarioFactory
    {
        public static ITestScenario Default(TextWriter outputWriter = null)
        {
            var testScenarioBuilder = new DecoratorBuilder<ITestScenario>();
            testScenarioBuilder.WrapWith(_ => new OutputTestScenario(outputWriter));
            testScenarioBuilder.WrapWith(u => new IndentionTestScenario(u));
            testScenarioBuilder.WrapWith(u => new PrefixedTestScenario(u));
            testScenarioBuilder.WrapWith(u => new OrderedTestScenario(u));
            return testScenarioBuilder.Build();
        }
    }
}