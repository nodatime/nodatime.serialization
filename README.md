# Noda Time serialization

This repository contains projects to do with serializing [Noda Time](http://nodatime.org)
data in various forms, such as JSON.

.NET binary serialization and XML serialization is built into Noda Time itself, but
other serialization mechanisms are expected to simply use the public API.

Separating the code into a "core" in the [nodatime](https://github.com/nodatime/nodatime)
repository and serialization code here allows for a greater separation between the two.
We will aim to release serialization packages shortly after major new releases of Noda Time,
but they may otherwise drift - there could easily be multiple releases of serialization
projects between releases of the core library, or vice versa.
