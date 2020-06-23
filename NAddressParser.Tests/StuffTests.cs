using NUnit.Framework;
using System.IO;
using System.Linq;
using NAddressParser.Interfaces;

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
            var countryExp = "Алтай, 45";
            
            var indexPatern  = "Россия, Алтай, 45, 555555";
            var indexExp = "Россия, Алтай, 45";

            var indexinPatern = "Россия, Алтай, 45, 555555, дом";
            var indexinExp = "Россия, Алтай, 45, , дом";

            var result  = AddressResult.ReplaceSubstring(countryPatern, country);
            Assert.AreEqual(result,countryExp);
            result =  AddressResult.ReplaceSubstring(indexPatern, "555555");
            Assert.AreEqual(result, indexExp);
            result = AddressResult.ReplaceSubstring(indexinPatern, "555555");
            Assert.AreEqual(result, indexinExp);
        }
        [Test]
        public void SimpleUsageTest()
        {
            var address = "426011, Удмуртская Республика, г. Ижевск, 10 Лет Октября,17 а, номера помещений на поэтажном плане 122-137";
            var parser = new AddressParser();
            var normalized = parser.Parse(address);
        }
        [Test]
        public void ReplaceTest()
        {
            var address = "Нерезиновск, ул. Большая ордынка, д. 5";
            var exp = "Москва, ул. Большая ордынка, д. 5";
            var parser = new AddressParser();
            var normalized = parser.Parse(address);
            Assert.AreEqual(exp, normalized.Raw);
        }
        [Test]
        public void ReplaceExTest()
        {
            var address = "100100, Нерезиновск, ул. Большая ордынка, д. 5";
            var exp = "100100, Москва, ул. Большая ордынка, д. 5";
            var result = address.ReplaceIgnoreCase("НеРезиновск", "Москва");
            
            Assert.AreEqual(exp, result);
        }
        [Test]
        public void FixerTest()
        {
            var address = "197373, г. Ленинград, Глухарская улица, дом 16, корпус 2, литера А";
            var exp = "197373, г. Санкт-Петербург, Глухарская улица, дом 16, корпус 2, литера А";
            var parser = new AddressParser();
            parser.AddFixer(new LeningradFixer());
            var normalized = parser.Parse(address);
            Assert.AreEqual(exp, normalized.Raw);
        }

        public class LeningradFixer : IAddressFix
        {
            public string Fix(string address) => address.ReplaceIgnoreCase("Ленинград", "Санкт-Петербург");
        }
    }
}