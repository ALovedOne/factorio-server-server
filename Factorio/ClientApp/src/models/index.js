
export class Game {
    key;
    name;
    description;
    targetVersion;
    mods = [];

    constructor(key, name, description, targetVersion, mods, currentExecution) {
        this.key = key;
        this.name = name;
        this.description = description;
        this.targetVersion = targetVersion || new Version();
        this.mods = mods || [];
        this.currentExecution = currentExecution || null;
    }

    toJson() {
        return JSON.stringify({
            key: this.key,
            name: this.name,
            description: this.description,
            targetVersion: this.targetVersion.toJson(),
            mods: this.mods.map(x => x.toJson())
        })
    }

    static fromJson(jsonObj) {
        return new Game(
            jsonObj.key,
            jsonObj.name,
            jsonObj.description,
            Version.fromJson(jsonObj.targetVersion),
            jsonObj.mods.map(Mod.fromJson),
            CurrentExecution.fromJson(jsonObj.currentExecution),
        );
    }
}

export class Mod {
    name;
    version;

    constructor(name, version) {
        this.name = name;
        this.targetVersion = version;
    }

    toJson() {
        return JSON.stringify({
            name: this.name,
            version: this.targetVersion
        });
    }

    static fromJson(jsonObj) {
        return new Mod(jsonObj.name, Version.fromJson(jsonObj.version));
    }
}

export class CurrentExecution {
    port;
    hostname;
    runningVersion;

    constructor(port, hostname, runningVersion) {
        this.port = port;
        this.hostname = hostname;
        this.runningVersion = runningVersion;
    }

    static fromJson(jsonObj) {
        if (jsonObj) {
            return new CurrentExecution(jsonObj.port, jsonObj.hostname, Version.fromJson(jsonObj.version));
        } else {
            return null;
        }
    }
}

export class Version {
    major;
    minor;
    patch;

    constructor(major, minor, patch) {
        this.major = major;
        this.minor = minor;
        this.patch = patch;
    }

    toJson() {
        return JSON.stringify({
            major: this.major,
            minor: this.minor,
            patch: this.patch
        });
    }

    static fromJson(jsonObj) {
        if (jsonObj) {
            return new Version(jsonObj.major, jsonObj.minor, jsonObj.patch);
        } else {
            return new Version();
        }
    }
}