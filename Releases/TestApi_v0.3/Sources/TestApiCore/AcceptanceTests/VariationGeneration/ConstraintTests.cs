// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Test.VariationGeneration;
using Microsoft.Test.VariationGeneration.Constraints;
using Xunit;

namespace Microsoft.Test.AcceptanceTests
{
    public class ConstraintTests
    {
        [Fact]
        public void BasicConstraintTest()
        {
            Parameter p1 = new Parameter("P1") { "one", "two" };
            Parameter p2 = new Parameter("P2") { 1, 2 };

            var parameters = new List<Parameter>
            {
                p1,
                p2,
                new Parameter("P3")
                {
                    1.0,
                    2.0
                }
            };

            var constraints = new List<Constraint>
            {
                p1.Equal("one").And(p2.Equal(2))
            };

            Model model = new Model(parameters, constraints);

            Assert.True(model.GenerateVariations().Count() == 2);
            Assert.True(model.GenerateVariations().All((v) => v["P1"].Equals("one")));
            Assert.True(model.GenerateVariations().All((v) => v["P2"].Equals(2)));
        }

        [Fact]
        public void DependentConstraintTest()
        {
            Parameter a = new Parameter("A")
            {
                "A0",
                "A1",
            };

            Parameter b = new Parameter("B")
            {
                "B0",
                "B1"
            };

            Parameter c = new Parameter("C")
            {
                "C0",
                "C1"
            };

            var parameters = new List<Parameter> { a, b, c };

            var constraints = new List<Constraint>
            {
                new IfThenConstraint
                {
                    If = a.Equal("A0"),
                    Then = b.Equal("B0")
                },
                new IfThenConstraint
                {
                    If = b.Equal("B0"),
                    Then = c.Equal("C0")
                },
                new IfThenConstraint
                {
                    If = c.Equal("C0"),
                    Then = a.Equal("A1")
                }
            };

            Model model = new Model(parameters, constraints);
            
            Assert.True(model.GenerateVariations().Count() == 3);
            // A0 should not appear in any variations
            Assert.False(model.GenerateVariations().Any((v) => v["A"].Equals("A0")));
        }

        [Fact]
        public void LargeConstraintTest()
        {
            Parameter type = new Parameter("Type")
            {
                "Single", "Spanned", "Striped", "Mirror", "RAID-5"
            };

            Parameter fileSystem = new Parameter("File System")
            {
                "FAT", "FAT32", "NTFS"
            };

            Parameter size = new Parameter("Size")
            {
                10, 100, 1000, 10000, 40000
            };

            Parameter clusterSize = new Parameter("Cluster Size")
            {
                512, 1024, 2048, 4096, 8192, 16384
            };

            var parameters = new List<Parameter>
            {
                type,
                size,
                new Parameter("Format Method")
                {
                    FormatMethod.Quick, FormatMethod.Slow
                },
                fileSystem,
                clusterSize,
                new Parameter("Compression")
                {
                    true, false
                }
            };

            var constraints = new List<Constraint>
            {
                new IfThenConstraint
                {
                    If = fileSystem.Equal("NTFS"),
                    Then = size.GreaterThan(1000)
                },
                new IfThenConstraint
                {
                    If = clusterSize.LessThan(2048),
                    Then = type.NotEqual("Mirror")
                },
                new IfThenConstraint
                {
                    If = fileSystem.Equal("FAT").And(type.NotEqual("Spanned")),
                    Then = size.Equal(100).Or(clusterSize.GreaterThanOrEqual(4096))
                }
            };

            Model model = new Model(parameters, constraints);

            Assert.True(model.GenerateVariations().Count() == 34);
            // if file system == NTFS then size > 1000
            Assert.True(model.GenerateVariations()
                .Where((v) => v[fileSystem.Name].Equals("NTFS"))
                .All((v) => (int)v[size.Name] > 1000));

            // if cluster size < 2048 then type != Mirror
            Assert.True(model.GenerateVariations()
                .Where((v) => (int)v[clusterSize.Name] < 2048)
                .All((v) => !v[type.Name].Equals("Mirror")));

            // if file system == FAT and type == Spanned then  size == 100 or clusterSize >= 4096
            Assert.True(model.GenerateVariations()
                .Where((v) => v[fileSystem.Name].Equals("FAT") && !v[type.Name].Equals("Spanned"))
                .All((v) => (int)v[size.Name] == 100 || (int)v[clusterSize.Name] >= 4096));
        }

