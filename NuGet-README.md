# NodaTime serialization

XML serialization is built into Noda Time itself, but other serialization mechanisms are
expected to simply use the public API. (Noda Time 1.x and 2.x also support .NET binary
serialization natively.)

Separating the code into a "core" in the [nodatime](https://github.com/nodatime/nodatime)
repository and serialization code in a
[separate repository](https://github.com/nodatime/nodatime.serialization)
makes it simpler to implement independent release cycles.

All Noda Time serialization packages (as published by the Noda Time
authors) start with a prefix "NodaTime.Serialization":

- [NodaTime.Serialization.JsonNet](https://www.nuget.org/packages/NodaTime.Serialization.JsonNet)
  (JSON serialization using Newtonsoft.Json)
- [NodaTime.Serialization.SystemTextJson](https://www.nuget.org/packages/NodaTime.Serialization.SystemTextJson)
  (JSON serialization using System.Text.Json)
- [NodaTime.Serialization.Protobuf](https://www.nuget.org/packages/NodaTime.Serialization.Protobuf)
  (Protocol Buffer serialization using Google.Protobuf)
  
The source code for all these packages is at
https://github.com/nodatime/nodatime.serialization issues should be
filed in that GitHub repository.
