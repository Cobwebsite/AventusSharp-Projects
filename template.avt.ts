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

        let sql = await this.select([
            { label: "Mssql" },
            { label: "Mysql" },
            { label: "Postgresql" },
            { label: "Sqlite" },
        ], {
            placeHolder: "Select a database"
        });

        if(sql == null) return;

        this.registerVar("db", sql.label);

        let storage = "";
        if(sql.label == "Mssql") storage = "MsSqlStorage";
        else if(sql.label == "Mysql") storage = "MySQLStorage";
        else if(sql.label == "Postgresql") storage = "PostgreSqlStorage";
        else if(sql.label == "Sqlite") storage = "SqliteStorage";
        this.registerVar("storage", storage);

        const isSqlite = sql.label == "Sqlite";
        this.registerBlock("normalDbConfig", {
            custom: (v) => {
                if(isSqlite) return "";
                return v.trim();
            }
        });
        this.registerBlock("sqliteDbConfig", {
            custom: (v) => {
                if(isSqlite) return v.trim();
                return "";
            }
        });

        await this.writeFile((file) => {
            if(file.relativePath.endsWith(".gitignore")) {
                return false;
            }
            if(file.relativePath.endsWith(".sln")) {
                return false;
            }
            if(file.relativePath.includes("obj")) {
                return false;
            }
            if(file.relativePath.includes("bin")) {
                return false;
            }
            if(file.relativePath.includes("generated")) {
                return false;
            }
            return true;
        });

        const uuid = await this.showProgress("Adding packages");
        await this.exec("dotnet add package AventusSharp.AspNetCore");
        if(sql.label == "Mssql") {
            await this.exec("dotnet add package AventusSharp.Data.Mssql");
        }
        else if(sql.label == "Mysql") {
            await this.exec("dotnet add package AventusSharp.Data.Mysql");
        }
        else if(sql.label == "Postgresql") {
            await this.exec("dotnet add package AventusSharp.Data.Postgresql");
        }
        else if(sql.label == "Sqlite") {
            await this.exec("dotnet add package AventusSharp.Data.Sqlite");
        }
        await this.exec("dotnet add package Serilog.AspNetCore");

        await this.hideProgress(uuid);

        await this.aventusCommand("aventus.emmet");

        await this.showInformationMessage("Projet " + name + " ready. You should restart your IDE to get true autocompletion");

    }

}