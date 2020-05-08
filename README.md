# conjur-api-net-core
Programmatic .NET Core access to the Conjur API.

# Installation

```
$ git clone https://github.com/AndrewCopeland/conjur-api-net-core
$ cd conjur-api-net-core/src
$ dotnet build --configuration Release
# .dll can be found here: ./ConjurClient/bin/Release/netcoreapp3.1/ConjurClient.dll
```

# Quick start

Fetching a Secret, for example:

Suppose there exists a variable `db/secret` with secret value `fde5c4a45ce573f9768987cd`

Create a c# .net core program using `ConjurClient` to fetch the secret value:

```c#
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConjurClient;
using ConjurClient.Resources;
using System.Collections.Generic;
using System;
using System.Security;

namespace YourAppNamespace
{
    public class YourAppClass
    {
        public void Main()
        {
            // In actual use case set these environment 
            // variables on the vm or container level.
            Environment.SetEnvironmentVariable("CONJUR_APPLIANCE_URL", "https://conjur-master");
            Environment.SetEnvironmentVariable("CONJUR_ACCOUNT", "conjur");
            Environment.SetEnvironmentVariable("CONJUR_AUTHN_LOGIN", "admin");
            Environment.SetEnvironmentVariable("CONJUR_AUTHN_API_KEY","35a9ej72v0q8ek25fghn52g1rjvm29qwxv738ts71j2d5hdwk1s34fbn");
            Environment.SetEnvironmentVariable("CONJUR_IGNORE_UNTRUSTED_SSL", "yes");

            Conjur conjur = new Conjur();
			// Authenticate the conjur client
            conjur.Authenticate();

			// Retrieve a specific secret
			SecureString secretValue = conjur.RetrieveSecret("db/secret");
			Console.WriteLine("Secret Value: {0}", Utilities.ToString(secretValue));
        }
    }
}
```

# Usage
The following methods can be used:

### Authenticate()
This method will authenticate to conjur. Conjur client access token is refreshed.

```c#
Conjur c = new Conjur();
c.Authenticate();
```

### RetrieveSecret(String variableId)
This method will retrieve a secret from conjur. Secret is returned as a SecureString.

```c#
Conjur c = new Conjur();
c.Authenticate();
c.RetrieveSecret("db/postgres/password");
```

### AddSecret(String variableId, SecureString secretValue)
This method will add a secret value into conjur. Provided secret value must be a SecureString

```c#
SecureString newPassword = "NewPasswordAsSecureString";
Conjur c = new Conjur();
c.Authenticate();
c.AddSecret("db/postgres/password", newPassword);
```

### ListResources()
This method will list all resources in conjur.

```c#
Conjur c = new Conjur();
c.Authenticate();
List<Resource> resources = c.ListResources();
```
### ListResource(ResourceKind kind, String search)
This method will list specific resources in conjur depending on the kind and search query.

```c#
Conjur c = new Conjur()
c.Authenticate();
// Retrieve all variables with 'postgres' in the id or annotation.
List<Resource> variables = c.ListResources(ResourceKind.variable, "postgres");
```

### AppendPolicy(String policyBranch, String policyContent)
This method appends a policy to a policy branch.

```c#
Conjur c = new Conjur()
c.Authenticate();
// This will create resource host/new-app on the root policy
c.AppendPolicy("root", "- !host new-app");
```

### UpdatePolicy(String policyBranch, String policyContent)
This method updates a policy in a policy branch

```c#
Conjur c = new Conjur()
c.Authenticate();
// This will create resource host/new-app on the root policy
c.UpdatePolicy("root", "- !host new-app");
```
### ReplacePolicy(String policyBranch, String policyContent)
This method replace a policy in a policy branch

```c#
Conjur c = new Conjur()
c.Authenticate();
// This will create resource host/new-app on the root policy and delete every other resource.
c.ReplacePolicy("root", "- !host new-app");
```


# Contributing

We welcome contributions of all kinds to this repository. For instructions on how to get started and descriptions of our development workflows, please see our [contributing
guide][contrib].

[contrib]: https://github.com/AndrewCopeland/conjur-api-net-core/blob/master/CONTRIBUTING.md

# License

This repository is licensed under Apache License 2.0 - see [`LICENSE`](LICENSE) for more details.