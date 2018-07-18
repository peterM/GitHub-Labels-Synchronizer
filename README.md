# GitHub Labels Synchronizer
Synchronize GitHub labels across organization repositories

## For what is this good for
Imagine that you have lot of repositories in your account and you want to have in all repositories the same label names, descriptions and colors. You can prepare labels in one repository and then use this repository as reference one. This tool will synchronize these labels and create in all other organization repositories.

## Technology behind
This tool is written in .Net Core 2.1 and using `Octokit.Net` accessible [here](https://github.com/octokit/octokit.net) on GitHub 

## GitHub.com, GitHub Enterprise
Synchronizer working well for cloud `GitHub.com` and also for on-premise version `GitHub Enterprise`. For `GitHub.com` use `-uri=https://github.com/` parameter

## How to use it
Open command prompt and execute with **all** parameters.

**_Example when we want synchronize labels across all organization repositories:_**
> `MalikP.GitHub.LabelSynchronizer -uri=https://github.domain.com/ -token=<personalToken> -source-org=<OrganisationName> -source-repo=<RepositoryName> [-target-org=<OrganisationName>] [-strict=<true|false>]` 

**_Example when we want synchronize labels only in specific repository from specific repository:_**
> `MalikP.GitHub.LabelSynchronizer -uri=https://github.domain.com/ -token=<personalToken> -source-org=<OrganisationName> -source-repo=<RepositoryName> [-target-org=<OrganisationName>] -target-repo=<RepositoryName> [-strict=<true|false>]` 

**_or when repositories are not in the same organization:_**
> `MalikP.GitHub.LabelSynchronizer -uri=https://github.domain.com/ -token=<personalToken> -source-org=<OrganisationName> -source-repo=<RepositoryName> -target-org=<OrganisationName> -target-repo=<RepositoryName> [-strict=<true|false>]` 

## Parameters
`-uri=` - this parameter defines GitHub server Uri <br/>
`-strict=` - this parameter defines if synchronization will be strict or not. Strict means that synchronizer will delete, update, create labels<br/>
`-token=` - this parameter defines personal access token <br/>
`-source-org=` - defines organization name in which reference repository is created <br/>
`-source-repo=` - defines name of reference repository used as label source <br/>
`-target-org=` - defines organization name in which target repository is created. When is not defined then is expected that both source and target repositories are in the same organization <br/>
`-target-repo=` - defines name of repository used as label target <br/>
