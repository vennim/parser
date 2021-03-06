﻿namespace GraphQLParser.Tests
{
    using GraphQLParser;
    using GraphQLParser.AST;
    using NUnit.Framework;
    using System.IO;
    using System.Linq;

    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Parse_FieldInput_HasCorrectEndLocationAttribute()
        {
            var document = ParseGraphQLFieldSource();

            Assert.AreEqual(9, document.Location.End);
        }

        [Test]
        public void Parse_FieldInput_HasCorrectStartLocationAttribute()
        {
            var document = ParseGraphQLFieldSource();

            Assert.AreEqual(0, document.Location.Start);
        }

        [Test]
        public void Parse_FieldInput_HasOneOperationDefinition()
        {
            var document = ParseGraphQLFieldSource();

            Assert.AreEqual(ASTNodeKind.OperationDefinition, document.Definitions.First().Kind);
        }

        [Test]
        public void Parse_FieldInput_NameIsNull()
        {
            var document = ParseGraphQLFieldSource();

            Assert.IsNull(GetSingleOperationDefinition(document).Name);
        }

        [Test]
        public void Parse_FieldInput_OperationIsQuery()
        {
            var document = ParseGraphQLFieldSource();

            Assert.AreEqual(OperationType.Query, GetSingleOperationDefinition(document).Operation);
        }

        [Test]
        public void Parse_FieldInput_ReturnsDocumentNode()
        {
            var document = ParseGraphQLFieldSource();

            Assert.AreEqual(ASTNodeKind.Document, document.Kind);
        }

        [Test]
        public void Parse_FieldInput_SelectionSetContainsSingleFieldSelection()
        {
            var document = ParseGraphQLFieldSource();

            Assert.AreEqual(ASTNodeKind.Field, GetSingleSelection(document).Kind);
        }

        [Test]
        public void Parse_FieldWithOperationTypeAndNameInput_HasCorrectEndLocationAttribute()
        {
            var document = ParseGraphQLFieldWithOperationTypeAndNameSource();

            Assert.AreEqual(22, document.Location.End);
        }

        [Test]
        public void Parse_FieldWithOperationTypeAndNameInput_HasCorrectStartLocationAttribute()
        {
            var document = ParseGraphQLFieldWithOperationTypeAndNameSource();

            Assert.AreEqual(0, document.Location.Start);
        }

        [Test]
        public void Parse_FieldWithOperationTypeAndNameInput_HasOneOperationDefinition()
        {
            var document = ParseGraphQLFieldWithOperationTypeAndNameSource();

            Assert.AreEqual(ASTNodeKind.OperationDefinition, document.Definitions.First().Kind);
        }

        [Test]
        public void Parse_FieldWithOperationTypeAndNameInput_NameIsNull()
        {
            var document = ParseGraphQLFieldWithOperationTypeAndNameSource();

            Assert.AreEqual("Foo", GetSingleOperationDefinition(document).Name.Value);
        }

        [Test]
        public void Parse_FieldWithOperationTypeAndNameInput_OperationIsQuery()
        {
            var document = ParseGraphQLFieldWithOperationTypeAndNameSource();

            Assert.AreEqual(OperationType.Mutation, GetSingleOperationDefinition(document).Operation);
        }

        [Test]
        public void Parse_FieldWithOperationTypeAndNameInput_ReturnsDocumentNode()
        {
            var document = ParseGraphQLFieldWithOperationTypeAndNameSource();

            Assert.AreEqual(ASTNodeKind.Document, document.Kind);
        }

        [Test]
        public void Parse_FieldWithOperationTypeAndNameInput_SelectionSetContainsSingleFieldWithOperationTypeAndNameSelection()
        {
            var document = ParseGraphQLFieldWithOperationTypeAndNameSource();

            Assert.AreEqual(ASTNodeKind.Field, GetSingleSelection(document).Kind);
        }

        [Test]
        public void Parse_KitchenSink_DoesNotThrowError()
        {
            new Parser(new Lexer()).Parse(new Source(LoadKitchenSink()));
        }

        [Test]
        public void Parse_NullInput_EmptyDocument()
        {
            var document = new Parser(new Lexer()).Parse(new Source(null));

            Assert.AreEqual(0, document.Definitions.Count());
        }

        [Test]
        public void Parse_VariableInlineValues_DoesNotThrowError()
        {
            new Parser(new Lexer()).Parse(new Source("{ field(complex: { a: { b: [ $var ] } }) }"));
        }

        private static GraphQLOperationDefinition GetSingleOperationDefinition(GraphQLDocument document)
        {
            return ((GraphQLOperationDefinition)document.Definitions.Single());
        }

        private static ASTNode GetSingleSelection(GraphQLDocument document)
        {
            return GetSingleOperationDefinition(document).SelectionSet.Selections.Single();
        }

        private static string LoadKitchenSink()
        {
            return @"﻿# Copyright (c) 2015, Facebook, Inc.
# All rights reserved.
#
# This source code is licensed under the BSD-style license found in the
# LICENSE file in the root directory of this source tree. An additional grant
# of patent rights can be found in the PATENTS file in the same directory.

query queryName($foo: ComplexType, $site: Site = MOBILE) {
  whoever123is: node(id: [123, 456]) {
    id ,
    ... on User @defer {
      field2 {
        id ,
        alias: field1(first:10, after:$foo,) @include(if: $foo) {
          id,
          ...frag
        }
      }
    }
    ... @skip(unless: $foo) {
      id
    }
    ... {
      id
    }
  }
}

mutation likeStory {
  like(story: 123) @defer {
    story {
      id
    }
  }
}

subscription StoryLikeSubscription($input: StoryLikeSubscribeInput) {
  storyLikeSubscribe(input: $input) {
    story {
      likers {
        count
      }
      likeSentence {
        text
      }
    }
  }
}

fragment frag on Friend {
  foo(size: $size, bar: $b, obj: {key: ""value""})
}

{
  unnamed(truthy: true, falsey: false),
  query
    }

# Copyright (c) 2015, Facebook, Inc.
# All rights reserved.
#
# This source code is licensed under the BSD-style license found in the
# LICENSE file in the root directory of this source tree. An additional grant
# of patent rights can be found in the PATENTS file in the same directory.

    schema {
  query: QueryType
  mutation: MutationType
}

type Foo implements Bar
{
    one: Type
  two(argument: InputType!): Type
  three(argument: InputType, other: String): Int
  four(argument: String = ""string""): String
  five(argument: [String] = [""string"", ""string""]): String
  six(argument: InputType = { key: ""value""}): Type
}

type AnnotatedObject @onObject(arg: ""value"")
{
    annotatedField(arg: Type = ""default"" @onArg): Type @onField
}

interface Bar
{
    one: Type
    four(argument: String = ""string""): String
}

interface AnnotatedInterface @onInterface {
  annotatedField(arg: Type @onArg): Type @onField
}

union Feed = Story | Article | Advert

union AnnotatedUnion @onUnion = A | B

scalar CustomScalar

scalar AnnotatedScalar @onScalar

enum Site
{
    DESKTOP
  MOBILE
}

enum AnnotatedEnum @onEnum {
  ANNOTATED_VALUE @onEnumValue
  OTHER_VALUE
}

input InputType
{
    key: String!
  answer: Int = 42
}

input AnnotatedInput @onInputObjectType {
  annotatedField: Type @onField
}

extend type Foo {
  seven(argument: [String]): Type
}

extend type Foo @onType { }

type NoFields { }

directive @skip(if: Boolean!) on FIELD | FRAGMENT_SPREAD | INLINE_FRAGMENT

directive @include(if: Boolean!)
  on FIELD
   | FRAGMENT_SPREAD
   | INLINE_FRAGMENT";
        }

        private static GraphQLDocument ParseGraphQLFieldSource()
        {
            return new Parser(new Lexer()).Parse(new Source("{ field }"));
        }

        private static GraphQLDocument ParseGraphQLFieldWithOperationTypeAndNameSource()
        {
            return new Parser(new Lexer()).Parse(new Source("mutation Foo { field }"));
        }
    }
}