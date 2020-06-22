using NUnit.Framework;
using System.IO;
using System.Linq;

namespace NAddressParser.Tests
{
    public class StuffTests
    {
        const string Address = @"Data\address.log";
        const string AddressOnly = @"Data\address_only.log";
        [Test]
        public void CheckEmptyTest()
        {
            var lines = File.ReadAllLines(Address);
            var count = lines.Where(string.IsNullOrEmpty).Count();
            var parser = new AddressParser();
            var parserCount = 0;
            foreach (var line in lines)
            {
                var result = parser.Parse(line);
                if (result.IsEmpty) parserCount++;
            }
            Assert.AreEqual(count, parserCount);
        }
        
        [Test]
        public void ReplaceSubstringTest()
        {
            var countryPatern = "������, �����, 45";
            var country = "������";
            var countryExp = " �����, 45";
            var indexPatern  = "������, �����, 45, 555555";
            var indexExp = "������, �����, 45, ";

            var indexinPatern = "������, �����, 45, 555555, ���";
            var indexinExp = "������, �����, 45,  ���";

            var result  = AddressResult.ReplaceSubstring(countryPatern, country);
            Assert.AreEqual(result,countryExp);
            result =  AddressResult.ReplaceSubstring(indexPatern, "555555");
            Assert.AreEqual(result, indexExp);
            result = AddressResult.ReplaceSubstring(indexinPatern, "555555");
            Assert.AreEqual(result, indexinExp);
        }
    }
}