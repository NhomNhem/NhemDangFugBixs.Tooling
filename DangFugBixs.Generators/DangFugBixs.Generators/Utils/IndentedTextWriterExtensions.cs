using System.CodeDom.Compiler;

namespace DangFugBixs.Generators.Utils;

public static class IndentedTextWriterExtensions {
    public static IDisposable Block(this IndentedTextWriter writer, string line) {
        writer.WriteLine(line);
        writer.WriteLine("{");
        writer.Indent++;
        return new AutoClosingBlock(writer);
    }
    
    private struct AutoClosingBlock : IDisposable {
        private readonly IndentedTextWriter _writer;

        public AutoClosingBlock(IndentedTextWriter writer) => _writer = writer;

        public void Dispose() {
            _writer.Indent--;
            _writer.WriteLine("}");
        }
    }
}
