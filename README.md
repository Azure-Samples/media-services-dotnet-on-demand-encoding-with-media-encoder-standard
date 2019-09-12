---
page_type: sample
languages:
- csharp
products:
- azure
description: "The sample shows how to encode media on demand with Media Encoder Standard."
urlFragment: media-services-dotnet-on-demand-encoding-with-media-encoder-standard
---

# Encode and Deliver Content on Demand with Azure Media Services using .NET SDK

The sample shows how to encode media on demand with Media Encoder Standard. The following media processing operations are shown:
 
- How to generate a thumbnail.
- How to encode to audio only.
- How to encoder to adaptive bitrate MP4s.


## Running this sample

1. Use Nuget to install the latest Azure Media Services .NET SDK extensions.
	
	[Install-Package windowsazure.mediaservices.extensions](http://www.nuget.org/packages/windowsazure.mediaservices.extensions).
2. Update the appSettings section of the app.config file with appropriate values. For more information, see [this](https://docs.microsoft.com/azure/media-services/media-services-use-aad-auth-to-access-ams-api) topic.

		<?xml version="1.0"?>
		<configuration>
		  <appSettings>
			    <add key="AMSAADTenantDomain" value="AADTenantDomain" />
			    <add key="AMSRESTAPIEndpoint" value="RESTAPIEndpoint" />
		  </appSettings>
		</configuration>

## About the code

For more information, see the following topics:

- [Get started with delivering content on demand using .NET SDK](http://azure.microsoft.com/documentation/articles/media-services-dotnet-get-started/) 
- [How to encode an asset using Media Encoder Standard](http://azure.microsoft.com/documentation/articles/media-services-dotnet-encode-with-media-encoder-standard/).

## More information

You can view AMS learning paths here:

- [AMS Live Streaming Workflow](http://azure.microsoft.com/documentation/learning-paths/media-services-streaming-live/)
- [AMS on Demand Streaming Workflow](http://azure.microsoft.com/documentation/learning-paths/media-services-streaming-on-demand/)
