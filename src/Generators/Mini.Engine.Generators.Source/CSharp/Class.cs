﻿using System.Collections.Generic;

namespace Mini.Engine.Generators.Source.CSharp
{
    public sealed class Class : Type
    {
        public Class(string name, params string[] modifiers)
            : base(name, modifiers) { }

        public override string TypeKeyword => "class";

        public static ClassBuilder<Class> Builder(string name, params string[] modifiers)
        {
            var @class = new Class(name, modifiers);
            return new ClassBuilder<Class>(@class, @class);
        }
    }

    public sealed class ClassBuilder<TPrevious> : Builder<TPrevious, Class>
    {
        internal ClassBuilder(TPrevious previous, Class current)
            : base(previous, current) { }

        public ClassBuilder(TPrevious previous, string name, params string[] modifiers)
            : base(previous, new Class(name, modifiers)) { }

        public ClassBuilder<TPrevious> Inherits(string name)
        {
            this.Output.InheritsFrom.Add(name);
            return this;
        }

        public MethodBuilder<ClassBuilder<TPrevious>> Method(string type, string name, params string[] modifiers)
        {
            var builder = new MethodBuilder<ClassBuilder<TPrevious>>(this, type, name, modifiers);
            this.Output.Methods.Add(builder.Output);
            return builder;
        }

        public ClassBuilder<TPrevious> Methods(IEnumerable<Method> methods)
        {
            this.Output.Methods.AddRange(methods);
            return this;
        }

        public FieldBuilder<ClassBuilder<TPrevious>> Field(string type, string name, params string[] modifiers)
        {
            var builder = new FieldBuilder<ClassBuilder<TPrevious>>(this, type, name, modifiers);
            this.Output.Fields.Add(new Field(type, name, modifiers));
            return builder;
        }

        public ClassBuilder<TPrevious> Fields(IEnumerable<Field> fields)
        {
            this.Output.Fields.AddRange(fields);
            return this;
        }

        public ConstructorBuilder<ClassBuilder<TPrevious>> Constructor(params string[] modifiers)
        {
            var builder = new ConstructorBuilder<ClassBuilder<TPrevious>>(this, this.Output.Name, modifiers);
            this.Output.Constructors.Add(builder.Output);
            return builder;
        }

        public ClassBuilder<TPrevious> InnerTypes(IEnumerable<Type> types)
        {
            this.Output.InnerTypes.AddRange(types);
            return this;
        }
    }
}