        [Fact]
        public void NonConditionalConstraintTest()
        {
            Parameter os1 = new Parameter("OS1") { "Vista", "Win7" };
            Parameter os2 = new Parameter("OS2") { "Vista", "Win7" };

            Parameter sku1 = new Parameter("SKU1") { "Enterprise", "Home Basic", "Home Premium", "Ultimate" };
            Parameter sku2 = new Parameter("SKU2") { "Enterprise", "Home Basic", "Home Premium", "Ultimate" };

            Parameter lang1 = new Parameter("Lang1") { "EN", "DEU", "JPN" };
            Parameter lang2 = new Parameter("Lang2") { "EN", "DEU", "JPN" };

            var parameters = new List<Parameter> { os1, os2, sku1, sku2, lang1, lang2 };
            var constraints = new List<Constraint> { os1.NotEqual(os2).And(sku1.NotEqual(sku2).And(lang1.NotEqual(lang2))) };

            Model model = new Model(parameters, constraints);

            var expectedVariations = new List<string>
            {
                "Vista Win7 Enterprise Home Basic EN DEU",
                "Win7 Vista Enterprise Home Premium DEU EN",
                "Vista Win7 Enterprise Ultimate JPN EN",
                "Vista Win7 Enterprise Home Premium DEU JPN",
                "Win7 Vista Home Basic Enterprise EN JPN",
                "Win7 Vista Home Basic Home Premium JPN DEU",
                "Vista Win7 Home Basic Ultimate DEU EN",
                "Vista Win7 Home Premium Enterprise DEU EN",
                "Win7 Vista Home Premium Home Basic JPN DEU",
                "Vista Win7 Ultimate Enterprise JPN DEU",
                "Win7 Vista Ultimate Home Basic DEU EN",
                "Vista Win7 Home Premium Home Basic EN JPN",
                "Win7 Vista Home Premium Ultimate EN DEU",
                "Vista Win7 Ultimate Home Premium EN JPN",
                "Win7 Vista Home Basic Ultimate EN JPN"
            };

            var actualVariations = ModelTests.WriteVariations(model.GenerateVariations());

            Assert.True(expectedVariations.Count == actualVariations.Count, "Expected: " + expectedVariations.Count + " Actual: " + actualVariations.Count);

            for (int i = 0; i < expectedVariations.Count; i++)
            {
                Assert.True(expectedVariations[i] == actualVariations[i], "Expected: " + expectedVariations[i] + " Actual: " + actualVariations[i]);
            }
        }

        [Fact]
        public void EmptyConstraintTest()
        {
            Parameter a = new Parameter("A")
            {
                "A0",
                "A1",
            };

            Parameter b = new Parameter("B")
            {
                "B0",
                "B1"
            };

            Parameter c = new Parameter("C")
            {
                "C0",
                "C1"
            };

            var parameters = new List<Parameter> { a, b, c };
            var constraints = new List<Constraint> { a.NotEqual(a) };

            Model model = new Model(parameters, constraints);

            Assert.True(model.GenerateVariations().Count() == 0);
        }

        [Fact]
        public void DependentConstraintTest2()
        {
            var clientType = new Parameter("ClientType") { "Win2k", "WinXP", "Win2k3" };

            var serverType = new Parameter("ServerType") 
            {
                "SameMachineSameProc", "SameMachineDiffProcs", "Win2kAdvancedServer", "XPPro", "NetServer"
            };

            var accounts = new Parameter("Accounts")
            { 
                "ClientRegularUserServerAdministrator", "BothAdministrator", "ClientRegularUserServerLocalSystem"
            };

            var communicationStyle = new Parameter("CommunicationStyle")
            {
                "InProc", "XProc", "DirectTcp", "RoutedTcp", "RoutedInProc"
            };

            var encryptionServer = new Parameter("EncryptionServer")
            {
                "None", "UsernamePassword", "WinAuth", "X509"
            };

            var signatureRequirements = new Parameter("SignatureRequirements")
            {
                "None", "UsernamePassword", "WinAuth", "X509", "Any", "All"
            };

            var tokenAvailability = new Parameter("TokenAvailability")
            {
                "None","UsernamePassword_Get","UsernamePassword_Set","WinAuth_Get","WinAuth_Set","X509_Get","X509_Set","All_Get","All_Set"
            };

            var permissionsTokenIssuance = new Parameter("PermissionsTokenIssuance") { true, false };

            var testMethodName = new Parameter("TestMethodName") { "Get", "Set", "GetNegative" };

            var parameters = new List<Parameter>
            {
                clientType,
                serverType,
                accounts,
                communicationStyle,
                encryptionServer,
                signatureRequirements,
                tokenAvailability,
                permissionsTokenIssuance,
                testMethodName
            };

            var constraints = new List<Constraint>
            {
                new IfThenConstraint
                {
                    If = communicationStyle.Equal("InProc").Or(communicationStyle.Equal("XProc")),
                    Then = serverType.Equal("SameMachineSameProc")
                },
                new IfThenConstraint
                {
                    If = serverType.Equal("SameMachineSameProc")
                            .And(tokenAvailability.Equal("WinAuth_Get")
                                .Or(tokenAvailability.Equal("All_Get"))),
                    Then = accounts.Equal("BothAdministrator")
                }
            };

            Model model = new Model(parameters, constraints);

            var variations = model.GenerateVariations();
            Assert.True(variations.Count() == 58);
            Assert.True(variations
                .Where((v) => (v[communicationStyle.Name].Equals("InProc") || v[communicationStyle.Name].Equals("XProc"))
                    && (v[tokenAvailability.Name].Equals("WinAuth_Get") || v[tokenAvailability.Name].Equals("All_Get")))
                .All((v) => v[accounts.Name].Equals("BothAdministrator")));
                
        }

