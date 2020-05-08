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
# Authentication
The following example will show how to use different Conjur authenticators.

### authn
When using a standard authentication you just need to set the following environment variables:
```bash
export CONJUR_APPLIANCE_URL="https://conjur-follower"
export CONJUR_ACCOUNT="myConjurAccount"
export CONJUR_AUTHN_LOGIN="host/app1"
export CONJUR_AUTHN_API_KEY="35a9ej72v0q8ek25fghn52g1rjvm29qwxv738ts71j2d5hdwk1s34fbn"
# set to 'yes' if conjur instance is using a self-signed certificate
export CONJUR_IGNORE_UNTRUSTED_SSL=no
```

### authn-iam
`authn-iam` authentication is slightly different since a request header needs to be created at runtime. 

```bash
export CONJUR_APPLIANCE_URL="https://conjur-follower"
export CONJUR_AUTHN_URL="https://conjur-follower/authn-iam/prod"
export CONJUR_ACCOUNT="myConjurAccount"
export CONJUR_AUTHN_LOGIN="host/awsApp1/8837277729373/iam-role-name"
# set to 'yes' if conjur instance is using a self-signed certificate
export CONJUR_IGNORE_UNTRUSTED_SSL=no
```

Now we can init the Conjur configuration from environment and then set the api key to the AWS request header.
```c#
Configuration config = Configuration.FromEnvironment();
config.ApiKey = AuthnIAMHelper.GetAuthenticationRequest("Your AWS access key", "Your AWS secret access key", "Your AWS token");
Conjur c = new Conjur(config);
c.Authenticate();
```

### authn-k8s
When using `authn-k8s` there should be a side car container that has already authenticated. In this case all you need to do is point to the file in which the Conjur access token has been mounted to. Set the following environment variables for the k8s container:

```bash
export CONJUR_APPLIANCE_URL="https://conjur-follower"
export CONJUR_ACCOUNT="myConjurAccount"
export CONJUR_AUTHN_TOKEN_FILE="/run/conjur/conjur-access-token"
```

# Usage
The following methods can be used on the Conjur object after initializing.

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
