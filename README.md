---
services: media-services
platforms: dotnet
author: Juliako
---

# Encode and Deliver Content on Demand with Azure Media Services using .NET SDK

The sample shows how to encode media on demand with Media Encoder Standard. The following media processing operations are shown:
 
- How to generate a thumbnail.
- How to encode to audio only.
- How to encoder to adaptive bitrate MP4s.


## Running this sample

1. Use Nuget to install the latest Azure Media Services .NET SDK extensions.
	
	[Install-Package windowsazure.mediaservices.extensions](http://www.nuget.org/packages/windowsazure.mediaservices.extensions).
2. Add the appSettings section to the app.config file, and set the values for your Media Services account name and account key.
		
		<?xml version="1.0"?>
		<configuration>
		  <appSettings>
		      <add key="MediaServicesAccountName" value="YouMediaServicesAccountName" />
		      <add key="MediaServicesAccountKey" value="YouMediaServicesAccountKey" />
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
