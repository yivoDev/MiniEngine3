﻿using System;
using System.Text;

namespace Mini.Engine.Generators.Source.CSharp
{
    public sealed class SourceWriter
    {
        private readonly StringBuilder Text;
        private int currentIndentation;
        private int targetIndentation;

        public SourceWriter()
        {
            this.Text = new StringBuilder();
            this.currentIndentation = 0;
            this.targetIndentation = 0;
        }

        public void Write(string text)
        {
            this.Indent();
            _ = this.Text.Append(text);
        }

        public void WriteMultiLine(string text)
        {
            var lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                this.WriteLine(line);
            }
        }

        public void WriteLine(string text)
        {
            this.Indent();
            _ = this.Text.AppendLine(text);
            this.currentIndentation = 0;
        }

        public void WriteLine()
        {
            _ = this.Text.AppendLine();
            this.currentIndentation = 0;
        }

        public void ConditionalEmptyLine(bool condition)
        {
            if (condition)
            {
                this.WriteLine();
            }
        }

        public void WriteModifiers(string[] modifiers)
        {
            if (modifiers.Length > 0)
            {
                var text = string.Join(" ", modifiers);
                this.Write($"{text} ");
            }
        }

        public void StartIndent() => this.targetIndentation++;
        public void EndIndent() => this.targetIndentation--;

        public void StartScope()
        {
            this.WriteLine("{");
            this.StartIndent();
        }

        public void EndScope()
        {
            this.EndIndent();
            this.WriteLine("}");
        }

        public override string ToString() => this.Text.ToString();

        public static string ToString(ISource source)
        {
            var writer = new SourceWriter();
            source.Generate(writer);
            return writer.ToString();
        }

        private void Indent()
        {
            while (this.currentIndentation < this.targetIndentation)
            {
                _ = this.Text.Append("    ");
                this.currentIndentation++;
            }
        }
    }
}
