type SourceControlInfo = {
    RepositoryUrl: string | null;
    Revision: string | null;
};

type SourceControlPathBuilder = (filePath: string, line: number) => string;

interface ISourceControlPathBuilderFactory {
    TryToBuild(sourceControlInfo: SourceControlInfo): SourceControlPathBuilder | null;
}

class GithubPathBuilderFactory implements ISourceControlPathBuilderFactory {
    public TryToBuild(sourceControlInfo: SourceControlInfo): SourceControlPathBuilder | null {
        const httpPattern = new RegExp(`https?://github\\.com/(?<user>[^/]+)/(?<repo>[^/]+)`);
        const httpMatch = httpPattern.exec(sourceControlInfo.RepositoryUrl || "");
        if (httpMatch && httpMatch.groups ) {
            return (filePath, line) => `https://github.com/${httpMatch.groups?.['user']}/${httpMatch.groups?.['repo']}/blob/${sourceControlInfo.Revision}/${filePath}#L${line}`;
        }

        const sshPattern = new RegExp(`(?:ssh://)?git@github\\.com:(?<user>[^/]+)/(?<repo>[^.]+)\\.git`);
        const sshMatch = sshPattern.exec(sourceControlInfo.RepositoryUrl || "");
        if (sshMatch && sshMatch.groups) {
            return (filePath, line) => `https://github.com/${sshMatch.groups?.['user']}/${sshMatch.groups?.['repo']}/blob/${sourceControlInfo.Revision}/${filePath}#L${line}`;
        }

        return null;
    }
}

class BitbucketServerPathBuilderFactory implements  ISourceControlPathBuilderFactory
{
    TryToBuild(sourceControlInfo: SourceControlInfo): SourceControlPathBuilder | null {
        const httpPattern = new RegExp(`https?://(?<domain>[^/]+)/scm/(?<user>[^/]+)/(?<repo>[^.]+)\\.git`);
        const httpMatch = httpPattern.exec(sourceControlInfo.RepositoryUrl || "");
        if (httpMatch && httpMatch.groups ) {
            return (filePath, line) => `https://${httpMatch.groups?.['domain']}/projects/${httpMatch.groups?.['user']}/repos/${httpMatch.groups?.['repo']}/browse/${filePath}?at=${sourceControlInfo.Revision}#${line}`;
        }

        const sshPattern = new RegExp(`(?:ssh://)?git@(?<domain>[^/]+)/(?<user>[^/]+)/(?<repo>[^.]+)\\.git`);
        const sshMatch = sshPattern.exec(sourceControlInfo.RepositoryUrl || "");
        if (sshMatch && sshMatch.groups) {
            return (filePath, line) => `https://${sshMatch.groups?.['domain']}/projects/${sshMatch.groups?.['user']}/repos/${sshMatch.groups?.['repo']}/browse/${filePath}?at=${sourceControlInfo.Revision}#${line}`;
        }

        return null;
    }

}

export interface ICodeLocation {
    FilePath: string;
    LineNumber: number;
}


export class RepoPathResolver{
    private static readonly _factories: ISourceControlPathBuilderFactory[] = [
        new GithubPathBuilderFactory(),
        new BitbucketServerPathBuilderFactory()
    ];

    private static MakePathRelative(rootPath: string, absolutePath: string): string {
        const rootUrl = new URL(rootPath, 'file://');
        const absoluteUrl = new URL(absolutePath, 'file://');
        const relativePath = absoluteUrl.href.substring(rootUrl.href.length);
        return relativePath.startsWith('/') ? relativePath.substring(1) : relativePath;
    }

    public static TryToGetPathBuilder(sourceControlInfo: SourceControlInfo, repoRootPath:string | null) : ((location: ICodeLocation) => string ){
        if(sourceControlInfo.RepositoryUrl != null && sourceControlInfo.Revision != null && repoRootPath != null)
        {
            for (const factory of this._factories) {
                const builder = factory.TryToBuild(sourceControlInfo);
                if (builder){
                    return (location:ICodeLocation) => {
                        const relativePath = RepoPathResolver.MakePathRelative(repoRootPath, location.FilePath);
                        return builder(relativePath, location.LineNumber);
                    }
                }
            }
        }

        return location =>  location.FilePath;
    }
}