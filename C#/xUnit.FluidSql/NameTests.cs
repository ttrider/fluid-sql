// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using Xunit;

namespace xUnit.FluidSql
{
    public class NameTests
    {
        [Fact]
        public void TestNameParsing()
        {
            var name = new Name("p1.[p2].\"p3\".`p4`");
            for (var i = 0; i < name.Count; i++)
            {
                Assert.Equal("p" + (i + 1), name[i]);
            }
            Assert.Equal("[p1].[p2].[p3].[p4]", name.GetFullName("[", "]"));
        }

        [Fact]
        public void TestNameFromStrings()
        {
            var name00 = new Name();
            Assert.Null(name00.LastPart);
            Assert.Null(name00.FirstPart);
            Assert.Equal("", name00.GetFullName());

            name00.Add("p1");
            Assert.Equal("[p1]", name00.GetFullName("[", "]"));

            name00.Add("p2", "p3");
            Assert.Equal("[p1].[p2].[p3]", name00.GetFullName("[", "]"));
            Assert.Equal("p1", name00.FirstPart);
            Assert.Equal("p3", name00.LastPart);

            var name01 = new Name();
            Assert.Null(name01.LastPart);
            Assert.Null(name01.FirstPart);
            Assert.Equal("", name01.GetFullName());

            name01.Add("p1", "p2", "p3");
            Assert.Equal("[p1].[p2].[p3]", name01.GetFullName("[", "]"));
            Assert.Equal("p1", name01.FirstPart);
            Assert.Equal("p3", name01.LastPart);

            var name02 = new Name();
            name02.Add("p1", "p2", "p3", "p4");
            Assert.Equal("[p1].[p2].[p3].[p4]", name02.GetFullName("[", "]"));
            Assert.Equal("p1", name02.FirstPart);
            Assert.Equal("p4", name02.LastPart);

            var name03 = new Name();
            name03.Add("p1", "p2", "p3", "p4");
            Assert.Equal("[p1].[p2].[p3].[p4]", name03.GetFullName("[", "]"));
            Assert.Equal("p1", name03.FirstPart);
            Assert.Equal("p4", name03.LastPart);

            var name04 = new Name();
            name04.Add("p1.p2.p3.p4");
            Assert.Equal("[p1].[p2].[p3].[p4]", name04.GetFullName("[", "]"));
            Assert.Equal("p1", name04.FirstPart);
            Assert.Equal("p4", name04.LastPart);


            var basename = new Name("b1.b2");
            var name05 = new Name();
            name05.Add(basename, "p1.p2");
            Assert.Equal("[b1].[b2].[p1].[p2]", name05.GetFullName("[", "]"));
            Assert.Equal("b1", name05.FirstPart);
            Assert.Equal("p2", name05.LastPart);

            name05 = new Name();
            name05.Add(basename, "p1", "p2");
            Assert.Equal("[b1].[b2].[p1].[p2]", name05.GetFullName("[", "]"));
            Assert.Equal("b1", name05.FirstPart);
            Assert.Equal("p2", name05.LastPart);

            name05 = new Name();
            name05.Add("p1.p2", basename);
            Assert.Equal("[p1].[p2].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.Equal("p1", name05.FirstPart);
            Assert.Equal("b2", name05.LastPart);

            name05 = new Name();
            name05.Add("p1", "p2", basename);
            Assert.Equal("[p1].[p2].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.Equal("p1", name05.FirstPart);
            Assert.Equal("b2", name05.LastPart);

            name05 = new Name();
            name05.Add("p1", "p2", "p3", basename);
            Assert.Equal("[p1].[p2].[p3].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.Equal("p1", name05.FirstPart);
            Assert.Equal("b2", name05.LastPart);

            name05 = new Name();
            name05.Add("p1", "p2.p3", basename);
            Assert.Equal("[p1].[p2].[p3].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.Equal("p1", name05.FirstPart);
            Assert.Equal("b2", name05.LastPart);
        }

        [Fact]
        public void TestNameFromStringsCtors()
        {
            var name00 = new Name("p1");
            Assert.Equal("[p1]", name00.GetFullName("[", "]"));
            Assert.Equal("p1", name00.FirstPart);
            Assert.Equal("p1", name00.LastPart);

            name00 = new Name("p1", "p2", "p3");
            Assert.Equal("[p1].[p2].[p3]", name00.GetFullName("[", "]"));
            Assert.Equal("p1", name00.FirstPart);
            Assert.Equal("p3", name00.LastPart);

            name00 = new Name("p1", "p2", "p3", "p4");
            Assert.Equal("[p1].[p2].[p3].[p4]", name00.GetFullName("[", "]"));
            Assert.Equal("p1", name00.FirstPart);
            Assert.Equal("p4", name00.LastPart);

            name00 = new Name("p1", "p2", "p3", "p4");
            Assert.Equal("[p1].[p2].[p3].[p4]", name00.GetFullName("[", "]"));
            Assert.Equal("p1", name00.FirstPart);
            Assert.Equal("p4", name00.LastPart);

            name00 = new Name("p1.p2.p3.p4");
            Assert.Equal("[p1].[p2].[p3].[p4]", name00.GetFullName("[", "]"));
            Assert.Equal("p1", name00.FirstPart);
            Assert.Equal("p4", name00.LastPart);

            var basename = new Name("b1.b2");
            var name05 = new Name();
            name05.Add(basename, "p1.p2");
            Assert.Equal("[b1].[b2].[p1].[p2]", name05.GetFullName("[", "]"));
            Assert.Equal("b1", name05.FirstPart);
            Assert.Equal("p2", name05.LastPart);

            name05 = new Name(basename, "p1", "p2");
            Assert.Equal("[b1].[b2].[p1].[p2]", name05.GetFullName("[", "]"));
            Assert.Equal("b1", name05.FirstPart);
            Assert.Equal("p2", name05.LastPart);

            name05 = new Name("p1.p2", basename);
            Assert.Equal("[p1].[p2].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.Equal("p1", name05.FirstPart);
            Assert.Equal("b2", name05.LastPart);

            name05 = new Name("p1", "p2", basename);
            Assert.Equal("[p1].[p2].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.Equal("p1", name05.FirstPart);
            Assert.Equal("b2", name05.LastPart);

            name05 = new Name("p1", "p2", "p3", basename);
            Assert.Equal("[p1].[p2].[p3].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.Equal("p1", name05.FirstPart);
            Assert.Equal("b2", name05.LastPart);

            name05 = new Name("p1", "p2.p3", basename);
            Assert.Equal("[p1].[p2].[p3].[b1].[b2]", name05.GetFullName("[", "]"));
            Assert.Equal("p1", name05.FirstPart);
            Assert.Equal("b2", name05.LastPart);
        }
    }
}