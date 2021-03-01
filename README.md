# Flow

### TeamCity integration

TeamCity provides the following environment variables:

|Name|Value|
|---|---|
|TEAMCITY_VERSION|TeamCity version|
|TEAMCITY_MSBUILD_LOGGER|Path to the TeamCity MSBuild logger|
|TEAMCITY_VSTEST_LOGGER|Path to the TeamCity VSTest logger|

To debug Flow under TeamCity add the environment variable _env.FLOW_DEBUG=true_, run a build and attach to the appropriate process pointed in a build log.