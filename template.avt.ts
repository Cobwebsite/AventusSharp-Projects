export class Template extends AventusTemplate {
    protected override meta(): TemplateInfo {
        return {
            name: "AventusSharp.Project",
            installationFolder: "AventusSharp/Project",
            description: "Create an AventusSharp project",
            organization: "Cobwebsite",
            isProject: true,
            tags: ["AventusSharp", "Project"],
            version: "0.0.4",
            allowQuick: false,
            documentation: "https://aventussharp.com",
            repository: "https://github.com/Cobwebsite/AventusSharp-Projects",
        };
    }
    protected override async run(destination: string): Promise<void> {

        const name = await this.input({ placeHolder: "Provide a name for your project" });
        if(!name) return;
        this.registerVar("projectName", name);

        let prefix = await this.input({
            placeHolder: "Provide a component prefix : (default is av)",
            validations: [{
                message: "Provide a valid prefix",
                regex: "^(?:[a-z]{2,})?$"
            }]
        });
        if(prefix == null) return;
        if(!prefix) {
            prefix = "av";
        }

        this.registerVar("componentPrefix", prefix);
        
        await this.writeFile();

        await this.exec("dotnet add package AventusSharp");
        await this.exec("dotnet add package Newtonsoft.Json");

    }

}