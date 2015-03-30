using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace FluidSqlTests
{
    [TestClass]
    public class NameTests
    {
        [TestMethod]
        public void TestNameParsing()
        {
            var name = new Name("p1.[p2].\"p3\".`p4`");
            for (int i = 0; i < name.Count; i++)
            {
                Assert.AreEqual("p" + (i + 1), name[i]);
            }
            Assert.AreEqual("[p1].[p2].[p3].[p4]", name.GetFullName("[", "]"));
        }


        [TestMethod]
        public void TestNameFromStrings()
        {
            var name00 = new Name();
            Assert.IsNull(name00.LastPart);
            Assert.IsNull(name00.FirstPart);
            Assert.AreEqual("",name00.GetFullName());

            name00.Add("p1");
            Assert.AreEqual("[p1]", name00.GetFullName("[","]"));

            name00.Add("p2", "p3");
            Assert.AreEqual("[p1].[p2].[p3]", name00.GetFullName("[", "]"));
            Assert.AreEqual("p1", name00.FirstPart);
            Assert.AreEqual("p3", name00.LastPart);

            var name01 = new Name();
            Assert.IsNull(name01.LastPart);
            Assert.IsNull(name01.FirstPart);
            Assert.AreEqual("", name01.GetFullName());

            name01.Add("p1", "p2", "p3");
            Assert.AreEqual("[p1].[p2].[p3]", name01.GetFullName("[", "]"));
            Assert.AreEqual("p1", name01.FirstPart);
            Assert.AreEqual("p3", name01.LastPart);

            var name02 = new Name();
            name02.Add("p1", "p2", "p3", "p4");
            Assert.AreEqual("[p1].[p2].[p3].[p4]", name02.GetFullName("[", "]"));
            Assert.AreEqual("p1", name02.FirstPart);
            Assert.AreEqual("p4", name02.LastPart);

            var name03 = new Name();
            name03.Add(new []{"p1", "p2", "p3", "p4"});
            Assert.AreEqual("[p1].[p2].[p3].[p4]", name03.GetFullName("[", "]"));
            Assert.AreEqual("p1", name03.FirstPart);
            Assert.AreEqual("p4", name03.LastPart);

            var name04 = new Name();
            name04.Add("p1.p2.p3.p4");
            Assert.AreEqual("[p1].[p2].[p3].[p4]", name04.GetFullName("[", "]"));
            Assert.AreEqual("p1", name04.FirstPart);
            Assert.AreEqual("p4", name04.LastPart);


            var basename = new Name("b1.b2");
            var name05 = new Name();
            name05.Add(basename,"p1.p2");
            Assert.AreEqual("[b1].[b2].[p1].[p2]", name05.GetFullName("[", "]"));
            Assert.AreEqual("b1", name05.FirstPart);
            Assert.AreEqual("p2", name05.LastPart);

            name05 = new Name();
            name05.Add(basename, "p1","p2");
            Assert.AreEqual("[b1].[b2].[p1].[p2]", name05.GetFullName("[", "]"));
            Assert.AreEqual("b1", name05.FirstPart);
            Assert.AreEqual("p2", name05.LastPart);

            name05 = new Name();
            name05.Add("p1.p2",basename);
            Assert.AreEqual("[p1].[p2].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.AreEqual("p1", name05.FirstPart);
            Assert.AreEqual("b2", name05.LastPart);

            name05 = new Name();
            name05.Add("p1","p2", basename);
            Assert.AreEqual("[p1].[p2].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.AreEqual("p1", name05.FirstPart);
            Assert.AreEqual("b2", name05.LastPart);

            name05 = new Name();
            name05.Add("p1", "p2","p3", basename);
            Assert.AreEqual("[p1].[p2].[p3].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.AreEqual("p1", name05.FirstPart);
            Assert.AreEqual("b2", name05.LastPart);

            name05 = new Name();
            name05.Add("p1", "p2.p3", basename);
            Assert.AreEqual("[p1].[p2].[p3].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.AreEqual("p1", name05.FirstPart);
            Assert.AreEqual("b2", name05.LastPart);
        }

        [TestMethod]
        public void TestNameFromStringsCtors()
        {
            var name00 = new Name("p1");
            Assert.AreEqual("[p1]", name00.GetFullName("[", "]"));
            Assert.AreEqual("p1", name00.FirstPart);
            Assert.AreEqual("p1", name00.LastPart);

            name00 = new Name("p1", "p2", "p3");
            Assert.AreEqual("[p1].[p2].[p3]", name00.GetFullName("[", "]"));
            Assert.AreEqual("p1", name00.FirstPart);
            Assert.AreEqual("p3", name00.LastPart);

            name00 = new Name("p1", "p2", "p3", "p4");
            Assert.AreEqual("[p1].[p2].[p3].[p4]", name00.GetFullName("[", "]"));
            Assert.AreEqual("p1", name00.FirstPart);
            Assert.AreEqual("p4", name00.LastPart);

            name00 = new Name(new[] { "p1", "p2", "p3", "p4" });
            Assert.AreEqual("[p1].[p2].[p3].[p4]", name00.GetFullName("[", "]"));
            Assert.AreEqual("p1", name00.FirstPart);
            Assert.AreEqual("p4", name00.LastPart);

            name00 = new Name("p1.p2.p3.p4");
            Assert.AreEqual("[p1].[p2].[p3].[p4]", name00.GetFullName("[", "]"));
            Assert.AreEqual("p1", name00.FirstPart);
            Assert.AreEqual("p4", name00.LastPart);

            var basename = new Name("b1.b2");
            var name05 = new Name();
            name05.Add(basename, "p1.p2");
            Assert.AreEqual("[b1].[b2].[p1].[p2]", name05.GetFullName("[", "]"));
            Assert.AreEqual("b1", name05.FirstPart);
            Assert.AreEqual("p2", name05.LastPart);

            name05 = new Name(basename, "p1", "p2");
            Assert.AreEqual("[b1].[b2].[p1].[p2]", name05.GetFullName("[", "]"));
            Assert.AreEqual("b1", name05.FirstPart);
            Assert.AreEqual("p2", name05.LastPart);

            name05 = new Name("p1.p2", basename);
            Assert.AreEqual("[p1].[p2].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.AreEqual("p1", name05.FirstPart);
            Assert.AreEqual("b2", name05.LastPart);

            name05 = new Name("p1", "p2", basename);
            Assert.AreEqual("[p1].[p2].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.AreEqual("p1", name05.FirstPart);
            Assert.AreEqual("b2", name05.LastPart);

            name05 = new Name("p1", "p2", "p3", basename);
            Assert.AreEqual("[p1].[p2].[p3].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.AreEqual("p1", name05.FirstPart);
            Assert.AreEqual("b2", name05.LastPart);

            name05 = new Name("p1", "p2.p3", basename);
            Assert.AreEqual("[p1].[p2].[p3].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.AreEqual("p1", name05.FirstPart);
            Assert.AreEqual("b2", name05.LastPart);
        }
    }
}
