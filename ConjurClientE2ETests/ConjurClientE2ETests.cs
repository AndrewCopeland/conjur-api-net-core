using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConjurClient;
using ConjurClient.Resources;
using System.Collections.Generic;
using System;

namespace ConjurClientE2ETests
{
    [TestClass]
    public class ConjurClientE2ETests
    {
        [TestMethod]
        public void ConjurClientE2ETest()
        {
            // Set environment variables for Conjur
            // export CONJUR_APPLIANCE_URL=https://conjur-master
            // export CONJUR_ACCOUNT=conjur
            // export CONJUR_AUTHN_LOGON=host/myApp
            // export CONJUR_AUTHN_API_KEY=35a9ej72v0q8ek25fghn52g1rjvm29qwxv738ts71j2d5hdwk1s34fbn
            Environment.SetEnvironmentVariable("CONJUR_APPLIANCE_URL", "https://conjur-master");
            Environment.SetEnvironmentVariable("CONJUR_ACCOUNT", "conjur");
            Environment.SetEnvironmentVariable("CONJUR_AUTHN_LOGIN", "admin");
            Environment.SetEnvironmentVariable("CONJUR_AUTHN_API_KEY", "35a9ej72v0q8ek25fghn52g1rjvm29qwxv738ts71j2d5hdwk1s34fbn");
            Environment.SetEnvironmentVariable("CONJUR_IGNORE_UNTRUSTED_SSL", "yes");

            Conjur conjur = new Conjur();

            // retrieve all the Conjur resources this application has access to
            List<Resource> resources =  conjur.ListResources();
            foreach (Resource r in resources)
            {
                Console.WriteLine("Printing out all variable resources");
                if (r.Kind == ResourceKind.variable)
                {
                    Console.WriteLine("Variable ID: {0}", r.Id);
                }
            }
        }
    }
}
