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
            var countryPatern = "Россия, Алтай, 45";
            var country = "Россия";
            var countryExp = " Алтай, 45";
            var indexPatern  = "Россия, Алтай, 45, 555555";
            var indexExp = "Россия, Алтай, 45, ";

            var indexinPatern = "Россия, Алтай, 45, 555555, дом";
            var indexinExp = "Россия, Алтай, 45,  дом";

            var result  = AddressResult.ReplaceSubstring(countryPatern, country);
            Assert.AreEqual(result,countryExp);
            result =  AddressResult.ReplaceSubstring(indexPatern, "555555");
            Assert.AreEqual(result, indexExp);
            result = AddressResult.ReplaceSubstring(indexinPatern, "555555");
            Assert.AreEqual(result, indexinExp);
        }
    }
}