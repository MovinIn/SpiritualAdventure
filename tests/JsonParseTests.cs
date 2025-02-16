using System;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SpiritualAdventure.utility;

namespace SpiritualAdventure.tests;

[TestFixture]
public class JsonParseTests
{
  
  [SetUp]
  public void SetUp() { }
  
  [Test]
  public void TestSpeechLineParsing() //troll test
  {
    var parser = new DynamicParser(null);
    var speechLine = parser.ParseSpeechLine(DynamicParser.ParseFromFile<JObject>(
      "C:/Users/steve/csharp-workspace/SpiritualAdventure/utility/json//SpeechLineTest.json"));
    // var speechLine = JsonParseUtils.ParseSpeech(JsonParseUtils.ParseFromFile<JObject>(
      // "C:/Users/steve/csharp-workspace/SpiritualAdventure/utility/json//SpeechLineTest.json"));

    try
    {
      Assert.That(speechLine.line, Is.EqualTo("start speech content"),
        "start content check failed");
      Assert.That(speechLine.next!.line, Is.EqualTo("a content"),
        "a content check failed");
      Assert.That(speechLine.next!.next!.line, Is.EqualTo("b content"),
        "b content check failed");
      Assert.That(speechLine.next!.options!.Count, Is.EqualTo(3),
        "options.count is not 3");
      Assert.That(speechLine.next!.options!["option 1"].line, Is.EqualTo("c content"),
        "line after option 1 not initialized");
      Assert.That(speechLine.next!.options!["option 2"].line, Is.EqualTo("c content"),
        "line after option 2 not initialized");
      Assert.That(speechLine.next!.options!["option 3"].line, Is.EqualTo("c content"),
        "line after option 3 not initialized");
    }
    catch(NullReferenceException)
    {
      Assert.Fail("Something is null");
    }
  }
}