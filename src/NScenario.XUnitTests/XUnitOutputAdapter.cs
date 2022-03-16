using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace NScenario.XunitTests
{
    public class XUnitOutputAdapter : TextWriter
    {
        public XUnitOutputAdapter(ITestOutputHelper output)
        {
            Output = output;
        }

        private ITestOutputHelper Output { get; }
        public override void WriteLine(string? value) => Output.WriteLine(value);
        public override Encoding Encoding { get; }
    }
}