﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace Makaretu.Dns
{

    [TestClass]
    public class DNSKEYRecordTest
    {
        byte[] key = Convert.FromBase64String("AQPSKmynfzW4kyBv015MUG2DeIQ3Cbl+BBZH4b/0PY1kxkmvHjcZc8nokfzj31GajIQKY+5CptLr3buXA10hWqTkF7H6RfoRqXQeogmMHfpftf6zMv1LyBUgia7za6ZEzOJBOztyvhjL742iU/TpPSEDhm2SNKLijfUppn1UaNvv4w==");

        [TestMethod]
        public void Roundtrip()
        {
            var a = new DNSKEYRecord
            {
                Name = "example.com",
                TTL = TimeSpan.FromDays(2),
                Flags = 256,
                Protocol = 3,
                Algorithm = SecurityAlgorithm.RSASHA1,
                PublicKey = key
            };
            var b = (DNSKEYRecord)new ResourceRecord().Read(a.ToByteArray());
            Assert.AreEqual(a.Name, b.Name);
            Assert.AreEqual(a.Class, b.Class);
            Assert.AreEqual(a.Type, b.Type);
            Assert.AreEqual(a.TTL, b.TTL);
            Assert.AreEqual(a.Flags, b.Flags);
            Assert.AreEqual(a.Protocol, b.Protocol);
            Assert.AreEqual(a.Algorithm, b.Algorithm);
            CollectionAssert.AreEqual(a.PublicKey, b.PublicKey);
        }

        [TestMethod]
        public void Roundtrip_Master()
        {
            var a = new DNSKEYRecord
            {
                Name = "example.com",
                TTL = TimeSpan.FromDays(2),
                Flags = 256,
                Protocol = 3,
                Algorithm = SecurityAlgorithm.RSASHA1,
                PublicKey = key
            };
            var b = (DNSKEYRecord)new ResourceRecord().Read(a.ToString());
            Assert.AreEqual(a.Name, b.Name);
            Assert.AreEqual(a.Class, b.Class);
            Assert.AreEqual(a.Type, b.Type);
            Assert.AreEqual(a.TTL, b.TTL);
            Assert.AreEqual(a.Flags, b.Flags);
            Assert.AreEqual(a.Protocol, b.Protocol);
            Assert.AreEqual(a.Algorithm, b.Algorithm);
            CollectionAssert.AreEqual(a.PublicKey, b.PublicKey);
        }

        [TestMethod]
        public void KeyTag()
        {
            // From https://tools.ietf.org/html/rfc4034#section-5.4
            var a = new DNSKEYRecord
            {
                Name = "dskey.example.com",
                TTL = TimeSpan.FromSeconds(86400),
                Flags = 256,
                Algorithm = SecurityAlgorithm.RSASHA1,
                PublicKey = Convert.FromBase64String(
                    @"AQOeiiR0GOMYkDshWoSKz9Xz
                      fwJr1AYtsmx3TGkJaNXVbfi/
                      2pHm822aJ5iI9BMzNXxeYCmZ
                      DRD99WYwYqUSdjMmmAphXdvx
                      egXd/M5+X7OrzKBaMbCVdFLU
                      Uh6DhweJBjEVv5f2wwjM9Xzc
                      nOf+EPbtG9DMBmADjFDc2w/r
                      ljwvFw==")
            };
            Assert.AreEqual(60485, a.KeyTag());
        }

        [TestMethod]
        public void FromRsaSha256()
        {
            // From https://tools.ietf.org/html/rfc5702#section-6.1
            var modulus = Convert.FromBase64String("wVwaxrHF2CK64aYKRUibLiH30KpPuPBjel7E8ZydQW1HYWHfoGmidzC2RnhwCC293hCzw+TFR2nqn8OVSY5t2Q==");
            var publicExponent = Convert.FromBase64String("AQAB");
            var dnsPublicKey = Convert.FromBase64String("AwEAAcFcGsaxxdgiuuGmCkVImy4h99CqT7jwY3pexPGcnUFtR2Fh36BponcwtkZ4cAgtvd4Qs8PkxUdp6p/DlUmObdk=");

            var parameters = new RSAParameters()
            {
                Exponent = publicExponent,
                Modulus = modulus,
            };
            var publicKey = RSA.Create();
            publicKey.ImportParameters(parameters);

            var dnskey = new DNSKEYRecord(publicKey, SecurityAlgorithm.RSASHA256);
            Assert.AreEqual(256, dnskey.Flags);
            Assert.AreEqual(3, dnskey.Protocol);
            Assert.AreEqual(SecurityAlgorithm.RSASHA256, dnskey.Algorithm);
            CollectionAssert.AreEqual(dnsPublicKey, dnskey.PublicKey);
            Assert.AreEqual(9033, dnskey.KeyTag());
        }

        [TestMethod]
        public void FromRsaSha512()
        {
            // From https://tools.ietf.org/html/rfc5702#section-6.2
            var modulus = Convert.FromBase64String("0eg1M5b563zoq4k5ZEOnWmd2/BvpjzedJVdfIsDcMuuhE5SQ3pfQ7qmdaeMlC6Nf8DKGoUPGPXe06cP27/WRODtxXquSUytkO0kJDk8KX8PtA0+yBWwy7UnZDyCkynO00Uuk8HPVtZeMO1pHtlAGVnc8VjXZlNKdyit99waaE4s=");
            var publicExponent = Convert.FromBase64String("AQAB");
            var dnsPublicKey = Convert.FromBase64String("AwEAAdHoNTOW+et86KuJOWRDp1pndvwb6Y83nSVXXyLA3DLroROUkN6X0O6pnWnjJQujX/AyhqFDxj13tOnD9u/1kTg7cV6rklMrZDtJCQ5PCl/D7QNPsgVsMu1J2Q8gpMpztNFLpPBz1bWXjDtaR7ZQBlZ3PFY12ZTSncorffcGmhOL");

            var parameters = new RSAParameters()
            {
                Exponent = publicExponent,
                Modulus = modulus,
            };
            var publicKey = RSA.Create();
            publicKey.ImportParameters(parameters);

            var dnskey = new DNSKEYRecord(publicKey, SecurityAlgorithm.RSASHA512);
            Assert.AreEqual(256, dnskey.Flags);
            Assert.AreEqual(3, dnskey.Protocol);
            Assert.AreEqual(SecurityAlgorithm.RSASHA512, dnskey.Algorithm);
            CollectionAssert.AreEqual(dnsPublicKey, dnskey.PublicKey);
            Assert.AreEqual(3740, dnskey.KeyTag());
        }

    }
}