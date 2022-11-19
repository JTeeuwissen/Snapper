using FluentAssertions;
using Newtonsoft.Json.Linq;
using Snapper.Exceptions;
using Snapper.Json;
using Xunit;

namespace Snapper.Internals.Tests.Json
{
    public class JsonSnapshotSanitiserTests
    {
        private readonly JsonSnapshotSanitiser _sanitiser;

        public JsonSnapshotSanitiserTests()
        {
            _sanitiser = new JsonSnapshotSanitiser(SnapshotSettings.New());
        }

        [Fact]
        public void SimpleJsonObjectTest()
        {
            var sanitisedObject = _sanitiser.SanitiseSnapshot(new
            {
                Key = "Value"
            });

            sanitisedObject.Should().BeEquivalentTo(new
            {
                Key = "Value"
            });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2.1)]
        [InlineData(true)]
        [InlineData('a')]
        [InlineData("string")]
        public void PrimitivesTest(object obj)
        {
            var sanitisedObject = _sanitiser.SanitiseSnapshot(obj);

            sanitisedObject.Should().BeEquivalentTo(new
            {
                AutoGenerated = obj
            });
        }

        [Fact]
        public void ValidJsonStringTest()
        {
            var sanitisedObject = _sanitiser.SanitiseSnapshot("{ " +
                                                              "\"Key\" : \"value\"" +
                                                              "}");

            sanitisedObject.Should().BeEquivalentTo(JObject.FromObject(new
            {
                Key = "value"
            }));
        }

        [Fact]
        public void InvalidJsonStringTest()
        {
            var sanitisedObject = _sanitiser.SanitiseSnapshot("{ " +
                                                              "\"Key\" : \"value\"");
            sanitisedObject.Should().BeEquivalentTo(new
            {
                AutoGenerated = "{ \"Key\" : \"value\""
            });
        }

        [Fact]
        public void MalformedJsonStringTest()
        {
            var exception = Record.Exception(() => _sanitiser.SanitiseSnapshot("{ " +
                                                               "\"Key\" ======== \"value\"" +
                                                               "}"));

            exception.Should().NotBeNull();
            exception.GetType().FullName.Should().Be(typeof(MalformedJsonSnapshotException).FullName);
            exception.Message.Should()
                .Be("The snapshot provided contains malformed JSON. See inner exception for details.");
        }
    }
}
