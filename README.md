# Dedupe

.NET Core project allowing deduplication (and subsequent reduplication) of files
in a directory, which (in some cases) may improve compression ratio and reduce
compression times.

# Build

I've still not gotten the hang of things in the .NET Core world, but I believe
it's a fairly vanilla build process, as far as things go.

You'll need to build the `Dedupe.Core` project before building `Dedupe.Console`.

Use the commands below in each project's directory and things *should* work as-is.

```
dotnet restore
dotnet build
```

Additionally, in the `Dedupe.Console` project, if you want an `exe`, run this:

```
dotnet publish
```

# Usage

Compress the specified directory:
```
Dedupe.Console compress <source-directory> <target-directory>
```

Compress the specified directory in-place:
```
Dedupe.Console compress <source-directory>
```

Expand the specified directory in-place:
```
Dedupe.Console expand <source-directory>
```

# License

Licensed under the [MIT License](./LICENSE.md)