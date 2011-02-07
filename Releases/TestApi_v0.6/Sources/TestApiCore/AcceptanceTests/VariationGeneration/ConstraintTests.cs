// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.VariationGeneration;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.VariationGeneration
{
    public class ConstraintTests
    {
        [Fact]
        public void BasicConstraintTest()
        {
            var p1 = new Parameter<string>("P1") { "one", "two" };
            var p2 = new Parameter<int>("P2") { 1, 2 };

            var parameters = new List<ParameterBase>
            {
                p1,
                p2,
                new Parameter<double>("P3")
                {
                    1.0,
                    2.0
                }
            };

            var constraints = new List<Constraint<Variation>>
            {
                Constraint<Variation>.Conditional(v => p1.GetValue(v) == "one" && p2.GetValue(v) == 2)
            };

            Model model = new Model(parameters, constraints);

            Assert.True(model.GenerateVariations(2).Count() == 2);
            Assert.True(model.GenerateVariations(2).All((v) => v["P1"].Equals("one")));
            Assert.True(model.GenerateVariations(2).All((v) => v["P2"].Equals(2)));
        }

        [Fact]
        public void DependentConstraintTest()
        {
            var a = new Parameter<string>("A")
            {
                "A0",
                "A1",
            };

            var b = new Parameter<string>("B")
            {
                "B0",
                "B1"
            };

            var c = new Parameter<string>("C")
            {
                "C0",
                "C1"
            };

            var parameters = new List<ParameterBase> { a, b, c };

            var constraints = new List<Constraint<Variation>>
            {
                Constraint<Variation>
                    .If(v => a.GetValue(v) == "A0")
                    .Then(v => b.GetValue(v) == "B0"),
                 Constraint<Variation>
                    .If(v => b.GetValue(v) == "B0")
                    .Then(v => c.GetValue(v) == "C0"),
                 Constraint<Variation>
                    .If(v => c.GetValue(v) == "C0")
                    .Then(v => a.GetValue(v) == "A1"),
            };

            Model model = new Model(parameters, constraints);

            Assert.True(model.GenerateVariations(2).Count() == 3);
            // A0 should not appear in any variations
            Assert.False(model.GenerateVariations(2).Any((v) => v["A"].Equals("A0")));
        }

        [Fact]
        public void LargeConstraintTest()
        {
            var type = new Parameter<string>("Type")
            {
                "Single", "Spanned", "Striped", "Mirror", "RAID-5"
            };

            var fileSystem = new Parameter<string>("File System")
            {
                "FAT", "FAT32", "NTFS"
            };

            var size = new Parameter<int>("Size")
            {
                10, 100, 1000, 10000, 40000
            };

            var clusterSize = new Parameter<int>("Cluster Size")
            {
                512, 1024, 2048, 4096, 8192, 16384
            };

            var parameters = new List<ParameterBase>
            {
                type,
                size,
                new Parameter<FormatMethod>("Format Method")
                {
                    FormatMethod.Quick, FormatMethod.Slow
                },
                fileSystem,
                clusterSize,
                new Parameter<bool>("Compression")
                {
                    true, false
                }
            };

            var constraints = new List<Constraint<Variation>>
            {
                Constraint<Variation>
                    .If(v => fileSystem.GetValue(v) == "NTFS")
                    .Then(v => size.GetValue(v) > 1000),
                Constraint<Variation>
                    .If(v => clusterSize.GetValue(v) < 2048)
                    .Then(v => type.GetValue(v) != "Mirror"),
                Constraint<Variation>
                    .If(v => fileSystem.GetValue(v) == "FAT" && type.GetValue(v) != "Spanned")
                    .Then(v => size.GetValue(v) == 100 || clusterSize.GetValue(v) >= 4096),
            };

            Model model = new Model(parameters, constraints);

            Assert.True(model.GenerateVariations(2).Count() == 34);
            // if file system == NTFS then size > 1000
            Assert.True(model.GenerateVariations(2)
                .Where((v) => v[fileSystem.Name].Equals("NTFS"))
                .All((v) => (int)v[size.Name] > 1000));

            // if cluster size < 2048 then type != Mirror
            Assert.True(model.GenerateVariations(2)
                .Where((v) => (int)v[clusterSize.Name] < 2048)
                .All((v) => !v[type.Name].Equals("Mirror")));

            // if file system == FAT and type == Spanned then  size == 100 or clusterSize >= 4096
            Assert.True(model.GenerateVariations(2)
                .Where((v) => v[fileSystem.Name].Equals("FAT") && !v[type.Name].Equals("Spanned"))
                .All((v) => (int)v[size.Name] == 100 || (int)v[clusterSize.Name] >= 4096));
        }

        [Fact]
        public void NonConditionalConstraintTest()
        {
            var os1 = new Parameter<string>("OS1") { "Vista", "Win7" };
            var os2 = new Parameter<string>("OS2") { "Vista", "Win7" };

            var sku1 = new Parameter<string>("SKU1") { "Enterprise", "Home Basic", "Home Premium", "Ultimate" };
            var sku2 = new Parameter<string>("SKU2") { "Enterprise", "Home Basic", "Home Premium", "Ultimate" };

            var lang1 = new Parameter<string>("Lang1") { "EN", "DEU", "JPN" };
            var lang2 = new Parameter<string>("Lang2") { "EN", "DEU", "JPN" };

            var parameters = new List<ParameterBase> { os1, os2, sku1, sku2, lang1, lang2 };
            var constraints = new List<Constraint<Variation>> 
            { 
                Constraint<Variation>.Conditional(v => os1.GetValue(v) != os2.GetValue(v) && sku1.GetValue(v) != sku2.GetValue(v) && lang1.GetValue(v) != lang2.GetValue(v))
            };

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
                "Win7 Vista Home Premium Ultimate EN JPN"
            };

            var actualVariations = ModelTests.WriteVariations(model.GenerateVariations(2));

            Assert.True(expectedVariations.Count == actualVariations.Count, "Expected: " + expectedVariations.Count + " Actual: " + actualVariations.Count);

            for (int i = 0; i < expectedVariations.Count; i++)
            {
                Assert.True(expectedVariations[i] == actualVariations[i], "Expected: " + expectedVariations[i] + " Actual: " + actualVariations[i]);
            }
        }

        [Fact]
        public void NonConditionalConstraintTestWithCustomVariation()
        {
            var os1 = new Parameter<string>("OS1") { "Vista", "Win7" };
            var os2 = new Parameter<string>("OS2") { "Vista", "Win7" };

            var sku1 = new Parameter<string>("SKU1") { "Enterprise", "Home Basic", "Home Premium", "Ultimate" };
            var sku2 = new Parameter<string>("SKU2") { "Enterprise", "Home Basic", "Home Premium", "Ultimate" };

            var lang1 = new Parameter<string>("Lang1") { "EN", "DEU", "JPN" };
            var lang2 = new Parameter<string>("Lang2") { "EN", "DEU", "JPN" };

            var parameters = new List<ParameterBase> { os1, os2, sku1, sku2, lang1, lang2 };
            var constraints = new List<Constraint<UpgradeVariation>> 
            { 
                Constraint<UpgradeVariation>.Conditional(v => v.OS1 != v.OS2 && v.SKU1 != v.SKU2 && v.Lang1 != v.Lang2)
            };

            var model = new Model<UpgradeVariation>(parameters, constraints);

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
                "Win7 Vista Home Premium Ultimate EN JPN"
            };

            var actualVariations = UpgradeVariation.WriteVariations(model.GenerateVariations(2));

            Assert.True(expectedVariations.Count == actualVariations.Count, "Expected: " + expectedVariations.Count + " Actual: " + actualVariations.Count);

            for (int i = 0; i < expectedVariations.Count; i++)
            {
                Assert.True(expectedVariations[i] == actualVariations[i], "Expected: " + expectedVariations[i] + " Actual: " + actualVariations[i]);
            }
        }

        [Fact]
        public void EmptyConstraintTest()
        {
            var a = new Parameter<string>("A")
            {
                "A0",
                "A1",
            };

            var b = new Parameter<string>("B")
            {
                "B0",
                "B1"
            };

            var c = new Parameter<string>("C")
            {
                "C0",
                "C1"
            };

            var parameters = new List<ParameterBase> { a, b, c };
            var constraints = new List<Constraint<Variation>> 
            { 
                Constraint<Variation>.Conditional(v => a.GetValue(v) != a.GetValue(v))
            };

            Model model = new Model(parameters, constraints);

            Assert.True(model.GenerateVariations(2).Count() == 0);
        }

        [Fact]
        public void DependentConstraintTest2()
        {
            var clientType = new Parameter<string>("ClientType") { "Win2k", "WinXP", "Win2k3" };

            var serverType = new Parameter<string>("ServerType") 
            {
                "SameMachineSameProc", "SameMachineDiffProcs", "Win2kAdvancedServer", "XPPro", "NetServer"
            };

            var accounts = new Parameter<string>("Accounts")
            { 
                "ClientRegularUserServerAdministrator", "BothAdministrator", "ClientRegularUserServerLocalSystem"
            };

            var communicationStyle = new Parameter<string>("CommunicationStyle")
            {
                "InProc", "XProc", "DirectTcp", "RoutedTcp", "RoutedInProc"
            };

            var encryptionServer = new Parameter<string>("EncryptionServer")
            {
                "None", "UsernamePassword", "WinAuth", "X509"
            };

            var signatureRequirements = new Parameter<string>("SignatureRequirements")
            {
                "None", "UsernamePassword", "WinAuth", "X509", "Any", "All"
            };

            var tokenAvailability = new Parameter<string>("TokenAvailability")
            {
                "None","UsernamePassword_Get","UsernamePassword_Set","WinAuth_Get","WinAuth_Set","X509_Get","X509_Set","All_Get","All_Set"
            };

            var permissionsTokenIssuance = new Parameter<bool>("PermissionsTokenIssuance") { true, false };

            var testMethodName = new Parameter<string>("TestMethodName") { "Get", "Set", "GetNegative" };

            var parameters = new List<ParameterBase>
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

            var constraints = new List<Constraint<Variation>>
            {
                Constraint<Variation>
                    .If(v => communicationStyle.GetValue(v) == "InProc" || communicationStyle.GetValue(v) == "XProc")
                    .Then(v => serverType.GetValue(v) == "SameMachineSameProc"),
                Constraint<Variation>
                    .If(v => serverType.GetValue(v) == "SameMachineSameProc" && (tokenAvailability.GetValue(v) == "WinAuth_Get" || tokenAvailability.GetValue(v) == "All_Get"))
                    .Then(v => accounts.GetValue(v) == "BothAdministrator"),
            };

            Model model = new Model(parameters, constraints);

            var variations = model.GenerateVariations(2);
            Assert.True(variations.Count() == 58);
            Assert.True(variations
                .Where((v) => (v[communicationStyle.Name].Equals("InProc") || v[communicationStyle.Name].Equals("XProc"))
                    && (v[tokenAvailability.Name].Equals("WinAuth_Get") || v[tokenAvailability.Name].Equals("All_Get")))
                .All((v) => v[accounts.Name].Equals("BothAdministrator")));

        }

        [Fact]
        public void DependentConstraintTest3()
        {
            var clientType = new Parameter<string>("ClientType") { "Win2k", "WinXP", "Win2k3" };

            var serverType = new Parameter<string>("ServerType") 
            {
                "SameMachineSameProc", "SameMachineDiffProcs", "Win2kAdvancedServer", "XPPro", "NetServer"
            };

            var accounts = new Parameter<string>("Accounts")
            { 
                "ClientRegularUserServerAdministrator", "BothAdministrator", "ClientRegularUserServerLocalSystem"
            };

            var communicationStyle = new Parameter<string>("CommunicationStyle")
            {
                "InProc", "XProc", "DirectTcp", "RoutedTcp", "RoutedInProc"
            };

            var encryptionServer = new Parameter<string>("EncryptionServer")
            {
                "None", "UsernamePassword", "WinAuth", "X509"
            };

            var signatureRequirements = new Parameter<string>("SignatureRequirements")
            {
                "None", "UsernamePassword", "WinAuth", "X509", "Any", "All"
            };

            var tokenAvailability = new Parameter<string>("TokenAvailability")
            {
                "None","UsernamePassword_Get","UsernamePassword_Set","WinAuth_Get","WinAuth_Set","X509_Get","X509_Set","All_Get","All_Set"
            };

            var permissionsTokenIssuance = new Parameter<bool>("PermissionsTokenIssuance") { true, false };

            var testMethodName = new Parameter<string>("TestMethodName") { "Get", "Set", "GetNegative" };

            var parameters = new List<ParameterBase>
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

            var constraints = new List<Constraint<Variation>>
            {
                Constraint<Variation>
                    .If(v =>  signatureRequirements.GetValue(v) == "X509")
                    .Then(v =>  tokenAvailability.GetValue(v) == "X509_Get" || tokenAvailability.GetValue(v) == "All_Get"),
                Constraint<Variation>
                    .If(v =>  signatureRequirements.GetValue(v) != "All")
                    .Then(v =>  tokenAvailability.GetValue(v) != "None"),
                Constraint<Variation>
                    .If(v =>  signatureRequirements.GetValue(v) == "All" || signatureRequirements.GetValue(v) == "None")
                    .Then(v =>  permissionsTokenIssuance.GetValue(v) == false),
            };

            Model model = new Model(parameters, constraints);

            var variations = model.GenerateVariations(2);
            Assert.True(variations.Count() == 55);
            Assert.True(variations
                .Where((v) => v[tokenAvailability.Name].Equals("None"))
                .All((v) => v[permissionsTokenIssuance.Name].Equals(false)));

        }

        [Fact]
        public void VeryLargeDependentConstraintSystemTest()
        {
            var client = new Parameter<string>("client") { "w2k", "xppro", "w2k3" };
            var servertype = new Parameter<string>("server") { "SameMachineSameProc", "SameMachineDiffProc", "w2kAS", "XP", "w2k3" };
            var accounts = new Parameter<string>("accounts") { "ClientRUServerA", "BothAdmin", "ClientRUServerLS" };
            var location = new Parameter<string>("location") { "InProc", "XProc", "DirectTcp", "RoutedTcp", "RoutedInProc" };
            var serverEncryption = new Parameter<string>("serverEncryption") { "None", "UP", "WinAuth", "X509" };
            var signature = new Parameter<string>("signature") { "None", "UP", "WinAuth", "X509", "Any", "All" };
            var tokenAvailability = new Parameter<string>("tokenAvailability") { "None", "UP_Get", "UP_Set", "WinAuth_Get", "WinAuth_Set", "X509_Get", "X509_Set", "All_Get", "All_Set" };
            var permissionToken = new Parameter<bool>("permissionToken") { true, false };
            var methodName = new Parameter<string>("methodName") { "Get", "Set", "GetNegative" };

            var parameters = new List<ParameterBase> { client, servertype, accounts, location, serverEncryption, signature, tokenAvailability, permissionToken, methodName };
            var constraints = new List<Constraint<Variation>>
            {
                Constraint<Variation>
                    .If(v => tokenAvailability.GetValue(v) == "None" || signature.GetValue(v) == "None")
                    .Then(v =>  methodName.GetValue(v) == "GetNegative"),
                Constraint<Variation>
                    .If(v => tokenAvailability.GetValue(v) == "UP_Get" || tokenAvailability.GetValue(v) == "WinAuth_Get" || tokenAvailability.GetValue(v) == "X509_Get" || tokenAvailability.GetValue(v) == "All_Get")
                    .Then(v => methodName.GetValue(v) == "Get" || methodName.GetValue(v) == "GetNegative"),
                Constraint<Variation>
                    .If(v => serverEncryption.GetValue(v) == "UP")
                    .Then(v => signature.GetValue(v) == "Any" || signature.GetValue(v) == "UP" || signature.GetValue(v) == "All"),
                Constraint<Variation>
                    .If(v => serverEncryption.GetValue(v) == "WinAuth")
                    .Then(v => signature.GetValue(v) == "Any" || signature.GetValue(v) == "WinAuth" || signature.GetValue(v) == "All"),
                Constraint<Variation>
                    .If(v => signature.GetValue(v) == "UP" || serverEncryption.GetValue(v) == "UP")
                    .Then(v => tokenAvailability.GetValue(v) == "UP_Set" || tokenAvailability.GetValue(v) == "UP_Get" || tokenAvailability.GetValue(v) == "All_Get" || tokenAvailability.GetValue(v) == "All_Set"),
                Constraint<Variation>
                    .If(v => signature.GetValue(v) == "WinAuth" || serverEncryption.GetValue(v) == "WinAuth")
                    .Then(v => tokenAvailability.GetValue(v) == "WinAuth_Set" || tokenAvailability.GetValue(v) == "WinAuth_Get" || tokenAvailability.GetValue(v) == "All_Get" || tokenAvailability.GetValue(v) == "All_Set"),
                Constraint<Variation>
                    .If(v => signature.GetValue(v) == "X509")
                    .Then(v => tokenAvailability.GetValue(v) == "X509_Set" || tokenAvailability.GetValue(v) == "X509_Get" || tokenAvailability.GetValue(v) == "All_Get" || tokenAvailability.GetValue(v) == "All_Set"),
                Constraint<Variation>
                    .If(v => signature.GetValue(v) == "Any")
                    .Then(v => tokenAvailability.GetValue(v) == "All_Get" || tokenAvailability.GetValue(v) == "All_Set"),
                Constraint<Variation>
                    .If(v => signature.GetValue(v) == "All")
                    .Then(v => tokenAvailability.GetValue(v) == "All_Get" || tokenAvailability.GetValue(v) == "All_Set"),
                Constraint<Variation>
                    .If(v => signature.GetValue(v) == "All" || signature.GetValue(v) == "None")
                    .Then(v => !permissionToken.GetValue(v)),
                Constraint<Variation>
                    .If(v => location.GetValue(v) == "XProc")
                    .Then(v => servertype.GetValue(v) == "SameMachineDiffProc" || servertype.GetValue(v) == "SameMachineSameProc"),
                Constraint<Variation>
                    .If(v => location.GetValue(v) == "InProc" || location.GetValue(v) == "RoutedInProc")
                    .Then(v => servertype.GetValue(v) == "SameMachineSameProc"),
                Constraint<Variation>
                    .If(v => 
                        servertype.GetValue(v) == "SameMachineSameProc" && 
                        (tokenAvailability.GetValue(v) == "WinAuth_Set" || tokenAvailability.GetValue(v) == "WinAuth_Get" || tokenAvailability.GetValue(v) == "All_Get" || tokenAvailability.GetValue(v) == "All_Set"))
                    .Then(v => accounts.GetValue(v) == "BothAdmin")
            };

            Model m = new Model(parameters, constraints);
            Assert.Equal(m.GenerateVariations(2).Count(), 72);
        }

        [Fact]
        public void SampleTest()
        {
            var destination = new Parameter<string>("Destination") { "Whistler", "Hawaii", "Las Vegas" };

            var hotelQuality = new Parameter<int>("Hotel Quality") { 5, 4, 3, 2, 1 };

            var activity = new Parameter<string>("Activity") { "gambling", "swimming", "shopping", "skiing" };

            var parameters = new List<ParameterBase> { destination, hotelQuality, activity };
            var constraints = new List<Constraint<Variation>>
            {
                Constraint<Variation>
                    .If(v => destination.GetValue(v) == "Whistler" || destination.GetValue(v) == "Hawaii")
                    .Then(v => activity.GetValue(v) != "gambling"),
                Constraint<Variation>
                    .If(v => destination.GetValue(v) == "Las Vegas" || destination.GetValue(v) == "Hawaii")
                    .Then(v => activity.GetValue(v) != "skiing"),
                Constraint<Variation>
                    .If(v => destination.GetValue(v) == "Whistler")
                    .Then(v => activity.GetValue(v) != "swimming"),
            };

            Model model = new Model(parameters, constraints);
            var variations = model.GenerateVariations(2);

            Assert.True(variations.Count() == 21);

            Assert.True(variations
                .Where((v) => destination.GetValue(v) == "Whistler" || destination.GetValue(v) == "Hawaii")
                .All((v) => activity.GetValue(v) != "gambling"));

            Assert.True(variations
               .Where((v) => destination.GetValue(v) == "Las Vegas" || destination.GetValue(v) == "Hawaii")
               .All((v) => activity.GetValue(v) != "skiing"));

            Assert.True(variations
              .Where((v) => destination.GetValue(v) == "Whistler")
              .All((v) => activity.GetValue(v) != "swimming"));
        }

        [Fact]
        public void SampleTestWithCustomVariation()
        {
            var destination = new Parameter<string>("Destination") { "Whistler", "Hawaii", "Las Vegas" };

            var hotelQuality = new Parameter<int>("HotelQuality") { 5, 4, 3, 2, 1 };

            var activity = new Parameter<string>("Activity") { "gambling", "swimming", "shopping", "skiing" };

            var parameters = new List<ParameterBase> { destination, hotelQuality, activity };
            var constraints = new List<Constraint<VacationVariation>>
            {
                Constraint<VacationVariation>
                    .If(v => v.Destination == "Whistler" || v.Destination == "Hawaii")
                    .Then(v => v.Activity != "gambling"),
                Constraint<VacationVariation>
                    .If(v => v.Destination == "Las Vegas" || v.Destination == "Hawaii")
                    .Then(v => v.Activity != "skiing"),
                Constraint<VacationVariation>
                    .If(v => v.Destination == "Whistler")
                    .Then(v => v.Activity != "swimming"),
            };

            var model = new Model<VacationVariation>(parameters, constraints);
            var variations = model.GenerateVariations(2);

            Assert.True(variations.Count() == 21);

            Assert.True(variations
                .Where((v) => v.Destination == "Whistler" || v.Destination == "Hawaii")
                .All((v) => v.Activity != "gambling"));

            Assert.True(variations
               .Where((v) => v.Destination == "Las Vegas" || v.Destination == "Hawaii")
               .All((v) => v.Activity != "skiing"));

            Assert.True(variations
              .Where((v) => v.Destination == "Whistler")
              .All((v) => v.Activity != "swimming"));
        }

        [Fact]
        public void SampleTestWithCustomVariationAndElse()
        {
            var destination = new Parameter<string>("Destination") { "Whistler", "Hawaii", "Las Vegas" };

            var hotelQuality = new Parameter<int>("HotelQuality") { 5, 4, 3, 2, 1 };

            var activity = new Parameter<string>("Activity") { "gambling", "swimming", "shopping", "skiing" };

            var parameters = new List<ParameterBase> { destination, hotelQuality, activity };
            var constraints = new List<Constraint<VacationVariation>>
            {
                Constraint<VacationVariation>
                    .If(v => v.Destination == "Whistler" || v.Destination == "Hawaii")
                    .Then(v => v.Activity != "gambling"),
                Constraint<VacationVariation>
                    .If(v => v.Destination == "Las Vegas" || v.Destination == "Hawaii")
                    .Then(v => v.Activity != "skiing"),
                Constraint<VacationVariation>
                    .If(v => v.Destination == "Whistler")
                    .Then(v => v.Activity != "swimming"),
                Constraint<VacationVariation>
                    .If(v => v.Destination == "Las Vegas")
                    .Then(v => v.HotelQuality > 3)
                    .Else(v => v.HotelQuality <= 4),
            };

            var model = new Model<VacationVariation>(parameters, constraints);
            var variations = model.GenerateVariations(2);

            Assert.True(variations.Count() > 0);

            Assert.True(variations
                .Where((v) => v.Destination == "Whistler" || v.Destination == "Hawaii")
                .All((v) => v.Activity != "gambling"));

            Assert.True(variations
               .Where((v) => v.Destination == "Las Vegas" || v.Destination == "Hawaii")
               .All((v) => v.Activity != "skiing"));

            Assert.True(variations
              .Where((v) => v.Destination == "Whistler")
              .All((v) => v.Activity != "swimming"));

            Assert.True(variations
                .Where((v) => v.Destination == "Las Vegas")
                .All((v) => v.HotelQuality > 3));

            Assert.True(variations
               .Where((v) => v.Destination != "Las Vegas")
               .All((v) => v.HotelQuality <= 4));

            Assert.True(variations
               .Where((v) => v.HotelQuality == 5)
               .All((v) => v.Destination == "Las Vegas"));
        }

        [Fact]
        public void NegativeSampleTestWithCustomVariation1()
        {
            var destination = new Parameter<string>("Destination") { "Whistler", "Hawaii", "Las Vegas" };

            var hotelQuality = new Parameter<int>("HotelQuality") { 5, 4, 3, 2, 1 };

            var activity = new Parameter<string>("Activity") { "gambling", "swimming", "shopping", "skiing" };

            var parameters = new List<ParameterBase> { destination, hotelQuality, activity };
            var constraints = new List<Constraint<VacationVariation>>
            {
                Constraint<VacationVariation>.Conditional(v => Filter(v))
            };

            var model = new Model<VacationVariation>(parameters, constraints);

            Assert.Throws(typeof(InvalidOperationException), () => model.GenerateVariations(2));
        }

        public static bool Filter(VacationVariation v)
        {
            return true;
        }

        [Fact]
        public void NegativeSampleTestWithCustomVariation2()
        {
            var destination = new Parameter<string>("Destination") { "Whistler", "Hawaii", "Las Vegas" };

            var hotelQuality = new Parameter<int>("HotelQuality") { 5, 4, 3, 2, 1 };

            var activity = new Parameter<string>("Activity") { "gambling", "swimming", "shopping", "skiing" };

            var parameters = new List<ParameterBase> { destination, hotelQuality, activity };
            var constraints = new List<Constraint<VacationVariation>>
            {
                Constraint<VacationVariation>.Conditional(v => v.Filter())
            };

            var model = new Model<VacationVariation>(parameters, constraints);

            Assert.Throws(typeof(InvalidOperationException), () => model.GenerateVariations(2));
        }

        [Fact]
        public void ExpectedResultsWithDependencies()
        {
            var p1 = new Parameter<string>("P1")
            {
                "positive",
                new ParameterValue<string>("negative") { Tag = false }
            };

            var p2 = new Parameter<string>("P2")
            {
                "value1",
                "value2"
            };

            var p3 = new Parameter<string>("P3")
            {
                "value1",
                "value2"
            };

            var p4 = new Parameter<string>("P4")
            {
                "positive",
                new ParameterValue<string>("negative") { Tag = false }
            };

            var parameters = new List<ParameterBase> { p1, p2, p3, p4 };

            var constraints = new List<Constraint<Variation>>
            {
                Constraint<Variation>
                    .If(v => p1.GetValue(v) == "negative")
                    .Then(v => p2.GetValue(v) == "value1"),
                Constraint<Variation>
                    .If(v => p2.GetValue(v) == "value1")
                    .Then(v => p3.GetValue(v) == "value1"),
                Constraint<Variation>
                    .If(v => p3.GetValue(v) == "value1")
                    .Then(v => p4.GetValue(v) == "negative"),
            };

            var model = new Model(parameters, constraints) { DefaultVariationTag = true };

            Assert.False(model.GenerateVariations(2).Any((v) => (string)v["P1"] == "negative"));
        }

        [Fact]
        public void ExpectedResultsOnSingleParameter()
        {
            var p1 = new Parameter<string>("P1")
            {
                "positive",
                new ParameterValue<string>("negative") { Tag = false }
            };

            var p2 = new Parameter<string>("P2")
            {
                "value1",
                "value2"
            };

            var p3 = new Parameter<string>("P3")
            {
                "value1",
                "value2"
            };

            var p4 = new Parameter<string>("P4")
            {
                "value1",
                "value2"
            };

            var parameters = new List<ParameterBase> { p1, p2, p3, p4 };

            var constraints = new List<Constraint<Variation>>
            {
                Constraint<Variation>
                    .If(v => p1.GetValue(v) == "negative")
                    .Then(v => p2.GetValue(v) == "value1"),
                Constraint<Variation>
                    .If(v => p2.GetValue(v) == "value1")
                    .Then(v => p3.GetValue(v) == "value1"),
            };

            var model = new Model(parameters, constraints) { DefaultVariationTag = true };

            Assert.True(model.GenerateVariations(2).Where((v) => (string)v["P1"] == "negative").All((v) => (bool)v.Tag == false));
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704")]
        public void StrncpyModel()
        {
            //#
            //# Simplified strncpy, just to show how negative test generation works
            //# ~ denotes negative (aka invalid) values of a parameter, there wille
            //# be only one such value in any given test case
            //#
            //Src:  1B, 10B, ~nalloc, ~null
            //Dst:  1B, 10B, ~nalloc, ~null
            //Size: 1, 2, 0, ~-1

            var src = new Parameter<string>("Src")
            {
                "1B",
                "10B",
                new ParameterValue<string>("nalloc") { Tag = false },
                new ParameterValue<string>("null") { Tag = false }
            };

            var dst = new Parameter<string>("Dst")
            {
                "1B",
                "10B",
                new ParameterValue<string>("nalloc") { Tag = false },
                new ParameterValue<string>("null") { Tag = false }
            };

            var size = new Parameter<int>("Size")
            {
                1,
                2,
                0,
                new ParameterValue<int>(-1) { Tag = false }
            };

            var model = new Model(new List<ParameterBase> { src, dst, size }) { DefaultVariationTag = true };

            var variations = model.GenerateVariations(2);

            Assert.True(variations
                .Where((v) => (string)v["Src"] == "nalloc" || (string)v["Src"] == "null")
                .All((v) => (bool)v.Tag == false));

            Assert.True(variations
                .Where((v) => (string)v["Dst"] == "nalloc" || (string)v["Dst"] == "null")
                .All((v) => (bool)v.Tag == false));

            Assert.True(variations
                .Where((v) => (int)v["Size"] == -1)
                .All((v) => (bool)v.Tag == false));

            Assert.True(variations
                .Where((v) => (string)v["Src"] == "nalloc" || (string)v["Src"] == "null")
                .All((v) => (string)v["Dst"] != "nalloc" && (string)v["Dst"] != "null" && (int)v["Size"] != -1));

            Assert.True(variations
                .Where((v) => (string)v["Dst"] == "nalloc" || (string)v["Dst"] == "null")
                .All((v) => (string)v["Src"] != "nalloc" && (string)v["Src"] != "null" && (int)v["Size"] != -1));

            Assert.True(variations
                .Where((v) => (int)v["Size"] == -1)
                .All((v) => (string)v["Src"] != "nalloc" && (string)v["Src"] != "null" && (string)v["Dst"] != "nalloc" && (string)v["Dst"] != "null"));
        }

        [Fact]
        public void SampleWithExpectedResultsTest()
        {
            var destination = new Parameter<string>("Destination") 
            { 
                "Whistler", 
                "Hawaii",
                new ParameterValue<string>("Las Vegas") { Weight = 5.0 },
                new ParameterValue<string>("Cleveland") { Tag = Results.ReturnsFalse }
            };

            var hotelQuality = new Parameter<int>("Hotel Quality") 
            { 5, 
              4,
              3,
              2,
              1,
              new ParameterValue<int>(-1){ Tag = Results.ThrowsOutOfRangeException } 
            };

            var activity = new Parameter<string>("Activity") { "gambling", "swimming", "shopping", "skiing" };

            var parameters = new List<ParameterBase> { destination, hotelQuality, activity };
            var constraints = new List<Constraint<Variation>>
            {
                Constraint<Variation>
                    .If(v => destination.GetValue(v) == "Whistler" || destination.GetValue(v) == "Hawaii")
                    .Then(v => activity.GetValue(v) != "gambling"),
                Constraint<Variation>
                    .If(v => destination.GetValue(v) == "Las Vegas" || destination.GetValue(v) == "Hawaii")
                    .Then(v => activity.GetValue(v) != "skiing"),
                Constraint<Variation>
                    .If(v => destination.GetValue(v) == "Whistler")
                    .Then(v => activity.GetValue(v) != "swimming"),
            };

            Model model = new Model(parameters, constraints) { DefaultVariationTag = Results.Default };

            foreach (var variation in model.GenerateVariations(2))
            {
                switch ((Results)variation.Tag)
                {
                    case Results.ReturnsFalse:
                        Assert.False(
                            CallVacationPlanner(
                                destination.GetValue(variation),
                                hotelQuality.GetValue(variation),
                                activity.GetValue(variation)));
                        break;
                    case Results.ThrowsOutOfRangeException:
                        Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
                            CallVacationPlanner(
                                destination.GetValue(variation),
                                hotelQuality.GetValue(variation),
                                activity.GetValue(variation)));
                        break;
                    default:
                        Assert.True
                            (CallVacationPlanner(
                                destination.GetValue(variation),
                                hotelQuality.GetValue(variation),
                                activity.GetValue(variation)));
                        break;
                }
            }

            var variations = model.GenerateVariations(2);

            Assert.True(variations
                .Where((v) => destination.GetValue(v) == "Whistler" || destination.GetValue(v) == "Hawaii")
                .All((v) => activity.GetValue(v) != "gambling"));

            Assert.True(variations
               .Where((v) => destination.GetValue(v) == "Las Vegas" || destination.GetValue(v) == "Hawaii")
               .All((v) => activity.GetValue(v) != "skiing"));

            Assert.True(variations
              .Where((v) => destination.GetValue(v) == "Whistler")
              .All((v) => activity.GetValue(v) != "swimming"));
        }

        public static bool CallVacationPlanner(string destination, int hotelQuality, string activity)
        {
            if (hotelQuality < 1 || hotelQuality > 5)
            {
                throw new ArgumentOutOfRangeException("hotelQuality");
            }

            return destination == "Cleveland" ? false : true;
        }
    }

    public enum Results
    {
        Default,
        ReturnsFalse,
        ThrowsOutOfRangeException
    }

    public class VacationVariation
    {
        public string Destination { get; set; }
        public int HotelQuality { get; set; }
        public string Activity { get; set; }

        public bool Filter()
        {
            return true;
        }
    }

    public class UpgradeVariation
    {
        public string OS1 { get; set; }
        public string OS2 { get; set; }
        public string SKU1 { get; set; }
        public string SKU2 { get; set; }
        public string Lang1 { get; set; }
        public string Lang2 { get; set; }

        public static IList<string> WriteVariations(IEnumerable<UpgradeVariation> variations)
        {
            List<string> strings = new List<string>();
            foreach (var v in variations)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(v.OS1 + " ");
                builder.Append(v.OS2 + " ");
                builder.Append(v.SKU1 + " ");
                builder.Append(v.SKU2 + " ");
                builder.Append(v.Lang1 + " ");
                builder.Append(v.Lang2 + " ");                
                strings.Add(builder.ToString().TrimEnd());
            }

            return strings;
        }
    }
}