        [Fact]
        public void DependentConstraintTest3()
        {
            var clientType = new Parameter("ClientType") { "Win2k", "WinXP", "Win2k3" };

            var serverType = new Parameter("ServerType") 
            {
                "SameMachineSameProc", "SameMachineDiffProcs", "Win2kAdvancedServer", "XPPro", "NetServer"
            };

            var accounts = new Parameter("Accounts")
            { 
                "ClientRegularUserServerAdministrator", "BothAdministrator", "ClientRegularUserServerLocalSystem"
            };

            var communicationStyle = new Parameter("CommunicationStyle")
            {
                "InProc", "XProc", "DirectTcp", "RoutedTcp", "RoutedInProc"
            };

            var encryptionServer = new Parameter("EncryptionServer")
            {
                "None", "UsernamePassword", "WinAuth", "X509"
            };

            var signatureRequirements = new Parameter("SignatureRequirements")
            {
                "None", "UsernamePassword", "WinAuth", "X509", "Any", "All"
            };

            var tokenAvailability = new Parameter("TokenAvailability")
            {
                "None","UsernamePassword_Get","UsernamePassword_Set","WinAuth_Get","WinAuth_Set","X509_Get","X509_Set","All_Get","All_Set"
            };

            var permissionsTokenIssuance = new Parameter("PermissionsTokenIssuance") { true, false };

            var testMethodName = new Parameter("TestMethodName") { "Get", "Set", "GetNegative" };

            var parameters = new List<Parameter>
            {
                clientType,
                serverType,
                accounts,
                communicationStyle,
                encryptionServer,
                signatureRequirements,
                tokenAvailability,
                permissionsTokenIssuance,
                testMethodName
            };

            var constraints = new List<Constraint>
            {
                new IfThenConstraint
                {
                    If = signatureRequirements.Equal("X509"),
                    Then = tokenAvailability.Equal("X509_Get")
                            .Or(tokenAvailability.Equal("All_Get"))

                },
                new IfThenConstraint
                {
                    If = signatureRequirements.NotEqual("All"),
                    Then = tokenAvailability.NotEqual("None")
                },
                new IfThenConstraint
                {
                    If = signatureRequirements.Equal("All").Or(signatureRequirements.Equal("None")),
                    Then = permissionsTokenIssuance.Equal(false)
                },
            };

            Model model = new Model(parameters, constraints);

            var variations = model.GenerateVariations();
            Assert.True(variations.Count() == 55);
            Assert.True(variations
                .Where((v) => v[tokenAvailability.Name].Equals("None"))
                .All((v) => v[permissionsTokenIssuance.Name].Equals(false)));                

        }

        [Fact]
        public void SampleTest()
        {
            var destination = new Parameter("Destination") { "Whistler", "Hawaii", "Las Vegas" };

            var hotelQuality = new Parameter("Hotel Quality") { 5, 4, 3, 2, 1 };

            var activity = new Parameter("Activity") { "gambling", "swimming", "shopping", "skiing" };

            var parameters = new List<Parameter> { destination, hotelQuality, activity };
            var constraints = new List<Constraint>
            {
                new IfThenConstraint
                {
                    If = destination.Equal("Whistler").Or(destination.Equal("Hawaii")),
                    Then = activity.NotEqual("gambling")                        
                },
                new IfThenConstraint
                {
                    If = destination.Equal("Las Vegas").Or(destination.Equal("Hawaii")),
                    Then = activity.NotEqual("skiing")
                },
                new IfThenConstraint
                {
                    If = destination.Equal("Whistler"),
                    Then = activity.NotEqual("swimming")
                },
            };

            Model model = new Model(parameters, constraints);
            var variations = model.GenerateVariations();

            Assert.True(variations.Count() == 21);

            Assert.True(variations
                .Where((v) => v[destination.Name].Equals("Whistler") || v[destination.Name].Equals("Hawaii"))
                .All((v) => !v[activity.Name].Equals("gambling")));

            Assert.True(variations
               .Where((v) => v[destination.Name].Equals("Las Vegas") || v[destination.Name].Equals("Hawaii"))
               .All((v) => !v[activity.Name].Equals("skiing")));

            Assert.True(variations
              .Where((v) => v[destination.Name].Equals("Whistler"))
              .All((v) => !v[activity.Name].Equals("swimming")));
        }
    }


}
