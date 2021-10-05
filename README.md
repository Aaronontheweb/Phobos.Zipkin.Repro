# Phobos.Zipkin.Repro
This is a simple repository for testing [Zipkin](https://zipkin.io/) with [Phobos](https://phobos.petabridge.com/).

## Running This Sample
To run this sample, add the following NuGet.config file to the root of this solution:

```xml
   
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <solution>
    <add key="disableSourceControlIntegration" value="true" />
  </solution>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="phobos" value="{your phobos key}" />
  </packageSources>
</configuration>
```

Next, run the following command to run `docker-compose` to spin up Zipkin with MySQL storage:

```
PS> cd zipkin
PS> docker-compose -f docker-compose-mysql.yml up
```

This will start up [Zipkin 2.22.1](https://github.com/openzipkin/zipkin/releases/tag/2.22.1) on http://localhost:9411/ using MySQL storage on the backend.

Finally, compile and run this sample by running `dotnet run -c Release` in the root directory.

You'll be able to search for Zipkin traces using the UI like so:

![Zipkin service, operation name, and tag search](/images/zipkin-mysql.gif)

> To run a tag search in Zipkin, you'll need to specify the following syntax in the tag dialog window:
> `tagQuery={tagName} and {tagName}={exactValue}`