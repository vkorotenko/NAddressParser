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
            var countryPatern = "������, �����, 45";
            var country = "������";
            var countryExp = "�����, 45";
            
            var indexPatern  = "������, �����, 45, 555555";
            var indexExp = "������, �����, 45";

            var indexinPatern = "������, �����, 45, 555555, ���";
            var indexinExp = "������, �����, 45, , ���";

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
            var address = "426011, ���������� ����������, �. ������, 10 ��� �������,17 �, ������ ��������� �� ��������� ����� 122-137";
            var parser = new AddressParser();
            var normalized = parser.Parse(address);
        }
        [Test]
        public void ReplaceTest()
        {
            var address = "�����������, ��. ������� �������, �. 5";
            var exp = "������, ��. ������� �������, �. 5";
            var parser = new AddressParser();
            var normalized = parser.Parse(address);
            Assert.AreEqual(exp, normalized.Raw);
        }
        [Test]
        public void ReplaceExTest()
        {
            var address = "100100, �����������, ��. ������� �������, �. 5";
            var exp = "100100, ������, ��. ������� �������, �. 5";
            var result = address.ReplaceIgnoreCase("�����������", "������");
            
            Assert.AreEqual(exp, result);
        }
        [Test]
        public void FixerTest()
        {
            var address = "197373, �. ���������, ���������� �����, ��� 16, ������ 2, ������ �";
            var exp = "197373, �. �����-���������, ���������� �����, ��� 16, ������ 2, ������ �";
            var parser = new AddressParser();
            parser.AddFixer(new LeningradFixer());
            var normalized = parser.Parse(address);
            Assert.AreEqual(exp, normalized.Raw);
        }

        public class LeningradFixer : IAddressFix
        {
            public string Fix(string address) => address.ReplaceIgnoreCase("���������", "�����-���������");
        }
    }
}