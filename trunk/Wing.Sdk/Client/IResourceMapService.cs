using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Client
{
    public interface IResourceMapService
    {
        void MapResource(string name, ResourceType resType, string url, ResourceLoadMode loadMode, string loadBefore = "", string loadAfter = "");

        IEnumerable<IResourceMap> GetResourcesByMode(ResourceLoadMode mode);
        IEnumerable<IResourceMap> Mappings { get; }
    }

    public enum ResourceType
    {
        Script,
        Style
    }

    public enum ResourceLoadMode
    {
        // O recurso será carregado após o wui e o jquery serem inicializados
        // e antes da inicialização do wing e do shell.
        // scripts dessa categorias são carregados no shell e nas paginas de conteudo.
        // USO: Para frameworks de terceiros, como por exemplo um plugin do jquery.
        Plugin = 1,

        // é incluído tanto no shell como nas paginas de conteudo e antes
        // dos recursos ShellAddin e ContentAddin
        GlobalAddin,

        // O recurso será carregado após o wing ter sido inicializado, inclusive o shell e o pipeline.
        ShellAddin,

        // Como o ShellAddin, mas ao contrario, só é carregado nas paginas Content.
        ContentAddin,

        // apenas o mapeamento é feito, o script é carregado atraves do Loader.dependsOn() pelo usuário.
        // esta opção é ideal para registrar recursos compartilhados entre os aplicativos que não sejam plugins.
        // Por exemplo: uma aplicação financeira pode registrar um recursos que disponibiliza funções 
        // de interoperação com outros aplicativos. Registrar como OnDemand faz com que o recurso
        // seja registrado no shell mas não carregado, assim ele é carregado somente por quem precisa dele.
        OnDemand
    }
}
