# Introduction 
This is a service that provides the cast of all the tv shows in the TVMaze database, It enrichs the metadata system with this information.<br/> 
The TVMaze database provides a public REST API for querying the data.<br/>

# Implementation Summary 
I used .NetCore 3.1 and .NetStandard 2.0 <br/>
I created API , Abstractions and Implementations projects, those could indeed get further narrowed to different logical layers (e.g. data , services ,entities and etc) in separate projects.<br/>
<br/> 
- Abstractions project includes :<br/>
• Entities<br/>
• IShowRepository that is responsible for fetching shows and persists them in a paginated manner. <br/>
• ITvMazeSyncService which performs shows synchronization (from API to persistent layer)<br/>
<br/>
- Implementations project contains :<br/>
• ShowRepository that uses EFCore and Sqlite to persist the data <br/>
• ITvMazeApiClient interface and its default implementation as TvMazeApiClient. It is responsible to fetch shows from TvMaze API and handles rate limited scenarios. <br/>
• TvMazeSyncService : Handles synchronization logic (it depends on ITvMazeApiClient implementation) the sync logic checks for the last persisted show and calculates the next page, API client gets the shows for that page and repository persists them and same logic will continue over the next page! this way I ensure I proceed to the next page only if the last page was successfully retrieved from API and saved by repository. The procedure ends until nothing is returned from API. <br/>
<br/>
- API project has:<br/>
• Host and DI configurations<br/>
• ShowController to use IShowRepository implementation and exposes paginated shows.<br/>
• Mappers (Entities to DTOs).<br/>
• TvMazeSyncHostedService which is a hosted background service and uses an ITvMazeSyncService implementation to synchronize the shows in a timely manner at background.<br/> 

# Build and Run
Configure the TvMazeApiHostAddress and ConnectionStrings in 'appsettings.json' inside TvMaze.Scraper.API <br/>
Build and run the soltion via VisualStudio or use dotnet core CLI 

# Further Improvements
I’ve not implemented everything needed for a production ready application.<br/>
If I have a chance to further continue, I would go for the following tasks/features: <br/>
• Enhance logging <br/>
• Enhance exception handling <br/>
• Add tests <br/>
• Improve resilience and scalability<br/>
The sync logic in TvMazeSyncService is not very optimized.<br/> 
It could proceed to persist the returned shows while the next page shows are being retrieved from API. <br/> 
Also TvMazeSyncHostedService is not able to handle scaling scenarios.<br/> 
So if multiple instances are running then they may conflict/duplicate each other tasks.<br/> 
To overcome above concerns I could use a proper centralized queuing mechanism to gracefully handle the orders and failures.<br/> 
<br/>

/api/shows?page=<"intStartedByzero">&pagesize=<"int">