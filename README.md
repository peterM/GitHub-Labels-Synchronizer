# GitHub Labels Synchronizer
Synchronize GitHub labels across organization repositories

## For what is this good for
Imagine that you have lot of repositories in your account and you want to have in all repositories the same label names, descriptions and colors. You can prepare labels in one repository and then use this repository as reference one. This tool will synchronize these labels and create in all other organization repositories.

## Technology behind
This tool is written in .Net Core 2.1 and using `Octokit.Net` accessible [here](https://github.com/octokit/octokit.net) on GitHub 

## How to use it
Open command prompt and execute with **all** parameters.

**_Example:_**
> `MalikP.GitHub.LabelSynchronizer -uri=https://github.domain.com/ -token=<personalToken> -org=<OrganisationName> -repo=<RepositoryName>` 


## Parameters
`-uri=` - this parameter defines GitHub server Uri <br/>
`-token=` - this parameter defines personal access token <br/>
`-org=` - defines organization name in which reference repository is created <br/>
`-repo=` - defines name of reference repository used as label source <br/>
